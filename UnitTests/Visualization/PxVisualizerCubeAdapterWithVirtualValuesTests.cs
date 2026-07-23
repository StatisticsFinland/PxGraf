#nullable enable
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Visualization;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Fixtures;

namespace UnitTests.Visualization
{
    [TestFixture]
    internal class PxVisualizerCubeAdapterWithVirtualValuesTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        private static readonly List<DimensionParameters> VarParams =
        [
            new DimensionParameters(DimensionType.Content, 1),
            new DimensionParameters(DimensionType.Other, 2),
            new DimensionParameters(DimensionType.Time, 3),
        ];

        private static readonly List<VirtualValueDefinition> VirtualDefs =
        [
            new SumDefinition { Code = "virtual_1", OperandCodes = ["value-0", "value-1"] }
        ];

        /// <summary>
        /// Builds a real matrix (2 Other values), a pre-augmented matrix (3 Other values including the virtual),
        /// the query (with virtual defs set on variable-1), and visualization settings derived from the real matrix.
        /// </summary>
        private static (
            Matrix<DecimalDataValue> realMatrix,
            Matrix<DecimalDataValue> augmentedMatrix,
            MatrixQuery query,
            VisualizationSettings settings)
            BuildTestInputs()
        {
            Matrix<DecimalDataValue> realMatrix = TestDataCubeBuilder.BuildTestMatrix(VarParams);

            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(VarParams);
            query.DimensionQueries["variable-1"].VirtualValueDefinitions = [.. VirtualDefs];

            // Use the real computation service to produce an augmented matrix that the mock will return.
            VirtualValueComputationService realService = new(NullLogger<VirtualValueComputationService>.Instance);
            Matrix<DecimalDataValue> augmentedMatrix = realService.ApplyVirtualValues(realMatrix, query);

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.LineChart
            };
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(realMatrix.Metadata, query);

            return (realMatrix, augmentedMatrix, query, settings);
        }

        [Test]
        public void BuildVisualizationResponse_WithPrecomputedVirtualValues_VirtualValueIncludedInMetadata()
        {
            // Arrange — virtual values are applied by the caller before the adapter is invoked.
            (Matrix<DecimalDataValue> _,
             Matrix<DecimalDataValue> augmentedMatrix,
             MatrixQuery query,
             _) = BuildTestInputs();

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.LineChart
            };
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(augmentedMatrix.Metadata, query);

            // Act
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(
                augmentedMatrix, query, settings);

            // Assert
            Variable? otherVar = result.MetaData.FirstOrDefault(v => v.Code == "variable-1");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(otherVar, Is.Not.Null);
                Assert.That(otherVar!.Values.Select(v => v.Code), Does.Contain("virtual_1"));
            }
        }

        [Test]
        public void BuildVisualizationResponse_WithoutPrecomputedVirtualValues_OnlyRealValuesInMetadata()
        {
            // Arrange — virtual defs are present in the query but the caller has not applied them;
            // the adapter must not compute them on its own.
            (Matrix<DecimalDataValue> realMatrix,
             _,
             MatrixQuery query,
             VisualizationSettings settings) = BuildTestInputs();

            // Act
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(
                realMatrix, query, settings);

            // Assert — only the 2 real values should be present
            Variable? otherVar = result.MetaData.FirstOrDefault(v => v.Code == "variable-1");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(otherVar, Is.Not.Null);
                Assert.That(otherVar!.Values.Count, Is.EqualTo(2));
                Assert.That(otherVar.Values.Select(v => v.Code), Does.Not.Contain("virtual_1"));
            }
        }

        [Test]
        public void BuildVisualizationResponse_ArchiveMatrix_IncludesPrecomputedVirtualValue()
        {
            // Arrange — archive matrices already contain the computed virtual values.
            (_, Matrix<DecimalDataValue> augmentedMatrix, MatrixQuery query, _) = BuildTestInputs();

            // Settings derived from the augmented matrix since that is the archive cube.
            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.LineChart
            };
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(augmentedMatrix.Metadata, query);

            // Act
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(
                augmentedMatrix, query, settings);

            // Assert — the pre-computed virtual value must appear in the response
            Variable? otherVar = result.MetaData.FirstOrDefault(v => v.Code == "variable-1");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(otherVar, Is.Not.Null);
                Assert.That(otherVar!.Values.Select(v => v.Code), Does.Contain("virtual_1"));
            }
        }
    }
}
