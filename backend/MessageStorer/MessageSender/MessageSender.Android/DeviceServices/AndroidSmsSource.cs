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
                var projection = new[] { "_id", "ct_t", "normalized_date" };
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

                            if (type == "application/vnd.wap.multipart.related")
                                yield return GetMms(id, smsDate);
                            else
                                yield return GetSms(id, smsDate);
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
                var debug = cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Address));

                return new Sms
                {
                    Content = cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Body)),
                    Date = date,
                    PhoneNumber = TryParsePhoneNumber(cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Address))),
                    WriterType = cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Type)) == ((int)SmsMessageType.Sent).ToString()
                        ? Sms.WRITER_ME : Sms.WRITER_CONTACT
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

            string msgData = "";
            for (int idx = 0; idx < cursor.ColumnCount; idx++)
            {
                msgData += cursor.GetColumnName(idx) + ":" + cursor.GetString(idx) + '\n';
            }
            var subject = cursor.GetString(
                        cursor.GetColumnIndex(Telephony.Mms.InterfaceConsts.Subject));
            var content = GetMmsContent(id);
            if(!string.IsNullOrEmpty(subject))
                content = $"{subject}\n{content}";
            var attachments = GetMmsAttachments(id).ToList();
            var phoneNumber = TryParsePhoneNumber(GetMmsPhoneNumber(id));

            return new Sms
            {
                Content = content,
                PhoneNumber = phoneNumber,
                Attachments = attachments,
                Date = date,
                WriterType = cursor.GetString(
                        cursor.GetColumnIndex("msg_box")) == "2"
                        ? Sms.WRITER_ME : Sms.WRITER_CONTACT
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
                        if (type != "text/plain" && type != "application/smil")
                        {
                            yield return new Attachment
                            {
                                Content = GetAttachmentContent(partId),
                                ContentType = type
                            };
                        }
                    } while (cursor.MoveToNext());
                }
            }
        }
        private byte[] GetAttachmentContent(string id)
        {
            var partURI = Android.Net.Uri.Parse("content://mms/part/" + id);
            using(var content = _contentResolver.OpenInputStream(partURI))
            {
                using (var memoryStream = new MemoryStream())
                {
                    content.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
        private string GetMmsPhoneNumber(string id)
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
                        var address = cursor.GetString(cursor.GetColumnIndex("address"));
                        if (address != "insert-address-token")
                            return address;
                       
                    } while (cursor.MoveToNext());
                }
                return "";
            }
        }

    }
}