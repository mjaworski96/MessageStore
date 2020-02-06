using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IMessageRepository
    {
        Task Add(Message message);
        Task Save();
        Task<Message> GetNewest(string appUser, string application);
    }
    public class MessageRepository: IMessageRepository
    {
        private readonly MessagesStoreContext _messageStoreContext;

        public MessageRepository(MessagesStoreContext messageStoreContext)
        {
            _messageStoreContext = messageStoreContext;
        }
        public async Task Add(Message message)
        {
            await _messageStoreContext.AddAsync(message);
        }

        public Task<Message> GetNewest(string appUser, string application)
        {
            return _messageStoreContext
                .Message
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x =>
                    x.Contact.AppUser.Username == appUser
                    && x.Contact.Application.Name == application);
        }

        public async Task Save()
        {
            await _messageStoreContext.SaveChangesAsync();
        }
    }
}
