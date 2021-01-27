using System.Threading.Tasks;
using FacebookMessengerIntegration.Dto;
using FacebookMessengerIntegration.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FacebookMessengerIntegration.Controller
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

        [HttpPost()]
        public async Task<IActionResult> StartImport(ImportDto importDto)
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
    }
}