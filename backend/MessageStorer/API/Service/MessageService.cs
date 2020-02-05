using API.Dto;
using API.Persistance.Entity;
using API.Persistance.Repository;
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

        public MessageService(IMessageRepository messageRepository, IContactRepository contactRepository, IWriterTypeRepository writerTypeRepository)
        {
            _messageRepository = messageRepository;
            _contactRepository = contactRepository;
            _writerTypeRepository = writerTypeRepository;
        }

        public async Task<MessageDtoWithId> Create(MessageDto messageDto)
        {
            var message = new Message
            {
                Attachment = messageDto.Attachment,
                Contact = await _contactRepository.Get(messageDto.ContactId),
                Content = messageDto.Content,
                Date = messageDto.Date,
                WriterType = await _writerTypeRepository.Get(messageDto.WriterType)
            };

            await _messageRepository.Add(message);
            await _messageRepository.Save();

            return new MessageDtoWithId
            {
                Id = message.Id,
                Content = message.Content,
                Attachment = message.Attachment,
                Date = message.Date,
                WriterType = message.WriterType.Name,
                ContactId = message.Contact.Id
            };
        }
    }
}
