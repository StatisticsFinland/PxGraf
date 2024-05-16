using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Utility;
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
        /// Provides all valid sorting options for a visualization. Languages are based on the languages of the cube meta object.
        /// </summary>
        public static IReadOnlyList<SortingOption> Get(IReadOnlyCubeMeta meta, VisualizationSettingsRequest request)
        {
            return request.SelectedVisualization switch
            {
                VisualizationType.VerticalBarChart => GetVerticalBarChartOptions(),
                VisualizationType.GroupVerticalBarChart => GetGroupVerticalBarChartOptions(),
                VisualizationType.StackedVerticalBarChart => GetStackedVerticalBarChartOptions(),
                VisualizationType.PercentVerticalBarChart => GetStackedVerticalBarChartOptions(),
                VisualizationType.HorizontalBarChart => GetHorizontalBarChartOptions(meta.Languages),
                VisualizationType.GroupHorizontalBarChart or VisualizationType.StackedHorizontalBarChart or VisualizationType.PercentHorizontalBarChart => GetMultiDimHorizontalBarChartOptions(request.SelectedVisualization, meta, request),
                VisualizationType.PyramidChart => GetPyramidChartOptions(),
                VisualizationType.PieChart => GetPieChartOptions(meta.Languages),
                VisualizationType.LineChart => GetLineChartOptions(),
                VisualizationType.ScatterPlot => GetScatterPlotOptions(),
                _ => new List<SortingOption>(),
            };
        }

        /// <summary>
        /// One dimensional, ascending or descending sorting
        /// </summary>
        public static DataCube SortOneDimensionalCharts(DataCube cube, string sorting)
        {
            return cube.GetTransform(GetSortedMapForOneDimensionalCharts(cube, sorting));
        }

        /// <summary>
        /// Sum or variable value based, descending sorting
        /// </summary>
        public static DataCube SortMultidimHorizontalBarChart(VisualizationType visualization, DataCube cube, string sorting, bool pivotRequested)
        {
            return cube.GetTransform(GetSortedMapForMultidimHorizontalBarChart(visualization, cube, sorting, pivotRequested));
        }

        #region PRIVATES

        #region SORTING_OPTIONS

        // Not sortable at the moment
        private static IReadOnlyList<SortingOption> GetVerticalBarChartOptions() => new List<SortingOption>();

        // Not sortable at the moment
        private static IReadOnlyList<SortingOption> GetGroupVerticalBarChartOptions() => new List<SortingOption>();

        // Not sortable at the moment
        private static IReadOnlyList<SortingOption> GetStackedVerticalBarChartOptions() => new List<SortingOption>();

        private static IReadOnlyList<SortingOption> GetHorizontalBarChartOptions(IEnumerable<string> languages)
            => new List<SortingOption>()
            {
                GetDescendingSorting(languages),
                GetAscendingSorting(languages),
                GetSameAsDataSorting(languages)
            };

        private static IReadOnlyList<SortingOption> GetMultiDimHorizontalBarChartOptions(VisualizationType visualization, IReadOnlyCubeMeta meta, VisualizationSettingsRequest request)
        {
            // Selectable variables are excluded from sorting, OBS: '!'
            var multiselects = meta.GetMultivalueVariables().Where(mvv => !request.Query.VariableQueries[mvv.Code].Selectable).ToList();
            var sortingOptionsVariable = GetPivot(visualization, meta, request) ? multiselects[1] : multiselects[0];

            var options = new List<SortingOption>();
            options.AddRange(GetVariableSortingOptions(sortingOptionsVariable));
            // Sort time variables descendingly for GroupHorizontalBarChart
            if (request.SelectedVisualization == VisualizationType.GroupHorizontalBarChart && sortingOptionsVariable.Type == VariableType.Time)
            {
                options.Reverse();
            }
            options.Add(GetSumSorting(meta.Languages));
            options.Add(GetSameAsDataSorting(meta.Languages));
            return options;
        }

        // Not sortable at the moment
        private static IReadOnlyList<SortingOption> GetLineChartOptions() => new List<SortingOption>();

        private static IReadOnlyList<SortingOption> GetPieChartOptions(IEnumerable<string> languages)
            => new List<SortingOption>()
            {
                GetDescendingSorting(languages),
                GetAscendingSorting(languages),
                GetSameAsDataSorting(languages)
            };

        // Not sortable at the moment
        private static IReadOnlyList<SortingOption> GetPyramidChartOptions() => new List<SortingOption>();

        private static IReadOnlyList<SortingOption> GetScatterPlotOptions() => new List<SortingOption>();

        #endregion

        #region UTILITY_GETS

        /// <summary>
        /// One dimensional, ascending or descending sorting
        /// </summary>
        private static CubeMap GetSortedMapForOneDimensionalCharts(DataCube cube, string sorting)
        {
            if (sorting == DESCENDING || sorting == ASCENDING)
            {
                var multiselects = cube.Meta.GetMultivalueVariables();
                if (multiselects.Count == 0) return cube.Meta.BuildMap(); // Sorting one value does nothing.

                // If thre are selectable variables and therefore many multiselects, we want to select the last one because it will be in the view
                var multiselectVariable = multiselects[multiselects.Count - 1];
                if (sorting == DESCENDING)
                {
                    return cube.Meta.BuildMap().CloneWithEditedVariable(multiselectVariable.Code, DescendingSumSorting(multiselectVariable, cube));
                }
                else
                {
                    return cube.Meta.BuildMap().CloneWithEditedVariable(multiselectVariable.Code, SumSorting(multiselectVariable, cube));
                }
            }
            else
            {
                return cube.Meta.BuildMap();
            }
        }

        /// <summary>
        /// Sum or variable value based, descending sorting
        /// </summary>
        private static CubeMap GetSortedMapForMultidimHorizontalBarChart(VisualizationType visualization, DataCube cube, string sorting, bool pivotRequested)
        {
            if (sorting == NO_SORTING || string.IsNullOrEmpty(sorting)) return cube.Meta.BuildMap();

            var sortingVariable = GetSortingVariable(visualization, cube.Meta, pivotRequested);
            if (sorting == SUM)
            {
                return cube.Meta.BuildMap().CloneWithEditedVariable(sortingVariable.Code, DescendingSumSorting(sortingVariable, cube));
            }
            else // sorted based on particular variable value
            {
                //OBS: sortingVariable and sortingOptionsVariable are not the same thing!
                // sortingOptionsVariable = pick one value and sort the values of the sortingVariable besed on that value.
                // sortingVariable = these value will be sorted
                var sortingOptionsVariable = GetSortingOptionsVariable(visualization, cube.Meta, pivotRequested);
                var focusValue = sortingOptionsVariable.IncludedValues.First(vv => vv.Code == sorting);
                var sortingValueOrder = VariableValueSorting(focusValue, sortingOptionsVariable, sortingVariable, cube);
                var transformMap = cube.Meta.BuildMap().CloneWithEditedVariable(sortingVariable.Code, sortingValueOrder);

                // when the cube is sorted based on particualr variable value, it is moved to be the first value of that variable.
                var sortingOptionAsFirst = new List<IReadOnlyVariableValue> { focusValue };
                sortingOptionAsFirst.AddRange(sortingOptionsVariable.IncludedValues.ToList().FindAll(v => v != focusValue));

                return transformMap.CloneWithEditedVariable(sortingOptionsVariable.Code, sortingOptionAsFirst);
            }
        }

        private static IReadOnlyVariable GetSortingVariable(VisualizationType visualization, IReadOnlyCubeMeta meta, bool manualPivotRequested)
        {
            var multiselects = meta.GetMultivalueVariables();
            return GetPivot(visualization, meta, manualPivotRequested) ? multiselects[0] : multiselects[1];
        }

        private static IReadOnlyVariable GetSortingOptionsVariable(VisualizationType visualization, IReadOnlyCubeMeta meta, bool pivotRequested)
        {
            var multiselects = meta.GetMultivalueVariables();
            return GetPivot(visualization, meta, pivotRequested) ? multiselects[1] : multiselects[0];
        }

        private static IReadOnlyList<SortingOption> GetVariableSortingOptions(IReadOnlyVariable variable)
        {
            List<SortingOption> options = new();
            foreach (var val in variable.IncludedValues)
            {
                options.Add(new SortingOption(val.Code, val.Name));
            }

            return options;
        }

        private static bool GetPivot(VisualizationType visualization, IReadOnlyCubeMeta meta, bool manualPivotRequest)
        {
            bool autoPivot = AutoPivotRules.GetAutoPivot(visualization, meta);
            bool manualPivot = ManualPivotRules.GetManualPivotability(visualization, meta) && manualPivotRequest;
            return autoPivot ^ manualPivot;
        }

        private static bool GetPivot(VisualizationType visualization, IReadOnlyCubeMeta meta, VisualizationSettingsRequest request)
        {
            bool autoPivot = AutoPivotRules.GetAutoPivot(visualization, meta, request.Query);
            bool manualPivot = ManualPivotRules.GetManualPivotability(visualization, meta, request.Query) && request.PivotRequested;
            return autoPivot ^ manualPivot;
        }

        #endregion

        #region SORTING_TYPES

        private static SortingOption GetAscendingSorting(IEnumerable<string> languages)
        {
            MultiLanguageString translations = new();
            foreach (var lang in languages)
            {
                var local = Localization.FromLanguage(lang);
                translations.AddTranslation(lang, local.Translation.SortingOptions.Ascending);
            }

            return new SortingOption(ASCENDING, translations);
        }

        private static SortingOption GetDescendingSorting(IEnumerable<string> languages)
        {
            MultiLanguageString translations = new();
            foreach (var lang in languages)
            {
                var local = Localization.FromLanguage(lang);
                translations.AddTranslation(lang, local.Translation.SortingOptions.Descending);
            }

            return new SortingOption(DESCENDING, translations);
        }

        private static SortingOption GetSumSorting(IEnumerable<string> languages)
        {
            MultiLanguageString translations = new();
            foreach (var lang in languages)
            {
                var local = Localization.FromLanguage(lang);
                translations.AddTranslation(lang, local.Translation.SortingOptions.Sum);
            }

            return new SortingOption(SUM, translations);
        }

        private static SortingOption GetSameAsDataSorting(IEnumerable<string> languages)
        {
            MultiLanguageString translations = new();
            foreach (var lang in languages)
            {
                var local = Localization.FromLanguage(lang);
                translations.AddTranslation(lang, local.Translation.SortingOptions.NoSorting);
            }

            return new SortingOption(NO_SORTING, translations);
        }

        #endregion

        #region SORTING_FUNCTIONS

        private static IReadOnlyList<IReadOnlyVariableValue> DescendingSumSorting(IReadOnlyVariable sortingVariable, DataCube cube)
        {
            return sortingVariable.IncludedValues
                .OrderByDescending(v =>
                   {
                       var map = cube.Meta.BuildMap().CollapseOneVariableInMap(v, sortingVariable);
                       return cube.GetTransform(map).CalculateCubeSum();
                   }
                )
                .ToArray();
        }

        private static IReadOnlyList<IReadOnlyVariableValue> SumSorting(IReadOnlyVariable sortingVariable, DataCube data)
        {
            return sortingVariable.IncludedValues
                .OrderBy(v =>
                    {
                        var map = data.Meta.BuildMap().CollapseOneVariableInMap(v, sortingVariable);
                        return data.GetTransform(map).CalculateCubeSum();
                    }
                )
                .ToArray();
        }

        private static IReadOnlyList<IReadOnlyVariableValue> VariableValueSorting(
            IReadOnlyVariableValue sortingBaseVariableValue,
            IReadOnlyVariable sortingBaseVariable,
            IReadOnlyVariable sortingVariable,
            DataCube data)
        {
            return DescendingSumSorting(sortingVariable, data.GetTransform(data.Meta.BuildMap().CollapseOneVariableInMap(sortingBaseVariableValue, sortingBaseVariable)));
        }

        #endregion

        #endregion
    }
}