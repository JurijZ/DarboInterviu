using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<Interview> Interviews { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
    }
}