#nullable enable
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.ExtensionMethods;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Visualization
{
    /// <summary>
    /// Adapter for building visualization responses for cube visualizations from matrix data.
    /// </summary>
    public static class PxVisualizerCubeAdapter
    {
        private readonly struct DimensionLayout
        {
            public readonly List<string> SingleValueDimensions;
            public readonly List<string> RowDimensionCodes;
            public readonly List<string> ColumnDimensionCodes;
            public readonly List<string> SelectableDimensionCodes;

            public DimensionLayout()
            {
                SingleValueDimensions = [];
                RowDimensionCodes = [];
                ColumnDimensionCodes = [];
                SelectableDimensionCodes = [];
            }
        }

        /// <summary>
        /// Builds a visualization response for a saved query.
        /// </summary>
        /// <param name="matrix">Matrix to visualize. Virtual values must already be applied before calling this method.</param>
        /// <param name="savedQuery">Saved query containing the visualization settings.</param>
        /// <returns>Visualization response for the matrix based on the given saved query.</returns>
        public static VisualizationResponse BuildVisualizationResponse(Matrix<DecimalDataValue> matrix, SavedQuery savedQuery)
        {
            EnsureLayout(savedQuery, matrix.Metadata);

            return BuildVisualizationResponse(matrix, savedQuery.Query, savedQuery.Settings);
        }

        /// <summary>
        /// Builds a visualization response for a matrix and a query.
        /// </summary>
        /// <param name="matrix">Matrix to visualize. Virtual values must already be applied before calling this method.</param>
        /// <param name="query">Query containing information about the table and selected values for dimensions.</param>
        /// <param name="settings">Visualization settings.</param>
        /// <returns>Visualization response for the matrix based on the given query and settings.</returns>
        public static VisualizationResponse BuildVisualizationResponse(Matrix<DecimalDataValue> matrix, MatrixQuery query, VisualizationSettings settings)
        {
            Matrix<DecimalDataValue> resultMatrix = TransformVisualizationMatrix(matrix, query, settings);
            DimensionLayout layout = GetDimensionLayout(matrix.Metadata, query, settings);
            MatrixExtensions.DataAndNotesCollection dataAndNotes = resultMatrix.ExtractDataAndNotes();

            return new VisualizationResponse()
            {
                TableReference = query.TableReference,
                Data = dataAndNotes.Data,
                DataNotes = dataAndNotes.Notes,
                MissingDataInfo = dataAndNotes.MissingValueInfo,
                MetaData = BuildVariableList(query.DimensionQueries, resultMatrix.Metadata),
                SelectableDimensionCodes = layout.SelectableDimensionCodes,
                RowDimensionCodes = layout.RowDimensionCodes,
                ColumnDimensionCodes = layout.ColumnDimensionCodes,
                Header = HeaderBuildingUtilities.GetHeader(matrix.Metadata, query),
                VisualizationSettings = BuildVisualizationSettings(matrix, settings)
            };
        }

        public static void EnsureLayout(SavedQuery savedQuery, IReadOnlyMatrixMetadata metadata)
        {
            if (savedQuery.Settings.Layout is null)
            {
                bool legacyPivotRequested = savedQuery.LegacyProperties.TryGetValue("PivotRequested", out object? obj) && obj is true;
                savedQuery.Settings.Layout = LayoutRules.GetPivotBasedLayout(
                    savedQuery.Settings.VisualizationType,
                    metadata,
                    savedQuery.Query,
                    legacyPivotRequested);
            }
        }

        public static Matrix<DecimalDataValue> TransformVisualizationMatrix(
            Matrix<DecimalDataValue> matrix,
            MatrixQuery query,
            VisualizationSettings settings)
        {
            DimensionLayout layout = GetDimensionLayout(matrix.Metadata, query, settings);
            return matrix.GetTransform(BuildFinalMap(matrix.Metadata, layout));
        }

        public static IReadOnlyMatrixMetadata TransformVisualizationMetadata(
            IReadOnlyMatrixMetadata metadata,
            MatrixQuery query,
            VisualizationSettings settings)
        {
            DimensionLayout layout = GetDimensionLayout(metadata, query, settings);
            return metadata.GetTransform(BuildFinalMap(metadata, layout));
        }

        /// <summary>
        /// Builds the non-data portion of a visualization response for a saved query.
        /// </summary>
        public static VisualizationMetadataResponse BuildVisualizationMetadataResponse(
            IReadOnlyMatrixMetadata metadata,
            SavedQuery savedQuery)
        {
            if (savedQuery.Settings.Layout is null)
            {
                bool legacyPivotRequested = savedQuery.LegacyProperties.TryGetValue("PivotRequested", out object? obj) && obj is true;
                savedQuery.Settings.Layout = LayoutRules.GetPivotBasedLayout(
                    savedQuery.Settings.VisualizationType,
                    metadata,
                    savedQuery.Query,
                    legacyPivotRequested);
            }

            DimensionLayout layout = GetDimensionLayout(metadata, savedQuery.Query, savedQuery.Settings);
            IReadOnlyMatrixMetadata resultMetadata = metadata.GetTransform(BuildFinalMap(metadata, layout));

            return new VisualizationMetadataResponse
            {
                TableReference = savedQuery.Query.TableReference,
                MetaData = BuildVariableList(savedQuery.Query.DimensionQueries, resultMetadata),
                SelectableDimensionCodes = layout.SelectableDimensionCodes,
                RowDimensionCodes = layout.RowDimensionCodes,
                ColumnDimensionCodes = layout.ColumnDimensionCodes,
                Header = HeaderBuildingUtilities.GetHeader(metadata, savedQuery.Query),
                VisualizationSettings = BuildVisualizationSettings(metadata, savedQuery.Settings)
            };
        }

        public static VisualizationResponse.PxVisualizerSettings BuildVisualizationSettings(Matrix<DecimalDataValue> matrix, VisualizationSettings settings)
        {
            return BuildVisualizationSettings(matrix.Metadata, settings);
        }

        public static VisualizationResponse.PxVisualizerSettings BuildVisualizationSettings(IReadOnlyMatrixMetadata metadata, VisualizationSettings settings)
        {
            IReadOnlyList<string> timeDimensionCodes = metadata.GetTimeDimension().Values.Codes;
            return new()
            {
                VisualizationType = settings.VisualizationType,
                DefaultSelectableDimensionCodes = settings.DefaultSelectableDimensionCodes,
                MultiselectableDimensionCode = settings.MultiselectableDimensionCode,
                TimeDimensionIntervals = TimeDimensionIntervalParser.DetermineIntervalFromCodes(timeDimensionCodes),
                TimeSeriesStartingPoint = TimeDimensionIntervalParser.DetermineTimeDimStartingPointFromCode(timeDimensionCodes[0]),
                CutValueAxis = settings.CutYAxis,
                ShowLastLabel = settings.MatchXLabelsToEnd,
                MarkerSize = settings.MarkerSize,
                Sorting = settings.Sorting,
                ShowDataPoints = settings.ShowDataPoints
            };
        }

        private static MatrixMap BuildFinalMap(IReadOnlyMatrixMetadata metadata, DimensionLayout layout)
        {
            return new MatrixMap(
                [.. layout.SingleValueDimensions
                    .Concat(layout.SelectableDimensionCodes)
                    .Concat(layout.RowDimensionCodes)
                    .Concat(layout.ColumnDimensionCodes)
                    .Select(code => metadata.DimensionMaps.First(map => map.Code == code))]);
        }
        
        private static List<Variable> BuildVariableList(Dictionary<string, DimensionQuery> dimensionQueries, IReadOnlyMatrixMetadata meta)
        {
            return [.. meta.Dimensions.Select(dimension =>
            {
                MultilanguageString name = dimension.Name;
                if (dimensionQueries.TryGetValue(dimension.Code, out DimensionQuery? query) &&
                    query.NameEdit != null)
                {
                    name = query.NameEdit;
                }

                return new Variable(
                    code: dimension.Code,
                    name: name,
                    note: dimension.GetMultilanguageDimensionProperty(PxSyntaxConstants.NOTE_KEY, meta.DefaultLanguage),
                    type: dimension.Type,
                    values: [.. dimension.Values.Select(v =>
                        v.ConvertToVariableValue(dimension.GetEliminationValueCode(), query))]
                    );
            })];
        }

        private static DimensionLayout GetDimensionLayout(IReadOnlyMatrixMetadata meta, MatrixQuery query, VisualizationSettings settings)
        {
            DimensionLayout layout = new();
            List<string> remainingVars = [.. meta.Dimensions.Select(v => v.Code)];

            // Selectables are added first from multivalue dimensions, so that the data from each selection is grouped together
            foreach (string code in query.DimensionQueries
                .Where(vq => vq.Value.Selectable)
                .Select(vq => vq.Key))
            {
                layout.SelectableDimensionCodes.Add(code);
                remainingVars.Remove(code);
            }

            foreach (string code in settings.Layout.RowDimensionCodes.Where(code => remainingVars.Exists(vc => vc == code)))
            {
                layout.RowDimensionCodes.Add(code);
                remainingVars.Remove(code);
            }

            foreach (string code in settings.Layout.ColumnDimensionCodes.Where(code => remainingVars.Exists(vc => vc == code)))
            {
                layout.ColumnDimensionCodes.Add(code);
                remainingVars.Remove(code);
            }

            layout.SingleValueDimensions.AddRange(remainingVars);

            return layout;
        }
    }
}
