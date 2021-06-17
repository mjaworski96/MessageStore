using System.Threading.Tasks;
using API.Dto;
using API.Security;
using API.Service;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [NoInternalAccess]
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
        public async Task<ActionResult<AliasDtoWithIdList>> GetAll([FromQuery] string app, [FromQuery] bool internalOnly = true)
        {
            _logger.LogInformation($"Started GET /api/aliases?app={app}&internalOnly={internalOnly}");
            var result = await _aliasService.GetAll(app, internalOnly);
            _logger.LogInformation($"Ended GET /api/aliases?app={app}&internalOnly={internalOnly}");
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<AliasDtoWithId>> Create(CreateAliasDto createAlias)
        {
            _logger.LogInformation($"Started POST /api/aliases");
            var result = await _aliasService.Create(createAlias);
            _logger.LogInformation($"Ended POST /api/aliases");
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<AliasDtoWithId>> Update([FromRoute] int id, CreateAliasDto createAlias)
        {
            _logger.LogInformation($"Started PUT /api/aliases/{id}");
            var result = await _aliasService.Update(id, createAlias);
            _logger.LogInformation($"Ended PUT /api/aliases/{id}");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AliasDtoWithId>> Get([FromRoute] int id)
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
        [HttpPatch("{id}/name")]
        public async Task<ActionResult<AliasDtoWithId>> UpdateNam([FromRoute] int id, UpdateAliasNameDto updateName)
        {
            _logger.LogInformation($"Started PATCH /api/aliases/{id}/name");
            var result = await _aliasService.UpdateName(id, updateName);
            _logger.LogInformation($"Ended PATCH /api/aliases/{id}/name");
            return Ok(result);
        }
    }
}