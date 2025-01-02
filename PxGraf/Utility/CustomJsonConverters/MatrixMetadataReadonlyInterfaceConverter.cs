using Px.Utils.Models.Metadata;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PxGraf.Utility.CustomJsonConverters
{
    public class MatrixMetadataReadonlyInterfaceConverter : JsonConverter<IReadOnlyMatrixMetadata>
    {
        public override IReadOnlyMatrixMetadata Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => 
            JsonSerializer.Deserialize<MatrixMetadata>(ref reader, options);

        public override void Write(Utf8JsonWriter writer, IReadOnlyMatrixMetadata value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value, options);
    }
}
