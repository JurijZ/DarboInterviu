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
using Microsoft.Extensions.Logging;

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
        private readonly ILogger _logger;

        public VideoController(
            IHostingEnvironment hostingEnvironment, 
            IVideoService videoService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            ILogger<VideoController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _videoService = videoService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public IActionResult GetVideo()
        {
            _logger.LogInformation("GetVideo");

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

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<VideoDto>> GetVideosMetadataByInterviewId(string id)
        {
            _logger.LogInformation("GetVideosMetadataByInterviewId");

            var listOfVideosMetadata = _videoService.GetVideosMetadataByInterviewId(id);
            if (listOfVideosMetadata.Any())
            {
                return Ok(listOfVideosMetadata);
            }

            _logger.LogWarning("No files were found, returning HTTP 400 BadRequest");
            return BadRequest();
        }

        [HttpGet("record/{id}")]
        public IActionResult GetVideoById(string id)
        {
            _logger.LogInformation("Video file requested: " + id);

            var fs = _videoService.GetVideoById(id);
            if (fs != null)
            {                
                return new FileStreamResult(fs, new MediaTypeHeaderValue("video/webm").MediaType);
            }

            _logger.LogWarning("No file found, returning HTTP 400 BadRequest");
            return BadRequest();
        }
    }
}