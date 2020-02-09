using MessageSender.Model;
using MessageSender.Model.Http;
using MessageSender.ViewModel.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessageSender.ViewModel
{
    public class MessageSenderViewModel: INotifyPropertyChanged
    {
        private readonly ISmsSource _smsSource;
        private readonly IContactSource _contactSource;
        private readonly IPermisionsService _permisionsService;
        private double _currentProgress;

        public MessageSenderViewModel(ISmsSource smsSource,
            IContactSource contactSource,
            IPermisionsService permisionsService)
        {
            _smsSource = smsSource;
            _contactSource = contactSource;
            _permisionsService = permisionsService;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand SyncSmsCommand { get => new DelegateCommand(Sync, true, true); }
        public double CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = value;
                NotifyPropertyChanged("CurrentProgress");
            }
        }

        private async Task Sync()
        {
            _permisionsService.Request();
            Dictionary<string, int> contactNumberToId = new Dictionary<string, int>();
            CurrentProgress = 0;
            int currentSent = 0;
            int maxProgress = _contactSource.GetCount() + _smsSource.GetCount();

            using (var contactHttpSender = new ContactHttpSender())
            {
                foreach (var contact in _contactSource.GetAll())
                {
                    var contactWithId = await contactHttpSender.Send(contact);
                    if (!contactNumberToId.ContainsKey(contactWithId.PhoneNumber))
                        contactNumberToId.Add(contactWithId.PhoneNumber, contactWithId.Id);
                    UpdateProgress(ref currentSent, maxProgress);
                }

                using (var smsHttpSender = new SmsHttpSender())
                {
                    var lastSyncTime = await smsHttpSender.GetLastSyncTime();
                    foreach (var sms in _smsSource.GetAll())
                    {
                        if(sms.Date.Value > lastSyncTime)
                        {
                            if (!contactNumberToId.ContainsKey(sms.PhoneNumber))
                            {
                                var missingContact = await contactHttpSender.Send(new Contact
                                {
                                    Name = sms.PhoneNumber,
                                    PhoneNumber = sms.PhoneNumber,
                                });
                                if (!contactNumberToId.ContainsKey(missingContact.PhoneNumber))
                                    contactNumberToId.Add(missingContact.PhoneNumber, missingContact.Id);
                            }
                            sms.ContactId = contactNumberToId[sms.PhoneNumber];
                            await smsHttpSender.Send(sms);
                        }                    
                        UpdateProgress(ref currentSent, maxProgress);
                    }
                }
            }
            CurrentProgress = 0;
        }
        private void UpdateProgress(ref int current, int max)
        {
            current++;
            CurrentProgress = 1.0 * current / max;
        }
    }
}
