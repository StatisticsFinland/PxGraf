using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.SavedQueries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    public static class TestDataCubeBuilder
    {
        public static VisualizationTypeSelectionObject BuildTestVisualizationTypeSelectionObject(List<DimensionParameters> variables, bool negativeData = false)
        {
            // TODO: Fix
            throw new NotImplementedException();
            // return VisualizationTypeSelectionObject.FromQueryAndCube(BuildTestCubeQuery(variables), BuildTestMatrix(variables, negativeData));
        }

        public static MatrixQuery BuildTestCubeQuery(List<DimensionParameters> varParams)
        {
            MatrixQuery query = new()
            {
                TableReference = new PxTableReference()
                {
                    Name = "TestPxFile.px",
                    Hierarchy = ["testpath", "to", "test", "file"]
                },
                ChartHeaderEdit = null
            };

            Dictionary<string, DimensionQuery> variableQueries = [];
            for (int i = 0; i < varParams.Count; i++)
            {
                variableQueries[$"variable-{i}"] = BuildTestVariableQuery(varParams[i]);
            }

            query.DimensionQueries = variableQueries;
            return query;
        }

        public static SavedQuery BuildTestSavedQuery(List<DimensionParameters> varParams, bool archived, VisualizationSettings settings)
        {
            return new SavedQuery(BuildTestCubeQuery(varParams), archived, settings, DateTime.Now);
        }

        public static SavedQuery BuildTestSavedQuery(List<DimensionParameters> varParams, bool archived, VisualizationCreationSettings creationSettings, Matrix<DecimalDataValue> matrix)
        {
            MatrixQuery query = BuildTestCubeQuery(varParams);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(matrix.Metadata, query);

            return new SavedQuery(BuildTestCubeQuery(varParams), archived, settings, DateTime.Now);
        }

        public static ArchiveCube BuildTestArchiveCube(List<DimensionParameters> varParams, bool negativeData = false, bool missingData = false)
        {
            // TODO: Fix
            throw new NotImplementedException();
            // return ArchiveCube.FromMatrixAndQuery(BuildTestMatrix(varParams, negativeData, missingData));
        }

        public static DimensionQuery BuildTestVariableQuery(DimensionParameters varParams)
        {
            IValueFilter valueFilter;
            if (varParams.ValueFilter != null)
            {
                valueFilter = varParams.ValueFilter;
            }
            else
            {
                List<string> values = [];
                if (varParams.Type == DimensionType.Time && !varParams.Irregular)
                {
                    for (int i = 0; i < varParams.Size; i++)
                    {
                        values.Add($"{2000 + i}");
                    }
                }
                else
                {
                    for (int i = 0; i < varParams.Size; i++)
                    {
                        values.Add($"value-{i}");
                    }
                }
                valueFilter = new ItemFilter(values);
            }

            return new DimensionQuery()
            {
                ValueFilter = valueFilter,
                Selectable = varParams.Selectable
            };
        }

        public static Matrix<DecimalDataValue> BuildTestMatrix(List<DimensionParameters> dimParams, bool negativeData = false, bool missingData = false)
        {
            IReadOnlyMatrixMetadata meta = BuildTestMeta(dimParams);
            int dataSize = meta.Dimensions.Count == 0 ? 0 : meta.Dimensions.Aggregate(1, (acc, var) => acc * var.Values.Count);
            return new Matrix<DecimalDataValue>(meta, BuildTestData(dataSize, negativeData, missingData).ToArray());
        }

        public static MatrixMetadata BuildTestMeta(List<DimensionParameters> varParams)
        {
            if (varParams.Exists(v => v.Size < 1)) return null;

            List<string> languages = ["fi", "en"];
            List<Dimension> variables = BuildTestDimensions(varParams);
            Dictionary<string, string> headerTranslations = new()
            {
                { "fi", "Test Header" },
                { "en", "Test Header.en" }
            };
            MultilanguageString header = new(headerTranslations);
            Dictionary<string, string> noteTranslations = new()
            {
                { "fi", "Test note" },
                { "en", "Test note.en" }
            };
            MultilanguageString note = new(noteTranslations);
            return new MatrixMetadata(languages[0], languages, variables, []);
        }

        private static List<Dimension> BuildTestDimensions(List<DimensionParameters> parameters)
        {
            var results = new List<Dimension>();
            int i = 0;
            foreach (var param in parameters)
            {
                results.Add(BuildTestDimension(param.Name ?? $"variable-{i++}", param));
            }

            return results;
        }

        public static Dimension BuildTestDimension(string name, DimensionParameters param)
        {
            if (param.Size < 1) return null;
            Dictionary<string, string> nameTranslations = new()
            {
                { "fi", name },
                { "en", $"{name}.en" }
            };

            // TODO: implement
            throw new NotImplementedException();
        }

        public static Dimension BuildTestDimension(string name, List<string> valueNames, DimensionType type, int decimals = 0, bool sameUnit = false, bool sameSource = false, bool hasCombinationValue = false)
        {
            Dictionary<string, string> nameTranslations = new()
            {
                { "fi", name },
                { "en", $"{name}.en" }
            };
            // TODO: Implement
            throw new NotImplementedException();
        }

        private static List<DimensionValue> BuildVariableValues(DimensionParameters varParam)
        {
            List<DimensionValue> results = [];
            /*
            for (int i = 0; i < varParam.Size; i++)
            {
                string name = varParam.Type == VariableType.Time && !varParam.Irregular ? $"{2000 + i}" : $"value-{i}";
                Dictionary<string, string> nameTranslations = new()
                {
                    { "fi", name },
                    { "en", $"{name}.en" }
                };
                VariableValue vv = new(name, new MultiLanguageString(nameTranslations), null, varParam.HasCombinationValue && i == 0);
                if (varParam.Type == PxGraf.Enums.VariableType.Content)
                {
                    Dictionary<string, string> unitTranslations = new()
                    {
                        { "fi", varParam.SameUnit ? "testUnit" : $"{name}-unit" },
                        { "en", varParam.SameUnit ? "testUnit" : $"{name}-unit.en" }
                    };
                    Dictionary<string, string> sourceTranslations = new()
                    {
                        { "fi", varParam.SameSource ? "testSource" : $"{name}-source" },
                        { "en", varParam.SameSource ? "testSource" : $"{name}-source.en" }
                    };
                    vv.ContentComponent = new ContentComponent(
                        new MultiLanguageString(unitTranslations),
                        new MultiLanguageString(sourceTranslations),
                        varParam.Decimals, "2009-09-01T00:00:00.000Z");
                }
                results.Add(vv);
            }
            */
            // TODO: implement. Will propably need separate methods for the content dimension values.
            return results;
        }

        private static List<DimensionValue> BuildDimensionValues(List<string> valueNames, DimensionType type, int decimals, bool sameUnit, bool sameSource, bool hasCombinationValue)
        {
            List<DimensionValue> results = [];
            /*
            foreach (var name in valueNames)
            {
                Dictionary<string, string> nameTranslations = new()
                {
                    { "fi", name },
                    { "en", $"{name}.en" }
                };
                VariableValue vv = new(name, new MultilanguageString(nameTranslations), null, hasCombinationValue && valueNames[0] == name);
                if (type == DimensionType.Content)
                {
                    Dictionary<string, string> unitTranslations = new()
                    {
                        { "fi", sameUnit ? "testUnit" : $"{name}-unit" },
                        { "en", sameUnit ? "testUnit" : $"{name}-unit.en" }
                    };
                    Dictionary<string, string> sourceTranslations = new()
                    {
                        { "fi", sameSource ? "testSource" : $"{name}-source" },
                        { "en", sameSource ? "testSource" : $"{name}-source.en" }
                    };
                    vv.ContentComponent = new ContentComponent(
                        new MultilanguageString(unitTranslations),
                        new MultilanguageString(sourceTranslations),
                        decimals, "2009-09-01T00:00:00.000Z");
                }
                results.Add(vv);
            }
            */
            // TODO: implement. Will propably need separate methods for the content dimension values.
            return results;
        }

        private static IEnumerable<DecimalDataValue> BuildTestData(int amount, bool negative = false, bool missing = false)
        {
            int missingIndex = 1;
            for (uint i = 0; i < amount; i++)
            {
                decimal res = i + 0.123m;
                if (negative && i % 2 == 0) res *= -1.0m;
                if (missing && i % 3 == 0)
                {
                    if (missingIndex > 7) missingIndex = 1;

                    DataValueType missingDataValueType = (DataValueType)missingIndex;
                    DecimalDataValue newMissingData = new(res, missingDataValueType);
                    missingIndex++;
                    yield return newMissingData;
                }
                else
                {
                    yield return new(res, DataValueType.Exists);
                }
            }
        }
    }
}
