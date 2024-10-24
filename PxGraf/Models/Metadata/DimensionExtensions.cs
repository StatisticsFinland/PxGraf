#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.MetaProperties;
using Px.Utils.Models.Metadata;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Linq;
using System;
using Px.Utils.Models.Metadata.Enums;

namespace PxGraf.Models.Metadata
{
    /// <summary>
    /// Extension methods for <see cref="IReadOnlyDimension"/> objects.
    /// </summary>
    public static class DimensionExtensions
    {
        /// <summary>
        /// Converts the given <see cref="IReadOnlyDimension"/> object to a <see cref="Variable"/> object.
        /// </summary>
        /// <param name="input">The <see cref="IReadOnlyDimension"/> object to convert.</param>
        /// <param name="dimensionQueries">The dimension queries to use for the conversion.</param>"
        /// <param name="meta">The matrix metadata to append missing content value information from.</param>
        /// <returns>A <see cref="Variable"/> object created from the given <see cref="IReadOnlyDimension"/> object.</returns>
        public static Variable ConvertToVariable(this IReadOnlyDimension input, Dictionary<string, DimensionQuery> dimensionQueries, IReadOnlyMatrixMetadata meta)
        {
            if (input is not Dimension dimension)
                throw new ArgumentException("Input must be a Dimension object.");

            MultilanguageString name = dimension.Name;
            if (dimensionQueries.TryGetValue(dimension.Code, out DimensionQuery? query) &&
                query.NameEdit != null)
            {
                name = query.NameEdit;
            }

            return new(
                code: dimension.Code,
                name: name,
                note: dimension.GetMultilanguageDimensionProperty(PxSyntaxConstants.NOTE_KEY),
                type: dimension.Type,
                values: dimension.Values.Select(v => v
                    .ConvertToVariableValue(dimension.GetEliminationValueCode(), query, meta)).ToList());
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
                if (property is StringProperty sProp) return sProp.Value;
                else if (property is MultilanguageStringProperty mlsProp)
                {
                    return dimension.Values.FirstOrDefault(v => v.Name.Equals(mlsProp.Value))?.Code;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the value of the given property key from the dimension if it exists.
        /// </summary>
        /// <param name="dimension">The dimension to search the property from.</param>
        /// <param name="propertyKey">The key of the property to search for.</param>
        /// <param name="removeEntry">Whether to remove the property entry from the dimension after retrieval.</param>
        /// <returns></returns>
        public static MultilanguageString? GetMultilanguageDimensionProperty(this Dimension dimension, string propertyKey, bool removeEntry = false)
        {
            if (dimension.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) &&
                prop is MultilanguageStringProperty mlsProp) 
            {
                if (removeEntry)
                {
                    dimension.AdditionalProperties.Remove(propertyKey);
                }
                return mlsProp.Value;
            }
            else return null;
        }

        /// <summary>
        /// Assigns a dimension type to the given dimension based on its meta-id property.
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static DimensionType GetDimensionType(this Dimension dimension)
        {
            MultilanguageString? metaId = dimension.GetMultilanguageDimensionProperty(PxSyntaxConstants.META_ID_KEY, true);
            if (metaId is not null)
            {
                if (metaId.UniformValue().Equals(PxSyntaxConstants.ORDINAL_VALUE)) return DimensionType.Ordinal;
                else if (metaId.UniformValue().Equals(PxSyntaxConstants.NOMINAL_VALUE)) return DimensionType.Nominal;
            }

            return dimension.Type;
        }
    }
}
#nullable disable
