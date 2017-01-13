using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NBasis.Types
{
    public static class TypesExtensions
    {
        public static IServiceCollection AddAssembliesToFinder(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            return serviceCollection
                    .AddSingleton<ITypeResolveCache, TypeResolveCache>()
                    .AddSingleton<ITypeFinder>(new AssemblyTypeFinder(assemblies));
        }
    }
}
