using BadAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading;
using System.Runtime;
using System.Security;
using System.Timers;

namespace BadApi.Data
{
    public class BadDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public BadDbContext()
        {
        }

        public BadDbContext(DbContextOptions<BadDbContext> options)
            :base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=BadApiDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}