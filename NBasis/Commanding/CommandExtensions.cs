using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBasis.Commanding
{
    public static class CommandExtensions
    {
        public static Task Send(this ICommandBus bus, Envelope<ICommand> envelopedCommand)
        {
            return bus.SendAsync(envelopedCommand.Yield());
        }

        public static Task Send(this ICommandBus bus, ICommand command, IDictionary<string, object> headers = null)
        {
            return bus.Send(new Envelope<ICommand>(command, headers));
        }

        public static Task<TResult> Ask<TResult>(this ICommandBus bus, ICommand command, IDictionary<string, object> headers = null)
        {
            return bus.AskAsync<TResult>(new Envelope<ICommand>(command, headers));
        }

        public static IServiceCollection AddLocalCommanding(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                    .AddSingleton<ICommandBusFactory, LocalCommandBusFactory>()
                    .AddScoped<ICommandBus>((sp) =>
                    {
                        return sp.GetService<ICommandBusFactory>().GetCommandBus(new Container.NContainer(sp));
                    });
        }
    }
}
