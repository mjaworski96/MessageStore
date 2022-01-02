using Android;
using Android.App;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using MessageSender.ViewModel.Interfaces;

namespace MessageSender.Droid.DeviceServices
{
    public class AndroidPermisionsService : IPermisionsService
    {
        private readonly Activity _context;
        public AndroidPermisionsService(Activity context)
        {
            _context = context;
        }
        public void Request()
        {
            ActivityCompat.RequestPermissions(_context,
                new[] { Manifest.Permission.ReadContacts, Manifest.Permission.ReadSms },
                PermissionChecker.PermissionGranted);
        }
    }
}