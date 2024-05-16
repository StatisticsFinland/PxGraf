using Newtonsoft.Json;
using NUnit.Framework;
using PxGraf.Language;
using PxGraf.Utility;

namespace SerializerTests
{
    internal class CustomSerializersTests
    {
        [Test]
        public void MultiLanguageStringConverter_EmptyTranslation_IsSerializedToNull()
        {
            MultiLanguageString mls = new();

            var settings = new JsonSerializerSettings
            {
                Converters = { new MultiLanguageStringConverter() }
            };
            var serialized = JsonConvert.SerializeObject(mls, settings);

            Assert.AreEqual("null", serialized);
        }

        [Test]
        public void MultiLanguageStringConverter_NullValue_IsDeserializedToNull()
        {
            var settings = new JsonSerializerSettings
            {
                Converters = { new MultiLanguageStringConverter() }
            };

            var deserialized = JsonConvert.DeserializeObject<MultiLanguageString>("null", settings);

            Assert.AreEqual(null, deserialized);
        }
    }
}

