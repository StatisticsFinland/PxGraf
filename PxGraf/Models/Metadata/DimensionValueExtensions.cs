#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata.MetaProperties;
using Px.Utils.Models.Metadata;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using PxGraf.Utility;

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
        public static VariableValue ConvertToVariableValue(this IReadOnlyDimensionValue input, string? eliminationValueCode, DimensionQuery? dimensionQuery, IReadOnlyMatrixMetadata meta)
        {
            bool isSum = input.Code == eliminationValueCode;
            ContentComponent? cc = null;
            MultilanguageString? valueNote = input.GetDimensionValueMultilanguageProperty(PxSyntaxConstants.VALUENOTE_KEY);

            DimensionQuery.VariableValueEdition? valueEdits = null;
            dimensionQuery?.ValueEdits.TryGetValue(input.Code, out valueEdits);

            MultilanguageString name = valueEdits?.NameEdit is null ? input.Name : input.Name.CopyAndEdit(valueEdits.NameEdit);
            if (input is ContentDimensionValue cdv)
            {
                MultilanguageString? source = valueEdits?.ContentComponent.SourceEdit is null
                    ? cdv.GetSource(meta) : cdv.GetSource(meta)?.CopyAndEdit(valueEdits.ContentComponent.SourceEdit);
                MultilanguageString? unit = valueEdits?.ContentComponent.UnitEdit is null ?
                    cdv.Unit : cdv.Unit.CopyAndEdit(valueEdits.ContentComponent.UnitEdit);
                cc = new ContentComponent(unit, source, cdv.Precision, PxSyntaxConstants.FormatPxDateTime(cdv.LastUpdated));
            }
            return new VariableValue(input.Code, name, valueNote, isSum, cc);
        }

        /// <summary>
        /// Returns the source of the given content dimension value if it exists.
        /// If the value has no source, the source of the dimension or the table is returned.
        /// </summary>
        /// <param name="value">Dimension value to get the source from.</param>
        /// <param name="meta">Complete matrix metadata object.</param>
        /// <returns>Source of the given content dimension value if it exists, otherwise the source of the dimension or the table.</returns>
        private static MultilanguageString? GetSource(this ContentDimensionValue value, IReadOnlyMatrixMetadata meta)
        {
            // Primary use source information from the content dimension value.
            MultilanguageString? valueSource = value.GetDimensionValueMultilanguageProperty(PxSyntaxConstants.SOURCE_KEY);
            if (valueSource is not null) return valueSource;
            // If the value has no source, use the source of the content dimension.
            else if (meta.GetContentDimension().AdditionalProperties.TryGetValue(PxSyntaxConstants.SOURCE_KEY, out MetaProperty? prop) &&
                prop is MultilanguageStringProperty dimMlsp) return dimMlsp.Value;
            // If the dimension has no source, use the source of the table.
            else if (meta.AdditionalProperties.TryGetValue(PxSyntaxConstants.SOURCE_KEY, out MetaProperty? tableProp) &&
                tableProp is MultilanguageStringProperty tableMlsp) return tableMlsp.Value;

            return null;
        }

        private static MultilanguageString? GetDimensionValueMultilanguageProperty(this IReadOnlyDimensionValue value, string propertyKey)
        {
            if (value.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) &&
                prop is MultilanguageStringProperty mlsProp) return mlsProp.Value;
            else return null;
        }
    }
}
#nullable disable
