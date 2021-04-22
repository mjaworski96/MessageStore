using API.Persistance.Entity;
using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IAttachmentRepository
    {
        Task<Attachments> Get(int id);
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
                throw new NotFoundException($"Attachment with id {id} not found.", e);
            }
        }
    }
}
