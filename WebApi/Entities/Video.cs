using System;

namespace WebApi.Entities
{
    public class Video
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Candidate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}