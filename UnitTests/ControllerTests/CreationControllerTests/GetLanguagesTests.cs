using Moq;
using PxGraf.Controllers;
using PxGraf.Settings;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using PxGraf.PxWebInterface;
using UnitTests.TestDummies;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using PxGraf.PxWebInterface.Caching;
using Microsoft.Extensions.Configuration;
using UnitTests.Fixtures;
using System.Threading.Tasks;

namespace CreationControllerTests
{
    internal static class GetLanguagesTests
    {
        [Test]
        public static async Task GetLanguages_ReturnsCorrectLanguages()
        {
            // Arrange
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);

            PxWebApiDummy pxWebApiDummy = new([], []);
            ServiceCollection services = new();
            services.AddMemoryCache();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            IMemoryCache memoryCache = serviceProvider.GetService<IMemoryCache>();
            IPxWebApiResponseCache apiCache = new PxWebApiResponseCache(memoryCache);
            CreationController controller = new(new CachedPxWebConnection(pxWebApiDummy, apiCache), new Mock<ILogger<CreationController>>().Object);

            Configuration.Current.LanguageOptions.Available = ["foo", "bar"];

            // Act
            List<string> result = await controller.GetLanguagesAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Does.Contain("foo"));
            Assert.That(result, Does.Contain("bar"));
        }
    }
}
