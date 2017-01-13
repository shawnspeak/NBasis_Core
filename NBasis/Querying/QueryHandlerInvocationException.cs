using System;

namespace NBasis.Querying
{
    public class QueryHandlerInvocationException : Exception
    {
        public QueryHandlerInvocationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
