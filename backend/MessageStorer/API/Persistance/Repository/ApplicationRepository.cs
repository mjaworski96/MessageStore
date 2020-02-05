﻿using API.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IApplicationRepository
    {
        Task<Application> Get(string name);
    }
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly MessagesStoreContext _messagesStoreContext;
        private readonly IMemoryCache _cache;

        public ApplicationRepository(MessagesStoreContext messagesStoreContext, IMemoryCache cache)
        {
            _messagesStoreContext = messagesStoreContext;
            _cache = cache;
        }

        public async Task<Application> Get(string name)
        {
            try
            {
                return await _cache.GetOrCreateAsync($"Application_{name}", x =>
                {
                    return _messagesStoreContext
                    .Application
                    .FirstAsync(x => x.Name == name);
                });
            }
            catch(InvalidOperationException e)
            {
                throw new NotFoundException($"Application {name} not found", e);
            }
        }
    }
}
