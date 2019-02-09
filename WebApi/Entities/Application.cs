﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class Application
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string InterviewId { get; set; }
        public string Title { get; set; }
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }   
        public string CandidateSecret { get; set; }
        public string Status { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime Timestamp { get; set; }
    }
}