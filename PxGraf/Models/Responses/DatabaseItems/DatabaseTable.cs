using Px.Utils.Language;
using System;
using System.Collections.Generic;

namespace PxGraf.Models.Responses.DatabaseItems
{
    public class DatabaseTable(string code, MultilanguageString name, DateTime lastUpdated, List<string> languages)
    {
        public string Code { get; } = code;

        public MultilanguageString Name { get; } = name;

        public DateTime LastUpdated { get; } = lastUpdated;

        public List<string> Languages { get; } = languages;
    }
}
