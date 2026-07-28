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
        public static JsonStat2 Build(Matrix<DecimalDataValue> matrix, string? requestedLanguage, VisualizationResponse.PxVisualizerSettings? visualizationSettings = null)
        {
            IReadOnlyMatrixMetadata metadata = matrix.Metadata;
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
                JsonStat2.DimensionObj dimOutput = BuildDimension(dimension, language);
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
            
            (List<decimal?> values, Dictionary<string, string> statusMap) = BuildValues(matrix);
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
                Value = [.. values],
                Status = statusMap.Count > 0 ? statusMap : null,
                Role = role,
                Extension = new JsonStat2Extension()
                {
                    MissingValueDescriptions = BuildMissingValueDescriptions(language),
                    VisualizationSettings = visualizationSettings
                }
            };

            return result;
        }

        private static JsonStat2.DimensionObj BuildDimension(IReadOnlyDimension dimension, string language)
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
                labels[code] = value.Name[language];

                if (TryGetLocalizedMetaProperty(value.AdditionalProperties, PxSyntaxConstants.VALUENOTE_KEY, language, out string? note))
                {
                    notes[code] = [note!];
                }

                if (dimension.Type == DimensionType.Content)
                {
                    ContentDimensionValue contentValue = (ContentDimensionValue)value;

                    units[code] = new JsonStat2.DimensionObj.CategoryObj.UnitObj
                    {
                        Label = contentValue.Unit[language],
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
                Label = dimension.Name[language],
                Note = TryGetLocalizedMetaProperty(dimension.AdditionalProperties, PxSyntaxConstants.NOTE_KEY, language, out string? dimNote)
                    ? [dimNote!]
                    : null,
                Category = category
            };
        }

        private static (List<decimal?> values, Dictionary<string, string> statusMap) BuildValues(Matrix<DecimalDataValue> matrix)
        {
            List<decimal?> values = new(matrix.Data.Length);
            Dictionary<string, string> statusMap = [];

            for (int i = 0; i < matrix.Data.Length; i++)
            {
                DecimalDataValue cell = matrix.Data[i];
                if (cell.Type == DataValueType.Exists)
                {
                    values.Add(cell.UnsafeValue);
                    continue;
                }

                values.Add(null);
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