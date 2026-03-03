using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ConcurrencyDemo.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Timestamp] // This is the concurrency token
        public byte[] RowVersion { get; set; }
    }
}
