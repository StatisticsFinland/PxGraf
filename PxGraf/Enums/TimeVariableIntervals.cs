using System.Text.Json.Serialization;

namespace PxGraf.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TimeVariableInterval
    {
        Irregular = 0,
        Week = 1,
        Month = 2,
        Quarter = 3,
        HalfYear = 4,
        Year = 5,
    }
}
