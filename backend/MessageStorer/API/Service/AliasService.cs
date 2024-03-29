﻿using API.Dto;
using Common.Exceptions;
using API.Persistance.Entity;
using API.Persistance.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Service;
using System;

namespace API.Service
{
    public interface IAliasService
    {
        Task<AliasDtoWithIdList> GetAll(string app, bool internalOnly);
        Task<AliasDtoWithId> Create(CreateAliasDto createAlias);
        Task<AliasDtoWithId> Update(int id, CreateAliasDto updateAlias);
        Task<AliasDtoWithId> Get(int id);
        Task<AliasDtoWithId> UpdateName(int id, UpdateAliasNameDto updateName);
        Task Remove(int id);
    }
    public class AliasService : IAliasService
    {
        private readonly IAliasRepository _aliasRepository;
        private readonly IHttpMetadataService _httpMetadataService;
        private readonly ISecurityService _securityService;

        public AliasService(IAliasRepository aliasRepository, 
            IHttpMetadataService httpMetadataService,
            ISecurityService securityService)
        {
            _aliasRepository = aliasRepository;
            _httpMetadataService = httpMetadataService;
            _securityService = securityService;
        }
        
        public async Task<AliasDtoWithIdList> GetAll(string app, bool internalOnly)
        {
            var rawEntities = await _aliasRepository.GetAll(
                _httpMetadataService.UserId, app, internalOnly);

            var list = rawEntities
                .Select(x => CreateAliasDtoWithId(x)).ToList();
            return new AliasDtoWithIdList
            {
                Aliases = list
            };
        }
        
        public async Task<AliasDtoWithId> Create(CreateAliasDto createAlias)
        {
            ValidateName(createAlias.Name);
            List<Aliases> contacts = await GetValidatedAliasMembers(createAlias);
            var alias = new Aliases
            {
                Name = createAlias.Name,
                Internal = false,
            };
            alias.AliasesMembers = contacts.Select(
                x => new AliasesMembers
                {
                    Alias = alias,
                    Contact = x.AliasesMembers.First().Contact
                }).ToList();
            await _aliasRepository.Add(alias);
            return CreateAliasDtoWithId(alias);
        }

        private async Task<List<Aliases>> GetValidatedAliasMembers(CreateAliasDto createAlias)
        {
            var contacts = await _aliasRepository.GetAll(createAlias.Members.Select(x => x.Id));
            _securityService.CheckIfUserIsOwnerOfAliases(contacts);
            if (contacts.Count != createAlias.Members.Count)
            {
                throw new ContactsNotFoundException();
            }
            if (contacts.Any(x => !x.Internal))
            {
                throw new AliasCreationException();
            }

            return contacts;
        }

        public async Task<AliasDtoWithId> Update(int id, CreateAliasDto updateAlias)
        {
            ValidateName(updateAlias.Name);
            List<Aliases> contacts = await GetValidatedAliasMembers(updateAlias);
            var alias = await _aliasRepository.Get(id, true);
            _securityService.CheckIfUserIsOwnerOfAlias(alias);

            if (alias.Internal)
            {
                throw new RawAliasModificationException();
            }
            var newContactsId = updateAlias.Members.Select(x => x.Id).ToList();
            var existingMembersId = alias.AliasesMembers.Select(x => x.Id).ToList();
            alias.Name = updateAlias.Name;

            alias.AliasesMembers.Where(x => !newContactsId.Contains(x.Id))
                .ToList()
                .ForEach(member => alias.AliasesMembers.Remove(member));

            contacts.Where(x => !existingMembersId.Contains(x.Id))
                .ToList()
                .ForEach(member => alias.AliasesMembers.Add(new AliasesMembers
                {
                    Alias = alias,
                    Contact = member.AliasesMembers.First().Contact
                }));

            await _aliasRepository.Save();
            return CreateAliasDtoWithId(alias);
        }

        public async Task<AliasDtoWithId> Get(int id)
        {
            var alias = await _aliasRepository.Get(id, true);
            _securityService.CheckIfUserIsOwnerOfAlias(alias);
            return CreateAliasDtoWithId(alias);
        }

        public async Task Remove(int id)
        {
            var aliasToDelete = await _aliasRepository.Get(id, false);
            if (aliasToDelete != null)
            {
                _securityService.CheckIfUserIsOwnerOfAlias(aliasToDelete);
                if (aliasToDelete.Internal)
                {
                    throw new RawAliasDeletionException();
                }
                await _aliasRepository.Remove(aliasToDelete);
            }
        }

        private AliasDtoWithId CreateAliasDtoWithId(Aliases alias)
        {
            return new AliasDtoWithId
            {
                Id = alias.Id,
                Name = GetName(alias),
                Internal = alias.Internal,
                Application = alias.Internal ? alias.AliasesMembers.First().Contact.Application.Name : "",
                InApplicationId = alias.Internal ? alias.AliasesMembers.First().Contact.InApplicationId : "",
                Members = alias.AliasesMembers.Select(y => new AliasMemberDtoWithId
                {
                    Id = y.Contact.Id,
                    Name = y.Contact.Name,
                    Application = y.Contact.Application.Name,
                    InApplicationId = y.Contact.InApplicationId,
                    ContactMembers = y.Contact.ContactsMembers
                        .Select(z => new ContactMemberWithIdDto { Id =  z.Id, Name = z.Name}).ToList()
                }).ToList()
            };
        }

        private string GetName(Aliases alias)
        {
            if (alias.Internal)
            {
                if (!string.IsNullOrEmpty(alias.UserGivenName))
                {
                    return alias.UserGivenName;
                }
                if (alias.AliasesMembers.FirstOrDefault().Contact.ContactsMembers.Any())
                {
                    return string.Join(" ", alias.AliasesMembers.FirstOrDefault().Contact.ContactsMembers.Select(x => x.Name));
                }
            } 
            return alias.Name;
        }

        private void ValidateName(string aliasName)
        {
            if(string.IsNullOrEmpty(aliasName) || aliasName.Length > 256)
            {
                throw new InvalidAliasNameException();
            }
        }

        public async Task<AliasDtoWithId> UpdateName(int id, UpdateAliasNameDto updateName)
        {
            ValidateName(updateName.Name);
            var alias = await _aliasRepository.Get(id, true);
            _securityService.CheckIfUserIsOwnerOfAlias(alias);
            if (!alias.Internal)
            {
                throw new EditNotInernalAliasNameException();
            }
            alias.UserGivenName = updateName.Name;
            await _aliasRepository.Save();
            return CreateAliasDtoWithId(alias);
        }
    }
}
