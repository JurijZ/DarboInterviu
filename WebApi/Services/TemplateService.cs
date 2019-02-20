using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Dtos;

namespace WebApi.Services
{
    public interface ITemplateService
    {
        IEnumerable<Template> GetAllByUserId(string userId);
        //Template GetById(string id);
        Template Create(Template template);
        void Update(Template template);
        void Delete(string id);
    }

    public class TemplateService : ITemplateService
    {
        private DataContext _context;

        public TemplateService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Template> GetAllByUserId(string userId)
        {
            return _context.Templates.Where(t => t.UserId == userId).OrderByDescending(i => i.Timestamp);
        }

        //public Template GetById(string id)
        //{
        //    return _context.Templates.Find(id);
        //}

        public Template Create(Template template)
        {
            _context.Templates.Add(template);
            _context.SaveChanges();

            return template;
        }

        public void Update(Template templateParam)
        {
            var template = _context.Templates.Find(templateParam.Id);

            if (template == null)
                throw new AppException("Template not found");

            // update question properties
            template.Name = templateParam.Name;
            template.Timestamp = DateTime.Now;

            _context.Templates.Update(template);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var template = _context.Templates.Find(id);
            if (template != null)
            {
                _context.Templates.Remove(template);
                _context.SaveChanges();
            }
        }

    }
}