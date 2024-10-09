using Px.Utils.Language;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
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
            throw new NotImplementedException();
             /*
            JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
            DateTime creationTime = jsonDocument.RootElement.GetProperty(nameof(ArchiveCubeV10.CreationTime)).GetDateTime();
            CubeMeta meta = JsonSerializer.Deserialize<CubeMeta>(jsonDocument.RootElement.GetProperty(nameof(ArchiveCubeV10.Meta)).GetRawText());
            Dictionary<int, string> dataNotes = JsonSerializer.Deserialize<Dictionary<int, string>>(jsonDocument.RootElement.GetProperty(nameof(ArchiveCubeV10.DataNotes)));
            List<DecimalDataValue> values = ConvertData(JsonSerializer.Deserialize<List<decimal?>>(jsonDocument.RootElement.GetProperty(nameof(ArchiveCubeV10.Data)).GetRawText()), dataNotes);
            string[] languages = 
            return new ArchiveCubeV10(creationTime, meta, values, ConvertDataNotes(dataNotes));*/
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
                    if (notes[i][1] != '-')
                    {
                        missingValueType = (DataValueType)(notes[i].Length - 2);
                    }
                    result.Add(new DecimalDataValue(0, missingValueType));
                }
            }
            return result;
        }

        private static Dictionary<int, MultilanguageString> ConvertDataNotes(Dictionary<int, string> dataNotes)
        {
            throw new NotImplementedException();

            /*
            Dictionary<int, MultilanguageString> result = [];
            foreach (KeyValuePair<int, string> note in dataNotes)
            {
                result.Add(note.Key, new MultilanguageString(note.Value));
            }
            return result;*/
        }
    }
}
