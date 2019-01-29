using System;

namespace WebApi.Dtos
{
    public class QuestionDto
    {
        public string Id { get; set; }
        public string Interview { get; set; }
        public string Text { get; set; }
        public int Duration { get; set; }
        public int Order { get; set; }
    }
}