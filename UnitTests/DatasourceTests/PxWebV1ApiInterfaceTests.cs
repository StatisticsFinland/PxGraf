using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Datasource.PxWebInterface;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Settings;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.DatasourceTests
{
    internal class PxWebV1ApiInterfaceTests
    {
        private IConfiguration _configuration;

        [OneTimeSetUp]
        public void DoSetup()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(_configuration);

            Configuration.Current.LanguageOptions.Available = ["aa", "bb", "cc"];
            Configuration.Current.LanguageOptions.Default = "bb";
        }

        [Test]
        public async Task GetGroupContentAsyncTestListDatabases()
        {
            Mock<IPxWebConnection> mockConnection = new();
            Mock<ILogger<PxWebV1ApiInterface>> mockLogger = new();

            HttpResponseMessage mockAaResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"dbid\":\"FooBar\", \"text\":\"FooBarAa\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/aa/", "")).ReturnsAsync(mockAaResponse);
            
            HttpResponseMessage mockBbResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"dbid\":\"FooBar\", \"text\":\"FooBarBb\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/bb/", "")).ReturnsAsync(mockBbResponse);

            HttpResponseMessage mockCcResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"dbid\":\"FooBar\", \"text\":\"FooBarCc\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/cc/", "")).ReturnsAsync(mockCcResponse);
            
            PxWebV1ApiInterface objectUnderTest = new(mockConnection.Object, mockLogger.Object);
            DatabaseGroupContents dbContent = await objectUnderTest.GetDatabaseItemGroup([]);
            
            Assert.That(dbContent.Headers.Count, Is.EqualTo(1));
            Assert.That(dbContent.Files.Count, Is.EqualTo(0));
            Assert.That(dbContent.Headers[0].Name["aa"], Is.EqualTo("FooBarAa"));
            Assert.That(dbContent.Headers[0].Name["bb"], Is.EqualTo("FooBarBb"));
            Assert.That(dbContent.Headers[0].Name["cc"], Is.EqualTo("FooBarCc"));
        }
    }
}
