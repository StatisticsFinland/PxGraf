using System.Globalization;
using System;

namespace PxGraf.Utility
{
    /// <summary>
    /// TODO: document
    /// </summary>
    public static class PxSyntaxConstants
    {
        public const string PXWEB_DATETIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss";
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
        /// TODO: document
        /// </summary>
        /// <param name="dateTimeString"></param>
        /// <returns></returns>
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
        /// TODO: document
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string FormatPxDateTime(DateTime dateTime)
        {
            return dateTime.ToString(PXWEB_DATETIME_FORMAT);
        }
    }

}
