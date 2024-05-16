using System;

namespace PxGraf.Exceptions
{
    public class UnknownFilterTypeException : Exception
    {
        public UnknownFilterTypeException(string message) : base(message) 
        {
        }
    }
}
