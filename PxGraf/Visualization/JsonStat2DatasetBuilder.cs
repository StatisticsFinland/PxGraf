#nullable enable
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Datasource.ApiDatasource.SerializationModels;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PxGraf.Visualization
{
    public static class JsonStat2DatasetBuilder
    {
        public static JsonStat2 Build(Matrix<DecimalDataValue> matrix, string? requestedLanguage, VisualizationSettings? visualizationSettings = null, MatrixQuery? query = null)
        {
            (decimal?[] values, Dictionary<string, string> statusMap) = BuildValues(matrix);
            return Build(
                matrix.Metadata,
                requestedLanguage,
                BuildExtension(matrix.Metadata, requestedLanguage, visualizationSettings, query),
                query,
                values,
                statusMap.Count > 0 ? statusMap : null);
        }

        public static JsonStat2 BuildMetadata(
            IReadOnlyMatrixMetadata metadata,
            string? requestedLanguage,
            VisualizationSettings? visualizationSettings = null,
            MatrixQuery? query = null)
        {
            return Build(metadata, requestedLanguage, BuildExtension(metadata, requestedLanguage, visualizationSettings, query), query, [], null);
        }

        private static JsonStat2 Build(
            IReadOnlyMatrixMetadata metadata,
            string? requestedLanguage,
            JsonStat2Extension extension,
            MatrixQuery? query,
            decimal?[] values,
            Dictionary<string, string>? statusMap)
        {
            string language = ResolveLanguage(metadata, requestedLanguage);

            List<IReadOnlyDimension> dimensions = [.. metadata.Dimensions];
            List<string> id = [.. dimensions.Select(d => d.Code)];
            List<int> size = [.. dimensions.Select(d => d.Values.Count)];
            Dictionary<string, JsonStat2.DimensionObj> dimensionMap = [];
            List<string> timeRoles = [];
            List<string> metricRoles = [];
            List<string> geoRoles = [];

            foreach (IReadOnlyDimension dimension in dimensions)
            {
                string dimCode = dimension.Code;
                DimensionQuery? dimensionQuery = query?.DimensionQueries.TryGetValue(dimCode, out DimensionQuery? resolvedDimensionQuery) == true
                    ? resolvedDimensionQuery
                    : null;
                JsonStat2.DimensionObj dimOutput = BuildDimension(dimension, language, dimensionQuery);
                dimensionMap[dimCode] = dimOutput;

                if (dimension.Type == DimensionType.Time)
                {
                    timeRoles.Add(dimCode);
                }
                else if (dimension.Type == DimensionType.Content)
                {
                    metricRoles.Add(dimCode);
                }
                else if (dimension.Type == DimensionType.Geographical)
                {
                    geoRoles.Add(dimCode);
                }
            }
            
            JsonStat2.RoleObj role = new()
            {
                Time = [.. timeRoles],
                Metric = [.. metricRoles],
                Geo = geoRoles.Count > 0 ? [.. geoRoles] : null
            };

            string label = TryGetOptionalLocalizedMetaProperty(metadata.AdditionalProperties, PxSyntaxConstants.DESCRIPTION_KEY, language) ?? string.Empty;
            string source = ResolveSource(metadata, language);
            List<string>? note = TryGetOptionalLocalizedMetaProperty(metadata.AdditionalProperties, PxSyntaxConstants.NOTE_KEY, language) is string datasetNote
                ? [datasetNote]
                : null;
            string updated = ResolveUpdatedTimestamp(dimensions);

            JsonStat2 result = new()
            {
                Version = "2.0",
                Class = "dataset",
                Id = [.. id],
                Size = [.. size],
                Label = label,
                Source = source,
                Updated = updated,
                Note = note,
                Dimensions = dimensionMap,
                Value = values,
                Status = statusMap,
                Role = role,
                Extension = extension
            };

            result.Extension.MissingValueDescriptions = BuildMissingValueDescriptions(language);

            return result;
        }

        private static JsonStat2Extension BuildExtension(IReadOnlyMatrixMetadata metadata, string? requestedLanguage, VisualizationSettings? settings, MatrixQuery? query)
        {
            JsonStat2Extension extension = new()
            {
                JsonStatChart = BuildJsonStatChartExtension(metadata, requestedLanguage, query)
            };

            if (settings is not null)
            {
                Dictionary<string, List<string>> selectableSelections = query?.DimensionQueries
                    .Where(pair => pair.Value.Selectable)
                    .ToDictionary(
                        pair => pair.Key,
                        pair => metadata.Dimensions.First(dimension => dimension.Code == pair.Key).Values.Select(value => value.Code).ToList());

                extension.SelectableConfig = new SelectableConfig
                {
                    SelectableSelections = selectableSelections,
                    DefaultSelectableSelections = settings.DefaultSelectableDimensionCodes,
                    MultiSelectableDimensionCode = settings.MultiselectableDimensionCode
                };
                extension.VisualizationConfig = new JsonStatVisualizationConfig
                {
                    ChartType = MapChartType(settings.VisualizationType),
                    Layout = settings.Layout is null
                        ? null
                        : new JsonStatLayout
                    {
                        Rows = settings.Layout.RowDimensionCodes,
                        Columns = settings.Layout.ColumnDimensionCodes
                    },
                    CutValueAxis = settings.CutYAxis,
                    Sorting = settings.Sorting
                };
                extension.VisualizationSettings = PxVisualizerCubeAdapter.BuildVisualizationSettings(metadata, settings);
            }

            return extension;
        }

        private static JsonStatChartExtension? BuildJsonStatChartExtension(IReadOnlyMatrixMetadata metadata, string? requestedLanguage, MatrixQuery? query)
        {
            if (query is null)
            {
                return null;
            }

            string language = ResolveLanguage(metadata, requestedLanguage);
            Dictionary<string, string> dimensionSources = [];
            Dictionary<string, Dictionary<string, string>> categorySources = [];

            foreach (IReadOnlyDimension dimension in metadata.Dimensions.Where(dimension => dimension.Type == DimensionType.Content))
            {
                if (!query.DimensionQueries.TryGetValue(dimension.Code, out DimensionQuery? dimensionQuery))
                {
                    continue;
                }

                Dictionary<string, string> sourcesForDimension = [];
                foreach (IReadOnlyDimensionValue value in dimension.Values)
                {
                    string? source = ResolveEditedSource(dimensionQuery, value.Code, language);
                    if (source is null && TryGetLocalizedMetaProperty(value.AdditionalProperties, PxSyntaxConstants.SOURCE_KEY, language, out string? metadataSource))
                    {
                        source = metadataSource;
                    }

                    if (!string.IsNullOrWhiteSpace(source))
                    {
                        sourcesForDimension[value.Code] = source;
                    }
                }

                if (sourcesForDimension.Count > 0)
                {
                    categorySources[dimension.Code] = sourcesForDimension;
                }

                string? dimensionSource = sourcesForDimension.Values.Distinct().FirstOrDefault();
                if (dimensionSource is not null && sourcesForDimension.Values.Distinct().Count() == 1)
                {
                    dimensionSources[dimension.Code] = dimensionSource;
                }
            }

            if (dimensionSources.Count == 0 && categorySources.Count == 0)
            {
                return null;
            }

            return new JsonStatChartExtension
            {
                Sources = new JsonStatSourceExtension
                {
                    Dimension = dimensionSources.Count > 0 ? dimensionSources : null,
                    Category = categorySources.Count > 0 ? categorySources : null
                }
            };
        }

        private static string? ResolveEditedSource(DimensionQuery query, string valueCode, string language)
        {
            return query.ValueEdits.TryGetValue(valueCode, out DimensionQuery.DimensionValueEdition? valueEdition) &&
                valueEdition.ContentComponent?.SourceEdit is MultilanguageString sourceEdit &&
                sourceEdit.Languages.Contains(language)
                ? sourceEdit[language]
                : null;
        }

        private static string MapChartType(Enums.VisualizationType visualizationType) => visualizationType switch
        {
            Enums.VisualizationType.VerticalBarChart => "verticalBar",
            Enums.VisualizationType.GroupVerticalBarChart => "groupedVerticalBar",
            Enums.VisualizationType.StackedVerticalBarChart => "stackedVerticalBar",
            Enums.VisualizationType.PercentVerticalBarChart => "percentVerticalBar",
            Enums.VisualizationType.HorizontalBarChart => "horizontalBar",
            Enums.VisualizationType.GroupHorizontalBarChart => "groupedHorizontalBar",
            Enums.VisualizationType.StackedHorizontalBarChart => "stackedHorizontalBar",
            Enums.VisualizationType.PercentHorizontalBarChart => "percentHorizontalBar",
            Enums.VisualizationType.PyramidChart => "pyramid",
            Enums.VisualizationType.PieChart => "pie",
            Enums.VisualizationType.LineChart => "line",
            Enums.VisualizationType.ScatterPlot => "scatterPlot",
            Enums.VisualizationType.Table => "table",
            _ => throw new ArgumentOutOfRangeException(nameof(visualizationType), visualizationType, null)
        };

        private static JsonStat2.DimensionObj BuildDimension(IReadOnlyDimension dimension, string language, DimensionQuery? query)
        {
            Dictionary<string, string> labels = [];
            Dictionary<string, List<string>> notes = [];
            Dictionary<string, JsonStat2.DimensionObj.CategoryObj.UnitObj> units = [];
            Dictionary<string, int> index = [];

            for (int valueIndex = 0; valueIndex < dimension.Values.Count; valueIndex++)
            {
                IReadOnlyDimensionValue value = dimension.Values[valueIndex];
                string code = value.Code;
                index[code] = valueIndex;
                DimensionQuery.DimensionValueEdition? valueEdition = query?.ValueEdits.TryGetValue(code, out DimensionQuery.DimensionValueEdition? resolvedValueEdition) == true
                    ? resolvedValueEdition
                    : null;
                labels[code] = ResolveEditedValue(valueEdition?.NameEdit, value.Name[language], language);

                if (TryGetLocalizedMetaProperty(value.AdditionalProperties, PxSyntaxConstants.VALUENOTE_KEY, language, out string? note))
                {
                    notes[code] = [note!];
                }

                if (dimension.Type == DimensionType.Content)
                {
                    ContentDimensionValue contentValue = (ContentDimensionValue)value;

                    units[code] = new JsonStat2.DimensionObj.CategoryObj.UnitObj
                    {
                        Label = ResolveEditedValue(valueEdition?.ContentComponent?.UnitEdit, contentValue.Unit[language], language),
                        Decimals = contentValue.Precision
                    };
                }
            }

            JsonStat2.DimensionObj.CategoryObj category = new()
            {
                Index = index,
                Label = labels,
                Note = notes,
                Unit = units
            };

            return new JsonStat2.DimensionObj
            {
                Label = ResolveEditedValue(query?.NameEdit, dimension.Name[language], language),
                Note = TryGetLocalizedMetaProperty(dimension.AdditionalProperties, PxSyntaxConstants.NOTE_KEY, language, out string? dimNote)
                    ? [dimNote!]
                    : null,
                Category = category
            };
        }

        private static (decimal?[] values, Dictionary<string, string> statusMap) BuildValues(Matrix<DecimalDataValue> matrix)
        {
            decimal?[] values = new decimal?[matrix.Data.Length];
            Dictionary<string, string> statusMap = [];

            for (int i = 0; i < matrix.Data.Length; i++)
            {
                DecimalDataValue cell = matrix.Data[i];
                if (cell.Type == DataValueType.Exists)
                {
                    values[i] = cell.UnsafeValue;
                    continue;
                }

                statusMap[i.ToString(CultureInfo.InvariantCulture)] = MapMissingType(cell.Type);
            }

            return (values, statusMap);
        }

        private static string MapMissingType(DataValueType type)
        {
            return type switch
            {
                DataValueType.Missing => "1",
                DataValueType.CanNotRepresent => "2",
                DataValueType.Confidential => "3",
                DataValueType.NotAcquired => "4",
                DataValueType.NotAsked => "5",
                DataValueType.Empty => "6",
                DataValueType.Nill => "7",
                _ => throw new InvalidOperationException($"Unknown missing data value type '{type}'.")
            };
        }

        private static Dictionary<string, string> BuildMissingValueDescriptions(string language)
        {
            string translationLanguage = Localization.GetAllAvailableLanguages().Contains(language)
                ? language
                : Configuration.Current.LanguageOptions.Default;
            MissingDataTranslation translations = Localization.FromLanguage(translationLanguage).Translation.MissingData;
            return new Dictionary<string, string>
            {
                ["1"] = translations.Missing,
                ["2"] = translations.CannotRepresent,
                ["3"] = translations.Confidential,
                ["4"] = translations.NotAcquired,
                ["5"] = translations.NotAsked,
                ["6"] = translations.Empty,
                ["7"] = translations.Nill
            };
        }

        private static string ResolveSource(IReadOnlyMatrixMetadata metadata, string language)
        {
            foreach (IReadOnlyDimension dimension in metadata.Dimensions.Where(d => d.Type == DimensionType.Content))
            {
                foreach (IReadOnlyDimensionValue value in dimension.Values)
                {
                    if (TryGetLocalizedMetaProperty(value.AdditionalProperties, PxSyntaxConstants.SOURCE_KEY, language, out string? source))
                    {
                        return string.IsNullOrEmpty(source) ? string.Empty : source;
                    }
                }
            }

            return TryGetOptionalLocalizedMetaProperty(metadata.AdditionalProperties, PxSyntaxConstants.SOURCE_KEY, language) ?? string.Empty;
        }

        private static string ResolveEditedValue(MultilanguageString? edit, string originalValue, string language)
        {
            return edit?.Languages.Contains(language) == true ? edit[language] : originalValue;
        }

        private static string ResolveUpdatedTimestamp(IReadOnlyList<IReadOnlyDimension> dimensions)
        {
            List<DateTime> timestamps = [];
            foreach (IReadOnlyDimension dimension in dimensions.Where(d => d.Type == DimensionType.Content))
            {
                foreach (IReadOnlyDimensionValue value in dimension.Values)
                {
                    if (value is not ContentDimensionValue contentValue)
                    {
                        throw new InvalidOperationException($"Content dimension '{dimension.Code}' contains a non-content value.");
                    }

                    timestamps.Add(contentValue.LastUpdated);
                }
            }

            if (timestamps.Count == 0)
            {
                throw new InvalidOperationException("No metric update timestamps found.");
            }

            return PxSyntaxConstants.FormatPxDateTime(timestamps.Max());
        }

        private static string ResolveLanguage(IReadOnlyMatrixMetadata metadata, string? requestedLanguage)
        {
            if (!string.IsNullOrWhiteSpace(requestedLanguage))
            {
                if (!metadata.AvailableLanguages.Contains(requestedLanguage))
                {
                    throw new ArgumentException($"Language '{requestedLanguage}' is not supported by the dataset.");
                }

                return requestedLanguage;
            }

            if (metadata.AvailableLanguages.Contains(metadata.DefaultLanguage))
            {
                return metadata.DefaultLanguage;
            }

            return metadata.AvailableLanguages[0] ?? throw new ArgumentException("Dataset has no available languages.");
        }

        private static string? TryGetOptionalLocalizedMetaProperty(IReadOnlyDictionary<string, MetaProperty> properties, string key, string language)
        {
            return TryGetLocalizedMetaProperty(properties, key, language, out string? value) ? value : null;
        }

        private static bool TryGetLocalizedMetaProperty(IReadOnlyDictionary<string, MetaProperty> properties, string key, string language, out string? value)
        {
            value = string.Empty;
            if (!properties.TryGetValue(key, out MetaProperty? property))
            {
                return false;
            }

            MultilanguageString? mlValue = property switch
            {
                MultilanguageStringProperty mlsp => mlsp.Value,
                StringProperty sp => new MultilanguageString(language, sp.Value),
                _ => null
            };

            value = mlValue?[language];
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
#nullable disable