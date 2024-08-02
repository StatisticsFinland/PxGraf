using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection
{
    public interface IChartTypeSelector
    {
        IReadOnlyDictionary<VisualizationType, IReadOnlyList<ChartRejectionInfo>> GetRejectionReasons(MatrixQuery query, Matrix<DecimalDataValue> matrix);
        IReadOnlyList<VisualizationType> GetValidChartTypes(MatrixQuery query, Matrix<DecimalDataValue> matrix);
    }
}