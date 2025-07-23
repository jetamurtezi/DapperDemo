using Microsoft.EntityFrameworkCore;

namespace DapperDemo.Models.Data
{
    public class ApplicationDbContext : DbContext
    {

      public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) : base(options)
      {
      }

        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
    
    }
}
