using Microsoft.Extensions.DependencyInjection;
using NBasis.Querying;
using NBasis.Types;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace NBasis_xTests.Querying
{
    public class SimpleQuery : IQuery<SimpleResult>
    {
        public bool ReturnNull { get; set; }
    }

    public class SimpleResult
    {
        public string Name { get; set; }
    }

    public class SimpleQueryHandler : QueryHandler<SimpleQuery, SimpleResult>
    {
        public override Task<SimpleResult> Handle(SimpleQuery query)
        {
            if (query.ReturnNull)
                return Task.FromResult<SimpleResult>(null);

            return Task.FromResult(new SimpleResult()
            {
                Name = "Hello"
            });
        }
    }

    public class QueryStructureTests
    {
        private IServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddAssembliesToFinder(typeof(QueryStructureTests).GetTypeInfo().Assembly)
                             .AddLocalQueryProcessing();

            return serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task SimpleQueryStructureTestAsync()
        {
            var serviceProvider = GetServiceProvider();

            var processor = serviceProvider.GetService<IQueryProcessor>();

            var result = await processor.ExecuteAsync(new SimpleQuery() { ReturnNull = false });

            Assert.NotNull(result);
            Assert.Equal("Hello", result.Name);

            result = await processor.ExecuteAsync(new SimpleQuery() { ReturnNull = true });

            Assert.Null(result);
        }
    }
}
