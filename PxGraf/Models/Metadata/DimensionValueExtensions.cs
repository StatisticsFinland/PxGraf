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
    /// Extension methods for <see cref="IReadOnlyDimension"/> objects.
    /// </summary>
    public static class DimensionValueExtensions
    {
        /// <summary>
        /// Converts the given <see cref="IReadOnlyDimensionValue"/> object to a <see cref="VariableValue"/> object.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="eliminationValueCode"></param>
        /// <returns></returns>
        public static VariableValue ConvertToVariableValue(this IReadOnlyDimensionValue input, string? eliminationValueCode)
        {
            bool isSum = input.Code == eliminationValueCode;
            ContentComponent? cc = null;
            MultilanguageString? valueNote = input.GetDimensionValueProperty(PxSyntaxConstants.VALUENOTE_KEY);
            if (input is ContentDimensionValue cdv)
            {
                MultilanguageString? source = input.GetDimensionValueProperty(PxSyntaxConstants.SOURCE_KEY);
                cc = new ContentComponent(cdv.Unit, source, cdv.Precision, PxSyntaxConstants.FormatPxDateTime(cdv.LastUpdated, true));
            }
            return new VariableValue(input.Code, input.Name, valueNote, isSum, cc);
        }

        private static MultilanguageString? GetDimensionValueProperty(this IReadOnlyDimensionValue value, string propertyKey)
        {
            if (value.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) && prop.CanGetMultilanguageValue)
            {
                return prop.ValueAsMultilanguageString(PxSyntaxConstants.STRING_DELIMETER, value.Name.Languages.First());
            }

            return null;
        }
    }
}
