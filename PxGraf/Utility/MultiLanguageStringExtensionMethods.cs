#nullable enable
using Px.Utils.Language;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Utility
{
    public static class MultiLanguageStringExtensionMethods
    {
        /// <summary>
        /// Copy and edit a multilanguage string.
        /// </summary>
        /// <param name="input"><see cref="MultilanguageString"/> to be copied and edited.</param>
        /// <param name="edit"><see cref="MultilanguageString"/> to be used for editing.</param>
        /// <returns><see cref="MultilanguageString"/> that is copied and edited.</returns>
        public static MultilanguageString CopyAndEdit(this MultilanguageString input, MultilanguageString? edit)
        {
            if(edit is null) return input;

            Dictionary<string, string> editedLanguages = [];
            foreach (string language in input.Languages)
            {
                editedLanguages[language] = edit.Languages.Contains(language) && !string.IsNullOrEmpty(edit[language])
                    ? edit[language]
                    : input[language];
            }
            return new(editedLanguages);
        }
    }
}
#nullable disable
