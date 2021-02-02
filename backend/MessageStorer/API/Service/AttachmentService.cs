using API.Dto;
using API.Persistance.Entity;

namespace API.Service
{
    public interface IAttachmentService
    {
        Attachments CreateAttachment(AttachmentDto attachmentDto);
    }
    public class AttachmentService : IAttachmentService
    {
        public Attachments CreateAttachment(AttachmentDto attachmentDto)
        {
            return new Attachments
            {
                Content = attachmentDto.Content,
                ContentType = attachmentDto.ContentType
            };
        }
    }
}
