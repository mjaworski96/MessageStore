using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IContactRepository
    {
        Task<Contact> Get(string appName, string inAppId, string appUserName);
        Task AddIfNotExists(Contact entity);
        Task Save();
    }
    public class ContactRepository: IContactRepository
    {
        private readonly MessagesStoreContext _messageStoreContext;

        public ContactRepository(MessagesStoreContext messageStoreContext)
        {
            _messageStoreContext = messageStoreContext;
        }

        public async Task AddIfNotExists(Contact entity)
        {
            if(entity.Id == 0)
            {
                await _messageStoreContext.Contact.AddAsync(entity);
            }
        }

        public Task<Contact> Get(string appName, string inAppId, string appUserName)
        {
            return _messageStoreContext
                .Contact.FirstOrDefaultAsync(x =>
                x.Application.Name == appName &&
                x.InApplicationId == inAppId &&
                x.AppUser.Username == appUserName);
        }

        public async Task Save()
        {
            await _messageStoreContext.SaveChangesAsync();
        }
    }
}
