#nullable enable
using Px.Utils.Language;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace PxGraf.Models.Responses.DatabaseItems
{
    public class DatabaseTable
    {
        public string Code { get; private set; }

        public MultilanguageString Name { get; private set; }

        public DateTime? LastUpdated { get; private set; }

        public List<string> Languages { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Error { get; private set; } = null;

        public DatabaseTable(string code, MultilanguageString name, DateTime lastUpdated, List<string> languages)
        {
            Code = code;
            Name = name;
            LastUpdated = lastUpdated;
            Languages = languages;
        }

        private DatabaseTable(string code, MultilanguageString name, List<string> languages)
        {
            Code = code;
            Name = name;
            Languages = languages;
            Error = true;
        }

        public static DatabaseTable FromError(string code, MultilanguageString name, List<string> languages)
        {
            return new DatabaseTable(code, name, languages);
        }
    }
}
#nullable disable
