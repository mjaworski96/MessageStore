using Common.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IAppUserRepository
    {
        Task<AppUsers> Get(int id, bool throwExeptionIfNotFound);
        Task<AppUsers> GetByUsername(string name, bool throwExeptionIfNotFound);
        Task<AppUsers> GetByEmail(string email, bool throwExeptionIfNotFound);
        Task<AppUsers> Add(AppUsers user);
        Task Remove(int userId);
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

        public async Task<AppUsers> Get(int id, bool throwExeptionIfNotFound)
        {
            try
            {
                if (throwExeptionIfNotFound)
                    return await _messageStoreContext.AppUsers
                        .FirstAsync(x => x.Id == id);
                else
                    return await _messageStoreContext.AppUsers
                        .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (InvalidOperationException e)
            {
                throw new UserByIdNotFoundException(id, e);
            }
        }

        public async Task<AppUsers> GetByUsername(string name, bool throwExeptionIfNotFound)
        {
            try
            {
                if(throwExeptionIfNotFound)
                    return await _messageStoreContext.AppUsers
                        .FirstAsync(x => x.Username == name);
                else 
                    return await _messageStoreContext.AppUsers
                        .FirstOrDefaultAsync(x => x.Username == name);
            }
            catch(InvalidOperationException e)
            {
                throw new UserByUsernameNotFoundException(name, e);
            }
        }

        public async Task<AppUsers> GetByEmail(string email, bool throwExeptionIfNotFound)
        {
            try
            {
                if (throwExeptionIfNotFound)
                    return await _messageStoreContext.AppUsers
                        .FirstAsync(x => x.Email == email);
                else
                    return await _messageStoreContext.AppUsers
                        .FirstOrDefaultAsync(x => x.Email == email);
            }
            catch (InvalidOperationException e)
            {
                throw new UserByEmailException(email, e);
            }
        }

        public async Task Remove(int userId)
        {
            var userToDelete = await _messageStoreContext.AppUsers
                    .FirstOrDefaultAsync(x => x.Id == userId);
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
