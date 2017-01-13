using System;

namespace NBasis.Querying
{
    public class QueryHandlerNotFoundException : Exception
    {
        public QueryHandlerNotFoundException(Type queryType)
            : base(string.Format("No query handlers were found for '{0}'", queryType))
        {
        }
    }
}
