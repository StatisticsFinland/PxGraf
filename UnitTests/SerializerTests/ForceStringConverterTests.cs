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
            Assert.That(_converter.CanConvert(typeof(string)), Is.True);
        }

        [Test]
        public void CanConvert_ReturnsFalseForNonStringTypes()
        {
            Assert.That(_converter.CanConvert(typeof(int)), Is.False);
            Assert.That(_converter.CanConvert(typeof(DateTime)), Is.False);
            Assert.That(_converter.CanConvert(typeof(object)), Is.False);
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
            Assert.That(result, Is.EqualTo(expected));
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
            Assert.That(result, Is.EqualTo(nonDateValue));
        }
    }
}

