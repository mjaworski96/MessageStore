using API.Dto;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IAliasRepository
    {
        Task<List<Aliases>> GetAll(string appUser, string app, bool internalOnly);
        Task<AppUsers> GetOwner(int aliasId);
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
    }
}
