using System;
using System.Net;

namespace PxGraf.Exceptions
{
    public class BadPxWebResponseException(HttpStatusCode statusCode, string message) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; set; } = statusCode;
    }
}
