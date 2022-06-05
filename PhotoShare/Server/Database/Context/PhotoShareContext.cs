using Microsoft.EntityFrameworkCore;

namespace PhotoShare.Server.Database.Context
{
    public class PhotoShareContext : DbContext
    {
        public PhotoShareContext(DbContextOptions options) : base(options) { }
    }
      
}
