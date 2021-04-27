using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly ILogger<AttachmentsController> _logger;

        public AttachmentsController(IAttachmentService attachmentService, ILogger<AttachmentsController> logger)
        {
            _attachmentService = attachmentService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            _logger.LogInformation($"Started GET /api/attachments/{id}");
            var result = await _attachmentService.Get(id);
            _logger.LogInformation($"Ended GET /api/attachments/{id}");
            return File(result.Content, result.ContentType);
        }

        [HttpGet("{id}/stream")]
        public async Task<ActionResult> GetStream(int id)
        {
            _logger.LogInformation($"Started GET /api/attachments/{id}/stream");
            var result = await _attachmentService.GetMetadata(id);
            _logger.LogInformation($"Ended GET /api/attachments/{id}/stream");
            return PhysicalFile(result.Filename, "application/octet-stream", result.SaveAsFilename, true);
        }
    }
}