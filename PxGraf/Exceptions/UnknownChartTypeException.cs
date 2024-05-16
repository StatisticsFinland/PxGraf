using PxGraf.Enums;
using System;

namespace PxGraf.Exceptions
{
    /// <summary>
    /// Thrown when given chart type cannot be handled.
    /// </summary>
    public class UnknownChartTypeException : Exception
    {
        /// <summary>
        /// The type enum that caused the exception
        /// </summary>
        public VisualizationType Type { get; }

        /// <summary>
        /// Constructor with a message parameter
        /// </summary>
        /// <param name="message"></param>
        public UnknownChartTypeException(string message) : base(message) { }

        /// <summary>
        /// Constructor with a message and chart type parameters
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public UnknownChartTypeException(string message, VisualizationType type) : base(message)
        {
            Type = type;
        }

        /// <summary>
        /// Constructor with a message, inner exception and chart type parameters
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="type"></param>
        public UnknownChartTypeException(string message, Exception innerException, VisualizationType type) : base(message, innerException)
        {
            Type = type;
        }
    }
}
