#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Utility;
using System.Linq;

namespace PxGraf.Models.Metadata
{
    /// <summary>
    /// Extension methods for <see cref="IReadOnlyDimension"/> objects.
    /// </summary>
    public static class DimensionExtensions
    {
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
                prop is MultilanguageStringProperty mlsProp) 
            {
                return mlsProp.Value;
            }
            else return null;
        }

    }
}
#nullable disable
