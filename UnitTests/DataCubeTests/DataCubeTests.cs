using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using System.Collections.Generic;
using System.Linq;

namespace DataCubeTests
{
    class DataCubeTests
    {
        private static CubeMeta BuildTestCubeMeta(params int[] variabliSizes)
        {
            List<Variable> testVariables = new();
            for (int varInd = 0; varInd < variabliSizes.Length; varInd++)
            {
                string varCode = "var" + varInd;
                List<VariableValue> variableValues = new();
                for (int valInd = 0; valInd < variabliSizes[varInd]; valInd++)
                {
                    string valCode = "val" + valInd;
                    variableValues.Add(new VariableValue(valCode, new MultiLanguageString("fi", valCode), null, false));
                }

                Variable testVar = new(varCode, new MultiLanguageString("fi", varCode), null, VariableType.Unknown, variableValues);
                testVariables.Add(testVar);
            }

            return new CubeMeta(new List<string> { "fi" }, new MultiLanguageString("fi", "testHeader"), null, testVariables);
        }

        private static DataValue[] BuildTestData(params int[] variabliSizes)
        {
            List<DataValue> testData = new();
            double td = 0.0;
            int size = 1;
            foreach (int numOfValues in variabliSizes)
            {
                size *= numOfValues;
            }

            for (int i = 0; i < size; i++)
            {
                testData.Add(DataValue.FromRaw(td));
                td += 1.0;
            }

            return testData.ToArray();
        }

        private static double[] FromDataValues(IReadOnlyList<DataValue> input)
        {
            double[] result = new double[input.Count];
            for (int i = 0; i < input.Count; i++)
            {
                result[i] = input[i].GetOrDefault(double.MinValue);
            }
            return result;
        }

        [Test]
        public void DataCubeBasicConstructor_Pass()
        {
            int[] varSizes = new[] { 2, 1, 2, 3 };
            var cube = new DataCube(BuildTestCubeMeta(varSizes), BuildTestData(varSizes));
            double[] expected = new double[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 };
            Assert.AreEqual(expected, FromDataValues(cube.Data));
        }

        [Test]
        public void DataCubeBasicPick_Pass()
        {
            int[] varSizes = new[] { 2, 1, 2, 3 };
            var cube = new DataCube(BuildTestCubeMeta(varSizes), BuildTestData(varSizes));
            var transform = cube.GetTransform(BuildTestCubeMeta(2, 1, 2, 1).BuildMap());
            double[] expected = new double[] { 0.0, 3.0, 6.0, 9.0 };
            Assert.AreEqual(expected, FromDataValues(transform.Data));
        }

        [Test]
        public void DataCubeBasicPick2_Pass()
        {
            int[] varSizes = new[] { 2, 1, 2, 3 };
            var cube = new DataCube(BuildTestCubeMeta(varSizes), BuildTestData(varSizes));
            var pick = cube.GetTransform(BuildTestCubeMeta(2, 1, 1, 3).BuildMap());
            double[] expected = new double[] { 0.0, 1.0, 2.0, 6.0, 7.0, 8.0 };
            Assert.AreEqual(expected, FromDataValues(pick.Data));
        }

        [Test]
        public void DataCubeMutationPickTest1_Pass()
        {
            int[] varSizes = new[] { 2, 1, 2, 3 };
            var cube = new DataCube(BuildTestCubeMeta(varSizes), BuildTestData(varSizes));

            var mutatorMap = cube.Meta.Variables.Select(v => v.BuildVariableMap()).ToList();

            (mutatorMap[2], mutatorMap[0]) = (mutatorMap[0], mutatorMap[2]);
            var map = new CubeMap(mutatorMap);
            var transform = cube.GetTransform(map);
            double[] expected = new double[] { 0.0, 1.0, 2.0, 6.0, 7.0, 8.0, 3.0, 4.0, 5.0, 9.0, 10.0, 11.0 };
            var actual = FromDataValues(transform.Data);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataCubeMutationPickTest2_Pass()
        {
            // var0 var1 var2 var3 -> var3 var1 var0 var2

            int[] varSizes = new[] { 2, 1, 2, 3 };
            var cube = new DataCube(BuildTestCubeMeta(varSizes), BuildTestData(varSizes));

            var mutatorMaps = cube.Meta.Variables.Select(v => v.BuildVariableMap()).ToList();

            var tmp = mutatorMaps[0];
            mutatorMaps[0] = mutatorMaps[3];
            mutatorMaps[3] = mutatorMaps[2];
            mutatorMaps[2] = tmp;
            var map = new CubeMap(mutatorMaps);

            var transform = cube.GetTransform(map);
            double[] expected = new double[] { 0.0, 3.0, 6.0, 9.0, 1.0, 4.0, 7.0, 10.0, 2.0, 5.0, 8.0, 11.0 };
            var actual = FromDataValues(transform.Data);
            Assert.AreEqual(expected, actual);
        }
    }
}
