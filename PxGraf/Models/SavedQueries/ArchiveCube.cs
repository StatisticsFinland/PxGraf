using Px.Utils.Language;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Utility.CustomJsonConverters;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace PxGraf.Models.SavedQueries
{
    /// <summary>
    /// Used for serializing DataCube objects to a file.
    /// </summary>
    [JsonConverter(typeof(ArchiveMatrixSerializer))]
    public class ArchiveCube
    {
        /// <summary>
        /// When this archive object was originally created.
        /// </summary>
        public DateTime CreationTime { get; }

        /// <summary>
        /// Contains all metadata of this cube.
        /// </summary>
        public IReadOnlyMatrixMetadata Meta { get; }

        /// <summary>
        /// Collection of key (dimension value code coordinates) value (double or missing code) pairs.
        /// </summary>
        [JsonConverter(typeof(DecimalDataValueListConverter))]
        public IReadOnlyList<DecimalDataValue> Data { get; }

        /// <summary>
        /// Notes mapped to the indexes of the data array.
        /// </summary>
        public Dictionary<int, MultilanguageString> DataNotes { get; }

        /// <summary>
        /// Verison of the archive cube.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Parameterles constructor for json deserialization.
        /// </summary>
        public ArchiveCube() { }

        public ArchiveCube(Matrix<DecimalDataValue> matrix)
        {
            CreationTime = DateTime.Now;
            Meta = matrix.Metadata;
            Data = matrix.Data;
            DataNotes = [];
            Version = "1.1";
        }

        public ArchiveCube(DateTime creationTime, IReadOnlyMatrixMetadata meta, IReadOnlyList<DecimalDataValue> data, Dictionary<int, MultilanguageString> dataNotes, string version)
        {
            CreationTime = creationTime;
            Meta = meta;
            Data = data;
            DataNotes = dataNotes;
            Version = version;
        }
        
        /// <summary>
        /// Creates a data cube object based on the archive cube.
        /// </summary>
        /// <returns></returns>
        public Matrix<DecimalDataValue> ToMatrix()
        {
            Matrix<DecimalDataValue> matrix = new(Meta, [..Data]);
            return matrix;
        }
   }
}
