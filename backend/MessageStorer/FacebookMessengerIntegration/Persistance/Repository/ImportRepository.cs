using Common.Exceptions;
using FacebookMessengerIntegration.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FacebookMessengerIntegration.Persistance.Repository
{
    public interface IImportRepository
    {
        Task Add(Imports importEntity);
        Task Save();
        Task<Imports> Get(string importId);
    }
    public class ImportRepository : IImportRepository
    {
        private readonly MessengerIntegrationContext _messengerIntegrationContext;

        public ImportRepository(MessengerIntegrationContext messengerIntegrationContext)
        {
            _messengerIntegrationContext = messengerIntegrationContext;
        }

        public async Task Add(Imports importEntity)
        {
            await _messengerIntegrationContext.AddAsync(importEntity);
        }

        public async Task<Imports> Get(string importId)
        {
            try
            {
                return await _messengerIntegrationContext.Imports
                    .Include(x => x.Status)
                    .FirstAsync(x => x.Id == importId);
            }
            catch (InvalidOperationException e)
            {
                throw new NotFoundException($"Import with id: {importId} not found", e);
            }
        }

        public async Task Save()
        {
            await _messengerIntegrationContext.SaveChangesAsync();
        }
    }
}
