using NUnit.Framework;
using PxGraf.Data;
using System.Collections.Generic;
using System.Linq;

namespace DataCubeTests
{
    class CubeMapTests
    {
        private static CubeMap BuildTestMap(int var = 3, int val = 5)
        {
            List<VariableMap> testVars = new();
            for (int varInd = 1; varInd <= var; varInd++)
            {
                List<string> valueCodes = new();
                for (int valInd = 1; valInd <= val; valInd++)
                {
                    valueCodes.Add("val" + valInd);
                }
                testVars.Add(new VariableMap("var" + varInd, valueCodes));
            }
            return new CubeMap(testVars);
        }

        [Test]
        public void GetCoordinatesTestLastCoord_Pass()
        {
            var testMap = BuildTestMap();
            Dictionary<string, string> lastCoord = testMap.GetCoordinates().Last();

            Dictionary<string, string> expected = new()
            {
                ["var1"] = "val5",
                ["var2"] = "val5",
                ["var3"] = "val5"
            };

            Assert.AreEqual(expected, lastCoord);
        }

        [Test]
        public void GetCoordinatesTestFirstCoord_Pass()
        {
            var testMap = BuildTestMap();
            Dictionary<string, string> lastCoord = testMap.GetCoordinates().First();

            Dictionary<string, string> expected = new()
            {
                ["var1"] = "val1",
                ["var2"] = "val1",
                ["var3"] = "val1"
            };

            Assert.AreEqual(expected, lastCoord);
        }

        [Test]
        public void GetCoordinatesTestVarVals_Pass()
        {
            var testMap = BuildTestMap(2, 3);
            string[][] coords = new string[][]
            {
                testMap.GetCoordinates().Select(c => c["var1"]).ToArray(),
                testMap.GetCoordinates().Select(c => c["var2"]).ToArray()
            };

            string[][] expected = new string[][]
            {
                new string[] { "val1", "val1", "val1", "val2", "val2", "val2", "val3", "val3", "val3" },
                new string[] { "val1", "val2", "val3", "val1", "val2", "val3", "val1", "val2", "val3" }
            };

            Assert.AreEqual(expected, coords);
        }

        [Test]
        public void GetIndexTestFirst_0()
        {
            var testMap = BuildTestMap(2, 2);
            var testCoord = new CubeMap.Coordinate
            {
                ["var1"] = "val1",
                ["var2"] = "val1"
            };

            Assert.AreEqual(0, testMap.GetCoordinateIndex(testCoord));
        }

        [Test]
        public void GetIndexTestMass_Pass()
        {
            var testMap = BuildTestMap(5, 6);
            int i = 0;
            foreach (var coord in testMap.GetCoordinates())
            {
                Assert.AreEqual(i, testMap.GetCoordinateIndex(coord));
                i++;
            }
        }

        [Test]
        public void DataSizeMatchCoordsLen_Pass()
        {
            var testMap = BuildTestMap(5, 4);
            int numOfCoords = testMap.GetCoordinates().Count();
            Assert.AreEqual(testMap.DataMapSize, numOfCoords);
        }
    }
}
