using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Data
{
    /// <summary>
    /// Collection of utility functions used for sorting the data in data cubes
    /// </summary>
    public static class CubeSorting
    {
        /// <summary>
        /// From smallest to largest
        /// </summary>
        public const string ASCENDING = "ascending";

        /// <summary>
        /// From largest to smallest
        /// </summary>
        public const string DESCENDING = "descending";

        /// <summary>
        /// Based on the subset sum from largest to smallest
        /// </summary>
        public const string SUM = "sum";

        /// <summary>
        /// No sorting, keep the original data ordering
        /// </summary>
        public const string NO_SORTING = "no_sorting";

        /// <summary>
        /// Reversed from the original data ordering
        /// </summary>
        public const string REVERSED = "reversed";

        /// <summary>
        /// Provides all valid sorting options for a visualization. Languages are based on the languages of the cube meta object.
        /// </summary>
        public static VisualizationOption.SortingOptionsCollection Get(VisualizationType type, IReadOnlyMatrixMetadata meta, bool allowPivot, MatrixQuery query)
        {
            return type switch
            {
                VisualizationType.VerticalBarChart => GetVerticalBarChartOptions(),
                VisualizationType.GroupVerticalBarChart => GetGroupVerticalBarChartOptions(),
                VisualizationType.StackedVerticalBarChart => GetStackedVerticalBarChartOptions(),
                VisualizationType.PercentVerticalBarChart => GetStackedVerticalBarChartOptions(),
                VisualizationType.HorizontalBarChart => GetHorizontalBarChartOptions(meta.AvailableLanguages, allowPivot),
                VisualizationType.GroupHorizontalBarChart or
                VisualizationType.StackedHorizontalBarChart or
                VisualizationType.PercentHorizontalBarChart => GetMultiDimHorizontalBarChartOptions(type, meta, allowPivot, query),
                VisualizationType.PyramidChart => GetPyramidChartOptions(),
                VisualizationType.PieChart => GetPieChartOptions(meta.AvailableLanguages, allowPivot),
                VisualizationType.LineChart => GetLineChartOptions(),
                VisualizationType.ScatterPlot => GetScatterPlotOptions(),
                _ => new() { Default = null, Pivoted = null }
            };
        }

        #region SORTING_OPTIONS

        // Not sortable at the moment
        private static VisualizationOption.SortingOptionsCollection GetVerticalBarChartOptions() => new() { Default = null, Pivoted = null };

        // Not sortable at the moment
        private static VisualizationOption.SortingOptionsCollection GetGroupVerticalBarChartOptions() => new() { Default = null, Pivoted = null };

        // Not sortable at the moment
        private static VisualizationOption.SortingOptionsCollection GetStackedVerticalBarChartOptions() => new() { Default = null, Pivoted = null };

        private static VisualizationOption.SortingOptionsCollection GetHorizontalBarChartOptions(IEnumerable<string> languages, bool allowPivot)
        {
            List<SortingOption> sorting = 
                [
                    GetDescendingSorting(languages),
                    GetAscendingSorting(languages),
                    GetSameAsDataSorting(languages),
                    GetReversedFromDataSorting(languages)
                ];

            return new()
            {
                Default = sorting,
                Pivoted = allowPivot ? sorting : null
            };
        }

        private static VisualizationOption.SortingOptionsCollection GetMultiDimHorizontalBarChartOptions(VisualizationType visualization, IReadOnlyMatrixMetadata meta, bool allowPivot, MatrixQuery query)
        {
            List<IReadOnlyDimension> multiselects = [.. meta.GetMultivalueDimensions().Where(mvv => !query.DimensionQueries[mvv.Code].Selectable)];
            return new VisualizationOption.SortingOptionsCollection
            {
                Default = GetOptions(multiselects[0]),
                Pivoted = allowPivot ? GetOptions(multiselects[1]) : null
            };

            List<SortingOption> GetOptions(IReadOnlyDimension sortingDimension)
            {
                List<SortingOption> options = [.. GetDimensionSortingOptions(sortingDimension)];
                // Sort time dimensions descending for GroupHorizontalBarChart
                if (visualization == VisualizationType.GroupHorizontalBarChart && sortingDimension.Type == DimensionType.Time)
                {
                    options.Reverse();
                }
                options.Add(GetSumSorting(meta.AvailableLanguages));
                options.Add(GetSameAsDataSorting(meta.AvailableLanguages));
                options.Add(GetReversedFromDataSorting(meta.AvailableLanguages));
                return options;
            }
        }

        // Not sortable at the moment
        private static VisualizationOption.SortingOptionsCollection GetLineChartOptions() => new() { Default = null, Pivoted = null };

        private static VisualizationOption.SortingOptionsCollection GetPieChartOptions(IEnumerable<string> languages, bool allowPivot)
        {
            return new()
            {
                Default =
                [
                    GetDescendingSorting(languages),
                    GetAscendingSorting(languages),
                    GetSameAsDataSorting(languages)
                ],
                Pivoted = allowPivot ?
                [
                    GetDescendingSorting(languages),
                    GetAscendingSorting(languages),
                    GetSameAsDataSorting(languages)
                ] 
                : null
            };
        }

        // Not sortable at the moment
        private static VisualizationOption.SortingOptionsCollection GetPyramidChartOptions() => new() { Default = null, Pivoted = null };

        private static VisualizationOption.SortingOptionsCollection GetScatterPlotOptions() => new() { Default = null, Pivoted = null };

        #endregion

        private static List<SortingOption> GetDimensionSortingOptions(IReadOnlyDimension dimension)
        {
            List<SortingOption> options = [];
            foreach (IReadOnlyDimensionValue val in dimension.Values)
            {
                options.Add(new SortingOption(val.Code, val.Name));
            }

            return options;
        }

        #region SORTING_TYPES

        private static SortingOption GetAscendingSorting(IEnumerable<string> languages)
        {
            Dictionary<string, string> translations = [];
            foreach (string lang in languages)
            {
                Localization local = Localization.FromLanguage(lang);
                translations[lang] = local.Translation.SortingOptions.Ascending;
            }

            return new SortingOption(ASCENDING, new(translations));
        }

        private static SortingOption GetDescendingSorting(IEnumerable<string> languages)
        {
            Dictionary<string, string> translations = [];
            foreach (string lang in languages)
            {
                Localization local = Localization.FromLanguage(lang);
                translations[lang] = local.Translation.SortingOptions.Descending;
            }

            return new SortingOption(DESCENDING, new(translations));
        }

        private static SortingOption GetSumSorting(IEnumerable<string> languages)
        {
            Dictionary<string, string> translations = [];
            foreach (string lang in languages)
            {
                Localization local = Localization.FromLanguage(lang);
                translations[lang] = local.Translation.SortingOptions.Sum;
            }

            return new SortingOption(SUM, new(translations));
        }

        private static SortingOption GetSameAsDataSorting(IEnumerable<string> languages)
        {
            Dictionary<string, string> translations = [];
            foreach (string lang in languages)
            {
                Localization local = Localization.FromLanguage(lang);
                translations[lang] = local.Translation.SortingOptions.NoSorting;
            }

            return new SortingOption(NO_SORTING, new(translations));
        }

        private static SortingOption GetReversedFromDataSorting(IEnumerable<string> languages)
        {
            Dictionary<string, string> translations = [];
            foreach (string lang in languages)
            {
                Localization local = Localization.FromLanguage(lang);
                translations[lang] = local.Translation.SortingOptions.NoSortingReversed;
            }

            return new SortingOption(REVERSED, new(translations));
        }

        #endregion
    }
}