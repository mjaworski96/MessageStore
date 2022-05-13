using Common.Exceptions;
using MessengerIntegration.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerIntegration.Persistance.Repository
{
    public interface IImportRepository
    {
        Task Add(Imports importEntity);
        Task Save();
        Task<Imports> Get(string importId, bool throwExeptionIfNotFound);
        Task<List<Imports>> GetAll(int userId);
        Task<List<Imports>> GetQueued(int parallelImportsCount);
        Task Delete(Imports import);
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

        public async Task Delete(Imports import)
        {
            _messengerIntegrationContext.Remove(import);
            await _messengerIntegrationContext.SaveChangesAsync();
        }

        public async Task<Imports> Get(string importId, bool throwExeptionIfNotFound)
        {
            try
            {
                var query = _messengerIntegrationContext.Imports
                        .Include(x => x.Status);
                if (throwExeptionIfNotFound)
                {
                    return await query.FirstAsync(x => x.Id == importId);
                }
                else
                {
                    return await query.FirstOrDefaultAsync(x => x.Id == importId);
                }
                
            }
            catch (InvalidOperationException e)
            {
                throw new ImportNotFoundException(importId, e);
            }
        }

        public async Task<List<Imports>> GetAll(int userId)
        {
            return await _messengerIntegrationContext
                .Imports
                .Include(x => x.Status)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Imports>> GetQueued(int parallelImportsCount)
        {
            var imports = await _messengerIntegrationContext
               .Imports
               .Include(x => x.Status)
               .Where(x => x.Status.Name == Statuses.Queued &&
                    !_messengerIntegrationContext.Imports
                        .Where(y => y.UserId == x.UserId &&
                            y.Status.Name == Statuses.Processing).Any())
               .OrderBy(x => x.CreatedAt)
               .Take(parallelImportsCount)
               .ToListAsync();

            return imports.GroupBy(x => x.UserId)
               .Select(x => x.First())
               .ToList();
        }

        public async Task Save()
        {
            await _messengerIntegrationContext.SaveChangesAsync();
        }
    }
}
