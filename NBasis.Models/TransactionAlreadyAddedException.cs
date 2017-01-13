using System;

namespace NBasis.Models
{
    public class TransactionAlreadyAddedException : Exception
    {
        public TransactionAlreadyAddedException() : base() { }

        public TransactionAlreadyAddedException(string message) : base(message) { }
    }
}
