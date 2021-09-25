using API.Dto;
using API.Security;
using API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesImportsController : ControllerBase
    {
        private readonly IImportService _importService;
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesImportsController> _logger;

        public MessagesImportsController(IImportService importService, IMessageService messageService, ILogger<MessagesImportsController> logger)
        {
            _importService = importService;
            _messageService = messageService;
            _logger = logger;
        }

        [HttpGet]
        [NoInternalAccess]
        public async Task<ActionResult<ImportDtoList>> GetAll()
        {
            _logger.LogInformation($"Started GET /api/messagesImports");
            var result = await _importService.GetImports();
            _logger.LogInformation($"Ended GET /api/messagesImports");
            return Ok(result);
        }
        [HttpDelete("{importId}/messages")]
        [NoInternalAccess]
        public async Task<ActionResult<ImportDtoList>> Remove(string importId)
        {
            _logger.LogInformation($"Started DELETE /api/messagesImports/{importId}/messages");
            await _messageService.DeleteForImport(importId);
            _logger.LogInformation($"Ended DELETE /api/messagesImports/{importId}/messages");
            return NoContent();
        }
        [HttpPut("{importId}/finish")]
        public async Task<ActionResult<ImportDtoList>> Finish(string importId)
        {
            _logger.LogInformation($"Started PUT /api/messagesImports/{importId}/finish");
            await _importService.Finish(importId);
            _logger.LogInformation($"Ended PUT /api/messagesImports/{importId}/finish");
            return NoContent();
        }
        [HttpPut("refresh")]
        [NoInternalAccess]
        public async Task<ActionResult<ImportDtoList>> Refresh()
        {
            _logger.LogInformation($"Started PUT /api/messagesImports/refresh");
            var result = await _importService.RefreshDates();
            _logger.LogInformation($"Ended PUT /api/messagesImports/refresh");
            return Ok(result);
        }
    }
}