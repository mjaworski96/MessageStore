using MessageSender.Model.Http;
using MessageSender.ViewModel.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public ICommand SyncSmsCommand { get => new DelegateCommand(Sync, true, true); }

        private async Task Sync()
        {
            Dictionary<string, int> contactNumberToId = new Dictionary<string, int>();
            using (var contactHttpSender = new ContactHttpSender())
            {
                foreach (var contact in _contactSource.GetAll())
                {
                    var contactWithId = await contactHttpSender.Send(contact);
                    if(!contactNumberToId.ContainsKey(contactWithId.PhoneNumber))
                        contactNumberToId.Add(contactWithId.PhoneNumber, contactWithId.Id);
                }
            }
            

            //_smsSource.GetAll().ToList();
        }
    }
}
