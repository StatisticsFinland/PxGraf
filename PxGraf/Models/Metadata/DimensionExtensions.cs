#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.ExtensionMethods;
using PxGraf.Data.MetaData;
using PxGraf.Utility;
using System;
using System.Linq;

namespace PxGraf.Models.Metadata
{
    /// <summary>
    /// TODO: Summary
    /// </summary>
    public static class DimensionExtensions
    {
        /// <summary>
        /// TODO: Summary
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Variable ConvertToVariable(this IReadOnlyDimension input)
        {
            if (input is not Dimension)
            {
                throw new ArgumentException("Input must be of type Dimension");
            }

            return new(
                input.Code,
                input.Name,
                input.GetDimensionProperty(PxSyntaxConstants.NOTE_KEY, input.Name.Languages.First()),
                input.Type,
                input.Values.Select(v => v
                    .ConvertToVariableValue(input.GetEliminationValueCode())).ToList());
        }

        /// <summary>
        /// Returns the elimination value code from the given dimension if it exists.
        /// </summary>
        /// <param name="dimension"><see cref="IReadOnlyDimension"/> object to search the elimination value code for</param>
        /// <returns>Elimination value code from the given dimension if it exists, otherwise null.</returns>
        public static string? GetEliminationValueCode(this IReadOnlyDimension dimension)
        {
            if (dimension.AdditionalProperties.TryGetValue(PxSyntaxConstants.ELIMINATION_KEY, out MetaProperty? property))
            {
                if (property.CanGetStringValue) return property.ValueAsString(PxSyntaxConstants.STRING_DELIMETER);
                else
                {
                    MultilanguageString eliminationName = property.ValueAsMultilanguageString(PxSyntaxConstants.STRING_DELIMETER, dimension.Name.Languages.First());
                    return dimension.Values.FirstOrDefault(v => v.Name.Equals(eliminationName))?.Code;
                }
            }
            return null;
        }

        /// <summary>
        /// TODO: Summary
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="propertyKey"></param>
        /// <param name="defaultLang"></param>
        /// <returns></returns>
        public static MultilanguageString? GetDimensionProperty(this IReadOnlyDimension dimension, string propertyKey, string defaultLang)
        {
            if (dimension.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) && prop.CanGetMultilanguageValue)
            {
                return prop.ValueAsMultilanguageString(PxSyntaxConstants.STRING_DELIMETER, defaultLang);
            }

            return null;
        }
    }
}
