using Px.Utils.Models.Metadata.Enums;
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
        public static DimensionType ToEnum(string typeString)
        {
            return typeString switch
            {
                "T" => DimensionType.Time,
                "P" => DimensionType.Ordinal,
                "G" => DimensionType.Geographical,
                "C" => DimensionType.Content,
                "F" => DimensionType.Other,
                _ => DimensionType.Unknown,
            };
        }


        /// <summary>
        /// Converts DimensionType enums to their string prepresentation.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToString(DimensionType type)
        {
            return type switch
            {
                DimensionType.Time => "T",
                DimensionType.Ordinal => "P",
                DimensionType.Geographical => "G",
                DimensionType.Content => "C",
                DimensionType.Other => "F",
                _ => "N"
            };
        }

        /// <summary>
        /// Converts a collection of DimensionType enums to strings.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToString(IEnumerable<DimensionType> types)
        {
            foreach (var type in types) yield return ToString(type);
        }
    }
}
