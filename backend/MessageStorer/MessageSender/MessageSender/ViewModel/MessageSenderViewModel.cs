using MessageSender.Model;
using MessageSender.Model.Http;
using MessageSender.ViewModel.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessageSender.ViewModel
{
    public class MessageSenderViewModel : BaseViewModel, INotifyPropertyChanged, IExceptionHandler
    {
        private readonly ISmsSource _smsSource;
        private readonly IContactSource _contactSource;
        private readonly IPermisionsService _permisionsService;
        private readonly IPageChanger _pageChanger;
        private bool _canLogout;
        private double _currentProgress;
        private string _error;
        private bool _appStoped;

        public MessageSenderViewModel(ISmsSource smsSource,
            IContactSource contactSource,
            IPermisionsService permisionsService,
            IPageChanger pageChanger)
        {
            _smsSource = smsSource;
            _contactSource = contactSource;
            _permisionsService = permisionsService;
            _pageChanger = pageChanger;
            _canLogout = true;
            _appStoped = false;
            SyncSmsCommand = new DelegateCommand(Sync, this, true, true);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand SyncSmsCommand { get; set; }
        public ICommand LogoutCommand { get => new DelegateCommand(Logout, this, _canLogout, true); }
        
        public double CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = value;
                NotifyPropertyChanged("CurrentProgress");
            }
        }
        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                NotifyPropertyChanged("Error");
            }
        }
        public void Handle(Exception e)
        {
            if (e is ApiException apiException)
            {
                if (apiException.Code == 401)
                {
                    _pageChanger.ShowLoginPage();
                }
                else
                {
                    Error = $"{apiException.Code} - {apiException.Message}";
                }
            }
            else
            {
                Error = $"{e.GetType().Name}\n{e.Message}\n{e.StackTrace}";
            }
            UpdateCanLogout(true);
        }

        public void Clear()
        {
            Error = "";
        }
        public void OnSleep()
        {
            _appStoped = true;
        }
        public void OnResume()
        {
            _appStoped = false;
        }
        private async Task Sync()
        {
            UpdateCanLogout(false);

            _permisionsService.Request();
            var threadIdToContact = new Dictionary<string, ContactWithId>();
            CurrentProgress = 0;
            int currentSent = 0;
            var importId = Guid.NewGuid().ToString();
            using (var contactHttpSender = new ContactHttpSender(ServerIp))
            using (var smsHttpSender = new SmsHttpSender(ServerIp))
            using (var importHttpSender = new ImportApiSender(ServerIp))
            {
                var lastSyncTime = await smsHttpSender.GetLastSyncTime();
                int maxProgress = _smsSource.GetCount(lastSyncTime.From, lastSyncTime.To);
                var contacts = _contactSource.GetAll().ToList();
                foreach (var sms in _smsSource.GetAll(lastSyncTime.From, lastSyncTime.To))
                {
                    if (_appStoped)
                    {
                        break;
                    }
                    if (!threadIdToContact.ContainsKey(sms.ThreadId))
                    {
                        var rawContact = contacts.FirstOrDefault(x => x.InApplicationId == sms.ThreadId);
                        var contactFromApi = await contactHttpSender.Send(rawContact);
                        threadIdToContact.Add(rawContact.InApplicationId, contactFromApi);
                    }
                    var contact = threadIdToContact[sms.ThreadId];
                    sms.ContactId = contact.Id;
                    sms.ImportId = importId;
                    if (sms.Person != null)
                    {
                        var member = contact.Members.FirstOrDefault(x => x.InternalId == sms.Person.ToString());
                        if (member != null)
                        {
                            sms.ContactMemberId = member.Id;
                        }
                    }
                    await smsHttpSender.Send(sms);
                    UpdateProgress(ref currentSent, maxProgress);
                }
                if (currentSent > 0)
                {
                    await importHttpSender.Finish(importId);
                }
               
            }
            CurrentProgress = 0;
            UpdateCanLogout(true);
        }

        private void UpdateCanLogout(bool canLogout)
        {
            _canLogout = canLogout;
            NotifyPropertyChanged("LogoutCommand");
        }

        private async Task Logout()
        {
            var session = new SessionStorage();
            session.Clear();
            _pageChanger.ShowLoginPage();
            await Task.CompletedTask;
        }

        private void UpdateProgress(ref int current, int max)
        {
            current++;
            CurrentProgress = 1.0 * current / max;
        }
    }
}
