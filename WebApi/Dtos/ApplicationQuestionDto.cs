using System;

namespace WebApi.Dtos
{
    public class ApplicationQuestionDto
    {
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public string Text { get; set; }
        public int Duration { get; set; }
        public int Order { get; set; }
    }
}