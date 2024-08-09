using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Data.MetaData;
using PxGraf.Datasource;
using PxGraf.Datasource.PxWebInterface;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests;
using UnitTests.Fixtures;

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
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

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

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object);
            ActionResult<SaveQueryResponse> actionResult = await testController.SaveQueryAsync(testInput);
            Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryResponse>());
            mockSqFileInterface.Verify(
                s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()), Times.Once);
        }
    }
}
