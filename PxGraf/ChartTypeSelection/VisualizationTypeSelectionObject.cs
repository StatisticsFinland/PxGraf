#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Data;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Linq;
using System;

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

        public static VisualizationTypeSelectionObject FromQueryAndMatrix(MatrixQuery query, Matrix<DecimalDataValue> matrix)
        {
            var dimInfos = matrix.Metadata.Dimensions.Select(v =>
            {
                DimensionQuery varQuery = query.DimensionQueries[v.Code];
                return VariableInfo.FromQueryAndDimension(varQuery, v);
            }).ToList();

            VisualizationTypeSelectionObject result = new(dimInfos);

            AppendDataInformationToVTSO(result, matrix.Data);

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
            public DimensionType Type { get; }

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
            public string? CombinationValueCode { get; private set; }

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

            public static VariableInfo FromQueryAndDimension(DimensionQuery query, IReadOnlyDimension dimension)
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
                    size = dimension.Values.Count;
                    filterCanChangeToMultiValue = query.ValueFilter is FromFilter || query.ValueFilter is AllFilter; 
                }

                VariableInfo result = new (dimension.Code, dimension.Type, size)
                {
                    FilterCanChangeToMultiValue = filterCanChangeToMultiValue,
                };

                string? eliminationCode = dimension.GetEliminationValueCode();
                if (dimension.Values.FirstOrDefault(val => val.Code == eliminationCode) is IReadOnlyDimensionValue v)
                {
                    result.CombinationValueCode = v.Code;
                }

                if (dimension.Type == DimensionType.Content && dimension is ContentDimension cDim)
                {
                    if (query.Selectable) result.NumberOfUnits = 1;
                    else
                    {
                        // TODO replace with a mapper function once implemented to utils
                        List<MultilanguageString> units = [];
                        foreach(ContentDimensionValue cDimVal in cDim.Values)
                        {
                            units.Add(cDimVal.Unit);
                        }
                        result.NumberOfUnits = GetUniqueUnits(units);
                    }
                }
                else if (dimension.Type == DimensionType.Time && dimension is TimeDimension timeDim)
                {
                    result.IsIrregular = timeDim.Interval == TimeDimensionInterval.Irregular ||
                    TimeDimensionInterval.Irregular == Data.TimeVarIntervalParser.DetermineIntervalFromCodes(timeDim.Values.Codes);
                }

                return result;
            }

            private static int GetUniqueUnits(IEnumerable<MultilanguageString> units)
            {
                int result = 0;
                List<MultilanguageString> chkd = [];
                foreach (MultilanguageString unit in units)
                {
                    if (!chkd.Contains(unit))
                    {
                        result++;
                        chkd.Add(unit);
                    }
                }
                return result;
            }

            private VariableInfo(string code, DimensionType type, int size)
            {
                Code = code;
                Type = type;
                Size = size;
            }
        }

        private static void AppendDataInformationToVTSO(VisualizationTypeSelectionObject vtso, DecimalDataValue[] data)
        {
            foreach (DecimalDataValue value in data)
            {
                if (vtso.HasActualData && vtso.HasMissingData && vtso.HasNegativeData) break;

                if (value.Type == DataValueType.Exists)
                {
                    vtso.HasActualData = true;
                    if (value.UnsafeValue < 0) vtso.HasNegativeData = true;
                }
                else
                {
                    vtso.HasMissingData = true;
                }
            }
        }
    }
}
#nullable disable
