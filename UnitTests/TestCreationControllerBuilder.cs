#nullable enable
using Microsoft.Extensions.Logging;
using Moq;
using Px.Utils.Models.Metadata;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Services;
using System.Collections.Generic;

namespace UnitTests
{
    public static class TestCreationControllerBuilder
    {
        public static CreationController BuildController(List<DimensionParameters> cubeParams, List<DimensionParameters> metaParams, DatabaseGroupContents? mockContents = null)
        {
            Mock<ICachedDatasource> dataSource = new();
            Mock<ILogger<CreationController>> logger = new();
            Mock<IAuditLogService> auditLogService = new();

            dataSource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync((PxTableReference tableReference) =>
                {
                    return TestDataCubeBuilder.BuildTestMeta(metaParams);
                });

            dataSource.Setup(ds => ds.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<MatrixMetadata>()))
                .ReturnsAsync((PxTableReference tableReference, MatrixMetadata metadata) =>
                {
                    return TestDataCubeBuilder.BuildTestMatrix(cubeParams);
                });

            dataSource.Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<string[]>()))
                .ReturnsAsync((string[] hierarchy) =>
                {
                    return mockContents;
                });

            return new CreationController(dataSource.Object, logger.Object, auditLogService.Object);
        }
    }
}
