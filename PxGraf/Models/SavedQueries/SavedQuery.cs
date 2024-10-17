using PxGraf.Models.Queries;
using PxGraf.Utility.CustomJsonConverters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.SavedQueries
{
    [JsonConverter(typeof(SavedQueryConverter))]
    public sealed class SavedQuery
    {
        /// <summary>
        /// The query
        /// </summary>
        public MatrixQuery Query { get; set; }

        /// <summary>
        /// When was this query saved
        /// </summary>
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// If true there exists an archive data file that should be used as a primary source of data values.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Contains visualization type and the type spesific settings.
        /// </summary>
        public VisualizationSettings Settings { get; set; }

        /// <summary>
        /// Contains legacy properties to enable deserialization of older versions
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> LegacyProperties { get; }

        /// <summary>
        /// Query version
        /// </summary>
        public string Version { get; set; } = "1.1";

        /// <summary>
        /// Use this for builing the object outside of the serialization.
        /// </summary>
        public SavedQuery(MatrixQuery query, bool archived, VisualizationSettings settings, DateTime creationTime)
        {
            Query = query;
            Archived = archived;
            Settings = settings;
            CreationTime = creationTime;
            LegacyProperties = [];
        }

        /// <summary>
        /// Parameterless constructor required for serialization.
        /// </summary>
        public SavedQuery()
        {
            LegacyProperties = [];
        }
    }
}
