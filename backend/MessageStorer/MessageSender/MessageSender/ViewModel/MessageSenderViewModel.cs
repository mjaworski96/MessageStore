using MessageSender.Model;
using MessageSender.Model.Http;
using MessageSender.ViewModel.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace MessageSender.ViewModel
{
    public class MessageSenderViewModel : INotifyPropertyChanged, IExceptionHandler
    {
        private readonly ISmsSource _smsSource;
        private readonly IContactSource _contactSource;
        private readonly IPermisionsService _permisionsService;
        private double _currentProgress;
        private string _error;

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
        public ICommand SyncSmsCommand { get => new DelegateCommand(Sync, this, true, true); }
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
        private string GetPath(string path)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(basePath, path);
        }
        public Config Config
        {
            get
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Config));
                try
                {
                    using (TextReader reader = new StreamReader(GetPath("./config.xml")))
                    {
                        return (Config)deserializer.Deserialize(reader);
                    }
                }
                catch(IOException)
                {
                    return new Config { ServerAddress = "" };
                }
            }
            set
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                using (TextWriter writer = new StreamWriter(GetPath("./config.xml")))
                {
                    serializer.Serialize(writer, value);
                }
            }
        }
        public string ServerIp
        {
            get
            {
                return Config.ServerAddress;
            }
            set
            {
                var config = Config;
                config.ServerAddress = value;
                Config = config;
            }
        }
        public void Handle(Exception e)
        {
            Error = $"{e.GetType().Name}\n{e.Message}\n{e.StackTrace}";
        }

        public void Clear()
        {
            Error = "";
        }
        private async Task Sync()
        {
            _permisionsService.Request();
            Dictionary<string, int> contactNumberToId = new Dictionary<string, int>();
            CurrentProgress = 0;
            int currentSent = 0;


            using (var contactHttpSender = new ContactHttpSender(ServerIp))
            using (var smsHttpSender = new SmsHttpSender(ServerIp))
            {
                var lastSyncTime = await smsHttpSender.GetLastSyncTime();
                int maxProgress = _contactSource.GetCount() + _smsSource.GetCount(lastSyncTime);

                foreach (var contact in _contactSource.GetAll())
                {
                    var contactWithId = await contactHttpSender.Send(contact);
                    if (!contactNumberToId.ContainsKey(contactWithId.PhoneNumber))
                        contactNumberToId.Add(contactWithId.PhoneNumber, contactWithId.Id);
                    UpdateProgress(ref currentSent, maxProgress);
                }

                foreach (var sms in _smsSource.GetAll(lastSyncTime))
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
                    UpdateProgress(ref currentSent, maxProgress);
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
