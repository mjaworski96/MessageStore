using System.Threading.Tasks;
using API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AliasesController : ControllerBase
    {
        private readonly IAliasService _aliasService;
        private readonly ILogger<AliasesController> _logger;

        public AliasesController(IAliasService aliasService, ILogger<AliasesController> logger)
        {
            _aliasService = aliasService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string app, [FromQuery] bool internalOnly = true)
        {
            _logger.LogInformation($"Started GET /api/aliases?app={app}&internalOnly={internalOnly}");
            var result = await _aliasService.GetAll(app, internalOnly);
            _logger.LogInformation($"Ended GET /api/aliases?app={app}&internalOnly={internalOnly}");
            return Ok(result);
        }
    }
}