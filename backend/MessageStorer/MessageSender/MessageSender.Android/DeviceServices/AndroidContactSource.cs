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
using System.Linq;

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
            if (ContextCompat.CheckSelfPermission(_context, Manifest.Permission.ReadContacts) == (int)Permission.Granted)
            {  
                var projection = new [] {
                    Telephony.ThreadsColumns.Id, Telephony.ThreadsColumns.RecipientIds,
                };
                var uri =
                    Telephony.Threads.ContentUri.BuildUpon().AppendQueryParameter("simple", "true").Build();

                using (var threads = _contentResolver.Query(
                    uri, projection, null, null))
                {
                    if (threads.MoveToFirst())
                    {
                        var rawContacts = GetRawContacts().ToList();
                        var cannonical = GetCannonicalAddresses();

                        do
                        {
                            var id = threads.GetInt(threads.GetColumnIndex(Telephony.ThreadsColumns.Id));
                            var rawRecipientIds = threads.GetString(threads.GetColumnIndex(Telephony.ThreadsColumns.RecipientIds));
                            var recipientIds = rawRecipientIds.Split(" ").Select(x => int.Parse(x)).ToList();
                            List<ContactMember> contactMembers = null;
                            if (recipientIds.Count > 1)
                            {
                                contactMembers = recipientIds
                                    .Select(x => new ContactMember
                                    {
                                        InternalId = rawContacts.FirstOrDefault(y => y.Id == x)?.Id.ToString(),
                                        Name = rawContacts.FirstOrDefault(y => y.Id == x)?.Name
                                    })
                                    .ToList();
                            }
                            var phoneNumbers = rawContacts.Select(x => x.PhoneNumber).OrderBy(x => x).ToList();
                            var contactName = recipientIds.Count > 1 ? id.ToString() :
                                rawContacts.FirstOrDefault(x => x.PhoneNumber == cannonical[recipientIds.FirstOrDefault()])?.Name;
                            if (string.IsNullOrEmpty(contactName))
                            {
                                contactName = cannonical[recipientIds.FirstOrDefault()];
                            }
                            yield return new Contact
                            {
                                InApplicationId = id.ToString(),
                                Name = contactName,
                                Members = contactMembers,
                            };
                        } while (threads.MoveToNext());
                        yield break;
                    }
                }
            }
        }
        private IEnumerable<RawContact> GetRawContacts()
        {
            if (ContextCompat.CheckSelfPermission(_context, Manifest.Permission.ReadContacts) == (int)Permission.Granted)
            {
                using (var contacts = _contentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null))
                {
                    if (contacts.MoveToFirst())
                    {
                        do
                        {
                            yield return new RawContact
                            {
                                Id = contacts.GetInt(contacts.GetColumnIndex(
                                    ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Id)),
                                Name = contacts.GetString(contacts.GetColumnIndex(
                                    ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName)),
                                PhoneNumber = contacts.GetString(contacts.GetColumnIndex(
                                    ContactsContract.CommonDataKinds.Phone.NormalizedNumber)),
                            };

                        } while (contacts.MoveToNext());
                    }
                }
            }
        }
        private Dictionary<int, string> GetCannonicalAddresses()
        {
            var result = new Dictionary<int, string>();
            if (ContextCompat.CheckSelfPermission(_context, Manifest.Permission.ReadContacts) == (int)Permission.Granted)
            {
                using (var addresses = _contentResolver.Query(Uri.Parse("content://mms-sms/canonical-addresses"), null, null, null))
                {
                    if (addresses.MoveToFirst())
                    {
                        do
                        {
                            result.Add(
                                addresses.GetInt(addresses.GetColumnIndex(Telephony.CanonicalAddressesColumns.Id)),
                                addresses.GetString(addresses.GetColumnIndex(Telephony.CanonicalAddressesColumns.Address))
                                );
                        } while (addresses.MoveToNext());
                    }
                }
            }
            return result;
        }
    }
}