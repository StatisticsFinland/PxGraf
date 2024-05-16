using PxGraf.Data;
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
                SingleValueVariables = new List<string>();
                RowVariableCodes = new List<string>();
                ColumnVariableCodes = new List<string>();
                SelectableVariableCodes = new List<string>();
            }
        }

        public static VisualizationResponse BuildVisualizationResponse(DataCube cube, SavedQuery savedQuery)
        {
            if (savedQuery.Settings.Layout is null)
            {
                bool legacyPivotRequested = savedQuery.LegacyProperties.TryGetValue("PivotRequested", out object obj) && (bool)obj;

                savedQuery.Settings.Layout =
                    LayoutRules.GetPivotBasedLayout(savedQuery.Settings.VisualizationType, cube.Meta, savedQuery.Query, legacyPivotRequested);
            }

            return BuildVisualizationResponse(cube, savedQuery.Query, savedQuery.Settings);
        }

        public static VisualizationResponse BuildVisualizationResponse(DataCube cube, CubeQuery query, VisualizationSettings settings)
        {
            VariableLayout layout = GetVariableLayout(cube, query, settings);

            CubeMap finalMap = new(
                layout.SingleValueVariables
                    .Concat(layout.SelectableVariableCodes)
                    .Concat(layout.RowVariableCodes)
                    .Concat(layout.ColumnVariableCodes)
                    .Select(vc => cube.Meta.BuildMap().First(vm => vm.Code == vc)).ToList()
                );

            DataCube resultCube = cube.GetTransform(finalMap);

            DataCubeUtilities.DataAndNotesCollection dataAndNotes = resultCube.ExtractDataAndNotes();
            TimeVarIntervalParser.TimeVariableInformation timeVarInfo = TimeVarIntervalParser.Parse(cube.Meta);

            return new VisualizationResponse()
            {
                TableReference = query.TableReference,
                Data = dataAndNotes.Data,
                DataNotes = dataAndNotes.Notes,
                MissingDataInfo = dataAndNotes.MissingValueInfo,
                MetaData = resultCube.Meta.Variables,
                SelectableVariableCodes = layout.SelectableVariableCodes,
                RowVariableCodes = layout.RowVariableCodes,
                ColumnVariableCodes = layout.ColumnVariableCodes,
                Header = cube.Meta.GetHeaderWithoutTimePlaceholders(),
                VisualizationSettings = new()
                {
                    VisualizationType = settings.VisualizationType,
                    DefaultSelectableVariableCodes = settings.DefaultSelectableVariableCodes,
                    MultiselectableVariableCode = settings.MultiselectableVariableCode,
                    TimeVariableIntervals = timeVarInfo.Interval,
                    TimeSeriesStartingPoint = timeVarInfo.StartingPoint,
                    CutValueAxis = settings.CutYAxis,
                    ShowLastLabel = settings.MatchXLabelsToEnd,
                    MarkerSize = settings.MarkerSize,
                    Sorting = settings.Sorting,
                    ShowDataPoints = settings.ShowDataPoints
                }
            };
        }

        private static VariableLayout GetVariableLayout(DataCube cube, CubeQuery query, VisualizationSettings settings)
        {
            VariableLayout layout = new();
            List<string> remainingVars = cube.Meta.Variables.Select(v => v.Code).ToList();

            // Selectables are added first from multivalue variables, so that the data from each selection is grouped together
            foreach (string code in query.VariableQueries
                .Where(vq => vq.Value.Selectable)
                .Select(vq => vq.Key))
            {
                layout.SelectableVariableCodes.Add(code);
                remainingVars.Remove(code);
            }

            foreach (string code in settings.Layout.RowVariableCodes)
            {
                if (remainingVars.Exists(vc => vc == code))
                {
                    layout.RowVariableCodes.Add(code);
                    remainingVars.Remove(code);
                }
            }

            foreach (string code in settings.Layout.ColumnVariableCodes)
            {
                if (remainingVars.Exists(vc => vc == code))
                {
                    layout.ColumnVariableCodes.Add(code);
                    remainingVars.Remove(code);
                }
            }

            layout.SingleValueVariables.AddRange(remainingVars);

            return layout;
        }
    }
}
