using System;

namespace WebApi.Dtos
{
    public class ApplicationDto
    {
        public string Id { get; set; }
        public string InterviewId { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public DateTime Expiration { get; set; }
    }
}