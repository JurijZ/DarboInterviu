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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private IApplicationService _applicationService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        private IConfiguration _configuration { get; }

        public ApplicationController(
            IApplicationService applicationService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            ILogger<ApplicationController> logger,
            IConfiguration configuration)
        {
            _applicationService = applicationService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
            _configuration = configuration;
        }
        
        // Retrieve all the applications related to a particular Employer
        [AllowAnonymous]
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetAllByUserId(string id)
        {
            var applications = _applicationService.GetAllByUserId(id);
            var applicationDtos = _mapper.Map<IList<ActiveInteriewDto>>(applications);

            _logger.LogInformation("Employer requested his applications, the number is: " + applicationDtos.Count);

            return Ok(applicationDtos);
        }

        [AllowAnonymous]
        [HttpGet("application/{id}")]
        public IActionResult GetByApplicationId(string id)
        {
            _logger.LogInformation("Requested application id: " + id);

            var application = _applicationService.GetByApplicationId(id);
            var applicationDto = _mapper.Map<ApplicationDto>(application);            

            return Ok(applicationDto);
        }


        [AllowAnonymous]
        [HttpPost("Create")]
        public IActionResult Create([FromBody]ApplicationDto applicationDto)
        {
            // map dto to entity
            var application = _mapper.Map<Application>(applicationDto);

            _logger.LogInformation("Creating application: " + application.Title + " for the " + application.CandidateEmail);

            try
            {
                // save 
                application.Id = Guid.NewGuid().ToString();
                application.CandidateSecret = _applicationService.GetRandomNumber(1000, 9999).ToString();
                application.Timestamp = DateTime.Now;
                application.Status = InterviuStatus.NotStarted.ToString();
                application.StatusTimestamp = DateTime.Now;

                _applicationService.Create(application, applicationDto.UserId);
                return Ok(application);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("Share")]
        public IActionResult Share([FromBody]ShareDto shareDto)
        {
            _logger.LogInformation("Sharing application: " + shareDto.ApplicationId + " with " + shareDto.Email);

            try
            {
                var s = _applicationService.Share(shareDto.UserId, shareDto.ApplicationId, shareDto.Email);
                return Ok(shareDto);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _applicationService.Delete(id);
            return Ok();
        }        
    }
}
