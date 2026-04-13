using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Controllers;
using PxGraf.Models.Responses;
using PxGraf.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.ControllerTests
{
    [TestFixture]
    internal class HealthControllerTests
    {
        private Mock<IHealthCheckService> _mockHealthCheckService;
        private Mock<ILogger<HealthController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockHealthCheckService = new Mock<IHealthCheckService>();
            _mockLogger = new Mock<ILogger<HealthController>>();
        }

        private HealthController BuildController()
        {
            return new HealthController(_mockHealthCheckService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetHealth_AllHealthy_Returns200WithResponse()
        {
            // Arrange
            HealthResponse healthyResponse = new(
                "healthy",
                [new DatabaseHealthStatus("database", "healthy")],
                [
                    new ServiceHealthStatus("saved-query-storage", "healthy"),
                    new ServiceHealthStatus("archive-file-storage", "healthy")
                ]);
            _mockHealthCheckService
                .Setup(s => s.CheckHealthAsync())
                .ReturnsAsync(healthyResponse);

            HealthController controller = BuildController();

            // Act
            IActionResult result = await controller.GetHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<OkObjectResult>());
                OkObjectResult okResult = (OkObjectResult)result;
                HealthResponse response = (HealthResponse)okResult.Value;
                Assert.That(response.Status, Is.EqualTo("healthy"));
                Assert.That(response.Databases, Has.Count.EqualTo(1));
                Assert.That(response.Services, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public async Task GetHealth_Unhealthy_Returns503WithResponse()
        {
            // Arrange
            HealthResponse unhealthyResponse = new(
                "unhealthy",
                [new DatabaseHealthStatus("database", "unhealthy")],
                [
                    new ServiceHealthStatus("saved-query-storage", "healthy"),
                    new ServiceHealthStatus("archive-file-storage", "healthy")
                ]);
            _mockHealthCheckService
                .Setup(s => s.CheckHealthAsync())
                .ReturnsAsync(unhealthyResponse);

            HealthController controller = BuildController();

            // Act
            IActionResult result = await controller.GetHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<ObjectResult>());
                ObjectResult objectResult = (ObjectResult)result;
                Assert.That(objectResult.StatusCode, Is.EqualTo(503));
                HealthResponse response = (HealthResponse)objectResult.Value;
                Assert.That(response.Status, Is.EqualTo("unhealthy"));
                Assert.That(response.Databases[0].Status, Is.EqualTo("unhealthy"));
            }
        }

        [Test]
        public async Task GetHealth_ResponseContainsBothDatabasesAndServices()
        {
            // Arrange
            HealthResponse response = new(
                "unhealthy",
                [new DatabaseHealthStatus("database", "healthy")],
                [
                    new ServiceHealthStatus("saved-query-storage", "healthy"),
                    new ServiceHealthStatus("archive-file-storage", "unhealthy"),
                    new ServiceHealthStatus("publication-webhook", "healthy")
                ]);
            _mockHealthCheckService
                .Setup(s => s.CheckHealthAsync())
                .ReturnsAsync(response);

            HealthController controller = BuildController();

            // Act
            IActionResult result = await controller.GetHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<ObjectResult>());
                ObjectResult objectResult = (ObjectResult)result;
                Assert.That(objectResult.StatusCode, Is.EqualTo(503));
                HealthResponse body = (HealthResponse)objectResult.Value;
                Assert.That(body.Databases, Has.Count.EqualTo(1));
                Assert.That(body.Services, Has.Count.EqualTo(3));
            }
        }
    }
}
