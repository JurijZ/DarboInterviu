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
        private IVideoService _videoService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public VideoController(
            IHostingEnvironment hostingEnvironment, 
            IVideoService videoService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _hostingEnvironment = hostingEnvironment;
            _videoService = videoService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
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

        [HttpGet()]
        public ActionResult<IEnumerable<Video>> GetVideosMetadata()
        {
            NLog.LogManager.GetCurrentClassLogger().Info("GetVideosMetadata");

            var listOfVideosMetadata = _videoService.GetVideosMetadata();
            if (listOfVideosMetadata.Any())
            {
                return Ok(listOfVideosMetadata);
            }

            NLog.LogManager.GetCurrentClassLogger().Info("No files were found, returning HTTP 400 BadRequest");
            return BadRequest();
        }

        [HttpGet("{id}")]
        public IActionResult GetVideoById(string id)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Video file requested: " + id);

            var fs = _videoService.GetVideoById(id);
            if (fs != null)
            {                
                return new FileStreamResult(fs, new MediaTypeHeaderValue("video/webm").MediaType);
            }

            logger.Info("No file found, returning HTTP 400 BadRequest");
            return BadRequest();
        }
    }
}