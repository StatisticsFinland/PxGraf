#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata;
using System.Linq;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using PxGraf.Utility;
using Px.Utils.Models.Metadata.Enums;
using System;
using Px.Utils.Models.Metadata.MetaProperties;

namespace PxGraf.Models.Metadata
{
    public static class MatrixMetadataExtensions
    {
        /// <summary>
        /// Filters the dimension values of the given cube metadata based on the given query.
        /// </summary>
        /// <param name="input"><see cref="IReadOnlyMatrixMetadata"/> object to be used.</param>
        /// <param name="query">Query object to be used for filtering the dimension values.</param>
        /// <returns><see cref="IReadOnlyMatrixMetadata"/> object with dimensions filtered by query</returns>
        public static IReadOnlyMatrixMetadata FilterDimensionValues(this IReadOnlyMatrixMetadata input, MatrixQuery query)
        {
            List<IDimensionMap> dimensionMaps = [];
            foreach (IReadOnlyDimension dimension in input.Dimensions)
            {
                IValueFilter filter = query.DimensionQueries[dimension.Code].ValueFilter;
                List<string> valueCodes = filter.Filter(dimension.Values).Select(v => v.Code).ToList();
                dimensionMaps.Add(new DimensionMap(dimension.Code, valueCodes));
            }
            return input.GetTransform(new MatrixMap(dimensionMaps));
        }

        /// <summary>
        /// Returns the nuber of multivalue dimensions in the given meta object.
        /// Multivalue dimension is a dimension with more than one value.
        /// </summary>
        public static int GetNumberOfMultivalueDimensions(this IReadOnlyMatrixMetadata cubeMeta)
        {
            return cubeMeta.Dimensions.Count(dimension => dimension.Values.Count > 1);
        }

        /// <summary>
        /// Returns a list that contains only the multivalue dimensions from the cube meta in the same order as they are in the cube meta.
        /// </summary>
        public static IReadOnlyList<IReadOnlyDimension> GetMultivalueDimensions(this IReadOnlyMatrixMetadata cubeMeta)
        {
            return cubeMeta.Dimensions
                .Where(dimension => dimension.Values.Count > 1)
                .ToList();
        }

        /// <summary>
        /// Returns a list that contains only the multivalue dimensions from the cube meta in the same order as they are in the cube meta.
        /// </summary>
        public static IReadOnlyList<IReadOnlyDimension> GetSinglevalueDimensions(this IReadOnlyMatrixMetadata cubeMeta)
        {
            return cubeMeta.Dimensions
                .Where(dimension => dimension.Values.Count == 1)
                .ToList();
        }

        /// <summary>
        /// Returns a list of all of the multiselect dimensions from the given meta object ordered by the number of selected values in them.
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public static IReadOnlyList<IReadOnlyDimension> GetSortedMultivalueDimensions(this IReadOnlyMatrixMetadata meta)
        {
            return [.. meta.Dimensions
                .Where(diemnsion => diemnsion.Values.Count > 1)
                .OrderByDescending(x => x.Values.Count)];
        }

        /// <summary>
        /// Returns the second largest multiselect dimension from the given meta.
        /// Multiselect dimension is a dimension with more than one selected value.
        /// </summary>
        public static IReadOnlyDimension? GetLargestMultivalueDimension(this IReadOnlyMatrixMetadata meta)
        {
            var multiselects = GetSortedMultivalueDimensions(meta);
            return multiselects != null && multiselects.Any() ? multiselects[0] : null;

        }

        /// <summary>
        /// Returns the second largest multivalue dimension from the given cube meta.
        /// Multivalue dimension is a dimension that contains more than one value.
        /// </summary>
        public static IReadOnlyDimension? GetSmallerMultivalueDimension(this IReadOnlyMatrixMetadata meta)
        {
            var multiselects = GetSortedMultivalueDimensions(meta);
            return multiselects != null && multiselects.Count > 1 ? multiselects[1] : null;
        }

