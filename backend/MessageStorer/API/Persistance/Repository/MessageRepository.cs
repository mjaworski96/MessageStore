using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IMessageRepository
    {
        Task Add(Messages message);
        Task Save();
        Task<Messages> GetNewest(int userId, string application, int? contactId);
        Task<Messages> GetOldest(int userId, string application, int? contactId);
        Task<Messages> GetOldest(int importId);
        Task<Messages> GetNewest(int importId);
        Task<string> GetApplication(int importId);
        Task<List<Messages>> GetPage(int aliasId, int pageNumber, int pageSize);
        Task<List<Messages>> Find(int userId, string searchFor, List<int> aliasesIds, bool ignoreLetterSize,
            DateTime? dateFrom, DateTime? dateTo, bool hasAttachment);
        Task<long> GetRowNumber(int messageId, int aliasId);
        Task<List<string>> GetFilenamesToRemove(int importId);
        Task RemoveMessagesWithImportId(int importId);
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

        public Task<List<Messages>> Find(int userId, string searchFor, List<int> aliasesIds, bool ignoreLetterSize,
            DateTime? dateFrom, DateTime? dateTo, bool hasAttachment)
        {
            var query = _messageStoreContext
                .Messages
                .Include(x => x.WriterType)
                .Include(x => x.Contact)
                    .ThenInclude(x => x.Application)
                .Include(x => x.Contact)
                    .ThenInclude(x => x.AliasesMembers)
                        .ThenInclude(x => x.Alias)
                .Include(x => x.Attachments)
                .Include(x => x.Contact)
                    .ThenInclude(x => x.AppUser)
                .Include(x => x.ContactMember)
                
                .Where(x => x.Contact.AppUserId == userId);

            if (dateFrom != null)
            {
                query = query.Where(x => x.Date >= dateFrom);
            }
            if (dateTo != null)
            {
                query = query.Where(x => x.Date <= dateTo);
            }

            if (aliasesIds?.Any() ?? false)
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

            if (hasAttachment)
            {
                query = query.Where(x => x.Attachments.Any());
            }

            return query.ToListAsync();
        }

        public Task<Messages> GetNewest(int userId, string application, int? contactId)
        {
            return GetUserApplicationContactQuery(userId, application, contactId)
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync();
        }

        public Task<Messages> GetOldest(int userId, string application, int? contactId)
        {
            return GetUserApplicationContactQuery(userId, application, contactId)
                .OrderBy(x =>x.Date)
                .FirstOrDefaultAsync();
        }

        private IQueryable<Messages> GetUserApplicationContactQuery(int userId, string application, int? contactId)
        {
            var query = _messageStoreContext
                .Messages
                .Where(x =>
                    x.Contact.AppUserId == userId
                    && x.Contact.Application.Name == application);

            if (contactId.HasValue)
            {
                query = query.Where(x => x.ContactId == contactId);
            }

            return query;
        }

        public Task<List<Messages>> GetPage(int aliasId, int pageNumber, int pageSize)
        {
            return _messageStoreContext
                .Messages
                .Include(x => x.WriterType)
                .Include(x => x.Contact)
                    .ThenInclude(x => x.Application)
                .Include(x => x.Attachments)
                .Include(x => x.ContactMember)
                .Where(x => x.Contact.AliasesMembers
                    .Select(y => y.Alias.Id)
                        .Any(z => z == aliasId))
                .OrderByDescending(x => x.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<long> GetRowNumber(int messageId, int aliasId)
        {
            var queryResult = await _messageStoreContext.GetRowNumber(messageId, aliasId);
            return queryResult?.RowNum ?? -1L;
        }

        public async Task Save()
        {
            await _messageStoreContext.SaveChangesAsync();
        }

        public Task<Messages> GetOldest(int importId)
        {
            return _messageStoreContext.Messages
                .Where(x => x.ImportId == importId)
                .OrderBy(x => x.Date)
                .FirstOrDefaultAsync();
        }

        public Task<Messages> GetNewest(int importId)
        {
            return _messageStoreContext.Messages
                .Where(x => x.ImportId == importId)
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync();
        }
        public async Task<string> GetApplication(int importId)
        {
            return await _messageStoreContext.Messages
                .Where(x => x.ImportId == importId)
                .Select(x => x.Contact.Application.Name)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveMessagesWithImportId(int importId)
        {
            await _messageStoreContext.RemoveMessagesWithImportId(importId);
        }

        public async Task<List<string>> GetFilenamesToRemove(int importId)
        {
            return await _messageStoreContext.Attachments
                .Where(x => x.Message.ImportId == importId)
                .Select(x => x.Filename)
                .ToListAsync();
        }
    }
}
