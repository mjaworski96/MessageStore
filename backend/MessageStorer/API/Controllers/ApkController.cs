using System.IO;
using API.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApkController : ControllerBase
    {
        private readonly IApkConfig _apkConfig;
        private readonly ILogger<ApkController> _logger;

        public ApkController(IApkConfig apkConfig, ILogger<ApkController> logger)
        {
            _apkConfig = apkConfig;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"Started GET /api/apk");
            _logger.LogInformation($"Ended GET /api/apk");
            return PhysicalFile(Path.GetFullPath(_apkConfig.Filename),
                "application/vnd.android.package-archive");
        }
    }
}