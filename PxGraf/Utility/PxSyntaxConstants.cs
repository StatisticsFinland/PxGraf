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
        public const string PXWEB_DATETIME_FORMAT_ZULU = "yyyy-MM-dd'T'HH:mm:ss.fffZ";
        public const string SOURCE_KEY = "SOURCE";
        public const string TABLEID_KEY = "TABLEID";
        public const string DESCRIPTION_KEY = "DESCRIPTION";
        public const string NOTE_KEY = "NOTE";
        public const string VALUENOTE_KEY = "VALUENOTE";
        public const string ELIMINATION_KEY = "ELIMINATION";
        public const string LAST_UPDATED_KEY = "LAST-UPDATED";
        public const string UNIT_KEY = "UNIT";
        public const char STRING_DELIMETER = '"';
        public const string SQ_DATETIME_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffffffK";

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
        /// <remarks>Provided string must be in the <see cref="PXWEB_DATETIME_FORMAT"/> or <see cref="PXWEB_DATETIME_FORMAT_ZULU"/> format</remarks>
        public static DateTime ParsePxDateTime(string dateTimeString)
        {
            if (dateTimeString.EndsWith('Z'))
            {
                return DateTime.ParseExact(dateTimeString, PXWEB_DATETIME_FORMAT_ZULU, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
            }
            else
            {
                return DateTime.ParseExact(dateTimeString, PXWEB_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            }
        }

        /// <summary>
        /// Formats a DateTime object to a string in the Px datetime format
        /// </summary>
        /// <param name="dateTime">DateTime object to format</param>
        /// <returns>Formatted string</returns>
        public static string FormatPxDateTime(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToString(PXWEB_DATETIME_FORMAT_ZULU, CultureInfo.InvariantCulture);
            }
            else
            {
                return dateTime.ToString(PXWEB_DATETIME_FORMAT, CultureInfo.InvariantCulture);
            }
        }
    }
}
