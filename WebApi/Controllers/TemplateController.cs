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

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TemplateController : ControllerBase
    {
        private ITemplateService _templateService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public TemplateController(
            ITemplateService templateService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            ILogger<TemplateController> logger)
        {
            _templateService = templateService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
        }
        
        [AllowAnonymous]
        [HttpGet("{userId}")]
        public IActionResult GetAllByUserId(string userId)
        {
            var template =  _templateService.GetAllByUserId(userId);
            var interviewDtos = _mapper.Map<IList<TemplateDto>>(template);

            _logger.LogInformation("Number of templates: " + interviewDtos.Count);

            return Ok(interviewDtos);
        }

        //[AllowAnonymous]
        //[HttpGet("{id}")]
        //public IActionResult GetById(string id)
        //{
        //    var template = _templateService.GetById(id);
        //    var templateDto = _mapper.Map<TemplateDto>(template);

        //    _logger.LogInformation("Requested interview id: " + id);

        //    return Ok(templateDto);
        //}


        [AllowAnonymous]
        [HttpPost("Create")]
        public IActionResult Create([FromBody]TemplateDto templateDto)
        {
            _logger.LogInformation("New template id: " + templateDto.Id);

            try
            {
                // map dto to entity
                var template = _mapper.Map<Template>(templateDto);

                // save 
                template.Id = Guid.NewGuid().ToString();
                template.Timestamp = DateTime.Now;
                
                _templateService.Create(template);
                return Ok(template);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]TemplateDto templateDto)
        {
            // map dto to entity and set id
            var template = _mapper.Map<Template>(templateDto);

            _logger.LogInformation("Updating interview: " + template.Id);

            try
            {
                // save 
                _templateService.Update(template);
                _logger.LogInformation("Interview update was successful");
                //return Ok(question.Id);  //200
                return NoContent(); //204
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                _logger.LogInformation("Error while updating the interview");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _templateService.Delete(id);
            return Ok();
        }
    }
}
