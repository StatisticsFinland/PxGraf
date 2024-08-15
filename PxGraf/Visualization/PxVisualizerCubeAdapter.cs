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
    /// Contains the query information as it was received with the request.
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
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(matrix.Metadata.Dimensions, query, matrix.Metadata.AvailableLanguages);

            return new VisualizationResponse()
            {
                TableReference = query.TableReference,
                Data = dataAndNotes.Data,
                DataNotes = dataAndNotes.Notes,
                MissingDataInfo = dataAndNotes.MissingValueInfo,
                MetaData = resultMatrix.Metadata.Dimensions.Select(d => d.ConvertToVariable()).ToList(),
                SelectableVariableCodes = layout.SelectableVariableCodes,
                RowVariableCodes = layout.RowVariableCodes,
                ColumnVariableCodes = layout.ColumnVariableCodes,
                Header = HeaderBuildingUtilities.ReplaceTimePlaceholdersInHeader(header, matrix.Metadata.GetTimeDimension()),
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
