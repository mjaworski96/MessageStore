using System.Threading.Tasks;
using MessengerIntegration.Dto;
using MessengerIntegration.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessengerIntegration.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessengerImportsController : ControllerBase
    {
        private readonly IImportService _importService;
        private readonly ILogger<MessengerImportsController> _logger;
        public MessengerImportsController(IImportService importService,
            ILogger<MessengerImportsController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<ActionResult<ImportDtoWithIdList>> GetAllForUser()
        {
            _logger.LogInformation($"Started GET /api/messengerImports");
            var result = await _importService.GetAllForUser();
            _logger.LogInformation($"Ended GET /api/messengerImports");
            return Ok(result);
        }
        [HttpPost()]
        public async Task<ActionResult<ImportDtoWithId>> StartImport(ImportDto importDto)
        {
            _logger.LogInformation($"Started POST /api/messengerImports");
            var result = await _importService.Start(importDto);
            _logger.LogInformation($"Ended POST /api/messengerImports");
            return Ok(result);
        }
        [HttpPost("{id}/file")]
        public async Task<IActionResult> UploadFile(string id, FileDto fileDto)
        {
            _logger.LogInformation($"Started POST /api/messengerImports/{id}/file");
            await _importService.UploadFile(id, fileDto);
            _logger.LogInformation($"Ended POST /api/messengerImports/{id}/file");
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> FinishFileImport(string id)
        {
            _logger.LogInformation($"Started PUT /api/messengerImports/{id}");
            await _importService.Finish(id);
            _logger.LogInformation($"Ended PUT /api/messengerImports/{id}");
            return NoContent();
        }
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelImport(string id)
        {
            _logger.LogInformation($"Started PUT /api/messengerImports/{id}/cancel");
            await _importService.Cancel(id);
            _logger.LogInformation($"Ended PUT /api/messengerImports/{id}/cancel");
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImport(string id)
        {
            _logger.LogInformation($"Started DELETE /api/messengerImports/{id}");
            await _importService.Delete(id);
            _logger.LogInformation($"Ended DELETE /api/messengerImports/{id}");
            return NoContent();
        }
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteImportsForUser(int userId)
        {
            _logger.LogInformation($"Started DELETE /api/messengerImports/user/{userId}");
            await _importService.DeleteAllForUser(userId);
            _logger.LogInformation($"Ended DELETE /api/messengerImports/user/{userId}");
            return NoContent();
        }
    }
}