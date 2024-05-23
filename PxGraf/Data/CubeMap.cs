using PxGraf.Data.MetaData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Data
{
    /// <summary>
    /// Immutable class representing ordered variable and variable value information.
    /// </summary>
    public class CubeMap : IReadOnlyList<VariableMap>
    {
        /// <summary>
        /// Lists ALL variable values that define one individual data point.
        /// </summary>
        public class Coordinate : Dictionary<string, string>
        {
            public Coordinate() { }

            private Coordinate(int capacity) : base(capacity) { }

            /// <summary>
            /// A and be should not contains same dimensions, this will cause a crash.
            /// This method is used for building 2d arrays based on row and column maps.
            /// </summary>
            /// <returns>A new coordinate that contains all the dimensions from a and b.</returns>
            public static Coordinate Combined(Coordinate a, Coordinate b)
            {
                var coord = new Coordinate(a.Count + b.Count);

                foreach (var keyValuePair in a)
                {
                    coord.Add(keyValuePair.Key, keyValuePair.Value);
                }

                foreach (var keyValuePair in b)
                {
                    coord.Add(keyValuePair.Key, keyValuePair.Value);
                }

                return coord;
            }
        }

        public int DataMapSize => _dataSize;

        private readonly IReadOnlyList<VariableMap> _variableMap;

        #region Cache

        private readonly int[] _rcsp;
        private readonly int _dataSize;

        #endregion


        /// <summary>
        /// Constructor for the cube map.
        /// </summary>
        /// <param name="variableMap">List of variable maps that define the cube map.</param>
        public CubeMap(IReadOnlyList<VariableMap> variableMap)
        {
            _variableMap = variableMap;
            (_dataSize, _rcsp) = GetRCSP(variableMap);
        }

        #region IReadOnlyList implementation

        public int Count => _variableMap.Count;

        public VariableMap this[int index] => _variableMap[index];

        public IEnumerator<VariableMap> GetEnumerator()
        {
            return _variableMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_variableMap).GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Returns all the coordinates that can be created from the variable value combinations.
        /// </summary>
        /// <returns>An IEnumerable of Coordinates.</returns>
        public IEnumerable<Coordinate> GetCoordinates()
        {
            for (var dataIndex = 0; dataIndex < _dataSize; dataIndex++)
            {
                yield return GetCoordinate(dataIndex);
            }
        }

        /// <summary>
        /// Returns all the index-coordinate pairs that can be created from the variable value combinations.
        /// </summary>
        /// <returns>An IEnumerable of index-coordinate pairs.</returns>
        public IEnumerable<(int dataIndex, Coordinate coordinate)> GetIndexCoordinatePair()
        {
            for (var dataIndex = 0; dataIndex < _dataSize; dataIndex++)
            {
                yield return (dataIndex, GetCoordinate(dataIndex));
            }
        }

        /// <summary>
        /// Returns the coordinate the matches the given data point index.
        /// </summary>
        /// <param name="dataIndex">The index of the data point.</param>
        /// <returns>The coordinate that matches the given data point index.</returns>
        public Coordinate GetCoordinate(int dataIndex)
        {
            Coordinate coordinate = [];
            int size = this.Count;
            for (var variableIndex = 0; variableIndex < size; variableIndex++)
            {
                VariableMap variable = this[variableIndex];
                int variableValueIndex = (dataIndex / _rcsp[variableIndex]) % this[variableIndex].ValueCodes.Count;
                coordinate[variable.Code] = variable.ValueCodes[variableValueIndex];
            }

            return coordinate;
        }

        /// <summary>
        /// When all of the variablevalue combinations are written out as a list (GetCoordinates()), this method returns the index (ordinal)
        /// matching the given coordinate.
        /// Example: Varibale 1: "Area" Values: "Helsinki, Turku" Variable 2: "Colour" Values: "Blue", "Cyan".
        /// List of combinations: "Helsinki Blue", "Helsinki Cyan", "Turku Blue", "Turku Cyan"
        /// With parameter "Helsinki Cyan" this method would return 1.
        /// </summary>
        /// <param name="coordinate">Coordinate to search an index for.</param>
        /// <returns>Index of the data point in the given coordinate.</returns>
        public int GetCoordinateIndex(Coordinate coordinate)
        {
            var dataIndex = 0;
            for (var variableIndex = 0; variableIndex < Count; variableIndex++)
            {
                var valueCode = coordinate[this[variableIndex].Code];
                IReadOnlyList<string> valCodes = this[variableIndex].ValueCodes;
                int count = valCodes.Count;
                int valIndex = -1;
                for (int i = 0; i < count; i++)
                {
                    if (valCodes[i].Equals(valueCode))
                    {
                        valIndex = i;
                        break;
                    }
                }
                if (valIndex < 0) throw new KeyNotFoundException();
                dataIndex += _rcsp[variableIndex] * valIndex;
            }
            return dataIndex;
        }

        /// <summary>
        /// Creates a copy of the cube map and replaces the variable map matching the variable code.
        /// </summary>
        /// <param name="newVarMap">The new variable map to replace the old one.</param>
        /// <returns>A new cube map with the variable map matching the variable code replaced with the new variable map.</returns>
        public CubeMap CloneWithEditedVariable(VariableMap newVarMap)
        {
            var varMapListCopy = this.ToList();
            varMapListCopy[varMapListCopy.FindIndex(v => v.Code == newVarMap.Code)] = newVarMap;
            return new CubeMap(varMapListCopy);
        }

        /// <summary>
        /// Creates a copy of the cube map and replaces the variable map matching the variable code from the variable value list
        /// with a new variable map constructed from the variable value list.
        /// </summary>
        /// <param name="variableCode">The variable code of the variable map to replace.</param>
        /// <param name="newVarValues">The new variable values to replace the old ones.</param>
        /// <returns>A new cube map with the variable map matching the variable code replaced with the new variable map.</returns>
        public CubeMap CloneWithEditedVariable(string variableCode, IReadOnlyList<IReadOnlyVariableValue> newVarValues)
        {
            var newVarMap = new VariableMap(variableCode, newVarValues.Select(vv => vv.Code).ToList());
            return CloneWithEditedVariable(newVarMap);
        }

        /// <summary>
        /// Reverse Cumulative Size Products
        /// [0] product of sizes of variables [1] - [N],
        /// [1] product of sizes of variables [2] - [N],
        /// ...
        /// [k] product of sizes of variables [k - 1] - [N]
        /// </summary>
        private static (int dataSize, int[] products) GetRCSP(IReadOnlyList<VariableMap> variables)
        {
            var cnt = new int[variables.Count];
            var cumulativeMultiplier = 1;
            for (var i = variables.Count - 1; i >= 0; i--)
            {
                cnt[i] = cumulativeMultiplier;
                cumulativeMultiplier *= variables[i].ValueCodes.Count;
            }
            return (cumulativeMultiplier, cnt);
        }
    }
}
