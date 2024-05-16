using Newtonsoft.Json;
using NUnit.Framework;
using PxGraf.Utility;
using System;
using System.IO;

namespace UnitTests.SerializerTests
{
    internal class ForceStringConverterTests
    {
        private ForceStringConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _converter = new ForceStringConverter();
        }

        [Test]
        public void CanConvert_ReturnsTrueForStringType()
        {
            bool canConvertString = _converter.CanConvert(typeof(string));
            Assert.IsTrue(canConvertString);
        }

        [Test]
        public void CanConvert_ReturnsFalseForNonStringTypes()
        {
            Assert.IsFalse(_converter.CanConvert(typeof(int)));
            Assert.IsFalse(_converter.CanConvert(typeof(DateTime)));
            Assert.IsFalse(_converter.CanConvert(typeof(object)));
        }

        [Test]
        public void ReadJson_ConvertsDateTimeToJsonTokenString()
        {
            // Arrange
            DateTime date = new(2023, 4, 5, 14, 30, 0, DateTimeKind.Utc);
            string json = JsonConvert.SerializeObject(date);

            using StringReader stringReader = new(json);
            using JsonTextReader jsonReader = new(stringReader);

            jsonReader.Read();

            // Act
            string result = _converter.ReadJson(jsonReader, typeof(string), null, JsonSerializer.CreateDefault());

            // Assert: The result should be the date as an ISO 8601 formatted string.
            string expected = date.ToString("o");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ReadJson_ReturnsStringForNonDateToken()
        {
            // Arrange
            string nonDateValue = "Test String";
            string json = JsonConvert.SerializeObject(nonDateValue);

            using StringReader stringReader = new(json);
            using JsonTextReader jsonReader = new(stringReader);

            jsonReader.Read();

            // Act
            string result = _converter.ReadJson(jsonReader, typeof(string), null, JsonSerializer.CreateDefault());

            // Assert
            Assert.AreEqual(nonDateValue, result);
        }
    }
}

