using API.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IAppUserRepository
    {
        Task<AppUser> Get(string name);
    }
    public class AppUserRepository: IAppUserRepository
    {
        private readonly MessagesStoreContext _messageStoreContext;

        public AppUserRepository(MessagesStoreContext messageStoreContext)
        {
            _messageStoreContext = messageStoreContext;
        }

        public async Task<AppUser> Get(string name)
        {
            try
            {
                return await _messageStoreContext.AppUser
                    .FirstAsync(x => x.Username == name);
            }
            catch(InvalidOperationException e)
            {
                throw new NotFoundException($"User {name} not found.", e);
            }
        }
    }
}
