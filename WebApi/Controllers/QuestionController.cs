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
    public class QuestionController : ControllerBase
    {
        private IQuestionService _questionService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public QuestionController(
            IQuestionService questionService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _questionService = questionService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var questions =  _questionService.GetAll();
            var questionDtos = _mapper.Map<IList<QuestionDto>>(questions);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Number of questions: " + questionDtos.Count);

            return Ok(questionDtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var question = _questionService.GetById(id);
            var interviewDto = _mapper.Map<QuestionDto>(question);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Requested question id: " + id);

            return Ok(interviewDto);
        }


        [AllowAnonymous]
        [HttpPost("Create")]
        public IActionResult Create([FromBody]QuestionDto questionDto)
        {
            // map dto to entity
            var question = _mapper.Map<Question>(questionDto);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Creating question: " + question.Text);

            try
            {
                // save 
                question.Id = Guid.NewGuid().ToString();

                _questionService.Create(question);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]QuestionDto questionDto)
        {
            // map dto to entity and set id
            var question = _mapper.Map<Question>(questionDto);
            question.Id = id;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Updating question: " + question.Id);

            try
            {
                // save 
                _questionService.Update(question);
                return Ok();
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
            _questionService.Delete(id);
            return Ok();
        }
    }
}
