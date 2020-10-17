using API.Exceptions;
using API.Persistance.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Service
{
    public interface ISecurityService
    {
        Task CheckIfUserIsOwnerOfAlias(int aliasId);
        Task CheckIfUserIsOwnerOfContact(int contactId);
    }
    public class SecurityService: ISecurityService
    {
        private readonly IAliasRepository _aliasRepository;
        private readonly IHttpMetadataService _httpMetadataService;
        private readonly IContactRepository _contactRepository;

        public SecurityService(IAliasRepository aliasRepository, IHttpMetadataService httpMetadataService, IContactRepository contactRepository)
        {
            _aliasRepository = aliasRepository;
            _httpMetadataService = httpMetadataService;
            _contactRepository = contactRepository;
        }

        public async Task CheckIfUserIsOwnerOfAlias(int aliasId)
        {
            var owner = await _aliasRepository.GetOwner(aliasId);
            if (owner?.Username != _httpMetadataService.Username)
                throw new NotFoundException($"Alias with id = {aliasId} not found.");
        }


        public async Task CheckIfUserIsOwnerOfContact(int contactId)
        {
            var owner = await _contactRepository.GetOwner(contactId);
            if (owner.Username != _httpMetadataService.Username)
                throw new NotFoundException($"Contact with id = {contactId} not found.");
        }

    }
}
