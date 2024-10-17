using Px.Utils.Language;
using System.Collections.Generic;

namespace PxGraf.Models.Responses.DatabaseItems
{
    public class DatabaseGroupHeader(string code, List<string> languages, MultilanguageString name)
    {
        public string Code { get; } = code;
        public MultilanguageString Name { get; } = name;
        public List<string> Languages { get; } = languages;
    }
}
