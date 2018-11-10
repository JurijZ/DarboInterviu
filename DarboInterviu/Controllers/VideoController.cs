using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace app1.Controllers
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
            string webRootPath = _hostingEnvironment.WebRootPath;
            string path = Path.Combine(webRootPath, folderName);            

            if (System.IO.File.Exists(path))
            {
                var fs = new FileStream(path, FileMode.Open);
                return new FileStreamResult(fs, new MediaTypeHeaderValue("video/webm").MediaType);
            }

            return BadRequest();
        }
    }
}