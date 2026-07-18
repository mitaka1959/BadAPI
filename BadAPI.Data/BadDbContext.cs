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
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        public BadDbContext(DbContextOptions<BadDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BadDbContext).Assembly);
        }
    }
}