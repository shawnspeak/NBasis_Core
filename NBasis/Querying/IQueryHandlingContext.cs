using System.Collections.Generic;

namespace NBasis.Querying
{
    public interface IQueryHandlingContext<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TQuery Query { get; }

        IDictionary<string, object> Headers { get; }

        string CorrelationId { get; }
    }
}
