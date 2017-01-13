using NBasis.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NBasis.Container
{
    public class NContainer : IContainer
    {
        readonly IServiceProvider _serviceProvider;

        public NContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Resolve(Type type)
        {
            // get type resolve cache
            var typeResolveCache = _serviceProvider.GetService(typeof(ITypeResolveCache)) as ITypeResolveCache;

            object ret = null;
            if (typeResolveCache != null)
                ret = typeResolveCache.Resolve(_serviceProvider, type);
            else
                ret = _serviceProvider.GetService(type);

            // handle out property injecting
            Inject(ret);

            return ret;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        private void Inject(object input)
        {
            if (input == null) return;

            // get all properties
            var properties = input.GetType().GetRuntimeProperties();

            // look for inject
            List<PropertyInfo> injectables = new List<PropertyInfo>();
            properties.ForEach(p =>
            {
                if (p.GetCustomAttributes<InjectAttribute>().Any())
                    injectables.Add(p);
            });

            // handle injection
            injectables.ForEach(i =>
            {
                try
                {
                    i.SetValue(input, _serviceProvider.GetService(i.PropertyType));
                }
                catch (Exception ex)
                {
                    throw new InjectionFailureException();
                }
            });
        }
    }
}
