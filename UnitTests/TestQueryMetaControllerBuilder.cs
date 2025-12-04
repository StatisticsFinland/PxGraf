#nullable enable
using Microsoft.Extensions.Logging;
using Moq;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using PxGraf.Services;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class TestQueryMetaControllerBuilder
    {
        public static QueryMetaController BuildController(Dictionary<string, SavedQuery> savedQueries, string archiveRoot, List<DimensionParameters> dimParams, string[]? languages = null, Dictionary<string, ArchiveCube>? archiveCubes = null)
        {
            Mock<ISqFileInterface> sqFileInterface = new();
            Mock<ICachedDatasource> cachedDatasource = new();
            Mock<ILogger<QueryMetaController>> logger = new();
            Mock<IAuditLogService> auditLogService = new();

            sqFileInterface.Setup(fi => fi.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string sq, string archivePath) =>
                {
                    return savedQueries.ContainsKey($"{archiveRoot}/{sq}");
                });

            sqFileInterface.Setup(fi => fi.ReadSavedQueryFromFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string sq, string archivePath) =>
                {
                    return Task.FromResult(savedQueries[$"{archiveRoot}/{sq}"]);
                });

            cachedDatasource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync((PxTableReference tableReference) =>
                {
                    if (dimParams.Count == 0) return null;
                    return TestDataCubeBuilder.BuildTestMeta(dimParams, languages);
                });

            sqFileInterface.Setup(fi => fi.ArchiveCubeExists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string id, string archivePath) =>
                {
                    if (archiveCubes == null) return false;
                    return archiveCubes.ContainsKey($"{archiveRoot}/{id}");
                });

            sqFileInterface.Setup(fi => fi.ReadArchiveCubeFromFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string id, string archivePath) =>
                {
                    return Task.FromResult(archiveCubes?[$"{archiveRoot}/{id}"]);
                });

            return new QueryMetaController(sqFileInterface.Object, cachedDatasource.Object, logger.Object, auditLogService.Object);
        }
    }
}
