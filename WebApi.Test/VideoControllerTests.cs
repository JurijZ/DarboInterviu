using System;
using Xunit;
using WebApi;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using Moq;
using WebApi.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using WebApi.Controllers;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Test
{
    public class VideoControllerTests
    {
        [Fact]
        public void Videocontroller_WhenVideoExist_ShouldReturnVideoStream()
        {
            var mock1 = new Mock<IHostingEnvironment>();
            var mock2 = new Mock<IVideoService>();
            var mock3 = new Mock<IMapper>();
            var mock4 = new Mock<IOptions<AppSettings>>();

            // Mock the method of the service
            var f = GetFileStream();
            mock2.Setup(p => p.GetVideoById("1")).Returns(f);
            
            VideoController videoController = new VideoController(mock1.Object, mock2.Object, mock3.Object, mock4.Object);

            // Act
            var response = videoController.GetVideoById("1");

            // Assert
            Assert.NotNull(response);
            Assert.IsType<FileStreamResult>(response);
            Assert.Equal(31327, ((FileStreamResult)response).FileStream.Length);
            
        }

        private FileStream GetFileStream()
        {
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string file = dir + Path.DirectorySeparatorChar + "TestFiles" + Path.DirectorySeparatorChar + "Test.webm";
            return new FileStream(file, FileMode.Open);
        }
    }
}
