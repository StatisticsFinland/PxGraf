using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Metadata;
using PxGraf.Models.Requests;
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
        public static IReadOnlyList<SortingOption> Get(IReadOnlyMatrixMetadata meta, VisualizationSettingsRequest request)
        {
            return request.SelectedVisualization switch
            {
                VisualizationType.VerticalBarChart => GetVerticalBarChartOptions(),
                VisualizationType.GroupVerticalBarChart => GetGroupVerticalBarChartOptions(),
                VisualizationType.StackedVerticalBarChart => GetStackedVerticalBarChartOptions(),
                VisualizationType.PercentVerticalBarChart => GetStackedVerticalBarChartOptions(),
                VisualizationType.HorizontalBarChart => GetHorizontalBarChartOptions(meta.AvailableLanguages),
                VisualizationType.GroupHorizontalBarChart or
                VisualizationType.StackedHorizontalBarChart or
                VisualizationType.PercentHorizontalBarChart => GetMultiDimHorizontalBarChartOptions(request.SelectedVisualization, meta, request),
                VisualizationType.PyramidChart => GetPyramidChartOptions(),
                VisualizationType.PieChart => GetPieChartOptions(meta.AvailableLanguages),
                VisualizationType.LineChart => GetLineChartOptions(),
                VisualizationType.ScatterPlot => GetScatterPlotOptions(),
                _ => [],
            };
        }

        #region SORTING_OPTIONS

        // Not sortable at the moment
        private static List<SortingOption> GetVerticalBarChartOptions() => [];

        // Not sortable at the moment
        private static List<SortingOption> GetGroupVerticalBarChartOptions() => [];

        // Not sortable at the moment
        private static List<SortingOption> GetStackedVerticalBarChartOptions() => [];

        private static List<SortingOption> GetHorizontalBarChartOptions(IEnumerable<string> languages)
            =>
            [
                GetDescendingSorting(languages),
                GetAscendingSorting(languages),
                GetSameAsDataSorting(languages),
                GetReversedFromDataSorting(languages)
            ];

        private static List<SortingOption> GetMultiDimHorizontalBarChartOptions(VisualizationType visualization, IReadOnlyMatrixMetadata meta, VisualizationSettingsRequest request)
        {
            // Selectable dimensions are excluded from sorting, OBS: '!'
            List<IReadOnlyDimension> multiselects = [.. meta.GetMultivalueDimensions().Where(mvv => !request.Query.DimensionQueries[mvv.Code].Selectable)];
            IReadOnlyDimension sortingOptionsDimension = GetPivot(visualization, meta, request) ? multiselects[1] : multiselects[0];

            List<SortingOption> options = [.. GetDimensionSortingOptions(sortingOptionsDimension)];
            // Sort time dimensions descendingly for GroupHorizontalBarChart
            if (request.SelectedVisualization == VisualizationType.GroupHorizontalBarChart && sortingOptionsDimension.Type == DimensionType.Time)
            {
                options.Reverse();
            }
            options.Add(GetSumSorting(meta.AvailableLanguages));
            options.Add(GetSameAsDataSorting(meta.AvailableLanguages));
            options.Add(GetReversedFromDataSorting(meta.AvailableLanguages));
            return options;
        }

        // Not sortable at the moment
        private static List<SortingOption> GetLineChartOptions() => [];

        private static List<SortingOption> GetPieChartOptions(IEnumerable<string> languages)
            =>
            [
                GetDescendingSorting(languages),
                GetAscendingSorting(languages),
                GetSameAsDataSorting(languages)
            ];

        // Not sortable at the moment
        private static List<SortingOption> GetPyramidChartOptions() => [];

        private static List<SortingOption> GetScatterPlotOptions() => [];

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

        private static bool GetPivot(VisualizationType visualization, IReadOnlyMatrixMetadata meta, VisualizationSettingsRequest request)
        {
            bool autoPivot = AutoPivotRules.GetAutoPivot(visualization, meta, request.Query);
            bool manualPivot = ManualPivotRules.GetManualPivotability(visualization, meta, request.Query) && request.PivotRequested;
            return autoPivot ^ manualPivot;
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