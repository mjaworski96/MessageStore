using System.Threading.Tasks;
using API.Dto;
using API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpPost]
        public async Task<IActionResult> Create(CreateAliasDto createAlias)
        {
            _logger.LogInformation($"Started POST /api/aliases");
            var result = await _aliasService.Create(createAlias);
            _logger.LogInformation($"Ended POST /api/aliases");
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, CreateAliasDto createAlias)
        {
            _logger.LogInformation($"Started PUT /api/aliases/{id}");
            var result = await _aliasService.Update(id, createAlias);
            _logger.LogInformation($"Ended PUT /api/aliases/{id}");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            _logger.LogInformation($"Started GET /api/aliases/{id}");
            var result = await _aliasService.Get(id);
            _logger.LogInformation($"Ended GET /api/aliases/{id}");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove([FromRoute] int id)
        {
            _logger.LogInformation($"Started DELETE /api/aliases/{id}");
            await _aliasService.Remove(id);
            _logger.LogInformation($"Ended DELETE /api/aliases/{id}");
            return NoContent();
        }
    }
}