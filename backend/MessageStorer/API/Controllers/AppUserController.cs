using API.Dto;
using API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserService _appUserService;
        private readonly ILogger<AppUserController> _logger;

        public AppUserController(IAppUserService appUserService, ILogger<AppUserController> logger)
        {
            _appUserService = appUserService;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(AppUserLoginDetails loginDetails)
        {
            _logger.LogInformation($"Started POST /api/appUser/login");
            var result = await _appUserService.Login(loginDetails);
            Response.Headers["Authorization"] = $"Bearer {result.Token}";
            _logger.LogInformation($"Ended POST /api/appUser/login");

            return Ok(result.AppUser);
        }
    }
}