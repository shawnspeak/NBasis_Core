using System;

namespace NBasis.Types
{
    interface ITypeResolveCache
    {
        object Resolve(IServiceProvider serviceProvider, Type serviceType);
    }
}
