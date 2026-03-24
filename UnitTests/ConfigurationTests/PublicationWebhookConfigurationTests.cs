using NUnit.Framework;
using PxGraf.Services;
using PxGraf.Settings;

namespace UnitTests.ConfigurationTests
{
    [TestFixture]
    internal class PublicationWebhookConfigurationTests
    {
        #region IsEnabled

        [Test]
        public void IsEnabled_AllFieldsSet_ReturnsTrue()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                WebhookEndpointPath = "/api/publish",
                BodyContentPropertyNames = [PublicationPropertyType.Id]
            };

            Assert.That(config.IsEnabled, Is.True);
        }

        [Test]
        public void IsEnabled_BaseUrlNull_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = null,
                WebhookEndpointPath = "/api/publish",
                BodyContentPropertyNames = [PublicationPropertyType.Id]
            };

            Assert.That(config.IsEnabled, Is.False);
        }

        [Test]
        public void IsEnabled_BaseUrlEmpty_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "",
                WebhookEndpointPath = "/api/publish",
                BodyContentPropertyNames = [PublicationPropertyType.Id]
            };

            Assert.That(config.IsEnabled, Is.False);
        }

        [Test]
        public void IsEnabled_BaseUrlWhitespace_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "   ",
                WebhookEndpointPath = "/api/publish",
                BodyContentPropertyNames = [PublicationPropertyType.Id]
            };

            Assert.That(config.IsEnabled, Is.False);
        }

        [Test]
        public void IsEnabled_WebhookEndpointPathNull_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                WebhookEndpointPath = null,
                BodyContentPropertyNames = [PublicationPropertyType.Id]
            };

            Assert.That(config.IsEnabled, Is.False);
        }

        [Test]
        public void IsEnabled_WebhookEndpointPathWhitespace_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                WebhookEndpointPath = "  ",
                BodyContentPropertyNames = [PublicationPropertyType.Id]
            };

            Assert.That(config.IsEnabled, Is.False);
        }

        [Test]
        public void IsEnabled_BodyContentPropertyNamesNull_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                WebhookEndpointPath = "/api/publish",
                BodyContentPropertyNames = null
            };

            Assert.That(config.IsEnabled, Is.False);
        }

        [Test]
        public void IsEnabled_BodyContentPropertyNamesEmpty_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                WebhookEndpointPath = "/api/publish",
                BodyContentPropertyNames = []
            };

            Assert.That(config.IsEnabled, Is.False);
        }

        #endregion

        #region WebhookUrl

        [Test]
        public void WebhookUrl_ValidBaseAndPath_ReturnsCombinedUrl()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                WebhookEndpointPath = "/api/publish"
            };

            Assert.That(config.WebhookUrl, Is.EqualTo("https://example.com/api/publish"));
        }

        [Test]
        public void WebhookUrl_TrailingSlashOnBase_NormalizesCorrectly()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com/",
                WebhookEndpointPath = "/api/publish"
            };

            Assert.That(config.WebhookUrl, Is.EqualTo("https://example.com/api/publish"));
        }

        [Test]
        public void WebhookUrl_NoLeadingSlashOnPath_NormalizesCorrectly()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                WebhookEndpointPath = "api/publish"
            };

            Assert.That(config.WebhookUrl, Is.EqualTo("https://example.com/api/publish"));
        }

        [Test]
        public void WebhookUrl_WhitespaceInValues_TrimsAndCombines()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "  https://example.com  ",
                WebhookEndpointPath = "  /api/publish  "
            };

            Assert.That(config.WebhookUrl, Is.EqualTo("https://example.com/api/publish"));
        }

        [Test]
        public void WebhookUrl_BaseUrlNull_ReturnsNull()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = null,
                WebhookEndpointPath = "/api/publish"
            };

            Assert.That(config.WebhookUrl, Is.Null);
        }

        [Test]
        public void WebhookUrl_PathNull_ReturnsNull()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                WebhookEndpointPath = null
            };

            Assert.That(config.WebhookUrl, Is.Null);
        }

        [Test]
        public void WebhookUrl_MalformedBaseUrl_ReturnsNull()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "not-a-url",
                WebhookEndpointPath = "/api/publish"
            };

            Assert.That(config.WebhookUrl, Is.Null);
        }

        #endregion

        #region HealthCheckUrl

        [Test]
        public void HealthCheckUrl_ValidBaseAndPath_ReturnsCombinedUrl()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                HealthCheckEndpointPath = "/api/info"
            };

            Assert.That(config.HealthCheckUrl, Is.EqualTo("https://example.com/api/info"));
        }

        [Test]
        public void HealthCheckUrl_PathNull_ReturnsNull()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                HealthCheckEndpointPath = null
            };

            Assert.That(config.HealthCheckUrl, Is.Null);
        }

        [Test]
        public void HealthCheckUrl_PathWhitespace_ReturnsNull()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                HealthCheckEndpointPath = "   "
            };

            Assert.That(config.HealthCheckUrl, Is.Null);
        }

        #endregion

        #region HasHealthCheckEndpoint

        [Test]
        public void HasHealthCheckEndpoint_ValidPathAndBaseUrl_ReturnsTrue()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                HealthCheckEndpointPath = "/api/info"
            };

            Assert.That(config.HasHealthCheckEndpoint, Is.True);
        }

        [Test]
        public void HasHealthCheckEndpoint_PathNull_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                HealthCheckEndpointPath = null
            };

            Assert.That(config.HasHealthCheckEndpoint, Is.False);
        }

        [Test]
        public void HasHealthCheckEndpoint_PathEmpty_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                HealthCheckEndpointPath = ""
            };

            Assert.That(config.HasHealthCheckEndpoint, Is.False);
        }

        [Test]
        public void HasHealthCheckEndpoint_PathWhitespace_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com",
                HealthCheckEndpointPath = "   "
            };

            Assert.That(config.HasHealthCheckEndpoint, Is.False);
        }

        [Test]
        public void HasHealthCheckEndpoint_BaseUrlNull_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = null,
                HealthCheckEndpointPath = "/api/info"
            };

            Assert.That(config.HasHealthCheckEndpoint, Is.False);
        }

        [Test]
        public void HasHealthCheckEndpoint_MalformedBaseUrl_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "not-a-url",
                HealthCheckEndpointPath = "/api/info"
            };

            Assert.That(config.HasHealthCheckEndpoint, Is.False);
        }

        #endregion

        #region HasAccessToken

        [Test]
        public void HasAccessToken_BothSet_ReturnsTrue()
        {
            PublicationWebhookConfiguration config = new()
            {
                AccessTokenHeaderName = "Authorization",
                AccessTokenHeaderValue = "Bearer token"
            };

            Assert.That(config.HasAccessToken, Is.True);
        }

        [Test]
        public void HasAccessToken_NameNull_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                AccessTokenHeaderName = null,
                AccessTokenHeaderValue = "Bearer token"
            };

            Assert.That(config.HasAccessToken, Is.False);
        }

        [Test]
        public void HasAccessToken_ValueNull_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                AccessTokenHeaderName = "Authorization",
                AccessTokenHeaderValue = null
            };

            Assert.That(config.HasAccessToken, Is.False);
        }

        [Test]
        public void HasAccessToken_NameWhitespace_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                AccessTokenHeaderName = "   ",
                AccessTokenHeaderValue = "Bearer token"
            };

            Assert.That(config.HasAccessToken, Is.False);
        }

        [Test]
        public void HasAccessToken_ValueWhitespace_ReturnsFalse()
        {
            PublicationWebhookConfiguration config = new()
            {
                AccessTokenHeaderName = "Authorization",
                AccessTokenHeaderValue = "   "
            };

            Assert.That(config.HasAccessToken, Is.False);
        }

        #endregion

        #region CombineUrl edge cases

        [Test]
        public void WebhookUrl_BothWhitespace_ReturnsNull()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "   ",
                WebhookEndpointPath = "   "
            };

            Assert.That(config.WebhookUrl, Is.Null);
        }

        [Test]
        public void WebhookUrl_MultipleTrailingAndLeadingSlashes_NormalizesCorrectly()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com///",
                WebhookEndpointPath = "///api/publish"
            };

            Assert.That(config.WebhookUrl, Is.EqualTo("https://example.com/api/publish"));
        }

        [Test]
        public void WebhookUrl_BaseUrlWithPort_CombinesCorrectly()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com:8443",
                WebhookEndpointPath = "/api/publish"
            };

            Assert.That(config.WebhookUrl, Is.EqualTo("https://example.com:8443/api/publish"));
        }

        [Test]
        public void WebhookUrl_BaseUrlWithSubpath_CombinesCorrectly()
        {
            PublicationWebhookConfiguration config = new()
            {
                BaseUrl = "https://example.com/service",
                WebhookEndpointPath = "/api/publish"
            };

            Assert.That(config.WebhookUrl, Is.EqualTo("https://example.com/service/api/publish"));
        }

        #endregion

        #region Default configuration

        [Test]
        public void DefaultConfiguration_IsNotEnabled()
        {
            PublicationWebhookConfiguration config = new();

            Assert.That(config.IsEnabled, Is.False);
        }

        [Test]
        public void DefaultConfiguration_HasNoHealthCheckEndpoint()
        {
            PublicationWebhookConfiguration config = new();

            Assert.That(config.HasHealthCheckEndpoint, Is.False);
        }

        [Test]
        public void DefaultConfiguration_HasNoAccessToken()
        {
            PublicationWebhookConfiguration config = new();

            Assert.That(config.HasAccessToken, Is.False);
        }

        [Test]
        public void DefaultConfiguration_WebhookUrlIsNull()
        {
            PublicationWebhookConfiguration config = new();

            Assert.That(config.WebhookUrl, Is.Null);
        }

        [Test]
        public void DefaultConfiguration_HealthCheckUrlIsNull()
        {
            PublicationWebhookConfiguration config = new();

            Assert.That(config.HealthCheckUrl, Is.Null);
        }

        #endregion
    }
}
