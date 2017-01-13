using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace NBasis.Types
{
    public class AssemblyTypeFinder : ITypeFinder
    {
        readonly IEnumerable<Type> _allTypes;

        public AssemblyTypeFinder(IEnumerable<Assembly> assemblies)
        {
            _allTypes = LoadTypes(assemblies);
        }

        public IEnumerable<Type> AllLoadedTypes
        {
            get { return _allTypes; }
        }

        public IEnumerable<Type> GetDerivedTypes<TBase>() where TBase : class
        {
            var type = typeof(TBase);
            return (from derivedType in _allTypes
                      where type != derivedType
                      where type.GetTypeInfo().IsAssignableFrom(derivedType.GetTypeInfo())
                      select derivedType).ToArray();
        }

        public IEnumerable<Type> GetInterfaceImplementations<TInterface>() where TInterface : class
        {
            return GetInterfaceImplementations(typeof(TInterface));
        }

        public IEnumerable<Type> GetInterfaceImplementations(Type type)
        {
            return (from derivedType in _allTypes
                    where !derivedType.GetTypeInfo().IsInterface
                    from interfaceType in derivedType.GetTypeInfo().ImplementedInterfaces
                    where interfaceType == type
                    select derivedType).Distinct().ToArray();
        }

        private static IEnumerable<Type> LoadTypes(IEnumerable<Assembly> assemblies)
        {
            var loadedTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.ExportedTypes;
                    loadedTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException exception)
                {
                    exception.LoaderExceptions
                        .Select(e => e.Message)
                        .Distinct().ToList()
                        .ForEach(message => Debug.WriteLine(message));
                }
            }

            return loadedTypes;
        }
    }
}
