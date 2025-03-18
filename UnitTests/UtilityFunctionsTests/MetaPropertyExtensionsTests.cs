using NUnit.Framework;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Utility;
using System;

namespace UnitTests.UtilityFunctionsTests
{
    public class MetaPropertyExtensionsTests
    {
        [Test]
        public void AsMultiLanguageProperty_WithMultilanguageStringProperty_ReturnsSameInstance()
        {
            // Arrange
            string lang = "en";
            MultilanguageStringProperty originalProperty = new(new(lang, "TestValue"));

            // Act
            MultilanguageStringProperty result = originalProperty.AsMultiLanguageProperty(lang);

            // Assert
            Assert.That(originalProperty, Is.SameAs(result));
        }

        [Test]
        public void AsMultiLanguageProperty_WithStringProperty_ReturnsMultilanguageStringProperty()
        {
            // Arrange
            string lang = "en";
            StringProperty originalProperty = new("TestValue");

            // Act
            MultilanguageStringProperty result = originalProperty.AsMultiLanguageProperty(lang);

            // Assert
            Assert.That(result, Is.TypeOf<MultilanguageStringProperty>());
            Assert.That(result.Value[lang], Is.EqualTo("TestValue"));
        }

        [Test]
        public void AsMultiLanguageProperty_WithInvalidPropertyType_ThrowsArgumentException()
        {
            // Arrange
            string lang = "en";
            StringListProperty originalProperty = new(["foo", "bar", "föö", "bär"]);

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => originalProperty.AsMultiLanguageProperty(lang));
            Assert.That(exception.Message,
                Is.EqualTo("Property of type Px.Utils.Models.Metadata.MetaProperties.StringListProperty can not be converted to multilanguage string property."));
        }
    }
}
