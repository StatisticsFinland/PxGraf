#nullable enable
using Px.Utils.Language;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Utility
{
    public static class MultiLanguageStringUtilities
    {
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
