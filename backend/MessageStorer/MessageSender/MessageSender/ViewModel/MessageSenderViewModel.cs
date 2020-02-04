using MessageSender.ViewModel.Interfaces;
using System.Linq;
using System.Windows.Input;

namespace MessageSender.ViewModel
{
    public class MessageSenderViewModel
    {
        private ISmsSource _smsSource;
        private IContactSource _contactSource;

        public MessageSenderViewModel(ISmsSource smsSource,
            IContactSource contactSource)
        {
            _smsSource = smsSource;
            _contactSource = contactSource;
        }

        public ICommand SyncSmsCommand { get => new DelegateCommand(Sync); }

        private void Sync()
        {
            _contactSource.GetAll().ToList();
            _smsSource.GetAll().ToList();
        }
    }
}
