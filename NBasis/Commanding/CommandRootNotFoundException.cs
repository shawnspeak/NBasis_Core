using System;

namespace NBasis.Commanding
{
    public class CommandRootNotFoundException : Exception
    {
        public CommandRootNotFoundException() : base() { }

        public CommandRootNotFoundException(string message) : base(message) { }
    }
}
