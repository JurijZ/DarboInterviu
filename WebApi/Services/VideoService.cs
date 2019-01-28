using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IVideoService
    {
        IEnumerable<Video> GetVideosMetadata();
        FileStream GetVideoById(string id);
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
    }

    public class VideoService : IVideoService
    {
        private DataContext _context;
        private IHostingEnvironment _hostingEnvironment;

        public VideoService(DataContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public FileStream GetVideoById(string id)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            string[] parts = new string[] { _hostingEnvironment.ContentRootPath, "Upload", id };
            string path = Path.Combine(parts);

            logger.Info("Looking for a file: " + path);

            if (File.Exists(path))
            {
                return new FileStream(path, FileMode.Open);
            }

            logger.Info("File was not found");
            return null;
        }


        public IEnumerable<Video> GetVideosMetadata()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            // Read files from the folder
            string folderName = "Upload";
            string contentRootPath = _hostingEnvironment.ContentRootPath; //_hostingEnvironment.WebRootPath;
            string targetDirectory = Path.Combine(contentRootPath, folderName);

            logger.Info("Video files are located in: " + targetDirectory);

            string[] fileEntries;

            try
            {
                if (Directory.Exists(targetDirectory))
                {
                    //fileEntries = Directory.GetFiles(targetDirectory);
                    //fileEntries = new string[] { "Folder exist" };
                    DirectoryInfo diTop = new DirectoryInfo(targetDirectory);

                    return diTop.EnumerateFiles().Select(index => new Video
                    {
                        Timestamp = index.LastWriteTime,
                        Candidate = "Test",
                        Name = index.Name
                    });
                }
                else
                {
                    fileEntries = new string[] { "Error - Folder does not exist" };

                    return fileEntries.Select(index => new Video
                    {
                        Timestamp = DateTime.Now,
                        Candidate = "Empty",
                        Name = fileEntries[0]
                    });
                }
            }
            catch (Exception e)
            {
                //logger.Info(e.Message);
                
                var l = new List<Video>()
                {
                    new Video
                    {
                        Timestamp = DateTime.Now,
                        Candidate = "Exception",
                        Name = e.Message
                    }
                };

                return l;
            }
        }


        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.Username = userParam.Username;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
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