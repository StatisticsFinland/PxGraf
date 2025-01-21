#nullable enable
using Px.Utils.Language;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace PxGraf.Models.Responses.DatabaseItems
{
    /// <summary>
    /// Represents a px database table.
    /// </summary>
    public class DatabaseTable
    {
        /// <summary>
        /// Code of the database table.
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// Multilanguage name of the database table.
        /// </summary>
        public MultilanguageString Name { get; private set; }
        /// <summary>
        /// Last updated date of the database table.
        /// </summary>
        public DateTime? LastUpdated { get; private set; }
        /// <summary>
        /// Available languages of the database table.
        /// </summary>
        public List<string> Languages { get; private set; }
        /// <summary>
        /// Error flag.
        /// </summary>

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Error { get; private set; } = null;

        public DatabaseTable(string fileName, MultilanguageString name, DateTime lastUpdated, List<string> languages)
        {
            FileName = fileName;
            Name = name;
            LastUpdated = lastUpdated;
            Languages = languages;
        }

        private DatabaseTable(string fileName, MultilanguageString name, List<string> languages)
        {
            FileName = fileName;
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
