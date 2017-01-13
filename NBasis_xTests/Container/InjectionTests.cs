using Microsoft.Extensions.DependencyInjection;
using NBasis.Container;
using Xunit;

namespace NBasis_xTests.Container
{
    public class InjectionTests
    {
        public class ServiceA
        {
            public string Name { get { return "ServiceA"; } }
        }

        public class ServiceB
        {
            public string Name { get { return "ServiceB"; } }

            [Inject]
            public ServiceA ServiceA { get; set; }
        }

        [Fact]
        public void InjectAttributeIsWorking()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<ServiceA>()
                             .AddTransient<ServiceB>();

            var sp =  serviceCollection.BuildServiceProvider();

            var container = new NContainer(sp);

            var injectable = container.Resolve<ServiceB>();

            Assert.NotNull(injectable);
            Assert.Equal("ServiceB", injectable.Name);
            Assert.NotNull(injectable.ServiceA);
            Assert.Equal("ServiceA", injectable.ServiceA.Name);
        }
    }
}
