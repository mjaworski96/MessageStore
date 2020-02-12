﻿using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IMessageRepository
    {
        Task Add(Messages message);
        Task Save();
        Task<Messages> GetNewest(string appUser, string application);
        Task<List<Messages>> GetPage(int aliasId, int pageNumber, int pageSize);
        Task<List<Messages>> Find(string searchFor, List<int> aliasesIds, bool ignoreLetterSize);
        long GetRowNumber(int messageId, int aliasId);
    }
    public class MessageRepository: IMessageRepository
    {
        private readonly MessagesStoreContext _messageStoreContext;

        public MessageRepository(MessagesStoreContext messageStoreContext)
        {
            _messageStoreContext = messageStoreContext;
        }
        public async Task Add(Messages message)
        {
            await _messageStoreContext.AddAsync(message);
        }

        public Task<List<Messages>> Find(string searchFor, List<int> aliasesIds, bool ignoreLetterSize)
        {
            var query = _messageStoreContext
                .Messages
                .Include(x => x.WriterType)
                .Include(x => x.Contact)
                .ThenInclude(x => x.Application)
                .Include(x => x.Contact)
                .ThenInclude(x => x.AliasesMembers)
                .ThenInclude(x => x.Alias)
                .AsQueryable();

            if(aliasesIds?.Any() ?? false)
            {
                query = query
                    .Where(x => x.Contact.AliasesMembers
                        .Select(y => y.AliasId)
                            .Any(z => aliasesIds.Contains(z)));
            }
            if(ignoreLetterSize)
                query = query.Where(x => x.Content.ToLower().Contains(searchFor.ToLower()));
            else
                query = query.Where(x => x.Content.ToLower().Contains(searchFor));

            return query.ToListAsync();
        }

        public Task<Messages> GetNewest(string appUser, string application)
        {
            return _messageStoreContext
                .Messages
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x =>
                    x.Contact.AppUser.Username == appUser
                    && x.Contact.Application.Name == application);
        }

        public Task<List<Messages>> GetPage(int aliasId, int pageNumber, int pageSize)
        {
            return _messageStoreContext
                .Messages
                .Include(x => x.WriterType)
                .Include(x => x.Contact)
                .ThenInclude(x => x.Application)
                .Where(x => x.Contact.AliasesMembers
                    .Select(y => y.Alias.Id)
                        .Any(z => z == aliasId))
                .OrderByDescending(x => x.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public long GetRowNumber(int messageId, int aliasId)
        {
            var queryResult = _messageStoreContext.GetRowNumber(messageId, aliasId);
            return queryResult?.RowNum ?? -1L;
        }

        public async Task Save()
        {
            await _messageStoreContext.SaveChangesAsync();
        }
    }
}
