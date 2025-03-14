﻿using System.Globalization;
using System;
using System.Collections.Generic;
using Px.Utils.Models.Data;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace PxGraf.Utility
{
    /// <summary>
    /// Constants used for the Px syntax
    /// </summary>
    public static class PxSyntaxConstants
    {
        public const string PXWEB_DATETIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.fff";
        public const string PXWEB_DATETIME_FORMAT_TS_ZERO = "yyyy-MM-dd'T'HH:mm:ss.fffZ";
        public const string SOURCE_KEY = "SOURCE";
        public const string TABLEID_KEY = "TABLEID";
        public const string DESCRIPTION_KEY = "DESCRIPTION";
        public const string NOTE_KEY = "NOTE";
        public const string VALUENOTE_KEY = "VALUENOTE";
        public const string ELIMINATION_KEY = "ELIMINATION";
        public const string LAST_UPDATED_KEY = "LAST-UPDATED";
        public const string UNIT_KEY = "UNIT";
        public const char STRING_DELIMETER = '"';
        public const string DATETIME_FORMAT_WITH_MS = "yyyy-MM-dd'T'HH:mm:ss.fffffffK";
        public const string DATETIME_FORMAT_NO_MS = "yyyy-MM-dd'T'HH:mm:ssK";
        public const string DATETIME_FORMAT_NO_MS_TS_ZERO = "yyyy-MM-dd'T'HH:mm:ss'Z'";
        public const string META_ID_KEY = "META-ID";
        public const string ORDINAL_VALUE = "SCALE-TYPE=ordinal";
        public const string NOMINAL_VALUE = "SCALE-TYPE=nominal";
        public const string PX_FILE_FILTER = "*.px";
        public const string PX_FILE_EXTENSION = ".px";
        public const string ALIAS_FILE_FILTER = "*.txt";
        public const string ALIAS_FILE_PREFIX = "alias";

        //Indexed by DataValueType with offset of one
        public static readonly string[] MissingValueDotCodes =
        [
            "",
            ".",
            "..",
            "...",
            "....",
            ".....",
            "......",
            "-",
        ];

        private static Dictionary<string, DataValueType> missingValueTypes =
        new()
        {
            ["."] = DataValueType.Missing,
            [".."] = DataValueType.CanNotRepresent,
            ["..."] = DataValueType.Confidential,
            ["...."] = DataValueType.NotAcquired,
            ["....."] = DataValueType.NotAsked,
            ["......"] = DataValueType.Empty,
            ["-"] = DataValueType.Nill,
        };

        /// <summary> 
        /// Tries to parse a string to a DateTime object using several different formats.
        /// </summary>
        /// <param name="dateTimeString">String to parse</param>
        /// <returns>DateTime object</returns>
        public static DateTime ParseDateTime(string dateTimeString)
        {
            List<string> dateTimeFormats = [
                DATETIME_FORMAT_WITH_MS,
                DATETIME_FORMAT_NO_MS,
                PXWEB_DATETIME_FORMAT,
                DATETIME_FORMAT_NO_MS_TS_ZERO,
                PXWEB_DATETIME_FORMAT_TS_ZERO,
                ];

            foreach (string format in dateTimeFormats)
            {
                if (DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime dateTime))
                {
                    return dateTime;
                }
            }

            return DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats a DateTime object to a string in the Px datetime format
        /// </summary>
        /// <param name="dateTime">DateTime object to format</param>
        /// <returns>Formatted string</returns>
        public static string FormatPxDateTime(DateTime dateTime) =>
            dateTime.ToUniversalTime().ToString(DATETIME_FORMAT_NO_MS_TS_ZERO, CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets the <see cref="DataValueType"/> from a missing value dot code.
        /// </summary>
        /// <param name="code">Missing value code</param>
        /// <returns><see cref="DataValueType"/> which the input code represents</returns>
        /// <exception cref="ArgumentException">If the provided code does not match any missing data value types</exception>
        public static DataValueType GetMissingDataTypeFromCode(string code)
        {
            if (missingValueTypes.TryGetValue(code, out DataValueType type))
            {
                return type;
            }
            else
            {
                throw new ArgumentException($"Invalid missing value dot code {code}");
            }
        }
    }
}
