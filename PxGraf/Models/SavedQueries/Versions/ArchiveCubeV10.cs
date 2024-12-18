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
    public class ArchiveCubeV10 : VersionedArchiveCube
    {
        /// <summary>
        /// Contains all metadata of this cube.
        /// </summary>
        [JsonPropertyName("meta")]
        public CubeMeta Meta { get; set; }

        public ArchiveCubeV10() { }

        public ArchiveCubeV10(DateTime creationTime, CubeMeta meta, List<DecimalDataValue> data, Dictionary<int, MultilanguageString> dataNotes)
        {
            CreationTime = creationTime;
            Meta = meta;
            Data = data;
            DataNotes = dataNotes;
        }

        public override ArchiveCube ToArchiveCube()
        {
            return new ArchiveCube
            (
                CreationTime,
                Meta.ToMatrixMetadata(),
                Data,
                DataNotes,
                "1.0"
            );
        }
    }
}
