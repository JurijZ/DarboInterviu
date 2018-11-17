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

namespace WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SampleDataController : ControllerBase
    {
        //private readonly ILogger<SampleDataController> _logger;

        //public SampleDataController(ILogger<SampleDataController> logger)
        //{
        //    _logger = logger;
        //}
        private IHostingEnvironment _hostingEnvironment;

        public SampleDataController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();            

            // Read files from the folder
            string folderName = "Upload";
            string contentRootPath = _hostingEnvironment.ContentRootPath; //_hostingEnvironment.WebRootPath;
            string targetDirectory = Path.Combine(contentRootPath, folderName);

            logger.Info("ContentRootPath is: " + targetDirectory);

            string[] fileEntries;

            try
            {
                if (Directory.Exists(targetDirectory))
                {
                    //fileEntries = Directory.GetFiles(targetDirectory);
                    //fileEntries = new string[] { "Folder exist" };
                    DirectoryInfo diTop = new DirectoryInfo(targetDirectory);

                    return diTop.EnumerateFiles().Select(index => new WeatherForecast
                    {
                        DateFormatted = index.LastWriteTime.ToString(),
                        TemperatureC = 5,
                        Summary = index.Name
                    });
                }
                else
                {
                    fileEntries = new string[] { "Error - Folder does not exist" };

                    var rng = new Random();
                    return fileEntries.Select(index => new WeatherForecast
                    {
                        DateFormatted = DateTime.Now.ToString("d"),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = fileEntries[0]
                    });
                }
            }
            catch (Exception e)
            {
                
                //logger.Info(e.Message);

                var rng = new Random();

                var l = new List<WeatherForecast>()
                {
                    new WeatherForecast
                    {
                        DateFormatted = DateTime.Now.ToString("d"),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = "Exception happened"
                    }
                };

                return l;
            }                      
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[DisableFormValueModelBinding]
        //public async Task<IActionResult> UploadStreamingFile()
        //{
        //    // full path to file in temp location
        //    var filePath = Path.GetTempFileName();

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await Request.StreamFile(stream);
        //    }

        //    // process uploaded files
        //    // Don't rely on or trust the FileName property without validation.

        //    return Ok(new { filePath });
        //}

        
        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }
}
