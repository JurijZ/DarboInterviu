using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Dtos;

namespace WebApi.Services
{
    public interface IInterviewService
    {
        IEnumerable<Interview> GetAll();
        Interview GetById(string id);
        Interview Create(Interview interview);
        void Update(Interview interview);
        void Delete(string id);
    }

    public class InterviewService : IInterviewService
    {
        private DataContext _context;

        public InterviewService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Interview> GetAll()
        {
            return _context.Interviews.OrderByDescending(i => i.Timestamp);
        }

        public Interview GetById(string id)
        {
            return _context.Interviews.Find(id);
        }

        public Interview Create(Interview interview)
        {
            _context.Interviews.Add(interview);
            _context.SaveChanges();

            return interview;
        }

        public void Update(Interview interviewParam)
        {
            var interview = _context.Interviews.Find(interviewParam.Id);

            if (interview == null)
                throw new AppException("Interview not found");

            // update question properties
            interview.Name = interviewParam.Name;
            interview.Timestamp = DateTime.Now;

            _context.Interviews.Update(interview);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var interview = _context.Interviews.Find(id);
            if (interview != null)
            {
                _context.Interviews.Remove(interview);
                _context.SaveChanges();
            }
        }

    }
}