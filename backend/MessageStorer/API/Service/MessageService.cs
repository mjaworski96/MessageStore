using API.Dto;
using API.Infrastructure;
using API.Persistance.Entity;
using API.Persistance.Repository;
using Common.Exceptions;
using Common.Service;
using System;
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
        Task DeleteForImport(string importId);
    }
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IWriterTypeRepository _writerTypeRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IImportRepository _importRepository;
        private readonly ISecurityService _securityService;
        private readonly IHttpMetadataService _httpMetadataService;
        private readonly IMessengerIntegrationClient _messengerIntegrationClient;

        public MessageService(IMessageRepository messageRepository, IContactRepository contactRepository, IWriterTypeRepository writerTypeRepository, IAttachmentService attachmentService, IImportRepository importRepository, ISecurityService securityService, IHttpMetadataService httpMetadataService, IMessengerIntegrationClient messengerIntegrationClient)
        {
            _messageRepository = messageRepository;
            _contactRepository = contactRepository;
            _writerTypeRepository = writerTypeRepository;
            _attachmentService = attachmentService;
            _importRepository = importRepository;
            _securityService = securityService;
            _httpMetadataService = httpMetadataService;
            _messengerIntegrationClient = messengerIntegrationClient;
        }

        public async Task<MessageDtoWithId> Create(MessageDto messageDto)
        {
            var contact = await _contactRepository.Get(messageDto.ContactId);
            Validate(messageDto, contact);
           
            var import = await _importRepository.GetOrCreate(messageDto.ImportId, _httpMetadataService.UserId, contact.Application);
            _securityService.CheckIfUserIsOwnerOfImport(import.AppUserId);

            var message = new Messages
            {
                Contact = contact,
                Content = messageDto.Content,
                Date = messageDto.Date,
                WriterType = await _writerTypeRepository.Get(messageDto.WriterType),
                Attachments = await _attachmentService.CreateAttachments(messageDto.Attachments),
                Import = import,
                HasError = messageDto.HasError
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
                Attachments = message.Attachments.Select(x => _attachmentService.CreateAttachemtDtoWithId(x)).ToList(),
                Application = message.Contact.Application.Name,
                ContactName = GetContactName(message),
                HasError = message.HasError
            };
        }

        public async Task<SearchResultDtoList> Find(SearchQueryDto query)
        {
            var rawMessages = await _messageRepository.Find(_httpMetadataService.UserId,
                query.Query, query.AliasesIds, query.IgnoreLetterSize,
                query.From, query.To, query.HasAttachments);

            Func<Messages, int> aliasIdSelector = message =>
                message.Contact.AliasesMembers.FirstOrDefault(y => y.Alias.Internal).AliasId;

            var list = rawMessages
                .Select(x => new SearchResultDto
                {
                    MessageId = x.Id,
                    Content = x.Content,
                    Attachments = x.Attachments.Select(y => _attachmentService.CreateAttachemtDtoWithId(y)).ToList(),
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
                    HasError = x.HasError
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
            if (messageDto.Content?.Length > 307200)
            {
                throw new TooLongMessageContentException();
            }
            if (messageDto.ContactMemberId.HasValue)
            {
                if (messageDto.WriterType == "app_user")
                {
                    throw new AppUserContactMemberException();
                }
                if (!contact.ContactsMembers.Where(x => x.Id == messageDto.ContactMemberId.Value).Any())
                {
                    throw new MissingContactMemberException();
                }
            }
        }

        public async Task DeleteForImport(string importId)
        {
            var import = await _importRepository.Get(importId);
            if (import != null)
            {
                import.IsBeingDeleted = true;
                await _importRepository.Commit();

                var owner = await _importRepository.GetOwnerId(importId);
                _securityService.CheckIfUserIsOwnerOfImport(owner);
                var attachments = await _messageRepository.GetFilenamesToRemove(import.Id);

                foreach (var item in attachments)
                {
                    _attachmentService.Remove(item);
                }
                var application = await _messageRepository.GetApplication(import.Id);
                if (application == "messenger")
                {
                    await _messengerIntegrationClient.DeleteImport(importId);
                }
                await _messageRepository.RemoveMessagesWithImportId(import.Id);
                await _contactRepository.RemoveEmpty(_httpMetadataService.UserId);
                await _importRepository.Remove(import);
            }
        }
    }
}
