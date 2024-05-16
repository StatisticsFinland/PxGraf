using PxGraf.Data.MetaData;

namespace PxGraf.Data
{
    public class DataCube
    {
        /// <summary>
        /// Metadata about this cube. The data part is interpreted using the variable information in this meta object.
        /// </summary>
        public IReadOnlyCubeMeta Meta { get; }

        /// <summary>
        /// The first index matches the first value of each variable example v1.vals[0], v2.vals[0] v3.vals[0] assuming the meta object has 3 variables.
        /// </summary>
        public DataValue[] Data { get; }

        /// <summary>
        /// Default constructor, the provided data and metadata must match.
        /// </summary>
        public DataCube(IReadOnlyCubeMeta meta, DataValue[] data)
        {
            Meta = meta;
            Data = data;
        }

        public DataCube GetTransform(CubeMap map)
        {
            var newData = new DataValue[map.DataMapSize];
            var sourceMap = Meta.BuildMap();
            var destinationMeta = Meta.GetTransform(map);

            foreach ((int index, CubeMap.Coordinate coord) in destinationMeta.BuildMap().GetIndexCoordinatePair())
            {
                newData[index] = Data[sourceMap.GetCoordinateIndex(coord)];
            }
            return new DataCube(destinationMeta, newData);
        }
    }
}
