﻿using API.Dto;
using API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUsersController : ControllerBase
    {
        private readonly IAppUserService _appUserService;
        private readonly ILogger<AppUsersController> _logger;
        private readonly IHttpMetadataService _httpMetadataService;

        public AppUsersController(IAppUserService appUserService, ILogger<AppUsersController> logger, IHttpMetadataService httpMetadataService)
        {
            _appUserService = appUserService;
            _logger = logger;
            _httpMetadataService = httpMetadataService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Started GET /api/appUser");
            string username = _httpMetadataService.Username;
            var result = await _appUserService.GetUser(username);
            _logger.LogInformation($"Ended GET /api/appUser");

            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(AppUserLoginDetails loginDetails)
        {
            _logger.LogInformation($"Started POST /api/appUser/login");
            var result = await _appUserService.Login(loginDetails);
            Response.Headers["Authorization"] = result.Token;
            _logger.LogInformation($"Ended POST /api/appUser/login");

            return Ok(result.AppUser);
        }

        [HttpPost]
        public async Task<IActionResult> Register(AppUserRegisterDetails registerDetails)
        {
            _logger.LogInformation($"Started POST /api/appUser");
            var result = await _appUserService.Register(registerDetails);
            Response.Headers["Authorization"] = result.Token;
            _logger.LogInformation($"Ended POST /api/appUser");

            return StatusCode((int)HttpStatusCode.Created, result.AppUser);
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Modify(AppUserDto appUser)
        {
            _logger.LogInformation($"Started PUT /api/appUser");
            string username = _httpMetadataService.Username;
            var result = await _appUserService.Modify(username, appUser);
            Response.Headers["Authorization"] = result.Token;
            _logger.LogInformation($"Ended PUT /api/appUser");

            return Ok(result.AppUser);
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            _logger.LogInformation($"Started DELETE /api/appUser");
            string username = _httpMetadataService.Username;
            await _appUserService.Remove(username);
            _logger.LogInformation($"Ended DELETE /api/appUser");

            return NoContent();
        }
        [HttpPost]
        [Route("passwordChange")]
        [Authorize]
        public async Task<IActionResult> PasswordChange(AppUserPasswordChange appUserPasswordChange)
        {
            _logger.LogInformation($"Started POST /api/appUser/passwordChange");
            string username = _httpMetadataService.Username;
            await _appUserService.ChangePassword(username, appUserPasswordChange);
            _logger.LogInformation($"Ended POST /api/appUser/passwordChange");

            return NoContent();
        }
    }
}