using System;
using Xunit;
using WebApi;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using Moq;
using WebApi.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using WebApi.Services;

namespace WebApi.Test
{
    public class UserServiceTests
    {
        [Fact]
        public void GetById_WhenUserExist_ShouldReturnUserInfo()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<DataContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new DataContext(options))
                {
                    context.Database.EnsureCreated();
                }

                // Insert seed data into the database using one instance of the context
                using (var context = new DataContext(options))
                {
                    context.Users.Add(new User { Id = "1", FirstName = "A", LastName = "B", Username = "ab@gmail.com" });
                    context.Users.Add(new User { Id = "2", FirstName = "C", LastName = "D", Username = "cd@gmail.com" });
                    context.SaveChanges();
                }

                // Use a clean instance of the context to run the test
                using (var context = new DataContext(options))
                {
                    var service = new UserService(context);
                    var result = service.GetById("2");
                    Assert.Equal("C", result.FirstName);
                }
            }
            finally
            {
                connection.Close();
            }

        }
    }
}
