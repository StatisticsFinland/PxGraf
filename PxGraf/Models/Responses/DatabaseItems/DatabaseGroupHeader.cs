using Px.Utils.Language;
using System.Collections.Generic;

namespace PxGraf.Models.Responses.DatabaseItems
{
    /// <summary>
    /// Represents a database group.
    /// </summary>
    public class DatabaseGroupHeader(string code, List<string> languages, MultilanguageString name)
    {
        /// <summary>
        /// Code of the database group
        /// </summary>
        public string Code { get; } = code;
        /// <summary>
        /// Multilanguage name of the database group
        /// </summary>
        public MultilanguageString Name { get; } = name;
        /// <summary>
        /// Available languages of the database group
        /// </summary>
        public List<string> Languages { get; } = languages;
    }
}
