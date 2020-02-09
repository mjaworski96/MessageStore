using API.Dto;
using API.Persistance.Entity;
using API.Persistance.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IMessageService
    {
        Task<MessageDtoWithId> Create(MessageDto messageDto);
    }
    public class MessageService : IMessageService
    {
        private IMessageRepository _messageRepository;
        private IContactRepository _contactRepository;
        private IWriterTypeRepository _writerTypeRepository;
        private IAttachmentService _attachmentService;

        public MessageService(IMessageRepository messageRepository, IContactRepository contactRepository, IWriterTypeRepository writerTypeRepository, IAttachmentService attachmentService)
        {
            _messageRepository = messageRepository;
            _contactRepository = contactRepository;
            _writerTypeRepository = writerTypeRepository;
            _attachmentService = attachmentService;
        }

        public async Task<MessageDtoWithId> Create(MessageDto messageDto)
        {
            var message = new Messages
            {
                Contact = await _contactRepository.Get(messageDto.ContactId),
                Content = messageDto.Content,
                Date = messageDto.Date,
                WriterType = await _writerTypeRepository.Get(messageDto.WriterType),
                Attachments = (messageDto.Attachments ?? new List<AttachmentDto>())
                    .Select(x => _attachmentService.CreateAttachment(x)).ToList()
            };

            await _messageRepository.Add(message);
            await _messageRepository.Save();

            return new MessageDtoWithId
            {
                Id = message.Id,
                Content = message.Content,
                Date = message.Date,
                WriterType = message.WriterType.Name,
                ContactId = message.Contact.Id,
                Attachments = message.Attachments.Select(x => _attachmentService.Map(x)).ToList()
            };
        }
    }
}
