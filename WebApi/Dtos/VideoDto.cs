using System;

namespace WebApi.Dtos
{
    public class VideoDto
    {
        public int VideoId { get; set; }
        public string VideoFileName { get; set; }
        public bool VideoFileExists { get; set; }
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public DateTime Timestamp { get; set; }
    }
}