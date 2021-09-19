using API.Persistance.Entity;
using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IImportRepository
    {
        Task<Imports> GetOrCreate(string importId);
        Task<Imports> Get(string importId);
        Task<int?> GetOwnerId(string importId);
        Task<List<Imports>> GetAllForUser(int userId);
        Task Remove(Imports import);
        Task Commit();
    }
    public class ImportRepository : IImportRepository
    {
        private readonly MessagesStoreContext _messageStoreContext;

        public ImportRepository(MessagesStoreContext messageStoreContext)
        {
            _messageStoreContext = messageStoreContext;
        }

        public async Task<List<Imports>> GetAllForUser(int userId)
        {
            return await _messageStoreContext.Imports
                .Where(x => x.Messages
                    .Any(y => y.Contact.AppUserId == userId))
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<Imports> GetOrCreate(string importId)
        {
            var existing = await _messageStoreContext.Imports
                .SingleOrDefaultAsync(x => x.ImportId == importId);
            if (existing != null)
            {
                if (existing.IsBeingDeleted)
                {
                    throw new ImportIsBeingDeletedException();
                }
                return existing;
            }
            var newImport = new Imports
            {
                ImportId = importId,
                CreatedAt = DateTime.UtcNow
            };
            _messageStoreContext.Imports.Add(newImport);
            await _messageStoreContext.SaveChangesAsync();
            return newImport;
        }
        public async Task<Imports> Get(string importId)
        {
            var import = await _messageStoreContext.Imports
                .SingleOrDefaultAsync(x => x.ImportId == importId);

            if (import != null && import.IsBeingDeleted)
            {
                throw new ImportIsBeingDeletedException();
            }

            return import;
        }

        public async Task Remove(Imports import)
        {
            _messageStoreContext.Remove(import);
            await _messageStoreContext.SaveChangesAsync();
        }

        public async Task<int?> GetOwnerId(string importId)
        {
            var query = _messageStoreContext
                .Messages
                .Where(x => x.Import.ImportId == importId)
                .Select(x => x.Contact.AppUserId);
            var result = await query.FirstOrDefaultAsync();
            if (result == 0)
            {
                return null;
            }
            return result;
        }

        public async Task Commit()
        {
             await _messageStoreContext.SaveChangesAsync();
        }
    }
}
