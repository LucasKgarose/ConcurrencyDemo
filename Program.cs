using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ConcurrencyDemo.Data;
using ConcurrencyDemo.Models;

namespace ConcurrencyDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // 1️⃣ Seed data
            using (var context = new AppDbContext())
            {
                if (!context.People.Any())
                {
                    context.People.Add(new Person { FirstName = "John", LastName = "Doe" });
                    context.SaveChanges();
                    Console.WriteLine("Seeded John Doe in database.");
                }
            }

            // 2️⃣ Simulate concurrency conflict
            using (var context1 = new AppDbContext())
            using (var context2 = new AppDbContext())
            {
                var person1 = context1.People.Single(p => p.FirstName == "John");
                var person2 = context2.People.Single(p => p.FirstName == "John");

                // First context saves
                person1.FirstName = "Alice";
                context1.SaveChanges();
                Console.WriteLine("context1 saved FirstName = Alice");

                // Second context tries to save
                person2.FirstName = "Bob";

                try
                {
                    context2.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("⚠️ Concurrency Exception caught!");

                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is Person)
                        {
                            var databaseValues = entry.GetDatabaseValues();
                            Console.WriteLine($"Original attempted value: {entry.OriginalValues["FirstName"]}");
                            Console.WriteLine($"Current value tried to save: {entry.CurrentValues["FirstName"]}");
                            Console.WriteLine($"Database value now: {databaseValues["FirstName"]}");
                        }
                    }
                }
            }

            Console.WriteLine("Done!");
        }
    }
}
