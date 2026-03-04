using ConcurrencyDemo.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcurrencyDemo.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> People => Set<Person>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var rowVersion = modelBuilder.Entity<Person>()
                .Property(p => p.RowVersion)
                .IsConcurrencyToken();

            if (Database.ProviderName?.Contains("Sqlite") == true)
            {
                rowVersion.ValueGeneratedNever();
            }
            else
            {
                rowVersion.IsRowVersion();
            }
        }
    }
}
