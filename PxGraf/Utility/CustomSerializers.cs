using Newtonsoft.Json;
using Px.Utils.Language;
using System;
using System.Collections.Generic;

namespace PxGraf.Utility
{
    // TODO: Start using this converter for MultilanguageStrings

    /// <summary>
    /// JsonConverter for serializing/deserializing multilanguage strings in and out of json.
    /// Regards empty translations as nulls.
    /// </summary>
    public class MultilanguageStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            MultilanguageString mls = value as MultilanguageString;
            Dictionary<string, string> translations = [];

            foreach (string language in mls.Languages)
            {
                translations[language] = mls[language];
            }

            if (translations.Count > 0)
            {
                serializer.Serialize(writer, translations);
            }
            else
            {
                serializer.Serialize(writer, null);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> result = serializer.Deserialize<Dictionary<string, string>>(reader);
            if (result != null && result.Count > 0)
            {
                return new MultilanguageString(result);
            }
            else
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(MultilanguageString).IsAssignableFrom(objectType);
        }
    }
}
