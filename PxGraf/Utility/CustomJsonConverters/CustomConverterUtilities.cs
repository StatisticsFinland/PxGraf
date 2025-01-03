#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace PxGraf.Utility.CustomJsonConverters
{
    public static class CustomConverterUtilities
    {
        public static JsonElement GetPropertyCaseInsensitive(this JsonElement element, string propertyName)
        {
            return element.EnumerateObject().FirstOrDefault(prop => string.Equals(prop.Name, propertyName, System.StringComparison.OrdinalIgnoreCase)).Value;
        }

        public static bool TryGetPropertyCaseInsensitive(this JsonElement element, string propertyName, out JsonElement value)
        {
            bool propExists = element.EnumerateObject().Any(prop => string.Equals(prop.Name, propertyName, System.StringComparison.OrdinalIgnoreCase));
            if (!propExists)
            {
                value = default;
                return false;
            }
            value = element.GetPropertyCaseInsensitive(propertyName);
            return true;
        }
    }
}
#nullable disable
