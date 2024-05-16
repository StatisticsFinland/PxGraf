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
    }
}
