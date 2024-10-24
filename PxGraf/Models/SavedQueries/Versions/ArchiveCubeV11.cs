using Px.Utils.Models.Metadata;
using System.Text.Json.Serialization;
using PxGraf.Utility.CustomJsonConverters;

namespace PxGraf.Models.SavedQueries.Versions
{
    public class ArchiveCubeV11 : VersionedArchiveCube
    {
        /// <summary>
        /// Contains all metadata of this cube.
        /// </summary>
        [JsonConverter(typeof(MatrixMetadataReadonlyInterfaceConverter))]
        public IReadOnlyMatrixMetadata Meta { get; set; }

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

        public override ArchiveCube ToArchiveCube()
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
