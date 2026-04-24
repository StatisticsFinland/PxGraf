using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PxGraf.Settings;
using System.Collections.Generic;
using UnitTests.Fixtures;

namespace UnitTests.ConfigurationTests
{
    [TestFixture]
    internal class IsFeatureEnabledTests
    {
        private static void LoadConfigWithCreationAPI(bool enabled)
        {
            Dictionary<string, string> config = TestInMemoryConfiguration.Get();
            config["FeatureManagement:CreationAPI"] = enabled.ToString();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            Configuration.Load(configuration);
        }

        [Test]
        public void IsFeatureEnabled_CreationAPIEnabled_ReturnsTrue()
        {
            LoadConfigWithCreationAPI(true);

            Assert.That(Configuration.Current.IsFeatureEnabled("CreationAPI"), Is.True);
        }

        [Test]
        public void IsFeatureEnabled_CreationAPIDisabled_ReturnsFalse()
        {
            LoadConfigWithCreationAPI(false);

            Assert.That(Configuration.Current.IsFeatureEnabled("CreationAPI"), Is.False);
        }

        [Test]
        public void IsFeatureEnabled_UnknownFeature_ReturnsTrue()
        {
            LoadConfigWithCreationAPI(true);

            Assert.That(Configuration.Current.IsFeatureEnabled("NonExistentFeature"), Is.True);
        }

        [Test]
        public void IsFeatureEnabled_CaseInsensitive_ReturnsCorrectResult()
        {
            LoadConfigWithCreationAPI(true);

            Assert.That(Configuration.Current.IsFeatureEnabled("creationapi"), Is.True);
            Assert.That(Configuration.Current.IsFeatureEnabled("CREATIONAPI"), Is.True);
        }
    }
}
