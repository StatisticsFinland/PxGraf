using PxGraf.Language;
using PxGraf.PxWebInterface.SerializationModels;
using System.Collections.Generic;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Represents a listing item in the database. This can be a table, directory or a database.
    /// </summary>
    public class DataBaseListingItem
    {
        /// <summary>
        /// The id of the item.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Multi-language description of the item.
        /// </summary>
        public MultiLanguageString Text { get; set; }
        /// <summary>
        /// The languages the item is available in.
        /// </summary>
        public List<string> Languages { get; set; }
        /// <summary>
        /// String representing the type of the item, a database, subdirectory or a Px table.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The date the item was last updated.
        /// </summary>
        public string Updated { get; set; }

        /// <summary>
        /// Constructor for a table type listing item.
        /// </summary>
        /// <param name="table"><see cref="TableListResponseItem"/> object that stores the information of the table.</param>
        /// <param name="language">Language for the listing item.</param>
        public DataBaseListingItem(TableListResponseItem table, string language)
        {
            Id = table.Id;
            Text = new(language, table.Text);
            Type = table.Type;
            Updated = table.Updated;
            Languages = [language];
        }

        /// <summary>
        /// Constructor for a listing item.
        /// </summary>
        /// <param name="database"><see cref="DataBaseListResponseItem"/> object that stores the information of the database.</param>
        /// <param name="language">Language for the listing item.</param>
        public DataBaseListingItem(DataBaseListResponseItem database, string language)
        {
            Id = database.Dbid;
            Text = new(language, database.Text);
            Languages = [language];
        }
    }
}
