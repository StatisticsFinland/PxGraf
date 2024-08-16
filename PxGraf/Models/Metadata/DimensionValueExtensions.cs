#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.ExtensionMethods;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
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
        public static VariableValue ConvertToVariableValue(this IReadOnlyDimensionValue input, string? eliminationValueCode, DimensionQuery dimensionQuery)
        {
            bool isSum = input.Code == eliminationValueCode;
            ContentComponent? cc = null;
            MultilanguageString? valueNote = input.GetDimensionValueProperty(PxSyntaxConstants.VALUENOTE_KEY);
            bool edited = dimensionQuery.ValueEdits.TryGetValue(input.Code, out DimensionQuery.VariableValueEdition? valueEdit);
            if (input is ContentDimensionValue cdv)
            {
                bool ccEdited = edited && valueEdit?.ContentComponent != null;
                MultilanguageString? source = ccEdited ? 
                    valueEdit.ContentComponent.SourceEdit : 
                    input.GetDimensionValueProperty(PxSyntaxConstants.SOURCE_KEY);
                MultilanguageString? unit = ccEdited?
                    valueEdit.ContentComponent.UnitEdit :
                    cdv.Unit;
                cc = new ContentComponent(unit, source, cdv.Precision, PxSyntaxConstants.FormatPxDateTime(cdv.LastUpdated, true));
            }
            MultilanguageString name = input.Name;
            if (edited && valueEdit?.NameEdit != null)
            {
                name = valueEdit.NameEdit;
            }
            return new VariableValue(input.Code, name, valueNote, isSum, cc);
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
