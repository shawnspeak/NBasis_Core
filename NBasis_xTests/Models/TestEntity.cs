using System;
using NBasis.Models;
using System.ComponentModel.DataAnnotations;

namespace NBasis_xTests.Models
{
    public class TestEntity : IEntity<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }
    }
}
