using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Dtos;

namespace WebApi.Services
{
    public interface IApplicationService
    {
        IEnumerable<Application> GetAllByUserId(string userId);
        Application GetByApplicationId(string id);
        Application Create(Application application, string userId);
        string Share(string userId, string applicationId, string email);
        void Delete(string id);
        int GetRandomNumber(int min, int max);
    }

    public class ApplicationService : IApplicationService
    {
        private DataContext _context;
        private static readonly Random getrandom = new Random();

        public ApplicationService(DataContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Application> GetAllByUserId(string userId)
        {
            var applicationIds = _context.ApplicationUserMaps.Where(i => i.UserId == userId);

            var result = from a in _context.Applications
                         join i in applicationIds
                         on a.Id equals i.ApplicationId
                         select a;

            return result;

            //var v = _context.Applications.Where(q => q.UserId == userId);
        }

        public Application GetByApplicationId(string id)
        {
            return _context.Applications.Find(id);
        }

        public Application Create(Application application, string userId)
        {
            _context.Applications.Add(application);
            _context.ApplicationUserMaps.Add(new ApplicationUserMap { ApplicationId = application.Id, UserId = userId });

            _context.SaveChanges();

            // Send an email to the Candidate
            var x = AmazonAPI.SendApplicationEmailMessage(application);

            return application;
        }

        public string Share(string userId, string applicationId, string email)
        {
            //Check if the user with the email already exist
            if (_context.Users.Any(u => u.Username == email))
            {
                _context.ApplicationUserMaps.Add(new ApplicationUserMap { ApplicationId = applicationId, UserId = userId });

                _context.SaveChanges();
            }
            else //If not then create one
            {
                User user = new User();
                user.Username = email;

                string password = RandomString(8);

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("New user: " + email + " with password " + password + " was created");

                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _context.Users.Add(user);

                _context.ApplicationUserMaps.Add(new ApplicationUserMap { ApplicationId = applicationId, UserId = user.Id });

                _context.SaveChanges();
            }
            
            // Send an email to the User informing about the Share
            //var x = AmazonAPI.SendApplicationShareEmailMessage(application);

            return "OK";
        }

        public void Delete(string id)
        {
            var question = _context.Applications.Find(id);
            if (question != null)
            {
                _context.Applications.Remove(question);
                _context.SaveChanges();
            }
        }

        public int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[getrandom.Next(s.Length)]).ToArray());
        }
    }
}