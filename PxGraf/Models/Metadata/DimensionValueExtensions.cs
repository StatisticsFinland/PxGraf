#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.MetaProperties;
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
        /// <returns></returns>
        public static VariableValue ConvertToVariableValue(this IReadOnlyDimensionValue input, string? eliminationValueCode, DimensionQuery? dimensionQuery)
        {
            bool isSum = input.Code == eliminationValueCode;
            ContentComponent? cc = null;
            MultilanguageString? valueNote = input.GetDimensionValueMultilanguageProperty(PxSyntaxConstants.VALUENOTE_KEY);

            DimensionQuery.DimensionValueEdition? valueEdits = null;
            dimensionQuery?.ValueEdits.TryGetValue(input.Code, out valueEdits);

            MultilanguageString name = valueEdits?.NameEdit is null ? input.Name : input.Name.CopyAndEdit(valueEdits.NameEdit);
            if (input is ContentDimensionValue cdv)
            {
                MultilanguageString? source = input.GetDimensionValueMultilanguageProperty(PxSyntaxConstants.SOURCE_KEY);
                source = valueEdits?.ContentComponent?.SourceEdit is null
                    ? source : source?.CopyAndEdit(valueEdits.ContentComponent.SourceEdit);
                MultilanguageString? unit = valueEdits?.ContentComponent?.UnitEdit is null ?
                    cdv.Unit : cdv.Unit.CopyAndEdit(valueEdits.ContentComponent.UnitEdit);
                cc = new ContentComponent(unit, source, cdv.Precision, PxSyntaxConstants.FormatPxDateTime(cdv.LastUpdated));
            }
            return new VariableValue(input.Code, name, valueNote, isSum, cc);
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
