using System;
using Microsoft.Extensions.DependencyInjection;
using NBasis.Types;
using NBasis.Querying;
using Xunit;
using System.Reflection;

namespace NBasis_xTests
{
    public class QueryProcessorSetup
    {
        private IServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddAssembliesToFinder(typeof(CommandBusSetup).GetTypeInfo().Assembly)
                             .AddLocalQueryProcessing();

            return serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void QueryProcessorFactoryIsRegistered()
        {
            var serviceProvider = GetServiceProvider();

            var factory = serviceProvider.GetService<IQueryProcessorFactory>();

            Assert.NotNull(factory);
        }

        [Fact]
        public void QueryProcessorIsRegistered()
        {
            var serviceProvider = GetServiceProvider();

            var processor = serviceProvider.GetService<IQueryProcessor>();

            Assert.NotNull(processor);
        }
    }
}
