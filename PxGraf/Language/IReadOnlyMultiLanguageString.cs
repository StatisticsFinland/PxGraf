using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Language
{
    /// <summary>
    /// Interface for using multilanguage string that prevents mutating.
    /// </summary>
    public interface IReadOnlyMultiLanguageString : IEquatable<IReadOnlyMultiLanguageString>
    {
        /// <summary>
        /// Returns the languages of this object.
        /// </summary>
        public IEnumerable<string> Languages { get; }

        /// <summary>
        /// Returns the string matching the given language.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        string this[string language] { get; }

        /// <summary>
        /// Returns true if the object contains a given language.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool ContainsLanguage(string language);

        /// <summary>
        /// Tries to returns a string with a given language.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetLanguage(string language, [MaybeNullWhen(false)] out string value);

        /// <summary>
        /// Returns a complete COPY of this MultiLanguageString
        /// </summary>
        /// <returns></returns>
        public MultiLanguageString Clone();
    }
}
