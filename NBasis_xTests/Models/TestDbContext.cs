using Microsoft.EntityFrameworkCore;

namespace NBasis_xTests.Models
{
    public static class GlobalOptions
    {
        public static DbContextOptions<TestDbContext> Options;
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext() : base(GlobalOptions.Options)
        {

        }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {

        }

        public DbSet<TestEntity> Entities { get; set; }
    }
}
