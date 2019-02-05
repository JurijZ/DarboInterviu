using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Dtos;

namespace WebApi.Services
{
    public interface IQuestionService
    {
        IEnumerable<Question> GetAllByInterviewId(string interviewId);
        IEnumerable<Question> GetAll();
        Question GetById(string id);
        Question Create(Question interview);
        void Update(Question questionParam);
        void Delete(string id);
    }

    public class QuestionService : IQuestionService
    {
        private DataContext _context;

        public QuestionService(DataContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Question> GetAllByInterviewId(string interviewId)
        {
            return _context.Questions.Where(q => q.Interview == interviewId);
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
            _context.Questions.Add(question);
            _context.SaveChanges();

            return question;
        }

        public void Update(Question questionParam)
        {
            var question = _context.Questions.Find(questionParam.Id);

            if (question == null)
                throw new AppException("Question not found");

            // update question properties
            question.Text = questionParam.Text;
            question.Duration = questionParam.Duration;
            question.Order = questionParam.Order;
            question.Timestamp = questionParam.Timestamp;
            
            _context.Questions.Update(question);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var question = _context.Questions.Find(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                _context.SaveChanges();
            }
        }

    }
}