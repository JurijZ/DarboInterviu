using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class Video
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ApplicationId { get; set; }
        public string QuestionId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime Timestamp { get; set; }
    }
}