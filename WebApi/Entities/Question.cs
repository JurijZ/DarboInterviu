using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Interview { get; set; }
        public string QuestionText { get; set; }
        public int Duration { get; set; }
        public DateTime Timestamp { get; set; }
    }
}