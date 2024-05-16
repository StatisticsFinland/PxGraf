using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection
{
    public interface IChartTypeSelector
    {
        IReadOnlyDictionary<VisualizationType, IReadOnlyList<ChartRejectionInfo>> GetRejectionReasons(CubeQuery query, DataCube cube);
        IReadOnlyList<VisualizationType> GetValidChartTypes(CubeQuery query, DataCube cube);
        IReadOnlyList<VisualizationType> GetValidChartTypes(CubeQuery query, ArchiveCube cube);
        IReadOnlyList<VisualizationType> GetValidChartTypes(DataCube cube);
    }
}