#nullable enable
using System;
using System.Linq;
using System.Text.Json;

namespace PxGraf.Utility.CustomJsonConverters
{
    /// <summary>
    /// Contains utility functions for custom json converters.
    /// </summary>
    public static class CustomConverterUtilities
    {
        /// <summary>
        /// Gets the value of a property from a json element. Allows for case-insensitive property name matching if the options are set to do so.
        /// </summary>
        /// <param name="root">Root json element to search for the property.</param>
        /// <param name="propertyName">Name of the property to search for.</param>
        /// <param name="options">JsonSerializerOptions to use for case-insensitive property name matching.</param>
        /// <returns>The value of the property if it exists.</returns>
        public static JsonElement GetProperty(this JsonElement root, string propertyName, JsonSerializerOptions options)
        {
            if (options.PropertyNameCaseInsensitive)
            {
                return root.EnumerateObject()
                    .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    .Value;
            }
            else
            {
                return root.GetProperty(propertyName);
            }
        }

        /// <summary>
        /// Tries to get the value of a property from a json element. Allows for case-insensitive property name matching if the options are set to do so.
        /// </summary>
        /// <param name="root">Json element to search for the property.</param>
        /// <param name="propertyName">Name of the property to search for.</param>
        /// <param name="options">JsonSerializerOptions to use for case-insensitive property name matching.</param>
        /// <param name="value">The value of the property if it exists.</param>
        /// <returns>True if the property exists, false otherwise.</returns>
        public static bool TryGetProperty(this JsonElement root, string propertyName, JsonSerializerOptions options, out JsonElement value)
        {
            if (options.PropertyNameCaseInsensitive)
            {
                foreach (JsonProperty prop in root.EnumerateObject())
                {
                    if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        value = prop.Value;
                        return true;
                    }
                }
                value = default;
                return false;
            }
            else
            {
                return root.TryGetProperty(propertyName, out value);
            }
        }
    }
}
#nullable disable
