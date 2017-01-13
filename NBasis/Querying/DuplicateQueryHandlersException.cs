using System;

namespace NBasis.Querying
{
    public class DuplicateQueryHandlersException : Exception
    {
        public DuplicateQueryHandlersException(Type queryType)
            : base(string.Format("Duplicate query handlers were found for type {0}", queryType))
        {
        }
    }
}
