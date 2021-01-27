using Common.Exceptions;
using FacebookMessengerIntegration.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FacebookMessengerIntegration.Persistance.Repository
{
    public interface IStatusRepository
    {
        Task<Statuses> GetStatusByName(string name);
    }
    public class StatusRepository: IStatusRepository
    {
        private readonly MessengerIntegrationContext _messengerIntegrationContext;

        public StatusRepository(MessengerIntegrationContext messengerIntegrationContext)
        {
            _messengerIntegrationContext = messengerIntegrationContext;
        }

        public async Task<Statuses> GetStatusByName(string name)
        {
            try
            {
                return await _messengerIntegrationContext
                .Statuses
                .FirstAsync(x => x.Name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new NotFoundException($"Status {name} not found", e);
            }
        }
    }
}
