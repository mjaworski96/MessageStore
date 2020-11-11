using API.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IAppUserRepository
    {
        Task<AppUsers> Get(string name);
        Task<AppUsers> GetByEmail(string email);
        Task<AppUsers> Add(AppUsers user);
        Task Remove(string username);
        Task Save();
    }
    public class AppUserRepository: IAppUserRepository
    {
        private readonly MessagesStoreContext _messageStoreContext;

        public AppUserRepository(MessagesStoreContext messageStoreContext)
        {
            _messageStoreContext = messageStoreContext;
        }

        public async Task<AppUsers> Add(AppUsers user)
        {
            var entity = (await _messageStoreContext.AppUsers.AddAsync(user))
                .Entity;
            await _messageStoreContext.SaveChangesAsync();
            return entity;
        }

        public async Task<AppUsers> Get(string name)
        {
            try
            {
                return await _messageStoreContext.AppUsers
                    .FirstAsync(x => x.Username == name);
            }
            catch(InvalidOperationException e)
            {
                throw new NotFoundException($"User {name} not found.", e);
            }
        }

        public async Task<AppUsers> GetByEmail(string email)
        {
            try
            {
                return await _messageStoreContext.AppUsers
                    .FirstAsync(x => x.Email == email);
            }
            catch (InvalidOperationException e)
            {
                throw new NotFoundException($"User with email: {email} not found.", e);
            }
        }

        public async Task Remove(string username)
        {
            var userToDelete = await _messageStoreContext.AppUsers
                    .FirstOrDefaultAsync(x => x.Username == username);
            if (userToDelete != null)
            {
                _messageStoreContext.Remove(userToDelete);
                var aliasMembersToRemove = await _messageStoreContext
                    .Aliases
                    .Include(x => x.AliasesMembers)
                    .ThenInclude(x => x.Contact)
                    .Where(x => x.AliasesMembers
                        .All(y => y.Contact.AppUserId == userToDelete.Id))
                    .ToListAsync();
                _messageStoreContext.RemoveRange(aliasMembersToRemove);
                await _messageStoreContext.SaveChangesAsync();
            }
        }

        public async Task Save()
        {
            await _messageStoreContext.SaveChangesAsync();
        }
    }
}
