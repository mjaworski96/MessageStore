using API.Dto;
using API.Persistance.Repository;
using System;
using System.Threading.Tasks;

namespace API.Service
{
    public interface ILastSyncService
    {
        Task<LastSyncTime> Get();
    }
    public class LastSyncService : ILastSyncService
    {
        private readonly IHttpMetadataService _httpMetadataService;
        private readonly IMessageRepository _messageRepository;

        public LastSyncService(IHttpMetadataService httpMetadataService, IMessageRepository messageRepository)
        {
            _httpMetadataService = httpMetadataService;
            _messageRepository = messageRepository;
        }

        public async Task<LastSyncTime> Get()
        {
            var newestMessage = await _messageRepository.GetNewest(
                _httpMetadataService.Username,
                _httpMetadataService.Application);
            var date = newestMessage?.Date ?? new DateTime(1970, 1, 1);
            return new LastSyncTime
            {
                Time = date
            };
        }
    }
}
