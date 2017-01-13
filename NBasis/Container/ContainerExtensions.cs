using System;

namespace NBasis.Container
{
    public static class ContainerExtensions
    {
        public static TInstance Resolve<TInstance>(this IServiceProvider serviceProvider)
        {
            return new NContainer(serviceProvider).Resolve<TInstance>();
        }
    }
}
