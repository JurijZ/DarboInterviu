using System;

namespace WebApi.Dtos
{
    public class InterviewDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}