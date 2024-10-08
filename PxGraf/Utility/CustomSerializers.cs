﻿using Newtonsoft.Json;
using PxGraf.Language;
using System;
using System.Collections.Generic;

namespace PxGraf.Utility
{
    /// <summary>
    /// JsonConverter for serializing/deserializing multilanguage strings in and out of json.
    ///
    /// Regards empty translations as nulls.
    /// </summary>
    public class MultiLanguageStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            MultiLanguageString mls = value as MultiLanguageString;
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
                MultiLanguageString mls = new();
                foreach (var kvp in result)
                {
                    mls.AddTranslation(kvp.Key, kvp.Value);
                }
                return mls;
            }
            else
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(MultiLanguageString).IsAssignableFrom(objectType);
        }
    }
}
