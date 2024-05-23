using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.ChartTypeSelection
{
    /// <summary>
    /// Contains all nessesary information to determine which chart types are valid for the cube and/or query this object was constructed from.
    /// Serves as an adapter between queries/cubes and the chart type selector so changes to one do not force changes to the other and the related unit tests.
    /// </summary>
    public class VisualizationTypeSelectionObject
    {
        /// <summary>
        /// List containing one variable info item for each variable in the query/cube.
        /// </summary>
        public IReadOnlyList<VariableInfo> Variables { get; }

        /// <summary>
        /// True if the selection contains actual double values (Not only missing data codes).
        /// </summary>
        public bool HasActualData { get; private set; }

        /// <summary>
        /// True if data contains ONE OR MORE missing value codes.
        /// </summary>
        public bool HasMissingData { get; private set; }

        /// <summary>
        /// True if the data contains one or more actual double values that have negative value (less than 0).
        /// </summary>
        public bool HasNegativeData { get; private set; }

        /// <summary>
        /// Constructs a type selection object based on the query and the actual cube object. Query is used to gain information about dynamic value filters and selectability.
        /// Dynamic value filters = number of variable values selected may change over time
        /// </summary>
        public static VisualizationTypeSelectionObject FromQueryAndCube(CubeQuery query, DataCube cube)
        {
            var varInfos = cube.Meta.Variables.Select(v =>
            {
                var varQuery = query.VariableQueries[v.Code];
                return VariableInfo.FromQueryAndVariable(varQuery, v);
            }).ToList();

            var result = new VisualizationTypeSelectionObject(varInfos);

            AppendDataInformationToVTSO(result, cube.Data);

            return result;
        }

        /// <summary>
        /// Constructs a type selection object based on the query and an archive cube object. Query is used to gain information about dynamic value filters and selectability.
        /// Dynamic value filters = number of variable values selected may change over time
        /// </summary>
        public static VisualizationTypeSelectionObject FromQueryAndCube(CubeQuery query, ArchiveCube cube)
        {
            var varInfos = cube.Meta.Variables.Select(v =>
            {
                var varQuery = query.VariableQueries[v.Code];
                return VariableInfo.FromQueryAndVariable(varQuery, v);
            }).ToList();

            var result = new VisualizationTypeSelectionObject(varInfos);

            AppendDataInformationToVTSO(result, cube.Data);

            return result;
        }

        public static VisualizationTypeSelectionObject FromCube(DataCube cube)
        {
            var varInfos = cube.Meta.Variables.Select(v => VariableInfo.FromVariable(v)).ToList();
            var result = new VisualizationTypeSelectionObject(varInfos);

            AppendDataInformationToVTSO(result, cube.Data);

            return result;
        }

        public static VisualizationTypeSelectionObject FromCube(ArchiveCube cube)
        {
            var varInfos = cube.Meta.Variables.Select(v => VariableInfo.FromVariable(v)).ToList();
            var result = new VisualizationTypeSelectionObject(varInfos);

            AppendDataInformationToVTSO(result, cube.Data);

            return result;
        }

        private VisualizationTypeSelectionObject(IReadOnlyList<VariableInfo> variables)
        {
            Variables = variables;

            HasActualData = false;
            HasMissingData = false;
            HasNegativeData = false;
        }

        /// <summary>
        /// Contains information about a variable relevant for determining
        /// which visualization types are valid for some query containing said variable
        /// </summary>
        public class VariableInfo
        {
            /// <summary>
            /// Unique identifier of the variable.
            /// </summary>
            public string Code { get; }

            /// <summary>
            /// Visualization type, bar chart, table, etc.
            /// </summary>
            public VariableType Type { get; }

            /// <summary>
            /// How many values does the variable have
            /// </summary>
            public int Size { get; }

            /// <summary>
            /// If the filter is dynamic (all, top, from etc) the number of selected values can change in the future
            /// </summary>
            public bool FilterCanChangeToMultiValue { get; private set; } = false;

            /// <summary>
            /// If the variable has a combination value (sum on other values) this is the identifier
            /// </summary>
#nullable enable
            public string? CombinationValueCode { get; private set; }
#nullable disable

            /// <summary>
            /// How many different units does the variable have.
            /// Only content variables have units.
            /// </summary>
            public int? NumberOfUnits { get; private set; }

            /// <summary>
            /// Only time variables can be consecutive or irregular. IE: cons: 2020, 2021, 2022 and irregular: 2020, 2022, 2023
            /// True if the variable is a time variable and has irregular values, false if the time variable has cosecutive values.
            /// null when the variable is not a time variable.
            /// </summary>
            public bool? IsIrregular { get; private set; }

            public static VariableInfo FromVariable(IReadOnlyVariable variable)
            {
                VariableInfo result = new(variable.Code, variable.Type, variable.IncludedValues.Count);
                if (variable.IncludedValues.FirstOrDefault(val => val.IsSumValue) is VariableValue v)
                {
                    result.CombinationValueCode = v.Code;
                }

                if (variable.Type == VariableType.Content)
                {
                    result.NumberOfUnits = GetUniqueUnits(variable.IncludedValues.Select(vv => vv.ContentComponent.Unit));
                }
                else if (variable.Type == VariableType.Time)
                {
                    result.IsIrregular =
                    TimeVariableInterval.Irregular == TimeVarIntervalParser.DetermineIntervalFromCodes(variable.IncludedValues.Select(v => v.Code));
                }
                return result;
            }

            public static VariableInfo FromQueryAndVariable(VariableQuery query, IReadOnlyVariable variable)
            {
                int size;
                bool filterCanChangeToMultiValue;
                if (query.Selectable)
                {
                    size = 1;
                    filterCanChangeToMultiValue = false;
                }
                else
                {
                    size = variable.IncludedValues.Count;

                    if (query.ValueFilter is FromFilter || query.ValueFilter is AllFilter)
                    {
                        filterCanChangeToMultiValue = true;
                    }
                    else
                    {
                        filterCanChangeToMultiValue = false;
                    }
                }

                var result = new VariableInfo(variable.Code, variable.Type, size)
                {
                    FilterCanChangeToMultiValue = filterCanChangeToMultiValue,
                };

                if (variable.IncludedValues.FirstOrDefault(val => val.IsSumValue) is VariableValue v)
                {
                    result.CombinationValueCode = v.Code;
                }

                if (variable.Type == VariableType.Content)
                {
                    if (query.Selectable) result.NumberOfUnits = 1;
                    else
                    {
                        result.NumberOfUnits = GetUniqueUnits(variable.IncludedValues.Select(vv => vv.ContentComponent.Unit));
                    }
                }
                else if (variable.Type == VariableType.Time)
                {
                    result.IsIrregular =
                    TimeVariableInterval.Irregular == TimeVarIntervalParser.DetermineIntervalFromCodes(variable.IncludedValues.Select(v => v.Code));
                }


                return result;
            }

            private static int GetUniqueUnits(IEnumerable<IReadOnlyMultiLanguageString> units)
            {
                int result = 0;
                List<IReadOnlyMultiLanguageString> chkd = [];
                foreach (IReadOnlyMultiLanguageString unit in units)
                {
                    if (!chkd.Contains(unit))
                    {
                        result++;
                        chkd.Add(unit);
                    }
                }
                return result;
            }

            private VariableInfo(string code, VariableType type, int size)
            {
                Code = code;
                Type = type;
                Size = size;
            }
        }

        private static void AppendDataInformationToVTSO(VisualizationTypeSelectionObject vtso, IEnumerable<DataValue> data)
        {
            foreach (DataValue value in data)
            {
                if (vtso.HasActualData && vtso.HasMissingData && vtso.HasNegativeData) break;

                if (value.TryGetValue(out var v))
                {
                    vtso.HasActualData = true;
                    if (v < 0) vtso.HasNegativeData = true;
                }
                else
                {
                    vtso.HasMissingData = true;
                }
            }
        }

        private static void AppendDataInformationToVTSO(VisualizationTypeSelectionObject vtso, IEnumerable<double?> data)
        {
            foreach (double? value in data)
            {
                if (vtso.HasActualData && vtso.HasMissingData && vtso.HasNegativeData) break;

                if (value.HasValue)
                {
                    vtso.HasActualData = true;
                    if (value < 0) vtso.HasNegativeData = true;
                }
                else
                {
                    vtso.HasMissingData = true;
                }
            }
        }
    }
}
