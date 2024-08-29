#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.ExtensionMethods;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
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
        /// <param name="input">The dimension value to convert</param>
        /// <param name="eliminationValueCode">The code of ´the dimension's elimination value.</param>
        /// <param name="dimensionQuery">Dimension query object that contains the value edits.</param>
        /// <param name="meta">The matrix metadata to append missing content value information from.</param>
        /// <returns></returns>
        public static VariableValue ConvertToVariableValue(this IReadOnlyDimensionValue input, string? eliminationValueCode, DimensionQuery dimensionQuery, IReadOnlyMatrixMetadata meta)
        {
            bool isSum = input.Code == eliminationValueCode;
            ContentComponent? cc = null;
            MultilanguageString? valueNote = input.GetDimensionValueProperty(PxSyntaxConstants.VALUENOTE_KEY);
            bool edited = dimensionQuery.ValueEdits.TryGetValue(input.Code, out DimensionQuery.VariableValueEdition? valueEdit);
            if (input is ContentDimensionValue cdv)
            {
                MultilanguageString? source = valueEdit?.ContentComponent.SourceEdit != null ?
                    valueEdit.ContentComponent.SourceEdit :
                    cdv.GetDimensionValueSource(meta);
                MultilanguageString? unit = valueEdit?.ContentComponent.UnitEdit != null ?
                    valueEdit.ContentComponent.UnitEdit :
                    cdv.Unit;
                cc = new ContentComponent(unit, source, cdv.Precision, PxSyntaxConstants.FormatPxDateTime(cdv.LastUpdated));
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

        private static MultilanguageString? GetDimensionValueSource(this ContentDimensionValue value, IReadOnlyMatrixMetadata meta)
        {
            MultilanguageString? valueSource = value.GetDimensionValueProperty(PxSyntaxConstants.SOURCE_KEY);
            if (valueSource is not null)
            {
                return valueSource;
            }
            else if (meta.Dimensions.First(d => d.Values.Contains(value)).AdditionalProperties.TryGetValue(PxSyntaxConstants.SOURCE_KEY, out MetaProperty? prop) && prop.CanGetMultilanguageValue)
            {
                return prop.ValueAsMultilanguageString(PxSyntaxConstants.STRING_DELIMETER, value.Name.Languages.First());
            }
            else if (meta.AdditionalProperties.TryGetValue(PxSyntaxConstants.SOURCE_KEY, out MetaProperty? tableProp) && tableProp.CanGetMultilanguageValue)
            {
                return tableProp.ValueAsMultilanguageString(PxSyntaxConstants.STRING_DELIMETER, value.Name.Languages.First());
            }

            return null;
        }
    }
}
