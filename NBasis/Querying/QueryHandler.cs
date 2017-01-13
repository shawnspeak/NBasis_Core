using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace NBasis.Querying
{
    public abstract class QueryHandler<TQuery, TResult> : IHandleQueries<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private IQueryHandlingContext<TQuery, TResult> context;

        Task<TResult> IHandleQueries<TQuery, TResult>.Handle(IQueryHandlingContext<TQuery, TResult> handlingContext)
        {
            context = handlingContext;

            Validate(handlingContext.Query);

            return Handle(handlingContext.Query);
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

        public virtual void Validate(TQuery query)
        {
            var validationContext = new ValidationContext(query, null, null);
            Validator.ValidateObject(query, validationContext, true);
        }

        public abstract Task<TResult> Handle(TQuery query);
    }
}
