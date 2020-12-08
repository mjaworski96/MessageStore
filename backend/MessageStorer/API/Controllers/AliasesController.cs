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
    }
}