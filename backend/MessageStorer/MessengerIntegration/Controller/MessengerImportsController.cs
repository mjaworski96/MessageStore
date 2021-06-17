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
            _logger.LogInformation($"Started GET /api/import");
            var result = await _importService.GetAllForUser();
            _logger.LogInformation($"Ended GET /api/import");
            return Ok(result);
        }
        [HttpPost()]
        public async Task<ActionResult<ImportDtoWithId>> StartImport(ImportDto importDto)
        {
            _logger.LogInformation($"Started POST /api/import");
            var result = await _importService.Start(importDto);
            _logger.LogInformation($"Ended POST /api/import");
            return Ok(result);
        }
        [HttpPost("{id}/file")]
        public async Task<IActionResult> UploadFile(string id, FileDto fileDto)
        {
            _logger.LogInformation($"Started POST /api/import/{id}/file");
            await _importService.UploadFile(id, fileDto);
            _logger.LogInformation($"Ended POST /api/import/{id}/file");
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> FinishFileImport(string id)
        {
            _logger.LogInformation($"Started PUT /api/import/{id}");
            await _importService.Finish(id);
            _logger.LogInformation($"Ended PUT /api/import/{id}");
            return NoContent();
        }
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelImport(string id)
        {
            _logger.LogInformation($"Started PUT /api/import/{id}/cancel");
            await _importService.Cancel(id);
            _logger.LogInformation($"Ended PUT /api/import/{id}/cancel");
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImport(string id)
        {
            _logger.LogInformation($"Started DELETE /api/import/{id}");
            await _importService.Delete(id);
            _logger.LogInformation($"Ended DELETE /api/import/{id}");
            return NoContent();
        }
    }
}