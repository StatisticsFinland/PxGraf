using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Internal;
using PxGraf.Language;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace ControllerTests
{
    internal class GetQueryMetaTests
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
                {"archiveFileDirectory", "goesNowhere"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task GetQueryMetaTest_ReturnValidMeta()
        {
            // TODO: write test
            /*
            Assert.That(result.Value.Header["fi"], Is.EqualTo("value-0, value-0 2000-2009 muuttujana variable-2"));
            Assert.That(result.Value.HeaderWithPlaceholders["fi"], Is.EqualTo("value-0, value-0 [FIRST]-[LAST] muuttujana variable-2"));
            Assert.That(result.Value.Archived, Is.False);
            Assert.That(result.Value.Selectable, Is.False);
            Assert.That(result.Value.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
            Assert.That(result.Value.TableId, Is.EqualTo("TestPxFile.px"));
            Assert.That(result.Value.Description["fi"], Is.EqualTo("Test note"));
            Assert.That(result.Value.LastUpdated, Is.EqualTo("2009-09-01T00:00:00.000Z"));
            Assert.That(result.Value.TableReference.Name, Is.EqualTo("TestPxFile.px"));

            List<string> expectedHierarchy = ["testpath", "to", "test", "file"];
            Assert.That(result.Value.TableReference.Hierarchy, Is.EqualTo(expectedHierarchy));
            */
        }

        [Test]
        public async Task GetQueryMetaTest_ReturnSelectableTrue()
        {
            // TODO: write test
        }

        [Test]
        public async Task GetQueryMetaTest_NotFound()
        {
            // TODO: write test
        }

        [Test]
        public async Task GetQueryMetaTest_ArchivedQuery()
        {
            // TODO: write test
            /*
            Assert.That(result.Value.Header["fi"], Is.EqualTo("value-0, value-0 2000-2009 muuttujana variable-2"));
            Assert.That(result.Value.HeaderWithPlaceholders["fi"], Is.EqualTo("value-0, value-0 [FIRST]-[LAST] muuttujana variable-2"));
            Assert.That(result.Value.Archived, Is.True);
            Assert.That(result.Value.Selectable, Is.False);
            Assert.That(result.Value.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
            Assert.That(result.Value.TableId, Is.EqualTo("TestPxFile.px"));
            Assert.That(result.Value.Description["fi"], Is.EqualTo("Test note"));
            Assert.That(result.Value.LastUpdated, Is.EqualTo("2009-09-01T00:00:00.000Z"));
            Assert.That(result.Value.TableReference.Name, Is.EqualTo("TestPxFile.px"));

            List<string> expectedHierarchy = ["testpath", "to", "test", "file"];
            Assert.That(result.Value.TableReference.Hierarchy, Is.EqualTo(expectedHierarchy));
            */
        }

        [Test]
        public void GetQueryMetaTest_Table_Not_Found()
        {
            // TODO: write test
        }

        [Test]
        public void GetQueryMetaTest_ArchiveFileNotFound()
        {
            // TODO: write test
        }
    }
}
