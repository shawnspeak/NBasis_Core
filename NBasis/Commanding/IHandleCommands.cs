using System.Threading.Tasks;

namespace NBasis.Commanding
{
    /// <summary>
    /// Marker interface
    /// </summary>
    public interface IHandleCommands
    {
    }

    public interface IHandleCommands<TCommand> : IHandleCommands where TCommand : ICommand
    {
        Task Handle(ICommandHandlingContext<TCommand> handlingContext);
    }
}
