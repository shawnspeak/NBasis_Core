using System.Collections.Generic;

namespace NBasis.Commanding
{
    public interface ICommandHandlingContext<out TCommand> where TCommand : ICommand
    {
        TCommand Command { get; }

        IDictionary<string, object> Headers { get; }

        string CorrelationId { get; }

        void SetReturn(object value);

        T GetReturn<T>();
    }
}
