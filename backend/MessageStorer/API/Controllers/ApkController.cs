using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApkController : ControllerBase
    {
        private readonly ILogger<ApkController> _logger;

        public ApkController(ILogger<ApkController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"Started GET /api/apk");
            _logger.LogInformation($"Ended GET /api/apk");
            return PhysicalFile(Path.GetFullPath("APK/com.companyname.messagesender-Signed.apk"),
                "application/vnd.android.package-archive");
        }
    }
}