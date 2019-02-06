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
        Application Create(Application application);
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
            return _context.Applications.Where(q => q.UserId == userId);
        }

        public Application GetByApplicationId(string id)
        {
            return _context.Applications.Find(id);
        }

        public Application Create(Application application)
        {
            _context.Applications.Add(application);
            _context.SaveChanges();

            return application;
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
    }
}