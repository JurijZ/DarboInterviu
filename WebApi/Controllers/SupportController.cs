using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Dtos;
using WebApi.Entities;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SupportController : ControllerBase
    {
        private ISupportService _supportService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        private IConfiguration _configuration { get; }

        public SupportController(
            ISupportService supportService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            ILogger<SupportController> logger,
            IConfiguration configuration)
        {
            _supportService = supportService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
            _configuration = configuration;
        }
        
        [AllowAnonymous]
        [HttpGet("template")]
        public IActionResult GetAllTemplates()
        {
            var interviews = _supportService.GetAllTemplates();
            var interviewDtos = _mapper.Map<IList<TemplateDto>>(interviews);

            _logger.LogInformation("Number of templates: " + interviewDtos.Count);

            return Ok(interviewDtos);
        }

        [AllowAnonymous]
        [HttpGet("application")]
        public IActionResult GetAllApplications()
        {
            var applications = _supportService.GetAllApplications();
            var applicationDtos = _mapper.Map<IList<ActiveInteriewDto>>(applications);

            _logger.LogInformation("Number of interviews: " + applicationDtos.Count);

            return Ok(applicationDtos);
        }

        [AllowAnonymous]
        [HttpGet("video")]
        public ActionResult<IEnumerable<VideoDto>> GetVideosMetadata()
        {
            var listOfVideosMetadata = _supportService.GetVideosMetadata();
            if (listOfVideosMetadata.Any())
            {
                return Ok(listOfVideosMetadata);
            }

            _logger.LogInformation("No files were found, returning HTTP 400 BadRequest");
            return BadRequest();
        }
        
        [AllowAnonymous]
        [HttpGet("emailtest/{title}")]
        public IActionResult TestEmail(string title)
        {
            _logger.LogInformation("Test message: " + title);

            var mailgunApiKey = _configuration.GetSection("MAILGUNAPI").Value;
            _logger.LogInformation("MAILGUNAPI: " + mailgunApiKey);

            var response = MailgunAPI.SendTestMessage(title);

            return Ok("MailGun API response: " + response);
        }
    }
}
