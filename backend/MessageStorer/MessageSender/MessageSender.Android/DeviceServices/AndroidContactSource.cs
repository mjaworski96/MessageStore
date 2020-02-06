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
                var phones = _contentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null);
                if (phones.MoveToFirst())
                {
                    do
                    {
                        yield return new Contact
                        {
                            Name = phones.GetString(phones.GetColumnIndex(
                                ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName)),
                            PhoneNumber = phones.GetString(phones.GetColumnIndex(
                                ContactsContract.CommonDataKinds.Phone.NormalizedNumber))
                        };

                    } while (phones.MoveToNext());
                }
            }
        }

        public int GetCount()
        {
            RequestPermisions();
            if (ContextCompat.CheckSelfPermission(_context, Manifest.Permission.ReadContacts) == (int)Permission.Granted)
            {
                var phones = _contentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null);
                return phones.Count;
            }
            return 0;
        }

        private void RequestPermisions()
        {
            ActivityCompat.RequestPermissions(_context,
                new[] { Manifest.Permission.ReadContacts },
                (int)RequestPermissionCodes.READ_CONTACT);
        }
    }
}