using API.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IContactRepository
    {
        Task<Contacts> Get(int id);
        Task<Contacts> Get(string appName, string inAppId, string appUserName);
        Task<AppUsers> GetOwner(int contactId);
        Task AddIfNotExists(Contacts entity);
        Task Save();
    }
    public class ContactRepository: IContactRepository
    {
        private readonly MessagesStoreContext _messageStoreContext;

        public ContactRepository(MessagesStoreContext messageStoreContext)
        {
            _messageStoreContext = messageStoreContext;
        }

        public async Task AddIfNotExists(Contacts entity)
        {
            if(entity.Id == 0)
            {
                await _messageStoreContext.Contacts.AddAsync(entity);
            }
        }

        public async Task<Contacts> Get(string appName, string inAppId, string appUserName)
        {
            return await _messageStoreContext
                .Contacts
                .Include(x => x.AliasesMembers)
                .ThenInclude(x => x.Alias)
                .Include(x => x.AppUser)
                .FirstOrDefaultAsync(x =>
                x.Application.Name == appName &&
                x.InApplicationId == inAppId &&
                x.AppUser.Username == appUserName);
        }
        public Task<Contacts> Get(int id)
        {
            try
            {
                return _messageStoreContext
                .Contacts
                .FirstAsync(x => x.Id == id);
            }
            catch(InvalidOperationException e)
            {
                throw new NotFoundException($"Contact with id {id} not found.", e);
            }
        }

        public Task<AppUsers> GetOwner(int contactId)
        {
            return _messageStoreContext
                .Contacts
                .Include(x => x.AppUser)
                .Where(x => x.Id == contactId)
                .Select(x => x.AppUser)
                .FirstOrDefaultAsync();
        }

        public async Task Save()
        {
            await _messageStoreContext.SaveChangesAsync();
        }
    }
}
