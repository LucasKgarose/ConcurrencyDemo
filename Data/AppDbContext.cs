using ConcurrencyDemo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrencyDemo.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=localhost;Database=ConcurrencyDemoDB;Trusted_Connection=True;TrustServerCertificate=True;"
            );
        }
    }
}
