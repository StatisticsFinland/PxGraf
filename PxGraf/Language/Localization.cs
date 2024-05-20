using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PxGraf.Language
{
    public class Localization(Translation translation, CultureInfo cultureInfo)
    {
        private static Dictionary<string, Localization> localizations = null;
        private static Localization defaultLocalization = null;

        public CultureInfo Culture { get; } = cultureInfo;
        public Translation Translation { get; } = translation;

        public static Localization FromLanguage(string lang)
        {
            if (localizations.TryGetValue(lang, out var result))
            {
                return result;
            }
            else
            {
                return defaultLocalization;
            }
        }

        /// <summary>
        /// Gets all available languages
        /// </summary>
        /// <returns>Collection of language names</returns>
        public static IEnumerable<string> GetAllAvailableLanguages()
        {
            return localizations.Keys;
        }

        public static void Load(string configPath)
        {
            var json = File.ReadAllText(configPath);

            //Strict json deserialization
            LocalizationConfig config = JsonConvert.DeserializeObject<LocalizationConfig>(
                json,
                new JsonSerializerSettings()
                {
                    //Ensures json does include every property to avoid missing translations
                    ContractResolver = new RequireObjectPropertiesContractResolver(),
                    //Ensures that json does not have any unused extra fields to avoid carrying non-used translations around
                    MissingMemberHandling = MissingMemberHandling.Error,
                }
            );
            Load(config.DefaultLanguage, config.Translations);
        }

        public static void Load(string defaultLang, IReadOnlyDictionary<string, Translation> translations)
        {
            localizations = translations.ToDictionary(
                kvp => kvp.Key,
                kvp => new Localization(kvp.Value, new CultureInfo(kvp.Key))
            );
            defaultLocalization = localizations[defaultLang];
        }

        private sealed class LocalizationConfig
        {
            public string DefaultLanguage { get; } = "en";
            public Dictionary<string, Translation> Translations { get; } = [];
        }

        /// <summary>
        /// This contract resolver requires every object field by default.
        /// Individual properties can be still marked as optional with: [JsonProperty(Required = Required.Default)]
        /// </summary>
        private sealed class RequireObjectPropertiesContractResolver : DefaultContractResolver
        {
            protected override JsonObjectContract CreateObjectContract(System.Type objectType)
            {
                var contract = base.CreateObjectContract(objectType);
                contract.ItemRequired = Required.Always;
                return contract;
            }
        }
    }
}
