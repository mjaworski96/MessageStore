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
    [NoInternalAccess]
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
        public async Task<ActionResult<ImportDtoList>> GetAll()
        {
            _logger.LogInformation($"Started GET /api/messagesImports");
            var result = await _importService.GetImports();
            _logger.LogInformation($"Ended GET /api/messagesImports");
            return Ok(result);
        }
        [HttpDelete("{importId}/removeMessages")]
        public async Task<ActionResult<ImportDtoList>> Remove(string importId)
        {
            _logger.LogInformation($"Started DELETE /api/messagesImports/{importId}/removeMessages");
            await _messageService.DeleteForImport(importId);
            _logger.LogInformation($"Ended DELETE /api/messagesImports/{importId}/removeMessages");
            return NoContent();
        }
    }
}