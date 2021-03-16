using System.Threading.Tasks;
using API.Dto;
using API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ISyncDateTimeService _lastSyncService;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessageService messageService,
           ISyncDateTimeService lastSyncService,
           ILogger<MessagesController> logger)
        {
            _messageService = messageService;
            _lastSyncService = lastSyncService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MessageDto messageDto)
        {
            _logger.LogInformation($"Started POST /api/messages");
            var result = await _messageService.Create(messageDto);
            _logger.LogInformation($"Ended POST /api/messages");

            return Ok(result);
        }
        [HttpGet("syncDateTime")]
        public async Task<IActionResult> GetSyncDateTime([FromQuery] int? contactId)
        {
            _logger.LogInformation($"Started GET /api/messages/syncDateTime/{contactId}");
            var result = await _lastSyncService.Get(contactId);
            _logger.LogInformation($"Ended GET /api/messages/syncDateTime/{contactId}");
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetPage([FromQuery] int aliasId, [FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Started GET /api/messages");
            var result = await _messageService.GetPage(aliasId, pageNumber, pageSize);
            _logger.LogInformation($"Ended GET /api/messages");
            return Ok(result);
        }
        [HttpGet("search")]
        [HttpPost("search")]
        public async Task<IActionResult> Find([FromBody] SearchQueryDto query)
        {
            _logger.LogInformation($"Started GET /api/messages/search");
            var result = await _messageService.Find(query);
            _logger.LogInformation($"Ended GET /api/messages/search");
            return Ok(result);
        }
        [HttpGet("order")]
        public async Task<IActionResult> GetOrder([FromQuery] int messageId, [FromQuery] int aliasId)
        {
            _logger.LogInformation($"Started GET /api/messages/order?messageId={messageId}&aliasId={aliasId}");
            var result = await _messageService.GetOrder(messageId, aliasId);
            _logger.LogInformation($"Ended GET /api/messages/order?messageId={messageId}&aliasId={aliasId}");
            return Ok(result);
        }
    }
}