#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata.MetaProperties;
using Px.Utils.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Models.Metadata
{
    public static class MatrixMetadataExtensions
    {
        /// <summary>
        /// Filters the dimension values of the given cube metadata based on the given query.
        /// Virtual values are treated as if they appear at the end of the dimension value list
        /// when computing the filter window, so that filters like TopFilter count real and virtual
        /// values together. Only real value codes are retained in the output metadata.
        /// </summary>
        /// <param name="input"><see cref="IReadOnlyMatrixMetadata"/> object to be used.</param>
        /// <param name="query">Query object to be used for filtering the dimension values.</param>
        /// <returns><see cref="IReadOnlyMatrixMetadata"/> object with dimensions filtered by query</returns>
        public static IReadOnlyMatrixMetadata FilterDimensionValues(this IReadOnlyMatrixMetadata input, MatrixQuery query)
        {
            List<IDimensionMap> dimensionMaps = [];
            foreach (IReadOnlyDimension dimension in input.Dimensions)
            {
                DimensionQuery dimQuery = query.DimensionQueries[dimension.Code];
                IValueFilter filter = dimQuery.ValueFilter;
                IReadOnlyList<string> codesToFilter = dimension.ValueCodes;
                if (dimQuery.VirtualValueDefinitions?.Count > 0)
                {
                    codesToFilter = [.. dimension.ValueCodes, .. dimQuery.VirtualValueDefinitions.Select(v => v.Code)];
                }
                HashSet<string> realCodes = [.. dimension.ValueCodes];
                List<string> valueCodes = [.. filter.Filter(codesToFilter).Where(realCodes.Contains)];
                dimensionMaps.Add(new DimensionMap(dimension.Code, valueCodes));
            }
            return input.GetTransform(new MatrixMap(dimensionMaps));
        }

        /// <summary>
        /// Builds both the database-fetch metadata and the output map in a single pass,
        /// calling <see cref="FilterDimensionValues"/> exactly once.
        /// <para>
        /// <c>fetchMeta</c>: metadata containing the real value codes needed to read data from the
        /// database (user-selected values plus any additional operand codes for virtual value computation).
        /// </para>
        /// <para>
        /// <c>outputMap</c>: a <see cref="MatrixMap"/> of codes that should appear in the final result
        /// (user-selected real codes plus any virtual value codes that fall within the filter window).
        /// Check for empty dimensions with <c>outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0)</c>.
        /// </para>
        /// </summary>
        /// <param name="completeMeta">The complete unfiltered table metadata.</param>
        /// <param name="query">The query containing filters and virtual value definitions.</param>
        /// <returns>A tuple of (fetchMeta, outputMap).</returns>
        public static (IReadOnlyMatrixMetadata fetchMeta, MatrixMap outputMap) BuildVirtualValueMaps(
            this IReadOnlyMatrixMetadata completeMeta, MatrixQuery query)
        {
            List<IDimensionMap> fetchDimensionMaps = [];
            List<IDimensionMap> outputDimensionMaps = [];

            foreach (IReadOnlyDimension dimension in completeMeta.Dimensions)
            {
                DimensionQuery dimQuery = query.DimensionQueries[dimension.Code];
                HashSet<string> filteredValueCodes = dimQuery.ValueFilter.Filter(dimension.ValueCodes).ToHashSet();

                if (dimQuery.VirtualValueDefinitions?.Count > 0)
                {
                    List<string> virtualCodeSet = [.. dimQuery.VirtualValueDefinitions.Select(d => d.Code)];
                    HashSet<string> realOperandCodes = dimQuery.VirtualValueDefinitions.SelectMany(
                        def => def.GetOperandCodes().Where(codes => !virtualCodeSet.Contains(codes))
                    ).ToHashSet();

                    List<string> orderedFetchCodes = [];
                    int maxItems = filteredValueCodes.Count + realOperandCodes.Count;
                    foreach (string code in dimension.ValueCodes)
                    {
                        if (filteredValueCodes.Contains(code) || realOperandCodes.Contains(code)) orderedFetchCodes.Add(code);
                        if (orderedFetchCodes.Count >= maxItems) break;
                    }
                    fetchDimensionMaps.Add(new DimensionMap(dimension.Code, orderedFetchCodes));
                    List<string> allCodes = [.. dimension.ValueCodes, .. virtualCodeSet];
                    outputDimensionMaps.Add(new DimensionMap(dimension.Code, [.. dimQuery.ValueFilter.Filter(allCodes)]));
                }
                else
                {
                    List<string> filteredCodes = [.. dimQuery.ValueFilter.Filter(dimension.ValueCodes)];
                    fetchDimensionMaps.Add(new DimensionMap(dimension.Code, filteredCodes));
                    outputDimensionMaps.Add(new DimensionMap(dimension.Code, filteredCodes));
                }
            }
            return (completeMeta.GetTransform(new MatrixMap(fetchDimensionMaps)), new MatrixMap(outputDimensionMaps));
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
            return [.. cubeMeta.Dimensions.Where(dimension => dimension.Values.Count > 1)];
        }

        /// <summary>
        /// Returns a list that contains only the multivalue dimensions from the cube meta in the same order as they are in the cube meta.
        /// </summary>
        public static IReadOnlyList<IReadOnlyDimension> GetSinglevalueDimensions(this IReadOnlyMatrixMetadata cubeMeta)
        {
            return [.. cubeMeta.Dimensions.Where(dimension => dimension.Values.Count == 1)];
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
            IReadOnlyList<IReadOnlyDimension> multiselects = GetSortedMultivalueDimensions(meta);
            return multiselects != null && multiselects.Any() ? multiselects[0] : null;

        }

        /// <summary>
        /// Returns the second largest multivalue dimension from the given cube meta.
        /// Multivalue dimension is a dimension that contains more than one value.
        /// </summary>
        public static IReadOnlyDimension? GetSmallerMultivalueDimension(this IReadOnlyMatrixMetadata meta)
        {
            IReadOnlyList<IReadOnlyDimension> multiselects = GetSortedMultivalueDimensions(meta);
            return multiselects != null && multiselects.Count > 1 ? multiselects[1] : null;
        }

        /// <summary>
        /// Returns the time dimennsion if the query contains one, if not, returns a dimension which is ordinal and has the most values.
        /// If the query contains neither time or progressive dimensions, returns null.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyDimension? GetMultivalueTimeOrLargestOrdinal(this IReadOnlyMatrixMetadata meta)
        {
            IReadOnlyList<IReadOnlyDimension> multiselects = GetSortedMultivalueDimensions(meta); //OBS: multiselects are sorted here so First() can be used!
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
        /// <returns>PublicationPropertyType value as a <see cref="MultilanguageString"/> object if it exists, otherwise null.</returns>
        public static MultilanguageString? GetMatrixMultilanguageProperty(this IReadOnlyMatrixMetadata meta, string propertyKey)
        {
            if (meta.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) &&
                prop is MultilanguageStringProperty mlsProp) return mlsProp.Value;

            return null;
        }

        /// <summary>
        /// Assigns appropriate language properties to single-language metadata properties.
        /// </summary>
        /// <param name="meta">The matrix metadata to assign language properties to.</param>
        /// <param name="keys">List of property keys to process.</param>
        public static void AssignLanguageToSingleLangProperties(this MatrixMetadata meta, List<string> keys)
        {
            // Table level
            AssignLanguagePropertiesAtLevel(meta.AdditionalProperties, keys, meta.DefaultLanguage);

            foreach (Dimension dim in meta.Dimensions)
            {
                // Dimension level
                AssignLanguagePropertiesAtLevel(dim.AdditionalProperties, keys, meta.DefaultLanguage);

                // Dimension value level
                foreach (DimensionValue val in dim.Values)
                {
                    AssignLanguagePropertiesAtLevel(val.AdditionalProperties, keys, meta.DefaultLanguage);
                }
            }
        }

        /// <summary>
        /// Helper method to assign language properties at a specific metadata level.
        /// </summary>
        /// <param name="properties">The properties dictionary to process.</param>
        /// <param name="keys">List of property keys to process.</param>
        /// <param name="defaultLanguage">The default language to use.</param>
        private static void AssignLanguagePropertiesAtLevel(Dictionary<string, MetaProperty> properties, List<string> keys, string defaultLanguage)
        {
            foreach (string key in keys)
            {
                if (properties.TryGetValue(key, out MetaProperty? prop))
                {
                    properties[key] = prop.AsMultiLanguageProperty(defaultLanguage);
                }
            }
        }

    }
}
#nullable disable
