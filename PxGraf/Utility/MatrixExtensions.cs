using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Extracts data and notes from a matrix.
        /// </summary>
        /// <param name="matrix">Matrix to extract data and notes from.</param>
        /// <returns><see cref="DataAndNotesCollection"/> object that contains the data and data notes.</returns>
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
                Notes = notes,
                MissingValueInfo = missingValueInfo
            };
        }

        /// <summary>
        /// Finds missing values in data matrix from data notes and replaces them with the appropriate missing value.
        /// </summary>
        /// <param name="matrix">Matrix to apply missing values to.</param>
        /// <param name="dataNotes">Data notes to apply to the matrix.</param>
        public static void ApplyDataNotesToMissingData(this Matrix<DecimalDataValue> matrix, IReadOnlyDictionary<int, MultilanguageString> dataNotes)
        {
            if (dataNotes == null || dataNotes.Count == 0)
            {
                return;
            }

            string defaultLanguage = matrix.Metadata.DefaultLanguage;
            for (int i = 0; i < matrix.Data.Length; i++)
            {
                if (dataNotes.TryGetValue(i, out MultilanguageString value) && PxSyntaxConstants.MissingValueDotCodes.Contains(value[defaultLanguage]))
                {
                    DataValueType valueType = (DataValueType)Array.IndexOf(PxSyntaxConstants.MissingValueDotCodes, value[defaultLanguage]);
                    matrix.Data[i] = new DecimalDataValue(0, valueType);
                }
            }
        }
    }
}
