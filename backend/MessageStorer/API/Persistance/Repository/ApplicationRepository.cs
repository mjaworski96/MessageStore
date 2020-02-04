using API.Exceptions;
using API.Persistance.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IApplicationRepository
    {
        Application Get(string name);
    }
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly MessagesStoreContext _messagesStoreContext;

        public ApplicationRepository(MessagesStoreContext messagesStoreContext)
        {
            _messagesStoreContext = messagesStoreContext;
        }

        public Application Get(string name)
        {
            try
            {
                return _messagesStoreContext
                    .Application
                    .First(x => x.Name == name);
            }
            catch(InvalidOperationException e)
            {
                throw new NotFoundException($"Application {name} not found", e);
            }
        }
    }
}
