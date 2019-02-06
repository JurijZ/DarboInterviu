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
        [HttpGet("{id}")]
        public IActionResult GetApplicationById(string id)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Requested application id: " + id);

            var application = _candidateService.GetApplicationById(id);
            var applicationDto = _mapper.Map<ApplicationDto>(application);            

            return Ok(applicationDto);
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
                return BadRequest(new { message = "Username or password is incorrect" });

            if (authentication == 2)
                return BadRequest(new { message = "Interview has expired" });

            // Prepare JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, candidateDto.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Prepare Application Id
            var application = _candidateService.GetApplicationId(candidateDto.Email, candidateDto.Password);

            // return basic user info (without password) and token to store client side
            return Ok(new {
                Username = application.Name,
                Email = candidateDto.Email,
                ApplicationId = application.Id,
                Title = application.Title,
                Expiry = application.Expiration,
                Token = tokenString
            });
        }
    }
}
