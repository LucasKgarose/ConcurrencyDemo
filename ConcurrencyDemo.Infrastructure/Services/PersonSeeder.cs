using ConcurrencyDemo.Application.Models;
using ConcurrencyDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConcurrencyDemo.Infrastructure.Services
{
    public class PersonSeeder
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogger<PersonSeeder> _logger;

        public PersonSeeder(
            IDbContextFactory<AppDbContext> contextFactory,
            ILogger<PersonSeeder> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            if (await context.People.AnyAsync(cancellationToken))
            {
                return;
            }

            context.People.Add(new Person { FirstName = "John", LastName = "Doe" });
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded John Doe in database.");
        }
    }
}
