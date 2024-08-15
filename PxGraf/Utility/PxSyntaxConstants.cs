using System.Globalization;
using System;

namespace PxGraf.Utility
{
    /// <summary>
    /// Constants used for the Px syntax
    /// </summary>
    public static class PxSyntaxConstants
    {
        public const string PXWEB_DATETIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.fff";
        public const string SOURCE_KEY = "SOURCE";
        public const string TABLEID_KEY = "TABLEID";
        public const string DESCRIPTION_KEY = "DESCRIPTION";
        public const string NOTE_KEY = "NOTE";
        public const string VALUENOTE_KEY = "VALUENOTE";
        public const string ELIMINATION_KEY = "ELIMINATION";
        public const char STRING_DELIMETER = '"';

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

        /// <summary>
        /// Parses a string in the Px datetime format to a DateTime object
        /// </summary>
        /// <param name="dateTimeString">String to parse</param>
        /// <returns>DateTime object</returns>
        /// <remarks>Provided string must be in the <see cref="PXWEB_DATETIME_FORMAT"/> format</remarks>
        public static DateTime ParsePxDateTime(string dateTimeString)
        {
            if (DateTime.TryParseExact(dateTimeString, PXWEB_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime result))
            {
                return result;
            }
            else
            {
                const string format = PXWEB_DATETIME_FORMAT + "'Z'";
                return DateTime.ParseExact(dateTimeString, format, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Formats a DateTime object to a string in the Px datetime format
        /// </summary>
        /// <param name="dateTime">DateTime object to format</param>
        /// <param name="zuluTime">If true, the time is formatted as Zulu time</param>
        /// <returns>Formatted string</returns>
        public static string FormatPxDateTime(DateTime dateTime, bool zuluTime = false)
        {
            string format = zuluTime ? PXWEB_DATETIME_FORMAT + "'Z'" : PXWEB_DATETIME_FORMAT;
            return dateTime.ToString(format);
        }
    }

}
