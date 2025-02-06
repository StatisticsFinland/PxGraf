using System.Text.Json.Serialization;

namespace PxGraf.Models.Responses.DatabaseItems
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DatabaseTableError
    {
        ContentLoad,
        ContentDimensionMissing,
        TimeDimensionMissing,
    }
}
