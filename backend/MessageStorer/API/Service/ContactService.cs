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
    public interface IContactService
    {
        Task<ContactDtoWithId> AddIfNotExists(ContactDto contactDto);
    }
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IHttpMetadataService _httpMetadataService;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly ISecurityService _securityService;

        public ContactService(IContactRepository contactRepository,
            IHttpMetadataService httpMetadataService,
            IApplicationRepository applicationRepository,
            IAppUserRepository appUserRepository,
            ISecurityService securityService)
        {
            _contactRepository = contactRepository;
            _httpMetadataService = httpMetadataService;
            _applicationRepository = applicationRepository;
            _appUserRepository = appUserRepository;
            _securityService = securityService;
        }

        public async Task<ContactDtoWithId> AddIfNotExists(ContactDto contactDto)
        {
            Validate(contactDto);
            Contacts entity = await _contactRepository.Get(
                _httpMetadataService.Application,
                contactDto.InApplicationId,
                _httpMetadataService.UserId);
            if (entity == null)
            {
                entity = new Contacts
                {
                    AppUser = await _appUserRepository.Get(_httpMetadataService.UserId, true),
                    Application = await _applicationRepository.Get(_httpMetadataService.Application),
                };
                entity.AliasesMembers.Add(new AliasesMembers
                {
                    Alias = new Aliases
                    {
                        Internal = true
                    }
                });
            }
            else
            {
                _securityService.CheckIfUserIsOwnerOfContact(entity);
            }
            entity.Name = contactDto.Name;
            entity.InApplicationId = contactDto.InApplicationId;
            entity.AliasesMembers.First(x => x.Alias.Internal == true).Alias.Name = contactDto.Name;

            AddConactMembers(contactDto, entity);

            await _contactRepository.AddIfNotExists(entity);
            await _contactRepository.Save();
            return new ContactDtoWithId
            {
                Id = entity.Id,
                Name = entity.Name,
                InApplicationId = entity.InApplicationId,
                Members = entity.ContactsMembers.Select(x => new ContactMemberWithIdDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList()
            };
        }

        private void Validate(ContactDto contactDto)
        {
            if (contactDto.Name.Length > 256)
            {
                throw new TooLongContactNameException();
            }
            if (contactDto.InApplicationId.Length > 256)
            {
                throw new TooLongContactInApplicationIdException();
            }
            foreach (var item in contactDto.Members ?? Enumerable.Empty<ContactMemberDto>())
            {
                if (item.Name.Length > 256)
                {
                    throw new TooLongContactMemberNameException();
                }
            }
        }

        private static void AddConactMembers(ContactDto contactDto, Contacts entity)
        {
            if (contactDto.Members == null || contactDto.Members.Count == 0)
            {
                return;
            }
            var newMembers = contactDto.Members.Select(x => new MatchingContactMember<ContactMemberDto>(x)).ToList();

            foreach (var item in entity.ContactsMembers)
            {
                var matchingNew = newMembers.FirstOrDefault(x => !x.Matched && x.Original.Name == item.Name);
                if (matchingNew != null)
                {
                    matchingNew.Matched = true;
                }
            }

            foreach (var item in newMembers.Where(x => !x.Matched))
            {
                entity.ContactsMembers.Add(new ContactsMembers
                {
                    Name = item.Original.Name
                });
            }
        }
        class MatchingContactMember<T>
        {
            public MatchingContactMember(T original)
            {
                Original = original;
                Matched = false;
            }
            public bool Matched { get; set; }
            public T Original { get; set; }
        }
    }
}
