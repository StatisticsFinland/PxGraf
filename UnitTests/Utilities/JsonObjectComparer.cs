using Newtonsoft.Json;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace UnitTests.Utilities
{
    internal static partial class JsonUtils
    {
        public static void AreEqual(object expected, object actual)
        {
            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [GeneratedRegex("\\s")]
        private static partial Regex MyRegex();

        public static string NormalizeJsonString(string json)
        {
            return MyRegex().Replace(json, "");
        }
    }
}
