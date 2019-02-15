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
    public class TestController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private ITestService _testService;

        public TestController(IHostingEnvironment hostingEnvironment, ITestService testService)
        {
            _hostingEnvironment = hostingEnvironment;
            _testService = testService;
        }

        [Produces("application/json")]
        [HttpPost, DisableRequestSizeLimit]
        public ActionResult UploadFile()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                // Extract keys from the submitted form
                //var formKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                //var fileId = formKeys["fileId"];         

                var file = Request.Form.Files[0];
                logger.Info("Received file name: " + file.FileName);

                string folderName = "Test";
                string contentRootPath = _hostingEnvironment.ContentRootPath;
                string newPath = Path.Combine(contentRootPath, folderName);
                logger.Info("Upload to: " + newPath);
                

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
                return Json("Test Upload Successful.");
            }
            catch (System.Exception ex)
            {
                return Json("Test Upload Failed: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetVideoById(string id)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Video file requested: " + id);

            var fs = _testService.GetVideoById(id);
            if (fs != null)
            {
                return new FileStreamResult(fs, new MediaTypeHeaderValue("video/webm").MediaType);
            }

            logger.Info("No file found, returning HTTP 400 BadRequest");
            return BadRequest();
        }
    }
}