        /// <summary>
        /// Returns the time dimennsion if the query contains one, if not, returns a dimension which is ordinal and has the most values.
        /// If the query contains neither time or progressive dimensions, returns null.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyDimension? GetMultivalueTimeOrLargestOrdinal(this IReadOnlyMatrixMetadata meta)
        {
            var multiselects = GetSortedMultivalueDimensions(meta); //OBS: multiselects are sorted here so First() can be used!
            if (multiselects.FirstOrDefault(d => d.Type == DimensionType.Time) is IReadOnlyDimension timeDim) return timeDim;
            if (multiselects.FirstOrDefault(d => d.Type == DimensionType.Ordinal) is IReadOnlyDimension ordinalDim) return ordinalDim;

            return null;
        }

        /// <summary>
        /// Returns the latest update time from the content dimension values.
        /// </summary>
        public static DateTime? GetLastUpdated(this IReadOnlyMatrixMetadata meta)
        {
            if (meta.TryGetContentDimension(out ContentDimension? cd))
            {
                IEnumerable<DateTime> times = cd.Values.Map(cdv => cdv.LastUpdated);
                return times.OrderDescending().First();
            }
            return null;
        }

        /// <summary>
        /// Returns a property value from the matrix metadata based on the given key.
        /// </summary>
        /// <param name="meta">Metadata object to be searched.</param>
        /// <param name="propertyKey">Key of the property to be searched.</param>
        /// <returns>Property value as a <see cref="MultilanguageString"/> object if it exists, otherwise null.</returns>
        public static MultilanguageString? GetMatrixMultilanguageProperty(this IReadOnlyMatrixMetadata meta, string propertyKey)
        {
            if (meta.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) &&
                prop is MultilanguageStringProperty mlsProp) return mlsProp.Value;

            return null;
        }

        /// <summary>
        /// Ensures that source property exists for all content dimension values in the given metadata.
        /// Prioritises the source property of the content dimension value over the source property of the dimension and the table.
        /// </summary>
        /// <param name="meta">Metadata boject to be processed.</param>
        /// <exception cref="InvalidOperationException">Thrown if source property is not found from metadata.</exception>
        public static void AssignSourceToContentDimensionValues(this IReadOnlyMatrixMetadata meta)
        {
            foreach (ContentDimensionValue cdv in meta.GetContentDimension().Values)
            {
                MultilanguageString? source = cdv.GetSource(meta) ?? throw new InvalidOperationException("Source property not found from metadata.");
                MultilanguageStringProperty sourceProperty = new (source);
                cdv.AdditionalProperties.TryAdd(PxSyntaxConstants.SOURCE_KEY, sourceProperty);
            }
        }

        /// <summary>
        /// Assigns ordinal or nominal dimension types to dimensions that are not content or time dimensions based on their meta-id properties.
        /// </summary>
        /// <param name="meta">The matrix metadata to assign the dimension types to.</param>
        public static MatrixMetadata AssignOrdinalDimensionTypes(this MatrixMetadata meta)
        {
            List<Dimension> newDimensions = [..meta.Dimensions];
            for(int i = 0; i < meta.Dimensions.Count; i++)
            {
                if (meta.Dimensions[i].Type == DimensionType.Content ||
                    meta.Dimensions[i].Type == DimensionType.Time)
                {
                    continue;
                }

                DimensionType dimensionType = meta.Dimensions[i].GetDimensionType();
                newDimensions[i] = new(
                    meta.Dimensions[i].Code,
                    meta.Dimensions[i].Name,
                    meta.Dimensions[i].AdditionalProperties,
                    meta.Dimensions[i].Values,
                    dimensionType);
            }
            return new MatrixMetadata(
                meta.DefaultLanguage,
                meta.AvailableLanguages,
                newDimensions,
                meta.AdditionalProperties);
        }
    }
}
#nullable disable
