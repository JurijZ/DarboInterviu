using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class Interview
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Candidate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}