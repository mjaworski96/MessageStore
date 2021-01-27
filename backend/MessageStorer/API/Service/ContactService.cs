using API.Dto;
using API.Persistance.Entity;
using API.Persistance.Repository;
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
    public class ContactService: IContactService
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

            await _contactRepository.AddIfNotExists(entity);
            await _contactRepository.Save();
            return new ContactDtoWithId
            {
                Id = entity.Id,
                Name = entity.Name,
                InApplicationId = entity.InApplicationId
            };
        }
    }
}
