using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NBasis.Querying
{
    public static class QueryProcessorExtensions
    {
        public static Task<TResult> ExecuteAsync<TResult>(this IQueryProcessor proc, IQuery<TResult> query)
        {
            // get type of query
            var queryType = query.GetType();
            var resultType = typeof(TResult);

            // build envelope
            var envelopeType = typeof(Envelope<>).MakeGenericType(queryType);
            var envelope = Activator.CreateInstance(envelopeType, query);

            var methodInfo = typeof(IQueryProcessor).GetTypeInfo().GetDeclaredMethod("ExecuteAsync");
            
            return (Task<TResult>)methodInfo.MakeGenericMethod(queryType, resultType).Invoke(proc, new[] { envelope });
        }

        public static IServiceCollection AddLocalQueryProcessing(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                    .AddSingleton<IQueryProcessorFactory, LocalQueryProcessorFactory>()
                    .AddScoped<IQueryProcessor>((sp) =>
                    {
                        return sp.GetService<IQueryProcessorFactory>().GetQueryProcessor(new Container.NContainer(sp));
                    });
        }
    }
}
