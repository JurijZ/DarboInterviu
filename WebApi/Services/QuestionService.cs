using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Dtos;
using Microsoft.Extensions.Logging;

namespace WebApi.Services
{
    public interface IQuestionService
    {
        IEnumerable<Question> GetAllByTemplateId(string templateId);
        IEnumerable<Question> GetAll();
        Question GetById(string id);
        Question Create(Question interview);
        void Update(Question questionParam);
        void Delete(string id);
    }

    public class QuestionService : IQuestionService
    {
        private DataContext _context;
        private readonly ILogger _logger;

        public QuestionService(
            DataContext context,
            ILogger<QuestionService> logger
            )
        {
            _context = context;
            _logger = logger;
        }
        
        public IEnumerable<Question> GetAllByTemplateId(string templateId)
        {
            return _context.Questions.Where(q => q.TemplateId == templateId);
        }

        public IEnumerable<Question> GetAll()
        {
            return _context.Questions;
        }

        public Question GetById(string id)
        {
            return _context.Questions.Find(id);
        }

        public Question Create(Question question)
        {
            // update template timestamp
            var template = _context.Templates.Find(question.TemplateId);            
            template.Timestamp = DateTime.Now;

            _context.Templates.Update(template);
            _context.Questions.Add(question);
            _context.SaveChanges();

            return question;
        }

        public void Update(Question questionParam)
        {
            var question = _context.Questions.Find(questionParam.Id);
            var template = _context.Templates.Find(questionParam.TemplateId);

            if (question == null)
                throw new AppException("Question not found");

            // update question properties
            question.Text = questionParam.Text;
            question.Duration = questionParam.Duration;
            question.Order = questionParam.Order;
            question.Timestamp = questionParam.Timestamp;

            // update template timestamp
            template.Timestamp = DateTime.Now;

            _context.Templates.Update(template);
            _context.Questions.Update(question);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var question = _context.Questions.Find(id);
            if (question != null)
            {
                // update template timestamp
                var template = _context.Templates.Find(question.TemplateId);
                template.Timestamp = DateTime.Now;

                _context.Templates.Update(template);

                // delete question
                _context.Questions.Remove(question);
                _context.SaveChanges();
            }
        }

    }
}