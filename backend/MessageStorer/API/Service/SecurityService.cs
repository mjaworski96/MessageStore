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
        void CheckIfUserIsOwnerOfAttachment(Attachments attachment);
        void CheckIfUserIsOwnerOfImport(Imports import);
        void CheckIfUserCanDeleteAccount(int userId);
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
                throw new ForbiddenAliasException();
        }

        public void CheckIfUserIsOwnerOfAlias(Aliases alias)
        {
            if (alias.AliasesMembers.FirstOrDefault()?.Contact.AppUserId != 
                _httpMetadataService.UserId)
            {
                throw new ForbiddenAliasException();
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
                throw new ForbiddenContactException();
        }

        public void CheckIfUserIsOwnerOfContact(Contacts contact)
        {
            if (contact.AppUserId !=
                _httpMetadataService.UserId)
            {
                throw new ForbiddenContactException();
            }
        }

        public void CheckIfUserIsOwnerOfAttachment(Attachments attachment)
        {
            var ownerId = attachment.Message.Contact.AppUserId;
            if (ownerId != _httpMetadataService.UserId)
                throw new ForbiddenAttachmentException();
        }

        public void CheckIfUserIsOwnerOfImport(Imports import)
        {
            if (import.AppUserId != _httpMetadataService.UserId)
                throw new ForbiddenImportException();
        }

        public void CheckIfUserCanDeleteAccount(int userId)
        {
            if (userId != _httpMetadataService.UserId)
            {
                throw new ForbiddenUserDeleteException();
            }
        }
    }
}
