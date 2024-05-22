using NUnit.Framework;
using PxGraf.Language;
using PxGraf.Utility;

namespace UtilityFunctionsTests
{
    internal static class MultiLanguageStringUtilitiesTest
    {
        public static void TruncateTest_50Chars_NoTruncation()
        {
            string str = "123456789_123456789_123456789_123456789_123456789_";
            MultiLanguageString input = new("fi", str);
            input.AddTranslation("sv", str);
            input.AddTranslation("en", str);

            input.Truncate(50);

            Assert.That(input["fi"], Is.EqualTo(str));
            Assert.That(input["sv"], Is.EqualTo(str));
            Assert.That(input["en"], Is.EqualTo(str));
        }

        public static void TruncateTest_50Chars_Truncation()
        {
            string str = "123456789_123456789_123456789_123456789_123456789_";
            MultiLanguageString input = new("fi", str);
            input.AddTranslation("sv", str);
            input.AddTranslation("en", str);

            input.Truncate(30);

            string expected = "123456789_123456789_123456789_";
            Assert.That(input["fi"], Is.EqualTo(expected));
            Assert.That(input["sv"], Is.EqualTo(expected));
            Assert.That(input["en"], Is.EqualTo(expected));
        }
    }
}
