using Microsoft.EntityFrameworkCore;
using PhotoShare.Shared;

namespace PhotoShare.Server.Database.Context
{
    public class PhotoShareContext : DbContext
    {
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupKey> GroupKeys { get; set; }
        public PhotoShareContext() { }
        public PhotoShareContext(DbContextOptions options) : base(options) {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
      
}
