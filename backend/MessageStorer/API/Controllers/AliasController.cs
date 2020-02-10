using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AliasController : ControllerBase
    {
        private readonly IAliasService _aliasService;
        private readonly ILogger<AliasController> _logger;

        public AliasController(IAliasService aliasService, ILogger<AliasController> logger)
        {
            _aliasService = aliasService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string app, [FromQuery] bool internalOnly = true)
        {
            _logger.LogInformation($"Started GET /api/aliases?app={app}&internalOnly={internalOnly}");
            var result = await _aliasService.GetAll(app, internalOnly);
            _logger.LogInformation($"Ended GET /api/aliases?app={app}&internalOnly={internalOnly} with {JsonConvert.SerializeObject(result)}");
            return Ok(result);
        }
    }
}