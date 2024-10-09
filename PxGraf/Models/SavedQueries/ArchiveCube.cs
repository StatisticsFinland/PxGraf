﻿using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Utility;
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

        /// <summary>
        /// Parameterles constructor for json deserialization.
        /// </summary>
        public ArchiveCube() { }

        private ArchiveCube(IReadOnlyMatrixMetadata meta, IReadOnlyList<DecimalDataValue> data)
        {
            CreationTime = DateTime.Now;
            Meta = meta;

            int dataLen = data.Count;
            Data = [];
            DataNotes = [];

            for (int i = 0; i < dataLen; i++)
            {
                if (data[i].Type == DataValueType.Exists)
                {
                    Data.Add(data[i].UnsafeValue);
                }
                else
                {
                    Data.Add(null);
                    DataNotes[i] = PxSyntaxConstants.MissingValueDotCodes[(int)data[i].Type];
                }
            }
        }

        /// <summary>
        /// Creates a data cube object based on the archive cube.
        /// </summary>
        /// <returns></returns>
        public Matrix<DecimalDataValue> ToMatrix()
        {
            DecimalDataValue[] newData = new DecimalDataValue[Data.Count];
            for (int i = 0; i < newData.Length; i++)
            {
                if (Data[i].HasValue)
                {
                    newData[i] = new(Data[i].Value, DataValueType.Exists);
                }
                else
                {
                    newData[i] = DataNotes[i] switch
                    {
                        "." => new(0, DataValueType.Missing),
                        ".." => new(0, DataValueType.CanNotRepresent),
                        "..." => new(0, DataValueType.Confidential),
                        "...." => new(0, DataValueType.NotAcquired),
                        "....." => new(0, DataValueType.NotAsked),
                        "......" => new(0, DataValueType.Empty),
                        _ => throw new InvalidOperationException($"Can not convert missing value code {DataNotes[i]} to DataValueType")
                    };
                }
            }

            return new Matrix<DecimalDataValue>(Meta, newData);
        }

        public static ArchiveCube FromMatrixAndQuery(Matrix<DecimalDataValue> matrix, MatrixQuery query)
        {
            IReadOnlyMatrixMetadata queriedMeta = matrix.Metadata.FilterDimensionValues(query);
            return new ArchiveCube(queriedMeta, matrix.Data);
        }
    }
}
