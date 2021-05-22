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
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;
        private readonly ILogger<ImportController> _logger;
        public ImportController(IImportService importService,
            ILogger<ImportController> logger)
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
            var result = await _importService.StartProcess(importDto);
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
            await _importService.FinishUpload(id);
            _logger.LogInformation($"Ended PUT /api/import/{id}");
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