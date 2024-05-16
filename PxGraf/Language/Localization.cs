using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PxGraf.Language
{
    public class Localization
    {
        private static Dictionary<string, Localization> localizations = null;
        private static Localization defaultLocalization = null;



        public CultureInfo Culture { get; }
        public Translation Translation { get; }

        public Localization(Translation translation, CultureInfo cultureInfo)
        {
            Translation = translation;
            Culture = cultureInfo;
        }



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
            var config = JsonConvert.DeserializeObject<LocalizationConfig>(
                json,
                new JsonSerializerSettings()
                {
                    //Ensures json does include every property to avoid missing translations
                    ContractResolver = new RequireObjectPropertiesContractResolver(),
                    //Ensures that json does not have any unused extra fields to avoid carrying non-used translations around
                    MissingMemberHandling = MissingMemberHandling.Error,
                }
            );

            localizations = config.Translations.ToDictionary(
                p => p.Key,
                p =>
                {
                    CultureInfo cultureInfo = new(p.Key);
                    cultureInfo.NumberFormat.NegativeSign = "-"; // Override other minus signs
                    return new Localization(p.Value, cultureInfo);
                }
            );

            defaultLocalization = localizations[config.DefaultLanguage];
        }

        private sealed class LocalizationConfig
        {
            public string DefaultLanguage { get; } = "en";
            public Dictionary<string, Translation> Translations { get; } = new Dictionary<string, Translation>();
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
