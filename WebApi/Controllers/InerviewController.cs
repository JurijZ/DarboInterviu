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
    public class InterviewController : ControllerBase
    {
        private IInterviewService _interviewService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public InterviewController(
            IInterviewService interviewService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _interviewService = interviewService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var interviews =  _interviewService.GetAll();
            var interviewDtos = _mapper.Map<IList<InterviewDto>>(interviews);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Number of interviews: " + interviewDtos.Count);

            return Ok(interviewDtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var interview = _interviewService.GetById(id);
            var interviewDto = _mapper.Map<InterviewDto>(interview);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Requested interview id: " + id);

            return Ok(interviewDto);
        }


        [AllowAnonymous]
        [HttpPost("Create")]
        public IActionResult Create([FromBody]InterviewDto interviewDto)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("New interview id: " + interviewDto.Id);

            try
            {
                // map dto to entity
                var interview = _mapper.Map<Interview>(interviewDto);

                // save 
                interview.Id = Guid.NewGuid().ToString();
                interview.Timestamp = DateTime.Now;
                
                _interviewService.Create(interview);
                return Ok(interview);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]InterviewDto interviewDto)
        {
            // map dto to entity and set id
            var interview = _mapper.Map<Interview>(interviewDto);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Updating interview: " + interview.Id);

            try
            {
                // save 
                _interviewService.Update(interview);
                logger.Info("Interview update was successful");
                //return Ok(question.Id);  //200
                return NoContent(); //204
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                logger.Info("Error while updating the interview");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _interviewService.Delete(id);
            return Ok();
        }
    }
}
