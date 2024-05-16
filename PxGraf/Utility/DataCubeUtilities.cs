using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Language;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Utility
{
    /// <summary>
    /// Collection of static utility functions for DataCube manipulation.
    /// </summary>
    public static class DataCubeUtilities
    {
        public class DataAndNotesCollection
        {
            public double?[] Data { get; set; }
            public IReadOnlyDictionary<int, IReadOnlyMultiLanguageString> Notes { get; set; }

            public IReadOnlyDictionary<int, int> MissingValueInfo { get; set; }
        }

        /// <summary>
        /// Returns a subset of the cube where the values that are not selected have been removed from the selectable variables.
        /// </summary>
        public static DataCube ApplySelectableVariableSelections(DataCube dataCube, IReadOnlyDictionary<string, IReadOnlyList<string>> selections)
        {
            List<VariableMap> transformMap = new();
            foreach (var varMap in dataCube.Meta.BuildMap())
            {
                if (selections.ContainsKey(varMap.Code))
                {
                    var varSelections = selections[varMap.Code];
                    transformMap.Add(new VariableMap(varMap.Code, varSelections));
                }
                else
                {
                    transformMap.Add(varMap);
                }
            }

            return dataCube.GetTransform(new CubeMap(transformMap));
        }

        /// <summary>
        /// Calculates the sum of all data valus in the cube.
        /// If the cube contais missing data values, they are treated as a 0.
        /// </summary>
        public static double CalculateCubeSum(this DataCube cube)
        {
            double tempSum = 0;
            foreach (var value in cube.Data)
            {
                if (value.TryGetValue(out double maybeVal)) tempSum += maybeVal;
            }
            return tempSum;
        }

        public static DataAndNotesCollection ExtractDataAndNotes(this DataCube cube)
        {
            double?[] data = new double?[cube.Data.Length];
            // This will be empty as long as pxweb api doesn't return notes
            Dictionary<int, IReadOnlyMultiLanguageString> notes = new();
            Dictionary<int, int> missingValueInfo = new();
            IReadOnlyList<DataValue> inputData = cube.Data;

            int dataLength = cube.Data.Length;

            for (int i = 0; i < dataLength; i++)
            {
                if (inputData[i].Type == DataValueType.Exist)
                {
                    data[i] = inputData[i].UnsafeValue;
                }
                else
                {
                    data[i] = null;
                    missingValueInfo[i] = (int)inputData[i].Type;
                }
            }

            return new DataAndNotesCollection()
            {
                Data = data,
                Notes = notes,
                MissingValueInfo = missingValueInfo
            };
        }

        /// <summary>
        /// One variable from the map is reduced to have only one value.
        /// </summary>
        public static CubeMap CollapseOneVariableInMap(this CubeMap map, IReadOnlyVariableValue keepThisValue, IReadOnlyVariable fromThisVariable)
        {
            var varMapList = map.ToList();
            var variableIndex = varMapList.FindIndex(vs => vs.Code == fromThisVariable.Code);
            varMapList[variableIndex] = new VariableMap(varMapList[variableIndex].Code, new List<string> { keepThisValue.Code });

            return new CubeMap(varMapList);
        }
    }
}
