#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata.MetaProperties;
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
            if (input is not Dimension)
            {
                throw new ArgumentException("Input must be of type Dimension");
            }

            MultilanguageString name = input.Name;
            if (dimensionQueries.TryGetValue(input.Code, out DimensionQuery? query) &&
                query.NameEdit != null)
            {
                name = query.NameEdit;
            }

            return new(
                code: input.Code,
                name: name,
                note: input.GetMultilanguageDimensionProperty(PxSyntaxConstants.NOTE_KEY),
                type: input.Type,
                values: input.Values.Select(v => v
                    .ConvertToVariableValue(input.GetEliminationValueCode(), query, meta)).ToList());
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
        /// <returns></returns>
        public static MultilanguageString? GetMultilanguageDimensionProperty(this IReadOnlyDimension dimension, string propertyKey)
        {
            if (dimension.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) &&
                prop is MultilanguageStringProperty mlsProp) return mlsProp.Value;
            else return null;
        }
    }
}
#nullable disable
