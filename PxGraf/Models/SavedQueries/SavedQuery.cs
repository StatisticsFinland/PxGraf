using Newtonsoft.Json;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;

namespace PxGraf.Models.SavedQueries
{
    [JsonConverter(typeof(SavedQuerySerializer))]
    public class SavedQuery
    {
        /// <summary>
        /// The query
        /// </summary>
        public CubeQuery Query { get; set; }

        /// <summary>
        /// When was this query saved
        /// </summary>
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
        public SavedQuery(CubeQuery query, bool archived, VisualizationSettings settings, DateTime creationTime)
        {
            Query = query;
            Archived = archived;
            Settings = settings;
            CreationTime = creationTime;
            LegacyProperties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Parameterless constructor required for serialization.
        /// </summary>
        public SavedQuery()
        {
            LegacyProperties = new Dictionary<string, object>();
        }
    }
}
