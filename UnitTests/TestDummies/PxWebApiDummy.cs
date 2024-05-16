using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface;
using PxGraf.PxWebInterface.SerializationModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.TestDummies.DummyQueries;

namespace UnitTests.TestDummies
{
    internal class PxWebApiDummy : IPxWebApiInterface
    {
        private List<VariableParameters> CubeParams { get; }
        private List<VariableParameters> MetaParams { get; }
        private bool UseNegativeData { get; set; }

        public PxWebApiDummy(List<VariableParameters> cubeParams, List<VariableParameters> metaParams, bool useNegativeData = false)
        {
            CubeParams = cubeParams;
            MetaParams = metaParams;
            UseNegativeData = useNegativeData;
        }

        public Task<DataCube> GetPxTableDataAsync(PxFileReference tableReference, IReadOnlyCubeMeta table)
        {
            Task<DataCube> task = new(() => TestDataCubeBuilder.BuildTestDataCube(CubeParams, UseNegativeData));
            task.Start();
            return task;
        }

        public Task<IReadOnlyCubeMeta> GetPxTableMetaAsync(PxFileReference tableReference, IEnumerable<string> languages = null)
        {
            Task<IReadOnlyCubeMeta> task = new(() => TestDataCubeBuilder.BuildTestMeta(MetaParams));
            task.Start();
            return task;
        }

        public Task<List<DataBaseListResponseItem>> GetDataBaseListingAsync(string lang)
        {
            Task<List<DataBaseListResponseItem>> task = new(() => new List<DataBaseListResponseItem> { new () });
            task.Start();
            return task;
        }

        public Task<List<TableListResponseItem>> GetTableItemListingAsync(string lang, IReadOnlyList<string> path)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetPxFileUpdateTimeAsync(PxFileReference pxFile, IReadOnlyCubeMeta referenceMeta)
        {
            Task<string> task = new (DateTime.UtcNow.ToString);
            task.Start();
            return task;
        }
    }
}
