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
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;

        public VideoController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("[action]")]
        public IActionResult GetVideo()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("GetVideo");

            string folderName = @"Upload\1.webm";
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string path = Path.Combine(contentRootPath, folderName);            

            if (System.IO.File.Exists(path))
            {
                var fs = new FileStream(path, FileMode.Open);
                return new FileStreamResult(fs, new MediaTypeHeaderValue("video/webm").MediaType);
            }

            return BadRequest();
        }
    }
}