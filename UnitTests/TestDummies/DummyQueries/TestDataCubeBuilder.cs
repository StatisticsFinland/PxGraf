using PxGraf.ChartTypeSelection;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.SavedQueries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.TestDummies.DummyQueries
{
    public static class TestDataCubeBuilder
    {
        public static VisualizationTypeSelectionObject BuildTestVisualizationTypeSelectionObject(List<VariableParameters> variables, bool negativeData = false)
        {
            return VisualizationTypeSelectionObject.FromQueryAndCube(BuildTestCubeQuery(variables), BuildTestDataCube(variables, negativeData));
        }

        public static CubeQuery BuildTestCubeQuery(List<VariableParameters> varParams)
        {
            CubeQuery query = new()
            {
                TableReference = new PxFileReference()
                {
                    Name = "TestPxFile.px",
                    Hierarchy = ["testpath", "to", "test", "file"]
                },
                ChartHeaderEdit = null
            };

            Dictionary<string, VariableQuery> variableQueries = [];
            for (int i = 0; i < varParams.Count; i++)
            {
                variableQueries[$"variable-{i}"] = BuildTestVariableQuery(varParams[i]);
            }

            query.VariableQueries = variableQueries;
            return query;
        }

        public static SavedQuery BuildTestSavedQuery(List<VariableParameters> varParams, bool archived, VisualizationSettings settings)
        {
            return new SavedQuery(BuildTestCubeQuery(varParams), archived, settings, DateTime.Now);
        }

        public static SavedQuery BuildTestSavedQuery(List<VariableParameters> varParams, bool archived, VisualizationCreationSettings creationSettings, DataCube cube)
        {
            CubeQuery query = BuildTestCubeQuery(varParams);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(cube.Meta, query);

            return new SavedQuery(BuildTestCubeQuery(varParams), archived, settings, DateTime.Now);
        }

        public static ArchiveCube BuildTestArchiveCube(List<VariableParameters> varParams, bool negativeData = false, bool missingData = false)
        {
            return ArchiveCube.FromDataCube(BuildTestDataCube(varParams, negativeData, missingData));
        }

        public static VariableQuery BuildTestVariableQuery(VariableParameters varParams)
        {
            ValueFilter valueFilter;
            if (varParams.ValueFilter != null)
            {
                valueFilter = varParams.ValueFilter;
            }
            else
            {
                List<string> values = [];
                if (varParams.Type == VariableType.Time && !varParams.Irregular)
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

            return new VariableQuery()
            {
                ValueFilter = valueFilter,
                Selectable = varParams.Selectable
            };
        }

        public static DataCube BuildTestDataCube(List<VariableParameters> variables, bool negativeData = false, bool missingData = false)
        {
            CubeMeta meta = BuildTestMeta(variables);
            int dataSize = meta.Variables.Count == 0 ? 0 : meta.Variables.Aggregate(1, (acc, var) => acc * var.IncludedValues.Count);
            return new DataCube(meta, BuildTestData(dataSize, negativeData, missingData).ToArray());
        }

        public static CubeMeta BuildTestMeta(List<VariableParameters> varParams)
        {
            if (varParams.Exists(v => v.Size < 1)) return null;

            List<string> languages = ["fi", "en"];
            List<Variable> variables = BuildTestVariables(varParams);
            Dictionary<string, string> headerTranslations = new()
            {
                { "fi", "Test Header" },
                { "en", "Test Header.en" }
            };
            MultiLanguageString header = new(headerTranslations);
            Dictionary<string, string> noteTranslations = new()
            {
                { "fi", "Test note" },
                { "en", "Test note.en" }
            };
            MultiLanguageString note = new(noteTranslations);
            return new CubeMeta(languages, header, note, variables);
        }

        private static List<Variable> BuildTestVariables(List<VariableParameters> parameters)
        {
            var results = new List<Variable>();
            int i = 0;
            foreach (var param in parameters)
            {
                results.Add(BuildTestVariable(param.Name ?? $"variable-{i++}", param));
            }

            return results;
        }

        public static Variable BuildTestVariable(string name, VariableParameters param)
        {
            if (param.Size < 1) return null;
            Dictionary<string, string> nameTranslations = new()
            {
                { "fi", name },
                { "en", $"{name}.en" }
            };
            Variable variable = new
            (
                name,
                new MultiLanguageString(nameTranslations),
                null,
                param.Type,
                BuildVariableValues(param)
            );

            return variable;
        }

        public static Variable BuildTestVariable(string name, List<string> valueNames, VariableType type, int decimals = 0, bool sameUnit = false, bool sameSource = false, bool hasCombinationValue = false)
        {
            Dictionary<string, string> nameTranslations = new()
            {
                { "fi", name },
                { "en", $"{name}.en" }
            };
            Variable variable = new
            (
                name,
                new MultiLanguageString(nameTranslations),
                null,
                type,
                BuildVariableValues(valueNames, type, decimals, sameUnit, sameSource, hasCombinationValue)
            );

            return variable;
        }

        private static List<VariableValue> BuildVariableValues(VariableParameters varParam)
        {
            List<VariableValue> results = [];
            for (int i = 0; i < varParam.Size; i++)
            {
                string name = (varParam.Type == VariableType.Time && !varParam.Irregular) ? $"{2000 + i}" : $"value-{i}";
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
            return results;
        }

        private static List<VariableValue> BuildVariableValues(List<string> valueNames, VariableType type, int decimals, bool sameUnit, bool sameSource, bool hasCombinationValue)
        {
            List<VariableValue> results = [];
            foreach (var name in valueNames)
            {
                Dictionary<string, string> nameTranslations = new()
                {
                    { "fi", name },
                    { "en", $"{name}.en" }
                };
                VariableValue vv = new(name, new MultiLanguageString(nameTranslations), null, hasCombinationValue && valueNames[0] == name);
                if (type == VariableType.Content)
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
                        new MultiLanguageString(unitTranslations),
                        new MultiLanguageString(sourceTranslations),
                        decimals, "2009-09-01T00:00:00.000Z");
                }
                results.Add(vv);
            }
            return results;
        }

        private static IEnumerable<DataValue> BuildTestData(int amount, bool negative = false, bool missing = false)
        {
            var missingIndex = 1; 
            for (uint i = 0; i < amount; i++)
            {
                var res = i + 0.123;
                if (negative && i % 2 == 0) res *= -1.0;
                if (missing && i % 3 == 0)
                {
                    if (missingIndex > 7) missingIndex = 1;
                    
                    DataValueType missingDataValueType = (DataValueType)missingIndex;
                    DataValue newMissingData = new(res, missingDataValueType);
                    missingIndex++;
                    yield return newMissingData;
                }
                else
                {
                    yield return DataValue.FromRaw(res);
                }
            }
        }
    }
}
