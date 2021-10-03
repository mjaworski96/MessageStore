using API.Persistance.Entity;
using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IAttachmentRepository
    {
        Task<Attachments> Get(int id);
        Task<List<string>> GetAllAttachmentsFilenamesForUser(int userId);
    }
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly MessagesStoreContext _messagesStoreContext;

        public AttachmentRepository(MessagesStoreContext messagesStoreContext)
        {
            _messagesStoreContext = messagesStoreContext;
        }

        public async Task<Attachments> Get(int id)
        {
            try
            {
                return await _messagesStoreContext.Attachments
                    .Include(x => x.Message)
                    .ThenInclude(x => x.Contact)
                    .FirstAsync(x => x.Id == id);
            }
            catch (InvalidOperationException e)
            {
                throw new AttachmentNotFoundException(id, e);
            }
        }

        public async Task<List<string>> GetAllAttachmentsFilenamesForUser(int userId)
        {
            return await _messagesStoreContext.Attachments
                .Where(x => x.Message.Contact.AppUserId == userId)
                .Select(x => x.Filename)
                .ToListAsync();
        }
    }
}
