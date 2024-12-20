using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using PxGraf.Utility.CustomJsonConverters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        public List<DecimalDataValue> Data { get; }

        /// <summary>
        /// Notes mapped to the indexes of the data array.
        /// </summary>
        public Dictionary<int, MultilanguageString> DataNotes { get; }

        /// <summary>
        /// Verison of the archive cube.
        /// </summary>
        public string Version { get; }

        private ArchiveCube(IReadOnlyMatrixMetadata meta, IReadOnlyList<DecimalDataValue> data)
        {
            CreationTime = DateTime.Now;
            Meta = meta;

            Data = [.. data];
            DataNotes = [];
        }

        /// <summary>
        /// Creates a data cube object based on the archive cube.
        /// </summary>
        /// <returns></returns>
        public Matrix<DecimalDataValue> ToMatrix()
        {
            Matrix<DecimalDataValue> matrix = new(Meta, [..Data]);
            matrix.ApplyDataNotesToMissingData(DataNotes);
            return matrix;
        }

        public static ArchiveCube FromMatrixAndQuery(Matrix<DecimalDataValue> matrix, MatrixQuery query)
        {
            IReadOnlyMatrixMetadata queriedMeta = matrix.Metadata.FilterDimensionValues(query);
            return new ArchiveCube(queriedMeta, matrix.Data);
        }

        /// <summary>
        /// Parameterles constructor for json deserialization.
        /// </summary>
        public ArchiveCube() { }

        public ArchiveCube(DateTime creationTime, IReadOnlyMatrixMetadata meta, List<DecimalDataValue> data, Dictionary<int, MultilanguageString> dataNotes, string version)
        {
            CreationTime = creationTime;
            Meta = meta;
            Data = data;
            DataNotes = dataNotes;
            Version = version;
        }
   }
}
