using System;

namespace PxGraf.Exceptions
{
    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException(string message) : base(message)
        {
        }

        public TooManyRequestsException()
        {
        }
    }
}
