using Px.Utils.Models.Metadata;
using System.Collections.Generic;
using System;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Language;

namespace PxGraf.Models.SavedQueries.Versions
{
    public class ArchiveCubeV11 : IVersionedArchiveCube
    {
        /// <summary>
        /// When this archive object was originally created.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Contains all metadata of this cube.
        /// </summary>
        public IReadOnlyMatrixMetadata Meta { get; set; }

        /// <summary>
        /// Collection of key (variable value code coordinates) value (double or missing code) pairs.
        /// </summary>
        public List<DecimalDataValue> Data { get; set; }

        /// <summary>
        /// Notes mapped to the indexes of the data array.
        /// </summary>
        public Dictionary<int, MultilanguageString> DataNotes { get; set; }

        /// <summary>
        /// Verison of the archive cube.
        /// </summary>
        public string Version { get; set; }

        public ArchiveCubeV11() { }

        public ArchiveCubeV11(ArchiveCube cube)
        {
            CreationTime = cube.CreationTime;
            Meta = cube.Meta;
            Data = cube.Data;
            DataNotes = cube.DataNotes;
            Version = "1.1";
        }

        public ArchiveCube ToArchiveCube()
        {
            return new ArchiveCube
            {
                CreationTime = CreationTime,
                Meta = Meta,
                Data = Data,
                DataNotes = DataNotes,
                Version = "1.1"
            };
        }
    }
}
