using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface ICandidateService
    {
        int Authenticate(string username, string password);
        Application GetApplicationById(string applicationId);
        Application GetApplicationId(string email, string password);
        (int, int) GetInterviewOverview(string interviewId);
        IEnumerable<Question> GetInterviewQuestionsByApplicationId(string id);
        string SetInterviewStatus(string id, string status);
    }

    public class CandidateService : ICandidateService
    {
        private DataContext _context;

        public CandidateService(DataContext context)
        {
            _context = context;
        }

        public int Authenticate(string email, string password)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Authenticating: " + email + "/" + password);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return 1;

            var application = _context.Applications.SingleOrDefault(x => x.CandidateEmail == email && x.CandidateSecret == password);

            // check if application exists 
            if (application == null)
                return 1;

            // check if application is not expired
            logger.Info("Interview expiration: " + application.Expiration);
            if (application.Expiration < DateTime.Now)
                return 2;

            // authentication successful
            return 0;
        }

        public Application GetApplicationById(string applicationId)
        {
            if (string.IsNullOrEmpty(applicationId))
                return null;

            var application = _context.Applications.SingleOrDefault(x => x.Id == applicationId);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Extracted application interview: " + application.Title);

            return application;
        }

        public Application GetApplicationId(string email, string password)
        {
            var application = _context.Applications.SingleOrDefault(x => x.CandidateEmail == email && x.CandidateSecret == password);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Extracted application interview: " + application.Title);

            return application;
        }

        public (int, int) GetInterviewOverview(string interviewId)
        {
            var questions = _context.Questions.Where(x => x.Interview == interviewId);

            var numberOfQuestions = questions.Count();
            var totalInterviewDuration = questions.Sum(q => q.Duration);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Number of questions in the interview: " + numberOfQuestions);
            logger.Info("Interview total duration: " + totalInterviewDuration);

            return (numberOfQuestions, totalInterviewDuration);
        }

        public string SetInterviewStatus(string id, string status)
        {
            var application = _context.Applications.FirstOrDefault(a => a.Id == id);
            if (application == null)
            {
                return null;
            }

            application.Status = status ?? application.Status;

            _context.Applications.Update(application);
            _context.SaveChanges();

            return application.Status;
        }

        public IEnumerable<Question> GetInterviewQuestionsByApplicationId(string id)
        {
            var application = _context.Applications.FirstOrDefault(a => a.Id == id);

            if (application != null)
            {
                var questions = _context.Questions.Where(x => x.Interview == application.InterviewId);

                return questions;
            }
            else
            {
                return null;
            }
            
        }
    }
}