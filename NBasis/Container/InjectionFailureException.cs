using System;
using System.Collections.Generic;
using System.Text;

namespace NBasis.Container
{
    public class InjectionFailureException : Exception
    {
        public InjectionFailureException() : base() { }

        public InjectionFailureException(string message) : base(message) { }
    }
}
