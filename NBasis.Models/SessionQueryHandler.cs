using NBasis.Querying;
using NBasis.Container;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;

namespace NBasis.Models
{
    public abstract class SessionQueryHandler<TQuery, TResult> : IHandleQueries<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private IQueryHandlingContext<TQuery, TResult> _context;

        [Inject]
        public IModelSessionFactory SessionFactory { get; set; }

        public Task<TResult> Handle(IQueryHandlingContext<TQuery, TResult> handlingContext)
        {
            if (handlingContext == null)
                throw new ArgumentNullException(nameof(handlingContext));

            _context = handlingContext;

            Validate(handlingContext.Query);

            using (var session = SessionFactory.OpenSession())
            {
                return Handle(handlingContext.Query, session);
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

        public virtual void Validate(TQuery query)
        {
            var validationContext = new ValidationContext(query, null, null);
            Validator.ValidateObject(query, validationContext, true);
        }

        public abstract Task<TResult> Handle(TQuery query, IModelSession session);
    }
}
