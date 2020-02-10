using System.Threading.Tasks;
using API.Dto;
using API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILastSyncService _lastSyncService;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessageService messageService,
           ILastSyncService lastSyncService,
           ILogger<MessagesController> logger)
        {
            _messageService = messageService;
            _lastSyncService = lastSyncService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MessageDto messageDto)
        {
            _logger.LogInformation($"Started POST /api/messages for {JsonConvert.SerializeObject(messageDto)}");
            var result = await _messageService.Create(messageDto);
            _logger.LogInformation($"Ended POST /api/messages with {JsonConvert.SerializeObject(result)}");

            return Ok(result);
        }
        [HttpGet("lastSyncTime")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation($"Started GET /api/messages/lastSyncTime");
            var result = await _lastSyncService.Get();
            _logger.LogInformation($"Ended GET /api/messages/lastSyncTime with {JsonConvert.SerializeObject(result)}");
            return Ok(result);
        }
        [HttpGet()]
        public async Task<IActionResult> GetPage([FromQuery] int aliasId, [FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Started GET /api/messages");
            var result = await _messageService.GetPage(aliasId, pageNumber, pageSize);
            _logger.LogInformation($"Ended GET /api/messages with {JsonConvert.SerializeObject(result)}");
            return Ok(result);
        }
    }
}