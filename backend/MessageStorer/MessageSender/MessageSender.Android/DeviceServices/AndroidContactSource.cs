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
using static Android.Provider.Contacts;

namespace MessageSender.Droid.DeviceServices
{
    public class AndroidContactSource : IContactSource
    {
        private readonly ContentResolver _contentResolver;
        private readonly Activity _context;
        public AndroidContactSource(ContentResolver contentResolver,
            Activity context)
        {
            _contentResolver = contentResolver;
            _context = context;
        }

        public IEnumerable<Contact> GetAll()
        {
            RequestPermisions();
            if (ContextCompat.CheckSelfPermission(_context, Manifest.Permission.ReadContacts) == (int)Permission.Granted)
            {
                var uri = ContactsContract.Contacts.ContentUri;
                var contacts = _contentResolver.Query(uri, null, null, null);
                var phones = _contentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null);
                var test = phones.GetColumnNames();
                if (phones.MoveToFirst())
                {
                    do
                    {

                        string phonesData = "";
                        for (int idx = 0; idx < phones.ColumnCount; idx++)
                        {
                            phonesData += phones.GetColumnName(idx) + ":" + phones.GetString(idx) + '\n';
                        }

                    } while (phones.MoveToNext());
                }
                yield break;
            }

        }
        private void RequestPermisions()
        {
            ActivityCompat.RequestPermissions(_context,
                new[] { Manifest.Permission.ReadContacts },
                (int)RequestPermissionCodes.READ_CONTACT);
        }
    }
}