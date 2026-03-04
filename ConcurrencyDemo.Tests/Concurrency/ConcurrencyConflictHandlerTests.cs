using ConcurrencyDemo.Application.Models;
using ConcurrencyDemo.Infrastructure.Data;
using ConcurrencyDemo.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ConcurrencyDemo.Tests.Concurrency
{
    public class ConcurrencyConflictHandlerTests
    {
        [Fact]
        public async Task HandleAsync_StoreWins_ResetsEntryToDatabaseValues()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            await using (var setup = new TestDbContext(options))
            {
                await setup.Database.EnsureCreatedAsync();
                setup.People.Add(new Person
                {
                    FirstName = "John",
                    LastName = "Doe",
                    RowVersion = new byte[] { 1 }
                });
                await setup.SaveChangesAsync();
            }

            await using var context1 = new TestDbContext(options);
            await using var context2 = new TestDbContext(options);

            var person1 = await context1.People.SingleAsync();
            var person2 = await context2.People.SingleAsync();

            person1.FirstName = "Alice";
            person1.RowVersion = new byte[] { 2 };
            await context1.SaveChangesAsync();

            person2.FirstName = "Bob";
            person2.RowVersion = new byte[] { 3 };

            var handler = new ConcurrencyConflictHandler(
                NullLogger<ConcurrencyConflictHandler>.Instance);

            var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                () => context2.SaveChangesAsync());

            await handler.HandleAsync(exception);

            var entry = exception.Entries.Single();

            Assert.Equal(EntityState.Unchanged, entry.State);
            Assert.Equal("Alice", entry.CurrentValues[nameof(Person.FirstName)]);
        }

        private sealed class TestDbContext : AppDbContext
        {
            public TestDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Person>()
                    .Property(p => p.RowVersion)
                    .IsConcurrencyToken()
                    .ValueGeneratedNever();
            }
        }
    }
}
