using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NBasis.Models;
using NBasis.Querying;
using NBasis.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NBasis_xTests.Models
{
    public class GetEntityQuery : IQuery<TestEntity>
    {
        public Guid Id { get; set; }
    }

    public class GetEntityQueryHandler : StatelessQueryHandler<GetEntityQuery, TestEntity>
    {
        public override Task<TestEntity> Handle(GetEntityQuery query, IModelSession session)
        {
            return session.LoadAsync<TestEntity, Guid>(query.Id);
        }
    }

    public class TestStatelessQueryHandler
    {
        [Fact]
        public async Task StatelessQueryHandlerWorks()
        {
            // setup db
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                GlobalOptions.Options = new DbContextOptionsBuilder<TestDbContext>()
                    .UseSqlite(connection)
                    .Options;

                var id = Guid.NewGuid();

                using (var context = new TestDbContext())
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new TestDbContext())
                {
                    // store an entity
                    context.Entities.Add(new TestEntity()
                    {
                        Id = id,
                        Name = "shawn"
                    });

                    context.SaveChanges();
                }


                var serviceCollection = new ServiceCollection();

                serviceCollection.AddAssembliesToFinder(typeof(TestStatelessQueryHandler).GetTypeInfo().Assembly)
                                 .AddModels<TestDbContext>()
                                 .AddLocalQueryProcessing()
                                 .AddSingleton(new ConfigurationBuilder().Build());

                var serviceProvider = serviceCollection.BuildServiceProvider();

                var processor = serviceProvider.GetService<IQueryProcessor>();

                var entity = await processor.ExecuteAsync(new GetEntityQuery() { Id = id });

                Assert.NotNull(entity);
                Assert.Equal("shawn", entity.Name);
            }
            finally
            {
                connection.Close();
            }

        }
    }
}
