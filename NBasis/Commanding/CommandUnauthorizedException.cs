using System;

namespace NBasis.Commanding
{
    public class CommandUnauthorizedException : Exception
    {
        public CommandUnauthorizedException() : base() { }

        public CommandUnauthorizedException(string message) : base(message) { }
    }
}
