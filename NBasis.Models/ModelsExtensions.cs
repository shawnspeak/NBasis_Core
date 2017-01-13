using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NBasis.Models
{
    public static class ModelsExtensions
    {
        public static IServiceCollection AddModels<TContext>(this IServiceCollection serviceCollection) where TContext : DbContext, new()
        {
            return serviceCollection
                     .AddScoped<IModelSessionFactory, ModelSessionFactory<TContext>>() // per request
                     .AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
