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
        IEnumerable<TemplateQuestion> GetAllByTemplateId(string templateId);
        IEnumerable<TemplateQuestion> GetAll();
        TemplateQuestion GetById(string id);
        TemplateQuestion Create(TemplateQuestion interview);
        void Update(TemplateQuestion questionParam);
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
        
        public IEnumerable<TemplateQuestion> GetAllByTemplateId(string templateId)
        {
            return _context.TemplateQuestions.Where(q => q.TemplateId == templateId);
        }

        public IEnumerable<TemplateQuestion> GetAll()
        {
            return _context.TemplateQuestions;
        }

        public TemplateQuestion GetById(string id)
        {
            return _context.TemplateQuestions.Find(id);
        }

        public TemplateQuestion Create(TemplateQuestion question)
        {
            // update template timestamp
            var template = _context.Templates.Find(question.TemplateId);            
            template.Timestamp = DateTime.Now;

            _context.Templates.Update(template);
            _context.TemplateQuestions.Add(question);
            _context.SaveChanges();

            return question;
        }

        public void Update(TemplateQuestion questionParam)
        {
            var question = _context.TemplateQuestions.Find(questionParam.Id);
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
            _context.TemplateQuestions.Update(question);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var question = _context.TemplateQuestions.Find(id);
            if (question != null)
            {
                // update template timestamp
                var template = _context.Templates.Find(question.TemplateId);
                template.Timestamp = DateTime.Now;

                _context.Templates.Update(template);

                // delete question
                _context.TemplateQuestions.Remove(question);
                _context.SaveChanges();
            }
        }

    }
}