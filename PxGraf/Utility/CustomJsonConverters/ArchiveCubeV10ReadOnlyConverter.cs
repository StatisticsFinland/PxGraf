using Px.Utils.Language;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using PxGraf.Data.MetaData;
using PxGraf.Models.SavedQueries.Versions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PxGraf.Utility.CustomJsonConverters
{
    public class ArchiveCubeV10ReadOnlyConverter : JsonConverter<ArchiveCubeV10>
    {
        public override ArchiveCubeV10 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
            JsonElement root = jsonDocument.RootElement;
            DateTime creationTime = root.GetPropertyCaseInsensitive(nameof(ArchiveCubeV10.CreationTime)).GetDateTime();
            CubeMeta meta = JsonSerializer.Deserialize<CubeMeta>(root.GetPropertyCaseInsensitive(nameof(ArchiveCubeV10.Meta)).GetRawText(), options);
            Dictionary<int, string> dataNotes = JsonSerializer.Deserialize<Dictionary<int, string>>(root.GetPropertyCaseInsensitive(nameof(ArchiveCubeV10.DataNotes)), options);
            List<DecimalDataValue> values = ConvertData(JsonSerializer.Deserialize<List<decimal?>>(root.GetPropertyCaseInsensitive(nameof(ArchiveCubeV10.Data)).GetRawText(), options), dataNotes);
            return new ArchiveCubeV10(creationTime, meta, values, ConvertDataNotes(dataNotes, meta.Languages));
        }

        public override void Write(Utf8JsonWriter writer, ArchiveCubeV10 value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("This converter is read-only.");
        }

        private static List<DecimalDataValue> ConvertData(List<decimal?> data, Dictionary<int, string> notes)
        {
            List<DecimalDataValue> result = [];
            for (int i = 0; i < data.Count; i++)
            {
                decimal? value = data[i];
                if (value.HasValue)
                {
                    result.Add(new DecimalDataValue(value.Value, DataValueType.Exists));
                }
                else
                {
                    DataValueType missingValueType = DataValueType.Nill;
                    if (notes[i][0] != '-')
                    {
                        missingValueType = (DataValueType)(notes[i].Length);
                    }
                    result.Add(new DecimalDataValue(0, missingValueType));
                }
            }
            return result;
        }

        private static Dictionary<int, MultilanguageString> ConvertDataNotes(Dictionary<int, string> dataNotes, IReadOnlyList<string> languages)
        {
            Dictionary<int, MultilanguageString> result = [];
            foreach (KeyValuePair<int, string> note in dataNotes)
            {
                if (note.Value == string.Empty)
                {
                    continue;
                }
                Dictionary<string, string> translations = [];
                foreach (string language in languages)
                {
                    translations.Add(language, note.Value);
                }
                result.Add(note.Key, new MultilanguageString(translations));
            }
            return result;
        }
    }
}
