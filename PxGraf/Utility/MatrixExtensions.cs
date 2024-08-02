using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using System.Collections.Generic;

namespace PxGraf.Utility
{
    /// <summary>
    /// Collection of static utility functions for DataCube manipulation.
    /// </summary>
    public static class MatrixExtensions
    {
        public class DataAndNotesCollection
        {
            public decimal?[] Data { get; set; }
            public IReadOnlyDictionary<int, MultilanguageString> Notes { get; set; }

            public IReadOnlyDictionary<int, int> MissingValueInfo { get; set; }
        }

        public static DataAndNotesCollection ExtractDataAndNotes(this Matrix<DecimalDataValue> matrix)
        {
            decimal?[] data = new decimal?[matrix.Data.Length];
            // This will be empty as long as pxweb api doesn't return notes
            Dictionary<int, MultilanguageString> notes = [];
            Dictionary<int, int> missingValueInfo = [];

            int dataLength = matrix.Data.Length;

            for (int i = 0; i < dataLength; i++)
            {
                if (matrix.Data[i].Type == DataValueType.Exists)
                {
                    data[i] = matrix.Data[i].UnsafeValue;
                }
                else
                {
                    data[i] = null;
                    missingValueInfo[i] = (int)matrix.Data[i].Type;
                }
            }

            return new DataAndNotesCollection()
            {
                Data = data,
                Notes = notes, //TODO cellnotes
                MissingValueInfo = missingValueInfo
            };
        }
    }
}
