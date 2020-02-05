using API.Persistance.Entity;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IMessageRepository
    {
        Task Add(Message message);
        Task Save();
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
   
        public async Task Save()
        {
            await _messageStoreContext.SaveChangesAsync();
        }
    }
}
