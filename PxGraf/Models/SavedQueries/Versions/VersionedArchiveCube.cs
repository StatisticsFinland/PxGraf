using Px.Utils.Language;
using Px.Utils.Models.Data.DataValue;
using PxGraf.Utility.CustomJsonConverters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.SavedQueries.Versions
{
    public abstract class VersionedArchiveCube
    {
        /// <summary>
        /// When this archive object was originally created.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Collection of key (variable value code coordinates) value (double or missing code) pairs.
        /// </summary>
        [JsonConverter(typeof(DecimalDataValueListConverter))]
        public List<DecimalDataValue> Data { get; set; }

        /// <summary>
        /// Notes mapped to the indexes of the data array.
        /// </summary>
        public Dictionary<int, MultilanguageString> DataNotes { get; set; }

        public abstract ArchiveCube ToArchiveCube();
    }
}
