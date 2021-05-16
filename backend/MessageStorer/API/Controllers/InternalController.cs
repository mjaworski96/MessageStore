using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dto;
using API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalController : ControllerBase
    {
        private readonly IAppUserService _appUserService;
        private readonly ILogger<InternalController> _logger;

        public InternalController(IAppUserService appUserService, ILogger<InternalController> logger)
        {
            _appUserService = appUserService;
            _logger = logger;
        }

        [HttpGet("genereateInternalToken")]
        public async Task<IActionResult> GenerateInternalToken([FromQuery] int forUser)
        {
            _logger.LogInformation($"Started GET /api/internal/generateInternalToken?forUser={forUser}");
            var result = await _appUserService.CreateInternalToken(forUser);
            Response.Headers["Authorization"] = result.Token;

            _logger.LogInformation($"Ended GET /api/internal/generateInternalToken?forUser={forUser}");
            return NoContent();
        }

    }
}