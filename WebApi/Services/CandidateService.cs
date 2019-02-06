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

            var application = _context.Applications.SingleOrDefault(x => x.Email == email && x.UserId == password);

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

            // authentication successful
            return application;
        }

        public Application GetApplicationId(string email, string password)
        {
            var application = _context.Applications.SingleOrDefault(x => x.Email == email && x.UserId == password);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Extracted application interview: " + application.Title);

            // authentication successful
            return application;
        }
    }
}