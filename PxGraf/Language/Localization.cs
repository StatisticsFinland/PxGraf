using PxGraf.Settings;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

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
            LocalizationConfig config = JsonSerializer.Deserialize<LocalizationConfig>(json, GlobalJsonConverterOptions.Default);
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

        public sealed class LocalizationConfig
        {
            public string DefaultLanguage { get; set; } = "en";
            public Dictionary<string, Translation> Translations { get; set; } = [];
        }
    }
}
