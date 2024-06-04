using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Controllers;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.PxWebInterface;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace ControllerTests
{
    internal class SaveQueryAsyncTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task ValidSaveRequestReturnsSaveQueryResponseAndCallsSerializeToFile()
        {
            Mock<ICachedPxWebConnection> mockCachedPxWebConnection = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7),
            ];

            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            ];

            mockCachedPxWebConnection.Setup(c => c.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)TestDataCubeBuilder.BuildTestMeta(metaParams)));

            mockCachedPxWebConnection.Setup(c => c.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<CubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            SaveQueryParams testInput = new()
            {
                Query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams),
                Settings = new VisualizationCreationSettings()
                {
                    SelectedVisualization = VisualizationType.LineChart,
                    RowVariableCodes = ["variable-2"],
                    ColumnVariableCodes = ["variable-1"],
                    MultiselectableVariableCode = null
                }
            };

            SqController testController = new(mockCachedPxWebConnection.Object, mockSqFileInterface.Object, mockLogger.Object);
            ActionResult<SaveQueryResponse> actionResult = await testController.SaveQueryAsync(testInput);
            Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryResponse>());
            mockSqFileInterface.Verify(
                s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()), Times.Once);
        }
    }
}
