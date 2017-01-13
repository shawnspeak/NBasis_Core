using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace NBasis.Commanding
{
    public abstract class CommandHandler<TCommand> : IHandleCommands<TCommand> where TCommand : ICommand
    {
        private ICommandHandlingContext<TCommand> context;

        Task IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            context = handlingContext;

            Validate(handlingContext.Command);

            return Handle(handlingContext.Command);
        }

        public IDictionary<string, object> Headers
        {
            get
            {
                if ((context != null) && (context.Headers != null))
                    return context.Headers;
                return null;
            }
        }

        public string CorrelationId
        {
            get
            {
                if (context != null)
                    return context.CorrelationId;
                return null;
            }
        }

        public virtual void Validate(TCommand command)
        {
            var validationContext = new ValidationContext(command, null, null);
            Validator.ValidateObject(command, validationContext, true);
        }

        public abstract Task Handle(TCommand command);

        protected void SetReturnValue(object value)
        {
            context.SetReturn(value);
        }
    }
}
