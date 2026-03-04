using System;
using System.ComponentModel.DataAnnotations;

namespace ConcurrencyDemo.Application.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
