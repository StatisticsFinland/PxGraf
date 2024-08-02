using Newtonsoft.Json;
using NUnit.Framework;
using Px.Utils.Language;
using PxGraf.Utility;

namespace SerializerTests
{
    internal class CustomSerializersTests
    {
        [Test]
        public void MultiLanguageStringConverter_EmptyTranslation_IsSerializedToNull()
        {
            MultilanguageString mls = new([]);

            var settings = new JsonSerializerSettings
            {
                Converters = { new MultilanguageStringConverter() }
            };
            var serialized = JsonConvert.SerializeObject(mls, settings);

            Assert.That(serialized, Is.EqualTo("null"));
        }

        [Test]
        public void MultiLanguageStringConverter_NullValue_IsDeserializedToNull()
        {
            var settings = new JsonSerializerSettings
            {
                Converters = { new MultilanguageStringConverter() }
            };

            var deserialized = JsonConvert.DeserializeObject<MultilanguageString>("null", settings);

            Assert.That(deserialized, Is.Null);
        }
    }
}

