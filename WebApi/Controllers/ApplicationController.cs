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

        public ApplicationController(
            IApplicationService applicationService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _applicationService = applicationService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetAllByUserId(string id)
        {
            var applications = _applicationService.GetAllByUserId(id);
            var applicationDtos = _mapper.Map<IList<ApplicationDto>>(applications);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Number of questions: " + applicationDtos.Count);

            return Ok(applicationDtos);
        }

        [AllowAnonymous]
        [HttpGet("application/{id}")]
        public IActionResult GetByApplicationId(string id)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Requested application id: " + id);

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

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Creating application: " + application.Title + " for the " + application.Email);

            try
            {
                // save 
                application.Id = Guid.NewGuid().ToString();
                application.Timestamp = DateTime.Now;

                _applicationService.Create(application);
                return Ok(application);
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
