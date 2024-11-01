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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace PxGraf.Visualization
{
    /// <summary>
    /// Adapter for building visualization responses for cube visualizations from matrix data.
    /// </summary>
    public static class PxVisualizerCubeAdapter
    {
        private readonly struct VariableLayout
        {
            public readonly List<string> SingleValueVariables;
            public readonly List<string> RowVariableCodes;
            public readonly List<string> ColumnVariableCodes;
            public readonly List<string> SelectableVariableCodes;

            public VariableLayout()
            {
                SingleValueVariables = [];
                RowVariableCodes = [];
                ColumnVariableCodes = [];
                SelectableVariableCodes = [];
            }
        }

        /// <summary>
        /// Builds a visualization response for a saved query.
        /// </summary>
        /// <param name="matrix">Matrix to visualize.</param>
        /// <param name="savedQuery">Saved query containing the visualization settings.</param>
        /// <returns>Visualization response for the matrix based on the given saved query.</returns>
        public static VisualizationResponse BuildVisualizationResponse(Matrix<DecimalDataValue> matrix, SavedQuery savedQuery)
        {
            if (savedQuery.Settings.Layout is null)
            {
                bool legacyPivotRequested = savedQuery.LegacyProperties.TryGetValue("PivotRequested", out object obj) && (bool)obj;

                savedQuery.Settings.Layout =
                    LayoutRules.GetPivotBasedLayout(savedQuery.Settings.VisualizationType, matrix.Metadata, savedQuery.Query, legacyPivotRequested);
            }

            return BuildVisualizationResponse(matrix, savedQuery.Query, savedQuery.Settings);
        }

        /// <summary>
        /// Builds a visualization response for a matrix and a query.
        /// </summary>
        /// <param name="matrix">Matrix to visualize.</param>
        /// <param name="query">Query containing information about the table and selected values for dimensions.</param>
        /// <param name="settings">Visualization settings.</param>
        /// <returns>Visualization response for the matrix based on the given query and settings.</returns>
        public static VisualizationResponse BuildVisualizationResponse(Matrix<DecimalDataValue> matrix, MatrixQuery query, VisualizationSettings settings)
        {
            VariableLayout layout = GetVariableLayout(matrix.Metadata, query, settings);

            MatrixMap finalMap = new(
                layout.SingleValueVariables
                    .Concat(layout.SelectableVariableCodes)
                    .Concat(layout.RowVariableCodes)
                    .Concat(layout.ColumnVariableCodes)
                    .Select(vc => matrix.Metadata.DimensionMaps.First(vm => vm.Code == vc))
                    .ToList()
                );

            Matrix<DecimalDataValue> resultMatrix = matrix.GetTransform(finalMap);
            MatrixExtensions.DataAndNotesCollection dataAndNotes = resultMatrix.ExtractDataAndNotes();
            IReadOnlyList<string> timeDimensionCodes = matrix.Metadata.GetTimeDimension().Values.Codes;
            
            return new VisualizationResponse()
            {
                TableReference = query.TableReference,
                Data = dataAndNotes.Data,
                DataNotes = dataAndNotes.Notes,
                MissingDataInfo = dataAndNotes.MissingValueInfo,
                MetaData = resultMatrix.Metadata.Dimensions.Select(d => d.ConvertToVariable(query.DimensionQueries, matrix.Metadata)).ToList(),
                SelectableVariableCodes = layout.SelectableVariableCodes,
                RowVariableCodes = layout.RowVariableCodes,
                ColumnVariableCodes = layout.ColumnVariableCodes,
                Header = HeaderBuildingUtilities.GetHeader(matrix.Metadata, query),
                VisualizationSettings = new()
                {
                    VisualizationType = settings.VisualizationType,
                    DefaultSelectableVariableCodes = settings.DefaultSelectableVariableCodes,
                    MultiselectableVariableCode = settings.MultiselectableVariableCode,
                    TimeVariableIntervals = TimeVarIntervalParser.DetermineIntervalFromCodes(timeDimensionCodes),
                    TimeSeriesStartingPoint = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode(timeDimensionCodes[0]),
                    CutValueAxis = settings.CutYAxis,
                    ShowLastLabel = settings.MatchXLabelsToEnd,
                    MarkerSize = settings.MarkerSize,
                    Sorting = settings.Sorting,
                    ShowDataPoints = settings.ShowDataPoints
                }
            };
        }

        private static VariableLayout GetVariableLayout(IReadOnlyMatrixMetadata meta, MatrixQuery query, VisualizationSettings settings)
        {
            VariableLayout layout = new();
            List<string> remainingVars = meta.Dimensions.Select(v => v.Code).ToList();

            // Selectables are added first from multivalue variables, so that the data from each selection is grouped together
            foreach (string code in query.DimensionQueries
                .Where(vq => vq.Value.Selectable)
                .Select(vq => vq.Key))
            {
                layout.SelectableVariableCodes.Add(code);
                remainingVars.Remove(code);
            }

            foreach (string code in settings.Layout.RowVariableCodes.Where(code => remainingVars.Exists(vc => vc == code)))
            {
                layout.RowVariableCodes.Add(code);
                remainingVars.Remove(code);
            }

            foreach (string code in settings.Layout.ColumnVariableCodes.Where(code => remainingVars.Exists(vc => vc == code)))
            {
                layout.ColumnVariableCodes.Add(code);
                remainingVars.Remove(code);
            }

            layout.SingleValueVariables.AddRange(remainingVars);

            return layout;
        }
    }
}
