using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Controllers;
using PxGraf.Models.Responses;

namespace UnitTests.ControllerTests
{
    [TestFixture]
    internal class InfoControllerTests
    {
        private Mock<IWebHostEnvironment> _mockEnv;
        private Mock<ILogger<InfoController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.ApplicationName).Returns("PxGraf");
            _mockEnv.Setup(e => e.EnvironmentName).Returns("Development");
            _mockLogger = new Mock<ILogger<InfoController>>();
        }

        [Test]
        public void Get_ReturnsOkWithInfoResponse()
        {
            // Arrange
            InfoController controller = new(_mockEnv.Object, _mockLogger.Object);

            // Act
            IActionResult result = controller.Get();

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            OkObjectResult okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.TypeOf<InfoResponse>());
        }

        [Test]
        public void Get_ResponseContainsCorrectName()
        {
            // Arrange
            InfoController controller = new(_mockEnv.Object, _mockLogger.Object);

            // Act
            IActionResult result = controller.Get();

            // Assert
            OkObjectResult okResult = (OkObjectResult)result;
            InfoResponse response = (InfoResponse)okResult.Value;
            Assert.That(response.Name, Is.EqualTo("PxGraf"));
        }

        [Test]
        public void Get_ResponseContainsCorrectEnvironment()
        {
            // Arrange
            InfoController controller = new(_mockEnv.Object, _mockLogger.Object);

            // Act
            IActionResult result = controller.Get();

            // Assert
            OkObjectResult okResult = (OkObjectResult)result;
            InfoResponse response = (InfoResponse)okResult.Value;
            Assert.That(response.Environment, Is.EqualTo("Development"));
        }

        [Test]
        public void Get_ResponseContainsVersion()
        {
            // Arrange
            InfoController controller = new(_mockEnv.Object, _mockLogger.Object);

            // Act
            IActionResult result = controller.Get();

            // Assert
            OkObjectResult okResult = (OkObjectResult)result;
            InfoResponse response = (InfoResponse)okResult.Value;
            Assert.That(response.Version, Is.Not.Null);
        }
    }
}
