using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class ApplicationQuestion
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public string Text { get; set; }
        public int Duration { get; set; }
        public int Order { get; set; }
        public DateTime Timestamp { get; set; }
    }
}