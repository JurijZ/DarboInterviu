using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class Template
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }        
        public DateTime Timestamp { get; set; }
    }
}