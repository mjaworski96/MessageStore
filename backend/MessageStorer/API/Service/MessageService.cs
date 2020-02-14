using API.Dto;
using API.Persistance.Entity;
using API.Persistance.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IMessageService
    {
        Task<MessageDtoWithId> Create(MessageDto messageDto);
        Task<MessageDtoWithDetailsList> GetPage(int aliasId, int pageNumber, int pageSize);
        Task<SearchResultDtoList> Find(SearchQueryDto query);
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

        public async Task<SearchResultDtoList> Find(SearchQueryDto query)
        {
            var rawMessages = await _messageRepository.Find(query.Query, query.AliasesIds, query.IgnoreLetterSize);

            Func<Messages, int> aliasIdSelector = message => 
                message.Contact.AliasesMembers.FirstOrDefault(y => y.Alias.Internal).AliasId;

            var list = rawMessages
                .Select(x => new SearchResultDto
                {
                    MessageId = x.Id,
                    Content = x.Content,
                    Date = x.Date.Value,
                    Application = x.Contact.Application.Name,
                    ContactName = x.Contact.Name,
                    WriterType = x.WriterType.Name,
                    AliasId = aliasIdSelector(x),
                    AllAliases = x.Contact.AliasesMembers.Select(y => new SearchAlias
                    {
                        Id = y.Alias.Id,
                        Name = y.Alias.Name,
                        MessageIndexOf = _messageRepository.GetRowNumber(x.Id, y.Alias.Id)
                    }).ToList(),
                    MessageIndexOf = _messageRepository.GetRowNumber(x.Id, aliasIdSelector(x))
                }).ToList();

            return new SearchResultDtoList
            {
                Results = list
            };
        }

        public async Task<MessageDtoWithDetailsList> GetPage(int aliasId, int pageNumber, int pageSize)
        {
            var raw = await _messageRepository.GetPage(aliasId, pageNumber, pageSize);

            var list = raw.Select(x => new MessageDtoWithDetails
            {
                Id = x.Id,
                Attachments = x.Attachments.Select(y => _attachmentService.Map(y)).ToList(),
                ContactName = x.Contact.Name,
                Application = x.Contact.Application.Name,
                Content = x.Content,
                Date = x.Date,
                WriterType = x.WriterType.Name,
            }).ToList();

            return new MessageDtoWithDetailsList
            {
                Messages = list
            };
        }
    }
}
