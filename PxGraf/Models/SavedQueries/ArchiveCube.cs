using Newtonsoft.Json;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using System;
using System.Collections.Generic;

namespace PxGraf.Models.SavedQueries
{
    /// <summary>
    /// Used for serializing DataCube objects to a file.
    /// </summary>
    public class ArchiveCube
    {
        /// <summary>
        /// When this archive object was originally created.
        /// </summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Contains all metadata of this cube.
        /// </summary>
        [JsonProperty("meta")]
        public CubeMeta Meta { get; set; }

        /// <summary>
        /// Collection of key (variable value code coordinates) value (double or missing code) pairs.
        /// </summary>
        [JsonProperty("data")]
        public List<double?> Data { get; set; }

        /// <summary>
        /// Notes mapped to the indexes of the data array.
        /// </summary>
        [JsonProperty("dataNotes")]
        public Dictionary<int, string> DataNotes { get; set; }

        /// <summary>
        /// Parameterles constructor for json deserialization.
        /// </summary>
        public ArchiveCube() { }

        private ArchiveCube(DataCube cube)
        {
            CreationTime = DateTime.Now;

            Meta = cube.Meta.Clone();

            List<double?> data = [];
            Dictionary<int, string> dataNotes = [];

            DataValue[] dataList = cube.Data;
            int dataLen = dataList.Length;

            for (int i = 0; i < dataLen; i++)
            {
                if (dataList[i].Type == DataValueType.Exist)
                {
                    data.Add(dataList[i].UnsafeValue);
                }
                else
                {
                    data.Add(null);
                    dataNotes[i] = dataList[i].ToMachineReadableString(0);
                }
            }

            Data = data;
            DataNotes = dataNotes;
        }

        /// <summary>
        /// Creates a data cube object based on the archive cube.
        /// </summary>
        /// <returns></returns>
        public DataCube ToDataCube()
        {
            DataValue[] newData = new DataValue[Data.Count];
            int dataLen = Data.Count;
            for (int i = 0; i < dataLen; i++)
            {
                if (Data[i].HasValue)
                {
                    newData[i] = DataValue.FromRaw(Data[i].Value);
                }
                else
                {
                    newData[i] = DataValue.FromDotCode(DataNotes[i], DataValueType.Missing);
                }
            }

            return new DataCube(Meta, newData);
        }

        /// <summary>
        /// Builds an archive cube object based on a data cube.
        /// </summary>
        /// <param name="cube"></param>
        /// <returns></returns>
        public static ArchiveCube FromDataCube(DataCube cube)
        {
            return new ArchiveCube(cube);
        }
    }
}
