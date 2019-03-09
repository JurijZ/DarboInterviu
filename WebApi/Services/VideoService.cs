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
    public interface IVideoService
    {
        IEnumerable<VideoDto> GetVideosMetadataByInterviewId(string interviewId);
        Video SaveVideoMetaData(Video video);
        FileStream GetVideoById(string id);
    }

    public class VideoService : IVideoService
    {
        private DataContext _context;
        private IHostingEnvironment _hostingEnvironment;
        private readonly ILogger _logger;

        public VideoService(
            DataContext context, 
            IHostingEnvironment hostingEnvironment,
            ILogger<VideoService> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public Video SaveVideoMetaData(Video video)
        {
            _context.Videos.Add(video);
            _context.SaveChanges();

            return video;
        }

        public IEnumerable<VideoDto> GetVideosMetadataByInterviewId(string applicationId)
        {
            //var videos = _context.Videos.Where(v => v.ApplicationId == applicationId);

            var videoDtos =
               from videos in _context.Videos
               join questions in _context.ApplicationQuestions on videos.QuestionId equals questions.Id
               where videos.ApplicationId == applicationId
               select new VideoDto
               {
                   VideoId = videos.Id,
                   VideoFileName = videos.FileName,
                   VideoFileExists = true,
                   Question = questions.Text,
                   QuestionId = questions.Id,
                   Timestamp = videos.Timestamp
               };

            return videoDtos;
        }

        // Obsolete - Replaced with GetVideosMetadataByInterviewId
        public IEnumerable<Video> GetVideosMetadata()
        {
            // Read files from the folder
            string folderName = "Upload";
            string contentRootPath = _hostingEnvironment.ContentRootPath; //_hostingEnvironment.WebRootPath;
            string targetDirectory = Path.Combine(contentRootPath, folderName);

            _logger.LogInformation("Video files are located in: " + targetDirectory);

            string[] fileEntries;

            try
            {
                if (Directory.Exists(targetDirectory))
                {
                    //fileEntries = Directory.GetFiles(targetDirectory);
                    //fileEntries = new string[] { "Folder exist" };
                    DirectoryInfo diTop = new DirectoryInfo(targetDirectory);

                    return diTop.EnumerateFiles().Select(index => new Video
                    {
                        Timestamp = index.LastWriteTime,
                        FileName = index.Name
                    });
                }
                else
                {
                    fileEntries = new string[] { "Error - Folder does not exist" };

                    return fileEntries.Select(index => new Video
                    {
                        Timestamp = DateTime.Now,
                        FileName = fileEntries[0]
                    });
                }
            }
            catch (Exception e)
            {
                //logger.Info(e.Message);
                
                var l = new List<Video>()
                {
                    new Video
                    {
                        Timestamp = DateTime.Now,
                        FileName = e.Message
                    }
                };

                return l;
            }
        }

        public FileStream GetVideoById(string id)
        {
            string[] parts = new string[] { _hostingEnvironment.ContentRootPath, "Upload", id };
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