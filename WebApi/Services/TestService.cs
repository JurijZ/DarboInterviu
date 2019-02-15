using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Dtos;

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

        public TestService(DataContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        
        public FileStream GetVideoById(string id)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            string[] parts = new string[] { _hostingEnvironment.ContentRootPath, "Test", id };
            string path = Path.Combine(parts);

            logger.Info("Looking for a file: " + path);

            if (File.Exists(path))
            {
                return new FileStream(path, FileMode.Open);
            }

            logger.Info("File was not found");
            return null;
        }
    }
}