using API.Exceptions;
using API.Persistance.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IAppUserRepository
    {
        AppUser Get(string name);
    }
    public class AppUserRepository: IAppUserRepository
    {
        private readonly MessagesStoreContext _messageStoreContext;

        public AppUserRepository(MessagesStoreContext messageStoreContext)
        {
            _messageStoreContext = messageStoreContext;
        }

        public AppUser Get(string name)
        {
            try
            {
                return _messageStoreContext.AppUser
                    .First(x => x.Username == name);
            }
            catch(InvalidOperationException e)
            {
                throw new NotFoundException($"User {name} not found.", e);
            }
        }
    }
}
