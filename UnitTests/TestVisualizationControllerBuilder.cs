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
            MultiStateMemoryTaskCache.CacheEntryState entryState)
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
                .Returns(true);
            sqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null)));

            return new VisualizationController(sqFileInterface.Object, taskCache.Object, mockCachedDatasource.Object, logger.Object);
        }
    }
}
