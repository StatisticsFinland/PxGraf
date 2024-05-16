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

            Assert.AreEqual(str, input["fi"]);
            Assert.AreEqual(str, input["sv"]);
            Assert.AreEqual(str, input["en"]);
        }

        public static void TruncateTest_50Chars_Truncation()
        {
            string str = "123456789_123456789_123456789_123456789_123456789_";
            MultiLanguageString input = new("fi", str);
            input.AddTranslation("sv", str);
            input.AddTranslation("en", str);

            input.Truncate(30);

            string expected = "123456789_123456789_123456789_";
            Assert.AreEqual(expected, input["fi"]);
            Assert.AreEqual(expected, input["sv"]);
            Assert.AreEqual(expected, input["en"]);
        }
    }
}
