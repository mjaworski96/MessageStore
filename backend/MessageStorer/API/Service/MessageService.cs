using API.Dto;
using API.Persistance.Entity;
using API.Persistance.Repository;
using Common.Exceptions;
using Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IMessageService
    {
        Task<MessageDtoWithId> Create(MessageDto messageDto);
        Task<MessageDtoWithIdList> GetPage(int aliasId, int pageNumber, int pageSize);
        Task<SearchResultDtoList> Find(SearchQueryDto query);
        Task<MessageInAliasOrderDto> GetOrder(int messageId, int aliasId);
    }
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IWriterTypeRepository _writerTypeRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly ISecurityService _securityService;
        private readonly IHttpMetadataService _httpMetadataService;

        public MessageService(IMessageRepository messageRepository, IContactRepository contactRepository, IWriterTypeRepository writerTypeRepository, IAttachmentService attachmentService, ISecurityService securityService, IHttpMetadataService httpMetadataService)
        {
            _messageRepository = messageRepository;
            _contactRepository = contactRepository;
            _writerTypeRepository = writerTypeRepository;
            _attachmentService = attachmentService;
            _securityService = securityService;
            _httpMetadataService = httpMetadataService;
        }

        public async Task<MessageDtoWithId> Create(MessageDto messageDto)
        {
            var contact = await _contactRepository.Get(messageDto.ContactId);
            Validate(messageDto, contact);

            var message = new Messages
            {
                Contact = await _contactRepository.Get(messageDto.ContactId),
                Content = messageDto.Content,
                Date = messageDto.Date,
                WriterType = await _writerTypeRepository.Get(messageDto.WriterType),
                Attachments = (messageDto.Attachments ?? new List<AttachmentDto>())
                    .Select(x => _attachmentService.CreateAttachment(x)).ToList()
            };

            if (messageDto.ContactMemberId.HasValue)
            {
                message.ContactMember = contact.ContactsMembers.First(x => x.Id == messageDto.ContactMemberId.Value);
            }

            await _messageRepository.Add(message);
            await _messageRepository.Save();

            return GetMessageDtoWithId(message);
        }

        private MessageDtoWithId GetMessageDtoWithId(Messages message)
        {
            return new MessageDtoWithId
            {
                Id = message.Id,
                Content = message.Content,
                Date = message.Date,
                WriterType = message.WriterType.Name,
                ContactId = message.Contact.Id,
                ContactMemberId = message.ContactMemberId,
                Attachments = message.Attachments.Select(x => CreateAttachemtDtoWithId(x)).ToList(),
                Application = message.Contact.Application.Name,
                ContactName = GetContactName(message)
            };
        }

        public async Task<SearchResultDtoList> Find(SearchQueryDto query)
        {
            var rawMessages = await _messageRepository.Find(_httpMetadataService.UserId,
                query.Query, query.AliasesIds, query.IgnoreLetterSize);

            Func<Messages, int> aliasIdSelector = message =>
                message.Contact.AliasesMembers.FirstOrDefault(y => y.Alias.Internal).AliasId;

            var list = rawMessages
                .Select(x => new SearchResultDto
                {
                    MessageId = x.Id,
                    Content = x.Content,
                    Attachments = x.Attachments.Select(y => CreateAttachemtDtoWithId(y)).ToList(),
                    Date = x.Date.Value,
                    Application = x.Contact.Application.Name,
                    ContactName = GetContactName(x),
                    WriterType = x.WriterType.Name,
                    AliasId = aliasIdSelector(x),
                    AllAliases = x.Contact.AliasesMembers.Select(y => new SearchAlias
                    {
                        Id = y.Alias.Id,
                        Name = y.Alias.Name,
                    }).ToList(),
                }).ToList();

            return new SearchResultDtoList
            {
                Results = list
            };
        }

        public async Task<MessageInAliasOrderDto> GetOrder(int messageId, int aliasId)
        {
            await _securityService.CheckIfUserIsOwnerOfAlias(aliasId);
            return new MessageInAliasOrderDto
            {
                Order = await _messageRepository.GetRowNumber(messageId, aliasId)
            };
        }

        public async Task<MessageDtoWithIdList> GetPage(int aliasId, int pageNumber, int pageSize)
        {
            await _securityService.CheckIfUserIsOwnerOfAlias(aliasId);
            var raw = await _messageRepository.GetPage(aliasId, pageNumber, pageSize);

            var list = raw.Select(x => GetMessageDtoWithId(x)).ToList();

            return new MessageDtoWithIdList
            {
                Messages = list
            };
        }

        private AttachmentDtoWithId CreateAttachemtDtoWithId(Attachments attachment)
        {
            return new AttachmentDtoWithId
            {
                Id = attachment.Id,
                Content = attachment.Content,
                ContentType = attachment.ContentType
            };
        }
        private string GetContactName(Messages message)
        {
            if (message.ContactMember != null)
            {
                return message.ContactMember.Name;
            }
            return message.Contact.Name;
        }
        private void Validate(MessageDto messageDto, Contacts contact)
        {
            _securityService.CheckIfUserIsOwnerOfContact(contact);
            if (messageDto.Content.Length > 307200)
            {
                throw new BadRequestException("Message content can contains maximum of 307200 characters");
            }
            if (messageDto.ContactMemberId.HasValue)
            {
                if (messageDto.WriterType == "app_user")
                {
                    throw new BadRequestException("Message from app user can not be also from a contact member");
                }
                if (!contact.ContactsMembers.Where(x => x.Id == messageDto.ContactMemberId.Value).Any())
                {
                    throw new BadRequestException("Contact member is not a member of given contact");
                }
            }
        }
    }
}
