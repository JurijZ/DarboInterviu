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

        public SupportController(
            ISupportService supportService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            ILogger<SupportController> logger)
        {
            _supportService = supportService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
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
        [HttpPost("Email")]
        public IActionResult Create(string emailText)
        {
            _logger.LogInformation("Sending email with text: " + emailText);

            try
            {
                var status = AmazonAPI.SendEmailMessage(emailText);
                return Ok(status);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
