#nullable enable
using Px.Utils.Language;
using System.Collections.Generic;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Represents a listing item in the database. This can be a table, directory or a database.
    /// </summary>
    /// <remarks>
    /// Constructor for a table type listing item.
    /// </remarks>
    public class DataBaseListingItem(string id, MultilanguageString text, IReadOnlyList<string> langs, string updated, string? type = null)
    {
        /// <summary>
        /// The id of the item.
        /// </summary>
        public string Id { get; set; } = id;
        /// <summary>
        /// Multi-language description of the item.
        /// </summary>
        public MultilanguageString Text { get; set; } = text;
        /// <summary>
        /// The languages the item is available in.
        /// </summary>
        public List<string> Languages { get; set; } = [.. langs];
        /// <summary>
        /// String representing the type of the item, a database, subdirectory or a Px table.
        /// </summary>
        public string? Type { get; set; } = type;
        /// <summary>
        /// The date the item was last updated.
        /// </summary>
        public string Updated { get; set; } = updated;
    }
}
#nullable disable
