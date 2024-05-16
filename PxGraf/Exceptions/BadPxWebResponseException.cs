using System;
using System.Net;

namespace PxGraf.Exceptions
{
    public class BadPxWebResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public BadPxWebResponseException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
