using API.Dto;
using API.Exceptions;
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
        Task<List<Aliases>> GetAll(string appUser, string app, bool internalOnly);
        Task<AppUsers> GetOwner(int aliasId);
        Task<List<Aliases>> GetAll(IEnumerable<int> enumerable);
        Task<Aliases> Add(Aliases aliases);
        Task Save();
        Task<Aliases> Get(int id);
        Task RemoveIfInternal(int id);
    }

    public class AliasRepository: IAliasRepository
    {
        private readonly MessagesStoreContext _messagesStoreContext;

        public AliasRepository(MessagesStoreContext messagesStoreContext)
        {
            _messagesStoreContext = messagesStoreContext;
        }

        public Task<List<Aliases>> GetAll(string appUser, string app, bool internalOnly)
        {
            var query = _messagesStoreContext
                .Aliases
                .Include(x => x.AliasesMembers)
                .ThenInclude(x => x.Contact)
                .ThenInclude(x => x.Application)
                .Where(x => x.AliasesMembers
                    .Select(y => y.Contact.AppUser.Username)
                        .All(z => z == appUser));

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

        public Task<Aliases> Get(int id)
        {
            try
            {
                return _messagesStoreContext
                .Aliases
                .Include(x => x.AliasesMembers)
                .ThenInclude(x => x.Contact)
                .ThenInclude(x => x.Application)
                .FirstAsync(x => x.Id == id);
            }
            catch (InvalidOperationException e)
            {
                throw new NotFoundException($"Alias with id {id} not found.", e);
            }
        }

        public async Task RemoveIfInternal(int id)
        {
            var aliasToDelete = await _messagesStoreContext.Aliases
                    .FirstOrDefaultAsync(x => x.Id == id);
            if (aliasToDelete != null)
            {
                if (aliasToDelete.Internal)
                {
                    throw new BadRequestException("Raw aliases can't be deleted");
                }
                _messagesStoreContext.Remove(aliasToDelete);
                await _messagesStoreContext.SaveChangesAsync();
            }
        }
    }
}
