using System;

namespace PxGraf.Exceptions
{
    public class InvalidConfigurationException(string message) : Exception(message) { }
}
