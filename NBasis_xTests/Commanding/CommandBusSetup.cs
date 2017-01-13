using System;
using Microsoft.Extensions.DependencyInjection;
using NBasis.Types;
using NBasis.Commanding;
using Xunit;
using System.Reflection;

namespace NBasis_xTests
{
    public class CommandBusSetup
    {
        private IServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddAssembliesToFinder(typeof(CommandBusSetup).GetTypeInfo().Assembly)
                             .AddLocalCommanding();

            return serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void CommandBusFactoryIsRegistered()
        {
            var serviceProvider = GetServiceProvider();

            var busFactory = serviceProvider.GetService<ICommandBusFactory>();

            Assert.NotNull(busFactory);
        }

        [Fact]
        public void CommandBusIsRegistered()
        {
            var serviceProvider = GetServiceProvider();

            var bus = serviceProvider.GetService<ICommandBus>();

            Assert.NotNull(bus);
        }
    }
}
