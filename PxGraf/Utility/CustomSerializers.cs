using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Models.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Utility
{
    /// <summary>
    /// JsonConverter for serializing/deserializing multilanguage strings in and out of json.
    /// Regards empty translations as nulls.
    /// </summary>
    public class MultilanguageStringConverter : JsonConverter<MultilanguageString>
    {
        public override void WriteJson(JsonWriter writer, MultilanguageString value, JsonSerializer serializer)
        {
            Dictionary<string, string> translations = [];

            foreach (string language in value.Languages)
            {
                translations[language] = value[language].Trim(PxSyntaxConstants.STRING_DELIMETER);
            }

            if (translations.Count > 0)
            {
                serializer.Serialize(writer, translations);
            }
            else
            {
                serializer.Serialize(writer, null);
            }
        }

        public override MultilanguageString ReadJson(JsonReader reader, Type objectType, MultilanguageString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> result = serializer.Deserialize<Dictionary<string, string>>(reader);
            if (result != null && result.Count > 0)
            {
                return new MultilanguageString(result);
            }
            else
            {
                return null;
            }
        }
    }

    public class MatrixMetadataConverter : JsonConverter<IReadOnlyMatrixMetadata>
    {
        private readonly JsonConverter<Dimension> _dimensionConverter = new DimensionConverter();
        private readonly JsonConverter<MetaProperty> _metaPropertyConverter = new MetaPropertyConverter();

        public override IReadOnlyMatrixMetadata ReadJson(JsonReader reader, Type objectType, IReadOnlyMatrixMetadata existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jsonObject = JObject.Load(reader);
            try
            {
                return jsonObject.ToObject<MatrixMetadata>(serializer);
            }
            catch // If reading old archive files, metadata is provided as CubeMeta
            {
                CubeMeta cubeMeta = jsonObject.ToObject<CubeMeta>(serializer);
                return cubeMeta.ToMatrixMetadata();
            }
        }

        public override void WriteJson(JsonWriter writer, IReadOnlyMatrixMetadata value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("DefaultLanguage");
            writer.WriteValue(value.DefaultLanguage);

            writer.WritePropertyName("AvailableLanguages");
            serializer.Serialize(writer, value.AvailableLanguages);

            writer.WritePropertyName("Dimensions");
            writer.WriteStartArray();
            foreach (IReadOnlyDimension dimension in value.Dimensions)
            {
                _dimensionConverter.WriteJson(writer, dimension, serializer);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("AdditionalProperties");
            writer.WriteStartObject();
            foreach (var kvp in value.AdditionalProperties)
            {
                writer.WritePropertyName(kvp.Key);
                _metaPropertyConverter.WriteJson(writer, kvp.Value, serializer);
            }
            writer.WriteEndObject();

            writer.WriteEndObject();
        }
    }

    public class DimensionConverter : JsonConverter<Dimension>
    {
        private readonly JsonConverter<MultilanguageString> _multiLanguageStringConverter = new MultilanguageStringConverter();
        private readonly JsonConverter<DimensionValue> _valueConverter = new DimensionValueConverter();
        private readonly JsonConverter<MetaProperty> _metaPropertyConverter = new MetaPropertyConverter();

        public override Dimension ReadJson(JsonReader reader, Type objectType, Dimension existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jsonObject = JObject.Load(reader); 
            DimensionType dimensionType = jsonObject["DimensionType"].Value<DimensionType>();

            return dimensionType switch
            {
                DimensionType.Time => jsonObject.ToObject<TimeDimension>(serializer),
                DimensionType.Content => jsonObject.ToObject<ContentDimension>(serializer),
                _ => jsonObject.ToObject<Dimension>(serializer),
            };
        }

        public override void WriteJson(JsonWriter writer, Dimension value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Code");
            writer.WriteValue(value.Code);

            writer.WritePropertyName("Name");
            _multiLanguageStringConverter.WriteJson(writer, value.Name, serializer);

            writer.WritePropertyName("AdditionalProperties");
            writer.WriteStartObject();
            foreach (var kvp in value.AdditionalProperties)
            {
                writer.WritePropertyName(kvp.Key);
                _metaPropertyConverter.WriteJson(writer, kvp.Value, serializer);
            }
            writer.WriteEndObject();

            writer.WritePropertyName("Values");
            writer.WriteStartArray();
            foreach (IReadOnlyDimensionValue dimensionValue in value.Values)
            {
                _valueConverter.WriteJson(writer, dimensionValue, serializer);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("Type");
            writer.WriteValue(DimenionTypesEnumConverter.ToString(value.Type));

            if (value.AdditionalProperties.TryGetValue("NOTE", out MetaProperty noteProperty) &&
                noteProperty.TryGetRawValueMultilanguageString(out MultilanguageString noteValue))
            {        
                writer.WritePropertyName("Note");
                _multiLanguageStringConverter.WriteJson(writer, noteValue, serializer);
            }

            if (value is TimeDimension timeDimension)
            {
                writer.WritePropertyName("Interval");
                writer.WriteValue(timeDimension.Interval);
            }

            writer.WriteEndObject();
        }
    }

    public class DimensionValueConverter : JsonConverter<DimensionValue>
    {
        private readonly JsonConverter<MultilanguageString> _multiLanguageStringConverter = new MultilanguageStringConverter();
        private readonly JsonConverter<MetaProperty> _metaPropertyConverter = new MetaPropertyConverter();

        public override DimensionValue ReadJson(JsonReader reader, Type objectType, DimensionValue existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jsonObject = JObject.Load(reader);

            if (jsonObject["Unit"] != null && jsonObject["LastUpdated"] != null && jsonObject["Precision"] != null)
            {
                return jsonObject.ToObject<ContentDimensionValue>(serializer);
            }
            else
            {
                return jsonObject.ToObject<DimensionValue>(serializer);
            }
        }

        public override void WriteJson(JsonWriter writer, DimensionValue value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Code");
            writer.WriteValue(value.Code);

            writer.WritePropertyName("Name");
            _multiLanguageStringConverter.WriteJson(writer, value.Name, serializer);

            writer.WritePropertyName("AdditionalProperties");
            writer.WriteStartObject();
            foreach (var kvp in value.AdditionalProperties)
            {
                writer.WritePropertyName(kvp.Key);
                _metaPropertyConverter.WriteJson(writer, kvp.Value, serializer);
            }
            writer.WriteEndObject();

            writer.WritePropertyName("Virtual");
            writer.WriteValue(value.Virtual); 
            
            if (value is ContentDimensionValue contentDimensionValue)
            {
                writer.WritePropertyName("Unit");
                _multiLanguageStringConverter.WriteJson(writer, contentDimensionValue.Unit, serializer);
                writer.WritePropertyName("LastUpdated");
                writer.WriteValue(contentDimensionValue.LastUpdated);
                writer.WritePropertyName("Precision");
                writer.WriteValue(contentDimensionValue.Precision);
            }

            writer.WriteEndObject();
        }
    }

    public class MetaPropertyConverter : JsonConverter<MetaProperty>
    {
        private readonly JsonConverter<MultilanguageString> _multiLanguageStringConverter = new MultilanguageStringConverter();

        public override MetaProperty ReadJson(JsonReader reader, Type objectType, MetaProperty existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jsonObject = JObject.Load(reader);
            string keyWord = jsonObject["KeyWord"].Value<string>();

            if (jsonObject.TryGetValue("Entries", out JToken entriesToken))
            {
                if (entriesToken.Type == JTokenType.Object)
                {
                    MultilanguageString multiValueString = entriesToken.ToObject<MultilanguageString>();
                    return new(keyWord, multiValueString);
                }
                else if (entriesToken.Type == JTokenType.String)
                {
                    string rawString = entriesToken.Value<string>();
                    return new(keyWord, rawString);
                }
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, MetaProperty value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("KeyWord");
            writer.WriteValue(value.KeyWord);

            writer.WritePropertyName("Entries");
            if (value.TryGetRawValueMultilanguageString(out MultilanguageString multiValueString))
            {
                _multiLanguageStringConverter.WriteJson(writer, multiValueString, serializer);
            }
            else if (value.TryGetRawValueString(out string rawString))
            {
                writer.WriteValue(rawString);
            }

            writer.WritePropertyName("CanGetStringValue");
            writer.WriteValue(value.CanGetStringValue);
            writer.WritePropertyName("CanGetMultilanguageValue");
            writer.WriteValue(value.CanGetMultilanguageValue);

            writer.WriteEndObject();
        }
    }
}
