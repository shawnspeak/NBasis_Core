using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NBasis_xTests.Models
{
    public class TestDbTests
    {
        [Fact]
        public void InMemoryDbIsLegit()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<TestDbContext>()
                    .UseSqlite(connection)
                    .Options;

                var id = Guid.NewGuid();

                using (var context = new TestDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new TestDbContext(options))
                {
                    // store an entity
                    context.Entities.Add(new TestEntity()
                    {
                        Id = id,
                        Name = "shawn"
                    });

                    context.SaveChanges();
                }

                using (var context = new TestDbContext(options))
                {
                    // get entity
                    var entity = context.Entities.Where(e => e.Id == id).FirstOrDefault();

                    Assert.NotNull(entity);
                    Assert.Equal("shawn", entity.Name);
                }
            }
            finally
            {
                connection.Close();
            }            
        }
    }
}
