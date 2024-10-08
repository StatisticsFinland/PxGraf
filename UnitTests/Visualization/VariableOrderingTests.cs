﻿using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Visualization;
using System.Collections.Generic;
using System.Linq;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace Visualization
{
    internal static class VariableOrderingTests
    {
        // This is to make sure that the data order is the same in the two tests below.
        private readonly static IReadOnlyList<double?> expectedData = [
                0.123, 1.123, 2.123, 9.123, 10.123, 11.123, 18.123, 19.123, 20.123,
                3.123, 4.123, 5.123, 12.123, 13.123, 14.123, 21.123, 22.123, 23.123,
                6.123, 7.123, 8.123, 15.123, 16.123, 17.123, 24.123, 25.123, 26.123
            ];

        [Test]
        public static void BuildVisualizationResponseTest_TableOrdering_1_Selectable_ExplicitLayout_For_All()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.Time, 3),
            ];

            TableVisualizationSettings testSettings = new( new Layout(["variable-0", "variable-1"],["variable-2", "variable-3"]));

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, testSettings);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, testSettings);

            string[] expected = ["variable-2", "variable-0", "variable-1", "variable-3"];
            Assert.That(result.MetaData.Select(v => v.Code), Is.EqualTo(expected));
            Assert.That(result.Data, Is.EqualTo(expectedData));
        }

        [Test]
        public static void BuildVisualizationResponseTest_TableOrdering_1_Selectable_No_ExplicitLayout_For_All()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.Time, 3),
            ];

            TableVisualizationSettings testSettings = new(new Layout(["variable-1"], ["variable-3"]));

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, testSettings);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, testSettings);

            string[] expected = ["variable-0", "variable-2", "variable-1", "variable-3"];
            Assert.That(result.MetaData.Select(v => v.Code), Is.EqualTo(expected));
            Assert.That(result.Data, Is.EqualTo(expectedData));
        }

        [Test]
        public static void BuildVisualizationResponseTest_TableOrdering_2_Selectables_ExplicitLayout_For_All()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.Time, 4),
                new VariableParameters(VariableType.Ordinal, 5) { Selectable = true },

            ];

            TableVisualizationSettings testSettings = new(
                new Layout(
                    ["variable-1", "variable-0", "variable-4"],
                    ["variable-2", "variable-3"]
                ));

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, testSettings);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, testSettings);

            string[] expected = ["variable-2", "variable-4", "variable-1", "variable-0", "variable-3"];
            Assert.That(result.MetaData.Select(v => v.Code), Is.EqualTo(expected));
            Assert.That(result.Data.Count, Is.EqualTo(1 * 3 * 3 * 4 * 5));
        }
    }
}
