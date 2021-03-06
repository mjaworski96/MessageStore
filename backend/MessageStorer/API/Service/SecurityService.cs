﻿using Common.Exceptions;
using API.Persistance.Entity;
using API.Persistance.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Service;

namespace API.Service
{
    public interface ISecurityService
    {
        Task CheckIfUserIsOwnerOfAlias(int aliasId);
        void CheckIfUserIsOwnerOfAlias(Aliases alias);
        void CheckIfUserIsOwnerOfAliases(IEnumerable<Aliases> aliases);
        Task CheckIfUserIsOwnerOfContact(int contactId);
        void CheckIfUserIsOwnerOfContact(Contacts contact);
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
            if (owner?.Id != _httpMetadataService.UserId)
                throw new ForbiddenException($"You have no access for this alia");
        }

        public void CheckIfUserIsOwnerOfAlias(Aliases alias)
        {
            if (alias.AliasesMembers.FirstOrDefault()?.Contact.AppUserId != 
                _httpMetadataService.UserId)
            {
                throw new ForbiddenException($"You have no access for this alias");
            }
        }

        public void CheckIfUserIsOwnerOfAliases(IEnumerable<Aliases> aliases)
        {
            foreach (var alias in aliases)
            {
                CheckIfUserIsOwnerOfAlias(alias);
            }
        }

        public async Task CheckIfUserIsOwnerOfContact(int contactId)
        {
            var owner = await _contactRepository.GetOwner(contactId);
            if (owner.Id != _httpMetadataService.UserId)
                throw new ForbiddenException($"You have no access for this contact");
        }

        public void CheckIfUserIsOwnerOfContact(Contacts contact)
        {
            if (contact.AppUserId !=
                _httpMetadataService.UserId)
            {
                throw new ForbiddenException($"You have no access for this contact");
            }
        }
    }
}
