using System.Collections.Generic;

namespace PxGraf.Data
{
    /// <summary>
    /// Representation of variable's structure containing the variable code and its values' codes.
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    /// <param name="code">Code of the variable.</param>
    /// <param name="valueCodes">Codes for the values of the variable.</param>
    public class VariableMap(string code, IReadOnlyList<string> valueCodes)
    {
        /// <summary>
        /// Code of the variable.
        /// </summary>
        public string Code { get; } = code;

        /// <summary>
        /// Codes for the values of the variable.
        /// </summary>
        public IReadOnlyList<string> ValueCodes { get; } = valueCodes;
    }
}
