using ConcurrencyDemo.Application.Services;
using ConcurrencyDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConcurrencyDemo.Infrastructure.Services
{
    public class ConcurrencyDemoService : IConcurrencyDemoService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly PersonSeeder _seeder;
        private readonly ConcurrencyConflictHandler _conflictHandler;
        private readonly ILogger<ConcurrencyDemoService> _logger;

        public ConcurrencyDemoService(
            IDbContextFactory<AppDbContext> contextFactory,
            PersonSeeder seeder,
            ConcurrencyConflictHandler conflictHandler,
            ILogger<ConcurrencyDemoService> logger)
        {
            _contextFactory = contextFactory;
            _seeder = seeder;
            _conflictHandler = conflictHandler;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            await _seeder.SeedAsync(cancellationToken);

            await using var context1 = await _contextFactory.CreateDbContextAsync(cancellationToken);
            await using var context2 = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var person1 = await context1.People
                .SingleAsync(p => p.FirstName == "John" && p.LastName == "Doe", cancellationToken);
            var person2 = await context2.People
                .SingleAsync(p => p.FirstName == "John" && p.LastName == "Doe", cancellationToken);

            person1.FirstName = "Alice";
            await context1.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("context1 saved FirstName = Alice");

            person2.FirstName = "Bob";

            try
            {
                await context2.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning("Concurrency exception caught.");
                await _conflictHandler.HandleAsync(ex, cancellationToken);
            }

            _logger.LogInformation("Done!");
        }
    }
}
