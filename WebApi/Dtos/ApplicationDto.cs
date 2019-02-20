using System;

namespace WebApi.Dtos
{
    public class ApplicationDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string TemplateId { get; set; }
        public string Title { get; set; }
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }
        public DateTime Expiration { get; set; }
    }
}