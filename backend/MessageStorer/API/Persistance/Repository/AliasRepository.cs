using API.Dto;
using Common.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IAliasRepository
    {
        Task<List<Aliases>> GetAll(int userId, string app, bool internalOnly);
        Task<AppUsers> GetOwner(int aliasId);
        Task<List<Aliases>> GetAll(IEnumerable<int> enumerable);
        Task<Aliases> Add(Aliases aliases);
        Task Save();
        Task<Aliases> Get(int id, bool throwExeptionIfNotFound);
        Task Remove(Aliases alias);
    }

    public class AliasRepository: IAliasRepository
    {
        private readonly MessagesStoreContext _messagesStoreContext;

        public AliasRepository(MessagesStoreContext messagesStoreContext)
        {
            _messagesStoreContext = messagesStoreContext;
        }

        public Task<List<Aliases>> GetAll(int userId, string app, bool internalOnly)
        {
            var query = _messagesStoreContext
                .Aliases
                .Include(x => x.AliasesMembers)
                     .ThenInclude(x => x.Contact)
                        .ThenInclude(x => x.ContactsMembers)
                .Include(x => x.AliasesMembers)
                    .ThenInclude(x => x.Contact)
                        .ThenInclude(x => x.Application)
                .Where(x => x.AliasesMembers
                    .Select(y => y.Contact.AppUserId)
                        .All(z => z == userId));

            if(!string.IsNullOrEmpty(app))
            {
                query = query
                    .Where(x => x.AliasesMembers
                        .Select(y => y.Contact.Application.Name)
                            .All(z => z == app));
            }
            if(internalOnly)
            {
                query = query
                    .Where(x => x.AliasesMembers
                        .Select(y => y.Alias.Internal)
                            .All(z => z == true))
                    .Where(x => x.AliasesMembers.Count == 1);
            }
            
            return query.ToListAsync();
        }

        public Task<AppUsers> GetOwner(int aliasId)
        {
            return _messagesStoreContext
                .Aliases
                .Where(x => x.Id == aliasId)
                .SelectMany(x => x.AliasesMembers)
                .Select(x => x.Contact)
                .Select(x => x.AppUser)
                .FirstOrDefaultAsync();
        }

        public Task<List<Aliases>> GetAll(IEnumerable<int> ids)
        {
            return _messagesStoreContext
                .Aliases
                .Include(x => x.AliasesMembers)
                    .ThenInclude(x => x.Contact)
                        .ThenInclude(x => x.Application)
                .Include(x => x.AliasesMembers)
                    .ThenInclude(x => x.Contact)
                        .ThenInclude(x => x.AppUser)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<Aliases> Add(Aliases aliases)
        {
            var result = await _messagesStoreContext.Aliases.AddAsync(aliases);
            await Save();
            return result.Entity;
        }

        public async Task Save()
        {
            await _messagesStoreContext.SaveChangesAsync();
        }

        public async Task<Aliases> Get(int id, bool throwExeptionIfNotFound)
        {
            try
            {
                var query = _messagesStoreContext
                .Aliases
                .Include(x => x.AliasesMembers)
                     .ThenInclude(x => x.Contact)
                        .ThenInclude(x => x.ContactsMembers)
                .Include(x => x.AliasesMembers)
                    .ThenInclude(x => x.Contact)
                        .ThenInclude(x => x.Application)
                .Include(x => x.AliasesMembers)
                    .ThenInclude(x => x.Contact)
                        .ThenInclude(x => x.AppUser);

                if (throwExeptionIfNotFound)
                    return await query.FirstAsync(x => x.Id == id);
                else
                    return await query.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (InvalidOperationException e)
            {
                throw new NotFoundException($"Alias with id {id} not found.", e);
            }
        }

        public async Task Remove(Aliases alias)
        {
            _messagesStoreContext.Remove(alias);
            await _messagesStoreContext.SaveChangesAsync();
        }
    }
}
