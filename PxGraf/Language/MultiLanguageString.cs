using Newtonsoft.Json;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Language
{
    /// <summary>
    /// A class for multilanguage reprentation of a string.
    /// </summary>
    [JsonConverter(typeof(MultiLanguageStringConverter))]
    public sealed class MultiLanguageString : IReadOnlyMultiLanguageString
    {
        /// <summary>
        /// Returns the languages of this object.
        /// </summary>
        public IEnumerable<string> Languages => _translations.Keys;

        /// <summary>
        /// Returns the string matching the given language.
        /// </summary>
        /// <param name="language">Requested language.</param>
        /// <returns>String that matches the requested language.</returns>
        public string this[string language] => _translations[language];

        private readonly Dictionary<string, string> _translations;

        /// <summary>
        /// Constructor that initializes the given languages.
        /// The parameters must be in a matching order.
        /// </summary>
        /// <param name="langs">List of languages to for the translations.</param>
        /// <param name="strings">List of strings to match the given languages.</param>
        public MultiLanguageString(IReadOnlyList<string> langs, IReadOnlyList<string> strings)
        {
            Debug.Assert(langs.Count == strings.Count);

            _translations = [];
            for (int langIndex = 0; langIndex < langs.Count; langIndex++)
            {
                _translations[langs[langIndex]] = strings[langIndex];
            }
        }

        /// <summary>
        /// Constructor that initializes the given languages.
        /// </summary>
        /// <param name="lang">Language for the translation.</param>
        /// <param name="value">String that matches the given language.</param>
        public MultiLanguageString(string lang, string value)
        {
            Debug.Assert(!string.IsNullOrEmpty(lang) && !string.IsNullOrEmpty(value));

            _translations = new()
            {
                [lang] = value
            };
        }

        /// <summary>
        /// Default constructor with no languages.
        /// </summary>
        public MultiLanguageString()
        {
            _translations = [];
        }

        /// <summary>
        /// Constructor that initializes the given languages and translations from a dictionary.
        /// </summary>
        /// <param name="translations">Dictionary with languages and respective translations.</param>
        public MultiLanguageString(Dictionary<string, string> translations)
        {
            _translations = new Dictionary<string, string>(translations);
        }

        /// <summary>
        /// Adds a new translation for the string.
        /// If the string already has a translation for the given language, the string is not changed and the method throws.
        /// </summary>
        /// <param name="language">Language for the new translation.</param>
        /// <param name="content">Content of the translation.</param>
        /// <returns>
        /// The edited <see cref="MultiLanguageString"/> object.
        /// If the string already has a translation for the given language, the string is not changed and the method throws an exception.
        /// </returns>
        public MultiLanguageString AddTranslation(string language, string content)
        {
            if (ContainsLanguage(language))
            {
                throw new ArgumentException($"Multilanguage string already contains translation for the language {language}.");

            }
            else
            {
                _translations[language] = content;
            }

            return this;
        }

        /// <summary>
        /// If the string contains translation for the given language the trnaslation is replaced with the new one.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="content"></param>
        public void EditTranslation(string language, string content)
        {
            if (ContainsLanguage(language))
            {
                _translations[language] = content;
            }
            else
            {
                throw new ArgumentException($"Multilanguage string does not contain translation for the language {language}.");
            }
        }

        /// <summary>
        /// For all languages, if the string contains translation for the given language the translation is replaced with the new one.
        /// </summary>
        /// <param name="translations">An object that contains the new translations.</param>
        public void Edit([AllowNull] IReadOnlyMultiLanguageString translations)
        {
            if (translations == null) return;

            foreach (string lang in translations.Languages)
            {
                EditTranslation(lang, translations[lang]);
            }
        }

        /// <summary>
        /// Returns true if the object contains a given language.
        /// </summary>
        /// <param name="language">Language name to be searched for.</param>
        /// <returns>
        /// Returns true if the object contains a given language. Otherwise false.
        /// </returns>
        public bool ContainsLanguage(string language)
        {
            return _translations.ContainsKey(language);
        }

        /// <summary>
        /// Tries to return a string with a given language.
        /// </summary>
        /// <param name="language">THe language for which a translation is looked for.</param>
        /// <param name="value">The found translation for the given language, if found.</param>
        /// <returns>Returns true if a translation for the given language is found. Otherwise false.</returns>
        public bool TryGetLanguage(string language, [MaybeNullWhen(false)] out string value)
        {
            return _translations.TryGetValue(language, out value);
        }

        /// <summary>
        /// Returns a complete COPY of this MultiLanguageString
        /// </summary>
        /// <returns>A cloned <see cref="MultiLanguageString"/> object.</returns>
        public MultiLanguageString Clone()
        {
            var newStr = new MultiLanguageString();
            foreach (var lang in Languages)
            {
                newStr.AddTranslation(lang, _translations[lang]);
            }

            return newStr;
        }

        /// <summary>
        /// Checks if the given object is equal to this object.
        /// </summary>
        /// <param name="other">The object to compare against.</param>
        /// <returns>True if the two objects match, otherwise false.</returns>
        public bool Equals([AllowNull] IReadOnlyMultiLanguageString other)
        {
            foreach (string lang in Languages)
            {
                if (other.ContainsLanguage(lang))
                {
                    if (_translations[lang] != other[lang]) return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
