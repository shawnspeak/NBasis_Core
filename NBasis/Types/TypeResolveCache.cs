using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace NBasis.Types
{
    public class TypeResolveCache : ITypeResolveCache
    {
        readonly Func<Type, ObjectFactory> _createFactory = (type) => ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);
        readonly ConcurrentDictionary<Type, ObjectFactory> _typeResolveCache = new ConcurrentDictionary<Type, ObjectFactory>();

        public object Resolve(IServiceProvider serviceProvider, Type serviceType)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            var createFactory = _typeResolveCache.GetOrAdd(serviceType, _createFactory);
            return createFactory(serviceProvider, arguments: null);
        }
    }
}
