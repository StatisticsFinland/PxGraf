using System.Text.Json;

namespace PxGraf.Settings
{
    public static class GlobalJsonConverterOptions
    {
        public static JsonSerializerOptions Default { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}
