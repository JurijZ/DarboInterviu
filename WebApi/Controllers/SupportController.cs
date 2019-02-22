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

        public SupportController(
            ISupportService supportService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _supportService = supportService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        
        [AllowAnonymous]
        [HttpGet("template")]
        public IActionResult GetAllTemplates()
        {
            NLog.LogManager.GetCurrentClassLogger().Info("Support - GetAllTemplates");

            var interviews = _supportService.GetAllTemplates();
            var interviewDtos = _mapper.Map<IList<TemplateDto>>(interviews);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Number of templates: " + interviewDtos.Count);

            return Ok(interviewDtos);
        }

        [AllowAnonymous]
        [HttpGet("application")]
        public IActionResult GetAllApplications()
        {
            NLog.LogManager.GetCurrentClassLogger().Info("Support - GetAllApplications");

            var applications = _supportService.GetAllApplications();
            var applicationDtos = _mapper.Map<IList<ActiveInteriewDto>>(applications);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Number of interviews: " + applicationDtos.Count);

            return Ok(applicationDtos);
        }

        [AllowAnonymous]
        [HttpGet("video")]
        public ActionResult<IEnumerable<VideoDto>> GetVideosMetadata()
        {
            NLog.LogManager.GetCurrentClassLogger().Info("Support - GetVideosMetadata");

            var listOfVideosMetadata = _supportService.GetVideosMetadata();
            if (listOfVideosMetadata.Any())
            {
                return Ok(listOfVideosMetadata);
            }

            NLog.LogManager.GetCurrentClassLogger().Info("No files were found, returning HTTP 400 BadRequest");
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("Email")]
        public IActionResult Create(string emailText)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Sending email with text: " + emailText);

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
