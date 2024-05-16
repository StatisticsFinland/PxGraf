using NUnit.Framework;
using Moq;
using PxGraf.Controllers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using PxGraf.Enums;
using PxGraf.Data.MetaData;
using PxGraf.PxWebInterface;
using System.Collections.Generic;
using UnitTests.TestDummies.DummyQueries;
using UnitTests.TestDummies;
using PxGraf.Models.Queries;

namespace CreationControllerTests
{
    public class ValidateTableMetaDataTests
    {
        private Mock<ILogger<CreationController>> mockLogger;
        private Mock<ICachedPxWebConnection> mockConnection;
        private CreationController controller;
        private string tablePath;

        [OneTimeSetUp]
        public void Setup()
        {
            mockLogger = new Mock<ILogger<CreationController>>();
            mockConnection = new Mock<ICachedPxWebConnection>();
            controller = new CreationController(mockConnection.Object, mockLogger.Object);
            tablePath = "test/path";
        }

        [TestCase(VariableType.Content, VariableType.Time, true, true, true)]
        [TestCase(VariableType.Time, VariableType.Unknown, false, true, true)]
        [TestCase(VariableType.Content, VariableType.Unknown, true, false, true)]
        [TestCase(VariableType.Unknown, VariableType.Unknown, false, false, true)]
        [TestCase(VariableType.Content, VariableType.Time, false, false, false, 0)]
        public async Task ValidateTableMetaData_ReturnsExpectedResult (
            VariableType firstVariableType,
            VariableType secondVariableType,
            bool hasContentVariable,
            bool hasTimeVariable,
            bool noZeroSizedVariables,
            int firstVariableSize = 1)
        {
            // Arrange
            List<VariableParameters> variables = new()
            {
                new VariableParameters(firstVariableType, firstVariableSize),
                new VariableParameters(secondVariableType, 1)
            };

            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            mockConnection.Setup(m => m.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>())).ReturnsAsync(testMeta);

            // Act
            var result = await controller.ValidateTableMetaData(tablePath);

            // Assert
            Assert.AreEqual(hasContentVariable, result.TableHasContentVariable);
            Assert.AreEqual(hasTimeVariable, result.TableHasTimeVariable);
            Assert.AreEqual(noZeroSizedVariables, result.AllVariablesContainValues);
        }
    }
}