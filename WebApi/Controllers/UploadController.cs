using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
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
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private IVideoService _videoService;
        private readonly ILogger _logger;

        public UploadController(
            IHostingEnvironment hostingEnvironment, 
            IVideoService videoService,
            ILogger<UploadController> logger)
        {
            _hostingEnvironment = hostingEnvironment;            
            _videoService = videoService;
            _logger = logger;
        }

        [HttpPost, DisableRequestSizeLimit]
        public ActionResult UploadFile()
        {
            try
            {
                // Extract keys from the submitted form
                var formKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());                               

                var file = Request.Form.Files[0];
                _logger.LogInformation("Received file name: " + file.FileName);

                string folderName = "Upload";
                string contentRootPath = _hostingEnvironment.ContentRootPath;
                string newPath = Path.Combine(contentRootPath, folderName);
                _logger.LogInformation("Upload to: " + newPath);

                var videoMetadata = new Video();
                videoMetadata.ApplicationId = formKeys["applicationId"];
                videoMetadata.QuestionId = formKeys["questionId"];
                videoMetadata.FileName = file.FileName;
                videoMetadata.FilePath = newPath;
                videoMetadata.Timestamp = DateTime.Now;
                _videoService.SaveVideoMetaData(videoMetadata);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Json("Upload Successful.");
            }
            catch (System.Exception ex)
            {
                return Json("Upload Failed: " + ex.Message);
            }
        }
    }
}
