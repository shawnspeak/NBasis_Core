using NBasis.Commanding;
using NBasis.Container;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace NBasis.Models
{
    public abstract class SessionCommandHandler<TCommand> : IHandleCommands<TCommand> where TCommand : ICommand
    {
        private ICommandHandlingContext<TCommand> _context;

        [Inject]
        public IModelSessionFactory SessionFactory { get; set; }

        [Inject]
        public IUnitOfWork UnitOfWork { get; set; }

        Task IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            if (handlingContext == null)
                throw new ArgumentNullException(nameof(handlingContext));
            _context = handlingContext;

            Validate(handlingContext.Command);

            using (var session = SessionFactory.OpenSession())
            {
                return Handle(handlingContext.Command, session);
            }
        }

        public IDictionary<string, object> Headers
        {
            get
            {
                if ((_context != null) && (_context.Headers != null))
                    return _context.Headers;
                return null;
            }
        }

        public string CorrelationId
        {
            get
            {
                if (_context != null)
                    return _context.CorrelationId;
                return null;
            }
        }

        public virtual void Validate(TCommand command)
        {
            var validationContext = new ValidationContext(command, null, null);
            Validator.ValidateObject(command, validationContext, true);
        }

        public abstract Task Handle(TCommand command, IModelStatefullSession session);

        protected void Return(object value)
        {
            _context.SetReturn(value);
        }
    }
}
