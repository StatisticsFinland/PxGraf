using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxGraf.Utility
{
    public static partial class InputValidation
    {
        public static bool ValidateSqIdString(string idString)
        {
            if (string.IsNullOrEmpty(idString)) return false;
            else return SqIdValidationRegex().Match(idString).Success;
        }

        [GeneratedRegex("^[a-zA-Z0-9-]*$")]
        private static partial Regex SqIdValidationRegex();

        public static bool ValidateFilePathPart(string pathPart)
        {
            if (string.IsNullOrEmpty(pathPart) || pathPart.Length > 100 || pathPart.All(c => c == '.'))
                return false;

            if (pathPart.Any(c => Path.GetInvalidPathChars().Contains(c)))
                return false;

            // Check if input contains at least one letter or number
            return pathPart.Any(char.IsLetterOrDigit);
        }

        public static bool ValidateFileName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 100|| name.All(c => c == '.'))
                return false;

            if (name.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
                return false;

            // Check if input contains at least one letter or number
            return name.Any(char.IsLetterOrDigit);
        }
    }
}
