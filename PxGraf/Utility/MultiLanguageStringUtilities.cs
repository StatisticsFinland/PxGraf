using PxGraf.Language;

namespace PxGraf.Utility
{
    public static class MultiLanguageStringUtilities
    {
        public static void Truncate(this MultiLanguageString input, int maxLength)
        {
            foreach (string language in input.Languages)
            {
                if (input[language].Length > maxLength)
                {
                    input.EditTranslation(language, input[language][..maxLength]);
                }
            }
        }
    }
}
