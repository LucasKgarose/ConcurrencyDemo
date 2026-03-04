using ConcurrencyDemo.Application.Services;
using ConcurrencyDemo.Infrastructure.Data;
using ConcurrencyDemo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConcurrencyDemo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found.");
            }

            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<PersonSeeder>();
            services.AddScoped<ConcurrencyConflictHandler>();
            services.AddScoped<IConcurrencyDemoService, ConcurrencyDemoService>();

            return services;
        }
    }
}
