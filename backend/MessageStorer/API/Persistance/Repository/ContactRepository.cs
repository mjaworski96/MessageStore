﻿using Common.Exceptions;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance.Repository
{
    public interface IContactRepository
    {
        Task<Contacts> Get(int id);
        Task<Contacts> Get(string appName, string inAppId, int userId);
        Task<AppUsers> GetOwner(int contactId);
        Task AddIfNotExists(Contacts entity);
        Task RemoveEmpty(int appUserId);
        Task Save();
    }
    public class ContactRepository: IContactRepository
    {
        private readonly MessagesStoreContext _messageStoreContext;

        public ContactRepository(MessagesStoreContext messageStoreContext)
        {
            _messageStoreContext = messageStoreContext;
        }

        public async Task AddIfNotExists(Contacts entity)
        {
            if (entity.Id == 0)
            {
                await _messageStoreContext.Contacts.AddAsync(entity);
            }
        }

        public async Task<Contacts> Get(string appName, string inAppId, int userId)
        {
            return await _messageStoreContext
                .Contacts
                .Include(x => x.AliasesMembers)
                .ThenInclude(x => x.Alias)
                .Include(x => x.AppUser)
                .Include(x => x.ContactsMembers)
                .FirstOrDefaultAsync(x =>
                x.Application.Name == appName &&
                x.InApplicationId == inAppId &&
                x.AppUserId == userId);
        }
        public async Task<Contacts> Get(int id)
        {
            try
            {
                return await _messageStoreContext
                .Contacts
                .Include(x => x.ContactsMembers)
                .Include(x => x.Application)
                .FirstAsync(x => x.Id == id);
            }
            catch(InvalidOperationException e)
            {
                throw new ContactNotFoundException(id, e);
            }
        }

        public async Task<AppUsers> GetOwner(int contactId)
        {
            return await _messageStoreContext
                .Contacts
                .Include(x => x.AppUser)
                .Where(x => x.Id == contactId)
                .Select(x => x.AppUser)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveEmpty(int appUserId)
        {
            await _messageStoreContext.RemoveEmptyContacts(appUserId);
        }

        public async Task Save()
        {
            await _messageStoreContext.SaveChangesAsync();
        }
    }
}
