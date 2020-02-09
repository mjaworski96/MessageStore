using API.Dto;
using API.Persistance.Entity;

namespace API.Service
{
    public interface IAttachmentService
    {
        Attachments CreateAttachment(AttachmentDto attachmentDto);
        AttachmentDtoWithId Map(Attachments attachment);
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

        public AttachmentDtoWithId Map(Attachments attachment)
        {
            return new AttachmentDtoWithId
            {
                Id = attachment.Id,
                Content = attachment.Content,
                ContentType = attachment.ContentType
            };
        }
    }
}
