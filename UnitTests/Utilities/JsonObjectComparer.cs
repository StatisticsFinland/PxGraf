using NUnit.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace UnitTests.Utilities
{
    internal static partial class JsonUtils
    {
        public static void AreEqual(object expected, object actual)
        {
            string expectedJson = NormalizeJsonString(JsonConvert.SerializeObject(expected));
            string actualJson = NormalizeJsonString(JsonConvert.SerializeObject(actual));
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        public static void JsonStringsAreEqual(string expectedJson, string actualJson)
        {
            Assert.That(NormalizeJsonString(actualJson), Is.EqualTo(NormalizeJsonString(expectedJson)));
        }

        public static string NormalizeJsonString(string json)
        {
            char[] whiteSpace = [' ', '\n', '\r', '\t'];
            List<char> newStr = [];
            bool inString = false;
            for (int i = 0; i < json.Length; i++)
            {
                if(json[i] == '"')
                {
                    inString = !inString;
                }

                if (inString || Array.IndexOf(whiteSpace, json[i]) < 0)
                {
                    newStr.Add(json[i]);
                }
            }
            return new(newStr.ToArray());
        }
    }
}
