using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using MessageSender.Model;
using MessageSender.ViewModel.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MessageSender.Droid.DeviceServices
{
    public class AndroidSmsSource : ISmsSource
    {
        private readonly ContentResolver _contentResolver;
        private readonly Activity _context;
        public AndroidSmsSource(ContentResolver contentResolver,
            Activity context)
        {
            _contentResolver = contentResolver;
            _context = context;
        }

        public IEnumerable<Sms> GetAll(DateTime from, DateTime to)
        {
            if (ContextCompat.CheckSelfPermission(_context, Manifest.Permission.ReadSms) == (int)Permission.Granted)
            {
                var uri = Android.Net.Uri.Parse("content://mms-sms/complete-conversations");
                var projection = new[] { "_id", "ct_t", "normalized_date", "thread_id" };
                long fromMiliseconds = (long)from.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                long toMiliseconds = (long)to.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                var selection = $"normalized_date<{fromMiliseconds} OR normalized_date>{toMiliseconds}";
                using (var cursor = _contentResolver.Query(uri, projection, selection, null, "normalized_date"))
                {
                    if (cursor.MoveToFirst())
                    {
                        do
                        {
                            var type = cursor.GetString(cursor.GetColumnIndex("ct_t"));
                            var id = cursor.GetString(cursor.GetColumnIndex("_id"));

                            var normalizedDate = cursor.GetString(cursor.GetColumnIndex("normalized_date"));
                            var unixTimestamp = long.Parse(normalizedDate);
                            var smsDate = new DateTime(1970, 1, 1).AddMilliseconds(unixTimestamp);
                            Sms sms = type == "application/vnd.wap.multipart.related"
                                ? GetMms(id, smsDate)
                                : GetSms(id, smsDate);
                            if (!sms.ShouldBeIgnored)
                            {
                                yield return sms;
                            }
                        } while (cursor.MoveToNext());
                    }
                }
            }
        }
        public int GetCount(DateTime from, DateTime to)
        {
            if (ContextCompat.CheckSelfPermission(_context, Manifest.Permission.ReadSms) == (int)Permission.Granted)
            {
                var uri = Android.Net.Uri.Parse("content://mms-sms/complete-conversations");
                var projection = new[] { "_id" };
                long fromMiliseconds = (long)from.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                long toMiliseconds = (long)to.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                var selection = $"normalized_date<{fromMiliseconds} OR normalized_date>{toMiliseconds}";
                using (var cursor = _contentResolver.Query(uri, projection, selection, null, "normalized_date"))
                {
                    return cursor.Count;
                }

            }
            return 0;
        }
        private Sms GetSms(string id, DateTime date)
        {
            var uri = Telephony.Sms.ContentUri;
            var selection = "_id = " + id;
            using (var cursor = _contentResolver.Query(uri, null, selection, null, null))
            {
                cursor.MoveToFirst();
                var type = cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Type));
                return new Sms
                {
                    Content = cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Body)),
                    Person = ParseNullableInt(cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Person))),
                    Date = date,
                    ThreadId = TryParsePhoneNumber(cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.ThreadId))),
                    WriterType = type == ((int)SmsMessageType.Inbox).ToString()
                        ? Sms.WRITER_CONTACT : Sms.WRITER_ME,
                    ShouldBeIgnored = type == ((int)SmsMessageType.Draft).ToString()
                        || type == ((int)SmsMessageType.Outbox).ToString(),
                    HasError = type == ((int)SmsMessageType.Failed).ToString()
                };
            }
        }
        private string TryParsePhoneNumber(string address)
        {
            if (!address.Contains(" "))
                return address;
            var withoutSpaces = address.Replace(" ", "");
            if (long.TryParse(withoutSpaces.Replace("+", ""), out long result))
                return withoutSpaces;
            else
                return address;
        }
        private Sms GetMms(string id, DateTime date)
        {
            var uri = Telephony.Mms.ContentUri;
            var selection = "_id = " + id;
            var cursor = _contentResolver.Query(uri, null, selection, null, null);

            cursor.MoveToFirst();

            var subject = cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Mms.InterfaceConsts.Subject));
            var content = GetMmsContent(id);
            if (!string.IsNullOrEmpty(subject))
                content = $"{subject}\n{content}";
            var messageBox = cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Mms.InterfaceConsts.MessageBox));
            return new Sms
            {
                Content = content,
                ThreadId = cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Mms.InterfaceConsts.ThreadId)),
                Attachments = GetMmsAttachments(id).ToList(),
                Date = date,
                Person = GetMmsContactId(id),
                WriterType = messageBox == ((int)MessageBoxType.Inbox).ToString()
                        ? Sms.WRITER_CONTACT : Sms.WRITER_ME,
                ShouldBeIgnored = messageBox == ((int)MessageBoxType.Drafts).ToString()
                || messageBox == ((int)MessageBoxType.Outbox).ToString(),
                HasError = messageBox == ((int)MessageBoxType.Failed).ToString()
            };
        }

        private string GetMmsContent(string id)
        {
            var selectionPart = "mid=" + id;
            var uri = Android.Net.Uri.Parse("content://mms/part");
            var sb = new StringBuilder();
            using (var cursor = _contentResolver.Query(uri, null, selectionPart, null, null))
            {
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        var partId = cursor.GetString(cursor.GetColumnIndex("_id"));
                        var type = cursor.GetString(cursor.GetColumnIndex("ct"));
                        if (type == "text/plain")
                        {
                            var data = cursor.GetString(cursor.GetColumnIndex("_data"));
                            if (data != null)
                                sb.Append(GetMmsText(partId));
                            else
                                sb.Append(cursor.GetString(cursor.GetColumnIndex("text")));
                        }
                    } while (cursor.MoveToNext());
                }
            }
            return sb.ToString();
        }
        private string GetMmsText(string id)
        {
            var partURI = Android.Net.Uri.Parse("content://mms/part/" + id);
            using (var sr = new StreamReader(_contentResolver.OpenInputStream(partURI)))
            {
                return sr.ReadToEnd();
            }
        }
        private IEnumerable<Attachment> GetMmsAttachments(string id)
        {
            var selectionPart = "mid=" + id;
            var uri = Android.Net.Uri.Parse("content://mms/part");
            using (var cursor = _contentResolver.Query(uri, null, selectionPart, null, null))
            {
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        var partId = cursor.GetString(cursor.GetColumnIndex("_id"));
                        var type = cursor.GetString(cursor.GetColumnIndex("ct"));
                        var name = cursor.GetString(cursor.GetColumnIndex("name"));
                        if (type != "text/plain" && type != "application/smil")
                        {
                            yield return new Attachment
                            {
                                Content = GetAttachmentContent(partId),
                                ContentType = type,
                                SaveAsFilename = name
                            };
                        }
                    } while (cursor.MoveToNext());
                }
            }
        }
        private byte[] GetAttachmentContent(string id)
        {
            var partURI = Android.Net.Uri.Parse("content://mms/part/" + id);
            using (var content = _contentResolver.OpenInputStream(partURI))
            {
                using (var memoryStream = new MemoryStream())
                {
                    content.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
        private int? GetMmsContactId(string id)
        {
            var selectionAdd = "msg_id=" + id;
            var uriStr = $"content://mms/{id}/addr";
            var uri = Android.Net.Uri.Parse(uriStr);
            using (var cursor = _contentResolver.Query(uri, null, selectionAdd, null, null))
            {
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        var contactId = cursor.GetString(cursor.GetColumnIndex("contact_id"));
                        return ParseNullableInt(contactId);

                    } while (cursor.MoveToNext());
                }
                return null;
            }
        }
        private int? ParseNullableInt(string src)
        {
            if (string.IsNullOrEmpty(src) || src == "0")
            {
                return null;
            }
            return int.Parse(src);
        }
    }
}