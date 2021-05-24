using Common.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IWriterTypeRepository
    {
        Task<WriterTypes> Get(string name);
    }
    public class WriterTypeRepository : IWriterTypeRepository
    {
        private readonly MessagesStoreContext _messagesStoreContext;

        public WriterTypeRepository(MessagesStoreContext messagesStoreContext)
        {
            _messagesStoreContext = messagesStoreContext;
        }

        public async Task<WriterTypes> Get(string name)
        {
            try
            {
                return await _messagesStoreContext
                .WriterTypes
                .FirstAsync(x => x.Name == name);
            }
            catch(InvalidOperationException e)
            {
                throw new WriterTypeNotFoundException(name, e);
            }
        }
    }
 }
