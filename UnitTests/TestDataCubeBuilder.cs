#nullable enable
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.ChartTypeSelection;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.SavedQueries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    public static class TestDataCubeBuilder
    {
        public static VisualizationTypeSelectionObject BuildTestVisualizationTypeSelectionObject(List<DimensionParameters> variables, bool negativeData = false)
        {
            return VisualizationTypeSelectionObject.FromQueryAndMatrix(BuildTestCubeQuery(variables), BuildTestMatrix(variables, negativeData));
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
                variableQueries[varParams[i].Name ?? $"variable-{i}"] = BuildTestVariableQuery(varParams[i]);
            }

            query.DimensionQueries = variableQueries;
            return query;
        }

        public static MatrixQuery ApplyNameEdits(this MatrixQuery query, MatrixMetadata meta, Dictionary<string, MultilanguageString>? dimensionNameEdits = null, Dictionary<string, Dictionary<string, MultilanguageString>>? valueNameEdits = null)
        {
            if (dimensionNameEdits != null)
            {
                foreach (var dimNameEdit in dimensionNameEdits)
                {
                    MultilanguageString originalName = meta.Dimensions.First(d => d.Code == dimNameEdit.Key).Name;
                    MultilanguageString nameEdit = originalName.CopyAndEdit(dimNameEdit.Value);
                    query.DimensionQueries[dimNameEdit.Key].NameEdit = nameEdit;
                }
            }
            if (valueNameEdits != null)
            {
                foreach (var dimension in valueNameEdits)
                {
                    foreach (var value in dimension.Value)
                    {
                        MultilanguageString originalName = meta.Dimensions.First(d => d.Code == dimension.Key).Values.First(v => v.Code == value.Key).Name;
                        MultilanguageString nameEdit = originalName.CopyAndEdit(value.Value);
                        query.DimensionQueries[dimension.Key].ValueEdits[value.Key] = new()
                        {
                            NameEdit = nameEdit
                        };
                    }
                }
            }

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

        public static Matrix<DecimalDataValue> BuildTestMatrix(List<DimensionParameters> dimParams, bool negativeData = false, bool missingData = false, string[]? languages = null)
        {
            IReadOnlyMatrixMetadata meta = BuildTestMeta(dimParams, languages);
            int dataSize = meta.Dimensions.Count == 0 ? 0 : meta.Dimensions.Aggregate(1, (acc, var) => acc * var.Values.Count);
            return new Matrix<DecimalDataValue>(meta, BuildTestData(dataSize, negativeData, missingData).ToArray());
        }

        public static MatrixMetadata BuildTestMeta(List<DimensionParameters> varParams, string[]? languages = null)
        {
            if (varParams.Exists(v => v.Size < 1)) return null;

            languages ??= ["fi", "en"];
            List<Dimension> variables = BuildTestDimensions(varParams, languages);
            Dictionary<string, string> noteTranslation = [];
            for (int j = 0; j < languages.Length; j++)
            {
                noteTranslation[languages[j]] = $"{GetTextForLanguage("Test note", languages, j)}";
            }
            Dictionary<string, MetaProperty> additionalProperties = new()
            {
                { PxSyntaxConstants.NOTE_KEY, new MultilanguageStringProperty(new MultilanguageString(noteTranslation)) }
            };
            return new MatrixMetadata(languages[0], languages, variables, additionalProperties);
        }

        private static List<Dimension> BuildTestDimensions(List<DimensionParameters> parameters, string[] languages)
        {
            var results = new List<Dimension>();
            int i = 0;
            foreach (var param in parameters)
            {
                results.Add(BuildTestDimension(param.Name ?? $"variable-{i++}", param, languages));
            }

            return results;
        }

        public static Dimension BuildTestDimension(string name, DimensionParameters param, string[] languages)
        {
            if (param.Size < 1) return null;
            Dictionary<string, string> nameTranslations = [];
            for (int i = 0; i < languages.Length; i++)
            {
                nameTranslations[languages[i]] = GetTextForLanguage(name, languages, i);
            }
            MultilanguageString multilanguageName = new(nameTranslations);
            List<DimensionValue> values = BuildVariableValues(param, languages);
            Dictionary<string, MetaProperty> additionalProperties = [];
            if (param.HasCombinationValue)
            {
                additionalProperties.Add(name, new StringProperty($"\"{values[0].Code}\""));
            }

            if (param.Type == DimensionType.Time)
            {
                TimeDimension timeDimension = new(
                    name,
                    multilanguageName,
                    additionalProperties,
                    values,
                    TimeDimensionInterval.Year
                    );
                return timeDimension;
            }
            else if (param.Type == DimensionType.Content)
            {
                List<ContentDimensionValue> contentDimensionValues = [];
                for (int i = 0; i < values.Count; i++)
                {
                    contentDimensionValues.Add((ContentDimensionValue)values[i]);
                }
                ContentDimension contentDimension = new(
                    name,
                    multilanguageName,
                    additionalProperties,
                    contentDimensionValues
                    );
                return contentDimension;
            }
            Dimension dimension = new(
                name,
                multilanguageName,
                additionalProperties,
                values,
                param.Type
                );

            return dimension;
        }

        public static ArchiveCube BuildTestArchiveCube(List<DimensionParameters> metaParams, string[]? languages = null)
        {
            MatrixMetadata meta = BuildTestMeta(metaParams, languages);
            return new ArchiveCube()
            {
                CreationTime = PxSyntaxConstants.ParsePxDateTime("2024-08-19T14:00:00.000Z"),
                Meta = meta
            };
        }

        private static string GetTextForLanguage(string baseText, string[] languages, int index)
        {
            return index == 0 ? baseText : $"{baseText}.{languages[index]}";
        }

        private static List<DimensionValue> BuildVariableValues(DimensionParameters varParam, string[] languages)
        {
            List<DimensionValue> results = [];
            
            for (int i = 0; i < varParam.Size; i++)
            {
                string name = varParam.Type == DimensionType.Time && !varParam.Irregular ? $"{2000 + i}" : $"value-{i}";
                Dictionary<string, string> nameTranslations = [];
                for (int j = 0; j < languages.Length; j++)
                {
                    nameTranslations[languages[j]] = GetTextForLanguage(name, languages, j);
                }
                if (varParam.Type == DimensionType.Content)
                {
                    Dictionary<string, string> unitTranslations = [];
                    for (int k = 0; k < languages.Length; k++)
                    {
                        string langUnit = varParam.SameUnit ? "testUnit" : GetTextForLanguage($"{name}-unit", languages, k);
                        unitTranslations[languages[k]] = langUnit;
                    }
                    Dictionary<string, string> sourceTranslations = [];
                    for (int k = 0; k < languages.Length; k++)
                    {
                        string langSource = varParam.SameSource ? "testSource" : GetTextForLanguage($"{name}-source", languages, k) + ""; // Metaproprety values are stored enclosed
                        sourceTranslations[languages[k]] = langSource;
                    }

                    MultilanguageStringProperty source = new(new MultilanguageString(sourceTranslations));
                    ContentDimensionValue cvv = new(name, new MultilanguageString(nameTranslations), new(unitTranslations), PxSyntaxConstants.ParsePxDateTime("2009-09-01T00:00:00.000Z"), varParam.Decimals);
                    cvv.AdditionalProperties[PxSyntaxConstants.SOURCE_KEY] = source;
                    results.Add(cvv);
                }
                else
                {
                    DimensionValue vv = new(name, new MultilanguageString(nameTranslations));
                    results.Add(vv);
                }
            }
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
