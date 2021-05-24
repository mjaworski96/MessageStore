using Common.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IApplicationRepository
    {
        Task<Applications> Get(string name);
    }
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly MessagesStoreContext _messagesStoreContext;

        public ApplicationRepository(MessagesStoreContext messagesStoreContext)
        {
            _messagesStoreContext = messagesStoreContext;
        }

        public async Task<Applications> Get(string name)
        {
            try
            {
                return await _messagesStoreContext
                .Applications
                .FirstAsync(x => x.Name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ApplicationNotFoundException(name, e);
            }
        }
    }
}
