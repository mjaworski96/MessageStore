using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dto;
using API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        [HttpPut]
        public async Task<IActionResult> AddOrUpdate(ContactDto contactDto)
        {
            _logger.LogInformation($"Started PUT /api/contacts {JsonConvert.SerializeObject(contactDto)}");
            return Ok(await _contactService.AddIfNotExists(contactDto));
        }
    }
}