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
                        var names = cursor.GetColumnNames();

                        string msgData = "";
                        for (int idx = 0; idx < cursor.ColumnCount; idx++)
                        {
                            msgData += cursor.GetColumnName(idx) + ":" + cursor.GetString(idx) + '\n';
                        }
                        
                    } while (cursor.MoveToNext());
                }
                yield break;
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