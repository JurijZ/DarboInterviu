using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Dtos;
using Microsoft.Extensions.Logging;

namespace WebApi.Services
{
    public interface ITestService
    {
        FileStream GetVideoById(string id);
    }

    public class TestService : ITestService
    {
        private DataContext _context;
        private IHostingEnvironment _hostingEnvironment;
        private readonly ILogger _logger;

        public TestService(
            DataContext context, 
            IHostingEnvironment hostingEnvironment,
            ILogger<TestService> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }
        
        public FileStream GetVideoById(string id)
        {
            string[] parts = new string[] { _hostingEnvironment.ContentRootPath, "Test", id };
            string path = Path.Combine(parts);

            _logger.LogInformation("Looking for a file: " + path);

            if (File.Exists(path))
            {
                return new FileStream(path, FileMode.Open);
            }

            _logger.LogInformation("File was not found");
            return null;
        }
    }
}