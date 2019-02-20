using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Dtos;

namespace WebApi.Services
{
    public interface ISupportService
    {
        IEnumerable<Template> GetAllTemplates();
        IEnumerable<Application> GetAllApplications();
        IEnumerable<VideoDto> GetVideosMetadata();
    }

    public class SupportService : ISupportService
    {
        private DataContext _context;

        public SupportService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Template> GetAllTemplates()
        {
            return _context.Templates.OrderByDescending(t => t.Timestamp);
        }

        public IEnumerable<Application> GetAllApplications()
        {
            return _context.Applications.OrderByDescending(i => i.Timestamp);
        }

        public IEnumerable<VideoDto> GetVideosMetadata()
        {
            //var videos = _context.Videos.Where(v => v.ApplicationId == applicationId);

            var videoDtos =
               from videos in _context.Videos
               join questions in _context.Questions on videos.QuestionId equals questions.Id
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

    }
}