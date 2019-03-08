using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using Microsoft.Extensions.Logging;

namespace WebApi.Services
{
    public interface ICandidateService
    {
        int Authenticate(string username, string password);
        Application GetApplicationById(string applicationId);
        Application GetApplicationId(string email, string password);
        (int, int) GetInterviewOverview(string templateId);
        IEnumerable<Question> GetTemplateQuestionsByApplicationId(string id);
        string SetApplicationStatus(string id, string status);
    }

    public class CandidateService : ICandidateService
    {
        private DataContext _context;
        private readonly ILogger _logger;

        public CandidateService(
            DataContext context,
            ILogger<CandidateService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public int Authenticate(string email, string password)
        {
            _logger.LogInformation("Authenticating: " + email + "/" + password);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return 1;

            var application = _context.Applications.SingleOrDefault(x => x.CandidateEmail == email && x.CandidateSecret == password);

            // check if application exists 
            if (application == null)
            {
                return 1;
            }                

            // check if application is not expired            
            if (application.Expiration < DateTime.Now)
            {
                _logger.LogInformation("Candidate " + application.CandidateEmail + " requested expired interview: " + application.Expiration);
                return 2;
            }                

            // check if application/interview has already been completed            
            if (application.Status == "Completed")
            {
                _logger.LogInformation("Candidate " + application.CandidateEmail + "requested completed interview: " + application.CandidateEmail);
                return 3;
            }                

            // authentication successful
            return 0;
        }

        public Application GetApplicationById(string applicationId)
        {
            if (string.IsNullOrEmpty(applicationId))
                return null;

            var application = _context.Applications.SingleOrDefault(x => x.Id == applicationId);

            _logger.LogInformation("Extracted application template: " + application.Title);

            return application;
        }

        public Application GetApplicationId(string email, string password)
        {
            var application = _context.Applications.SingleOrDefault(x => x.CandidateEmail == email && x.CandidateSecret == password);

            _logger.LogInformation("Extracted application template: " + application.Title);

            return application;
        }

        public (int, int) GetInterviewOverview(string templateId)
        {
            var questions = _context.Questions.Where(x => x.TemplateId == templateId);

            var numberOfQuestions = questions.Count();
            var totalInterviewDuration = questions.Sum(q => q.Duration);

            _logger.LogInformation("Number of questions in the template: " + numberOfQuestions);
            _logger.LogInformation("Interview total duration: " + totalInterviewDuration);

            return (numberOfQuestions, totalInterviewDuration);
        }

        public string SetApplicationStatus(string id, string status)
        {
            var application = _context.Applications.FirstOrDefault(a => a.Id == id);
            if (application == null)
            {
                return null;
            }

            application.Status = status ?? application.Status;
            application.StatusTimestamp = DateTime.Now;

            _context.Applications.Update(application);
            _context.SaveChanges();

            //Send notification email to the Employer
            if (status == "Completed")
            {
                _logger.LogInformation(application.CandidateName + " completed interview.");

                // Get user emails by applicationID
                var applicationUserMaps = _context.ApplicationUserMaps.Where(a => a.ApplicationId == application.Id);

                var userEmails = (from u in _context.Users
                             join a in applicationUserMaps
                             on u.Id equals a.UserId
                             select u.Username).ToArray();

                MailgunAPI.SendInterviewCompletedEmailMessage(application, userEmails);
            }            

            return application.Status;
        }

        public IEnumerable<Question> GetTemplateQuestionsByApplicationId(string id)
        {
            var application = _context.Applications.FirstOrDefault(a => a.Id == id);

            if (application != null)
            {
                var questions = _context.Questions.Where(x => x.TemplateId == application.TemplateId);

                return questions;
            }
            else
            {
                return null;
            }
            
        }
    }
}