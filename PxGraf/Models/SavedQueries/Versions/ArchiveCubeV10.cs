using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;
using PxGraf.Data.MetaData;
using PxGraf.Models.Metadata;
using Px.Utils.Models.Data.DataValue;
using PxGraf.Utility.CustomJsonConverters;
using Px.Utils.Language;

namespace PxGraf.Models.SavedQueries.Versions
{
    [JsonConverter(typeof(ArchiveCubeV10ReadOnlyConverter))]
    public class ArchiveCubeV10 : IVersionedArchiveCube
    {
        /// <summary>
        /// When this archive object was originally created.
        /// </summary>
        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Contains all metadata of this cube.
        /// </summary>
        [JsonPropertyName("meta")]
        public CubeMeta Meta { get; set; }

        /// <summary>
        /// Collection of key (variable value code coordinates) value (double or missing code) pairs.
        /// </summary>
        [JsonPropertyName("data")]
        public List<DecimalDataValue> Data { get; set; }

        /// <summary>
        /// Notes mapped to the indexes of the data array.
        /// </summary>
        [JsonPropertyName("dataNotes")]
        public Dictionary<int, MultilanguageString> DataNotes { get; set; }

        public ArchiveCubeV10() { }

        public ArchiveCubeV10(DateTime creationTime, CubeMeta meta, List<DecimalDataValue> data, Dictionary<int, MultilanguageString> dataNotes)
        {
            CreationTime = creationTime;
            Meta = meta;
            Data = data;
            DataNotes = dataNotes;
        }

        public ArchiveCube ToArchiveCube()
        {
            return new ArchiveCube
            {
                CreationTime = CreationTime,
                Meta = Meta.ToMatrixMetadata(),
                Data = Data,
                DataNotes = DataNotes,
                Version = "1.0"
            };
        }
    }
}
