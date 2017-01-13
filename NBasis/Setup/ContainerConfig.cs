using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace NBasis.Setup
{
    public static class ContainerConfig
    {
        private static object _Lock = new object();
        private static IList<IContainerSetup> _Setups;

        //public static IServiceCollection ServiceCollection { get; private set; }

        //public static IServiceProvider Container { get; private set; }

        //public static void SetContainer(IUnityContainer container)
        //{
        //    if (Container != null)
        //        throw new ApplicationException("Container has already been set");

        //    Container = container;
        //}

        public static IServiceCollection SetupServices(this IServiceCollection collection, params IContainerSetup[] setups)
        {
            lock (_Lock)
            {
                if (_Setups != null)
                    return collection; // already built up
                _Setups = new List<IContainerSetup>();

                // if no container, create one automatically
                //ServiceCollection = new ServiceCollection();
                //if (ServiceCollection == null)
                //    SetContainer(new ServiceCollection());

                // run through build ups
                setups.ForEach((s) =>
                {
                    s.BuildUp(collection);
                    _Setups.Add(s);
                });

                //Container = ServiceCollection.BuildServiceProvider();
                return collection;
            }
        }

        public static void TearDownServices()
        {
            lock (_Lock)
            {
                if (_Setups == null)
                    return; // already torn down

                // run through teardowns
                _Setups.ForEach((s) =>
                {
                    s.TearDown();
                });

                _Setups.Clear();
                _Setups = null;

                //ServiceCollection.Dispose();
                //ServiceCollection = null;
            }
        }
    }
}
