using ConcurrencyDemo.Application.Models;
using ConcurrencyDemo.Infrastructure.Data;
using ConcurrencyDemo.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace ConcurrencyDemo.Tests.Seeding
{
    public class PersonSeederTests
    {
        [Fact]
        public async Task SeedAsync_WhenEmpty_SeedsSinglePerson()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var services = new ServiceCollection();
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite(connection));
            services.AddSingleton<Microsoft.Extensions.Logging.ILogger<PersonSeeder>>(
                NullLogger<PersonSeeder>.Instance);
            services.AddScoped<PersonSeeder>();

            await using (var setup = services.BuildServiceProvider())
            {
                var factory = setup.GetRequiredService<IDbContextFactory<AppDbContext>>();
                await using var context = await factory.CreateDbContextAsync();
                await context.Database.EnsureCreatedAsync();
            }

            await using var provider = services.BuildServiceProvider();
            var seeder = provider.GetRequiredService<PersonSeeder>();

            await seeder.SeedAsync();
            await seeder.SeedAsync();

            var factoryAfter = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            await using var verifyContext = await factoryAfter.CreateDbContextAsync();

            var count = await verifyContext.People.CountAsync();
            var person = await verifyContext.People.SingleAsync();

            Assert.Equal(1, count);
            Assert.Equal("John", person.FirstName);
            Assert.Equal("Doe", person.LastName);
        }
    }
}
