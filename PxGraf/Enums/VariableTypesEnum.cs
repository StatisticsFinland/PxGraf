using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PxGraf.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VariableType
    {
        Unknown, //Implicit default
        Time,
        Ordinal,
        Geological,
        Content,
        OtherClassificatory,
    }
}
