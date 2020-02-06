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

        public IEnumerable<Sms> GetAll()
        {
            RequestPermisions();
            if (ContextCompat.CheckSelfPermission(_context, Manifest.Permission.ReadSms) == (int)Permission.Granted)
            {
                var uri = Telephony.Sms.ContentUri;
                var cursor = _contentResolver.Query(uri, null, null, null);
                if(cursor.MoveToFirst())
                {
                    do
                    {
                        var date = long.Parse(cursor.GetString(
                                cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Date)));
                        
                        yield return new Sms
                        {
                            Content = cursor.GetString(
                                cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Body)),
                            Date = new DateTime(1970, 1, 1).AddMilliseconds(date),
                            PhoneNumber = cursor.GetString(
                                cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Address)),
                            WriterType = cursor.GetString(
                                cursor.GetColumnIndex(Telephony.Sms.InterfaceConsts.Type)) == ((int)SmsMessageType.Sent).ToString()
                                ? Sms.WRITER_ME : Sms.WRITER_CONTACT
                        };
                    } while (cursor.MoveToNext());
                }
            }
            
        }
        private void RequestPermisions()
        {
            ActivityCompat.RequestPermissions(_context,
                new[] { Manifest.Permission.ReadSms },
                (int)RequestPermissionCodes.READ_SMS);
        }
    }
}