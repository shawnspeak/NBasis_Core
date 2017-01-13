using System;

namespace NBasis.Commanding
{
    public class CommandHandlerInvocationException : Exception
    {
        public ICommandHandlingContext<ICommand> HandlingContext { get; set; }

        public CommandHandlerInvocationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
