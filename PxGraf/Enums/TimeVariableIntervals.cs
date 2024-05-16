using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PxGraf.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
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
