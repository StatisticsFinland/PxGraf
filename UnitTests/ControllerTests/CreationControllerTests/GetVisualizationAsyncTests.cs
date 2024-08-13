using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PxGraf.Language;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace CreationControllerTests
{
    internal class GetVisualizationAsyncTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            Dictionary<string, string> inMemorySettings = new()
            {
                {"pxwebUrl", "http://pxwebtesturl:12345/"},
                {"pxgrafUrl", "http://pxgraftesturl:8443/PxGraf"},
                {"savedQueryDirectory", "goesNowhere"},
                {"archiveFileDirectory", "goesNowhere"},
                {"LocalFilesystemDatabaseConfig:Encoding", "latin1"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task GetVisualizationTest_Fresh_Data_Is_Returned()
        {
            // TODO: write test
        }

        [Test]
        public async Task GetVisualizationTest_Volume_0_Cube_Returns_BadRequest()
        {
            // TODO: write test
        }

        [Test]
        public async Task GetVisualizationTest_Valid_VisualizationType_DoesNotReturn_BadRequest()
        {
            // TODO: write test
        }

        [Test]
        public async Task GetVisualizationTest_Invalid_VisualizationType_Returns_BadRequest()
        {
            // TODO: write test
        }
    }
}
