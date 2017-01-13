using System;

namespace NBasis.Commanding
{
    public class DuplicateCommandHandlersException : Exception
    {
        public DuplicateCommandHandlersException(Type commandType)
            : base(string.Format("Duplicate command handlers were found for type {0}", commandType))
        {
        }
    }
}
