using System;

namespace PxGraf.Exceptions
{
    /// <summary>
    /// Thrown when a saved query produces a matrix with one or more dimensions that have zero output values,
    /// typically because the underlying data has changed since the query was saved.
    /// </summary>
    public class EmptyDimensionException(string message) : Exception(message) { }
}
