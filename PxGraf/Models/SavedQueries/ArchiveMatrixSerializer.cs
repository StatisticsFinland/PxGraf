using Newtonsoft.Json;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using System;

namespace PxGraf.Models.SavedQueries
{
    public class ArchiveMatrixSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Matrix<DecimalDataValue>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
