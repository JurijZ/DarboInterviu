using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using Microsoft.Extensions.Logging;

namespace WebApi.Services
{
    public interface IAdminService
    {
        Admin Authenticate(string adminname, string password);
        IEnumerable<Admin> GetAll();
        Admin GetById(string id);
        Admin Create(Admin admin, string password);
        void Update(Admin admin, string password = null);
        void Delete(string id);
    }

    public class AdminService : IAdminService
    {
        private DataContext _context;
        private readonly ILogger _logger;

        public AdminService(
            DataContext context, 
            ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Admin Authenticate(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return null;

            var admin = _context.Admins.SingleOrDefault(x => x.UserName == userName);

            // check if adminname exists
            if (admin == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, admin.PasswordHash, admin.PasswordSalt))
                return null;

            // authentication successful
            return admin;
        }

        public IEnumerable<Admin> GetAll()
        {
            return _context.Admins;
        }

        public Admin GetById(string id)
        {
            _logger.LogInformation("Support Authentication");

            return _context.Admins.Find(id);
        }

        public Admin Create(Admin admin, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Admins.Any(x => x.UserName == admin.UserName))
                throw new AppException("Admin UserName \"" + admin.UserName + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            admin.PasswordHash = passwordHash;
            admin.PasswordSalt = passwordSalt;

            _context.Admins.Add(admin);
            _context.SaveChanges();

            return admin;
        }

        public void Update(Admin adminParam, string password = null)
        {
            var admin = _context.Admins.Find(adminParam.Id);

            if (admin == null)
                throw new AppException("Admin not found");

            if (adminParam.UserName != admin.UserName)
            {
                // adminname has changed so check if the new adminname is already taken
                if (_context.Admins.Any(x => x.UserName == adminParam.UserName))
                    throw new AppException("Admin UserName " + adminParam.UserName + " is already taken");
            }

            // update admin properties
            admin.FirstName = adminParam.FirstName;
            admin.LastName = adminParam.LastName;
            admin.UserName = adminParam.UserName;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                admin.PasswordHash = passwordHash;
                admin.PasswordSalt = passwordSalt;
            }

            _context.Admins.Update(admin);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var admin = _context.Admins.Find(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                _context.SaveChanges();
            }
        }

        // private helper methods

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

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}