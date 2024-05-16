using System.Collections.Generic;

namespace PxGraf.Data
{
    /// <summary>
    /// Representation of variable's structure containing the variable code and its values' codes.
    /// </summary>
    public class VariableMap
    {
        /// <summary>
        /// Code of the variable.
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// Codes for the values of the variable.
        /// </summary>
        public IReadOnlyList<string> ValueCodes { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="code">Code of the variable.</param>
        /// <param name="valueCodes">Codes for the values of the variable.</param>
        public VariableMap(string code, IReadOnlyList<string> valueCodes)
        {
            Code = code;
            ValueCodes = valueCodes;
        }
    }
}
