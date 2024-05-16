using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.Utility
{
    /// <summary>
    /// Contains functions for converting between DimensionType enums and their corresponding strings.
    /// </summary>
    public static class DimenionTypesEnumConverter
    {
        /// <summary>
        /// Converts dimension type string to DimensionType enums
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns></returns>
        public static VariableType ToEnum(string typeString)
        {
            return typeString switch
            {
                "T" => VariableType.Time,
                "P" => VariableType.Ordinal,
                "G" => VariableType.Geological,
                "C" => VariableType.Content,
                "F" => VariableType.OtherClassificatory,
                _ => VariableType.Unknown,
            };
        }


        /// <summary>
        /// Converts DimensionType enums to their string prepresentation.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToString(VariableType type)
        {
            return type switch
            {
                VariableType.Time => "T",
                VariableType.Ordinal => "P",
                VariableType.Geological => "G",
                VariableType.Content => "C",
                VariableType.OtherClassificatory => "F",
                _ => "N"
            };
        }

        /// <summary>
        /// Converts a collection of DimensionType enums to strings.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToString(IEnumerable<VariableType> types)
        {
            foreach (var type in types) yield return ToString(type);
        }
    }
}
