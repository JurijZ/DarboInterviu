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
using System.Linq;
using WebApi.Entities;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private ICandidateService _candidateService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public CandidateController(
            ICandidateService candidateService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _candidateService = candidateService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpGet("questions/{id}")]
        public IActionResult GetInterviewQuestionsByApplicationId(string id)
        {
            // If this method has been called then it means the interview has started, we change the status:
            var status = _candidateService.SetApplicationStatus(id, "Started");
            if (status == null)
            {
                return BadRequest("Unknown Application Id");
            }

            // Retrieve all the questions:
            var questions = _candidateService.GetTemplateQuestionsByApplicationId(id);

            if (questions != null && questions.Any()) {
                var questionDtos = _mapper.Map<IList<QuestionDto>>(questions);

                return Ok(questionDtos);
            }
            else
            {
                return BadRequest("Unknown Application Id");
            }
            
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetApplicationById(string id)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Requested application id: " + id);

            var application = _candidateService.GetApplicationById(id);
            var applicationDto = _mapper.Map<ApplicationDto>(application);

            // Prepare ApplicationId and Interview overview information
            
            var template = _candidateService.GetInterviewOverview(application.TemplateId);
            
            //return Ok(applicationDto);

            return Ok(new
            {
                TemplateId = application.TemplateId,
                Email = application.CandidateEmail,
                ApplicationId = application.Id,
                Expiration = application.Expiration,
                Name = application.CandidateName,
                Title = application.Title,
                NumberOfQuestions = template.Item1,
                InterviewDuration = template.Item2
            });
        }

        [AllowAnonymous]
        [HttpPost("status")]
        public IActionResult UpdateInterviewStatus([FromBody] ApplicationStatusDto applicationStatusDto)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Interview " + applicationStatusDto.ApplicationId + " status is set to - " + applicationStatusDto.Status);

            var authentication = _candidateService.SetApplicationStatus(applicationStatusDto.ApplicationId, applicationStatusDto.Status);

            // Return
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]CandidateDto candidateDto)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Authentication");

            var authentication = _candidateService.Authenticate(candidateDto.Email, candidateDto.Password);
            logger.Info("Authentication result: " + authentication);

            if (authentication == 1)
                return BadRequest(new { message = "Neteisingai įvestas elektroninio pašto adresas arba slaptažodis. Pataisykite ir bandykite prisijungti dar kartą." });

            if (authentication == 2)
                return BadRequest(new { message = "Šio interviu galiojimo laikas baigėsi." });

            // Prepare JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, candidateDto.Email),
                    new Claim(ClaimTypes.Role, "Candidate")
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Retriev application
            var application = _candidateService.GetApplicationId(candidateDto.Email, candidateDto.Password);

            // Return
            return Ok(new {
                Username = application.CandidateName,
                Email = application.CandidateEmail,
                ApplicationId = application.Id,                
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPut("unsubscribe/{email}")]
        public IActionResult Unsubscribe(string email)
        {
            try
            {
                // TODO - implement unsubscribe logic (not really relevant for the Candidate)
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Unsubscribing candidates email: " + email);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
