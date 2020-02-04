using API.Dto;
using API.Persistance.Entity;
using API.Persistance.Repository;
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

        public ContactService(IContactRepository contactRepository, IHttpMetadataService httpMetadataService, IApplicationRepository applicationRepository, IAppUserRepository appUserRepository)
        {
            _contactRepository = contactRepository;
            _httpMetadataService = httpMetadataService;
            _applicationRepository = applicationRepository;
            _appUserRepository = appUserRepository;
        }

        public async Task<ContactDtoWithId> AddIfNotExists(ContactDto contactDto)
        {
            Contact entity = await _contactRepository.Get(
                _httpMetadataService.Application,
                contactDto.InApplicationId,
                _httpMetadataService.Username);
            if (entity == null)
            {
                entity = new Contact
                {
                    AppUser = _appUserRepository.Get(_httpMetadataService.Username),
                    Application = _applicationRepository.Get(_httpMetadataService.Application)
                };
            }
            entity.Name = contactDto.Name;
            entity.InApplicationId = contactDto.InApplicationId;
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
