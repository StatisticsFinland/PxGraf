using Microsoft.Extensions.Logging;
using Moq;
using Px.Utils.Models.Metadata;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Datasource.Cache;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class TestVisualizationControllerBuilder
    {
        public static VisualizationController BuildController
            (List<DimensionParameters> cubeParams,
            List<DimensionParameters> metaParams,
            VisualizationResponse mockResponse,
            string testQueryId, 
            Mock<ICachedDatasource> mockCachedDatasource,
            MultiStateMemoryTaskCache.CacheEntryState entryState,
            bool savedQueryFound = true,
            bool archived = false)
        {
            Mock<ISqFileInterface> sqFileInterface = new();
            Mock<IMultiStateMemoryTaskCache> taskCache = new();
            Mock<ILogger<VisualizationController>> logger = new();

            mockCachedDatasource.Setup(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMeta(metaParams));
            mockCachedDatasource.Setup(x => x.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams));
            mockCachedDatasource.Setup(x => x.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams));

            taskCache.Setup(x => x.TryGet(It.IsAny<string>(), out It.Ref<Task<VisualizationResponse>>.IsAny))
                .Returns((string key, out Task<VisualizationResponse> value) =>
                {
                    value = Task.FromResult(mockResponse);
                    return entryState;
                });

            sqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(savedQueryFound);
            sqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, archived, new LineChartVisualizationSettings(null, false, null)));
            sqFileInterface.Setup(x => x.ArchiveCubeExists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => true);
            sqFileInterface.Setup(x => x.ReadArchiveCubeFromFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestArchiveCube(metaParams));

            return new VisualizationController(sqFileInterface.Object, taskCache.Object, mockCachedDatasource.Object, logger.Object);
        }
    }
}
