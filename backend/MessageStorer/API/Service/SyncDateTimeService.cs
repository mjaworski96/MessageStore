using API.Dto;
using API.Persistance.Repository;
using Common.Service;
using System;
using System.Threading.Tasks;

namespace API.Service
{
    public interface ISyncDateTimeService
    {
        Task<SyncDateTime> Get(int? contactId);
    }
    public class SyncDateTimeService : ISyncDateTimeService
    {
        private readonly IHttpMetadataService _httpMetadataService;
        private readonly IMessageRepository _messageRepository;

        public SyncDateTimeService(IHttpMetadataService httpMetadataService, IMessageRepository messageRepository)
        {
            _httpMetadataService = httpMetadataService;
            _messageRepository = messageRepository;
        }

        public async Task<SyncDateTime> Get(int? contactId)
        {
            var newestMessage = await _messageRepository.GetNewest(
                _httpMetadataService.UserId,
                _httpMetadataService.Application,
                contactId);
            var newestDate = newestMessage?.Date ?? new DateTime(1970, 1, 1);
            var oldestMessage = await _messageRepository.GetOldest(
                _httpMetadataService.UserId,
                _httpMetadataService.Application,
                contactId);
            var oldestDate = oldestMessage?.Date ?? new DateTime(2038, 1, 19, 3, 14, 7); //Last Unix timestamp (32bit)

            return new SyncDateTime
            {
                From = oldestDate,
                To = newestDate,        
            };
        }
    }
}
