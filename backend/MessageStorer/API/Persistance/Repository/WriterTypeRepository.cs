using API.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
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
        private readonly IMemoryCache _cache;

        public WriterTypeRepository(MessagesStoreContext messagesStoreContext, IMemoryCache cache)
        {
            _messagesStoreContext = messagesStoreContext;
            _cache = cache;
        }

        public Task<WriterTypes> Get(string name)
        {
            try
            {
                return _cache.GetOrCreateAsync($"WriterType_{name}", x =>
                {
                    return _messagesStoreContext
                    .WriterTypes
                    .FirstAsync(x => x.Name == name);
                });
            }
            catch(InvalidOperationException e)
            {
                throw new NotFoundException($"Writer type {name} not found.", e);
            }
        }
    }
 }
