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
                            .All(z => z == true));
            }
            
            return query.ToListAsync();
        }
    }
}
