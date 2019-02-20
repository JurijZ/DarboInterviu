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
    [Authorize(Policy = "Support")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private IAdminService _adminService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public AdminsController(
            IAdminService adminService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _adminService = adminService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AdminDto adminDto)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Authentication");

            var admin = _adminService.Authenticate(adminDto.UserName, adminDto.Password);

            if (admin == null)
                return BadRequest(new { message = "AdminName or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, admin.Id),
                    new Claim(ClaimTypes.Role, "Support")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic admin info (without password) and token to store on the client side
            return Ok(new {
                Id = admin.Id,
                Username = admin.UserName,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]AdminDto adminDto)
        {
            // map dto to entity
            var admin = _mapper.Map<Admin>(adminDto);

            try 
            {
                // save 
                _adminService.Create(admin, adminDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        //[AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var admins =  _adminService.GetAll();
            var adminDtos = _mapper.Map<IList<AdminDto>>(admins);
            return Ok(adminDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var admin =  _adminService.GetById(id);
            var adminDto = _mapper.Map<AdminDto>(admin);
            return Ok(adminDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]AdminDto adminDto)
        {
            // map dto to entity and set id
            var admin = _mapper.Map<Admin>(adminDto);
            admin.Id = id;

            try 
            {
                // save 
                _adminService.Update(admin, adminDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _adminService.Delete(id);
            return Ok();
        }
    }
}
