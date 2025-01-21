using PxGraf.Models.Queries;
using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;
using System;

namespace PxGraf.Models.SavedQueries.Versions
{
    public abstract class VersionedSavedQuery
    {
        public MatrixQuery Query { get; set; }

        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime CreationTime { get; set; }

        public bool Archived { get; set; }

        public abstract SavedQuery ToSavedQuery();
    }
}
