using NUnit.Framework;
using PxGraf.Utility.CustomJsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnitTests.SerializerTests
{
    public class ConverterUtilityTests
    {
        [Test]
        public void GetPropertyCaseInsensitiveCalledWithLowerCasePropertyNameReturnsProperty()
        {
            // Arrange
            string lowerCaseProperty = @"{""property"":""value""}";
            JsonElement element = JsonSerializer.Deserialize<JsonElement>(lowerCaseProperty);
            string propertyName = "Property";
            JsonElement value = element.GetProperty("property");

            // Act
            JsonElement result = CustomConverterUtilities.GetPropertyCaseInsensitive(element, propertyName);

            // Assert
            Assert.That(value.Equals(result));
        }

        [Test]
        public void GetPropertyCaseInsensitiveCalledWithUpperCasePropertyNameReturnsProperty()
        {
            // Arrange
            string upperCaseProperty = @"{""Property"":""value""}";
            JsonElement element = JsonSerializer.Deserialize<JsonElement>(upperCaseProperty);
            string propertyName = "property";
            JsonElement value = element.GetProperty("Property");

            // Act
            JsonElement result = CustomConverterUtilities.GetPropertyCaseInsensitive(element, propertyName);

            // Assert
            Assert.That(value.Equals(result));
        }

        [Test]
        public void TryGetPropertyCaseInsensitiveCalledWithLowerCasePropertyNameReturnsTrue()
        {
            // Arrange
            string lowerCaseProperty = @"{""property"":""value""}";
            JsonElement element = JsonSerializer.Deserialize<JsonElement>(lowerCaseProperty);
            string propertyName = "Property";
            JsonElement value = element.GetProperty("property");

            // Act
            bool result = CustomConverterUtilities.TryGetPropertyCaseInsensitive(element, propertyName, out JsonElement outValue);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(value.Equals(outValue));
        }

        [Test]
        public void TryGetPropertyCaseInsensitiveCalledWithUpperCasePropertyNameReturnsTrue()
        {
            // Arrange
            string upperCaseProperty = @"{""Property"":""value""}";
            JsonElement element = JsonSerializer.Deserialize<JsonElement>(upperCaseProperty);
            string propertyName = "property";
            JsonElement value = element.GetProperty("Property");

            // Act
            bool result = CustomConverterUtilities.TryGetPropertyCaseInsensitive(element, propertyName, out JsonElement outValue);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(value.Equals(outValue));
        }

        [Test]
        public void TryGetPropertyCaseInsensitiveCalledWithNonExistentPropertyNameReturnsFalse()
        {
            // Arrange
            string upperCaseProperty = @"{""Property"":""value""}";
            JsonElement element = JsonSerializer.Deserialize<JsonElement>(upperCaseProperty);
            string propertyName = "nonExistentProperty";

            // Act
            bool result = CustomConverterUtilities.TryGetPropertyCaseInsensitive(element, propertyName, out JsonElement _);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
