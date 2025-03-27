using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Controllers;
using System.IO;
using System;

namespace UnitTests.ControllerTests
{
    internal class ErrorControllerTests
    {
        [Test]
        public void ErrorControllerTestIOExceptionReturnsBadRequestAndLogsCritical()
        {
            // Arrange
            Mock<ILogger<ErrorController>> logger = new();
            logger.Setup(l => l.Log(
                LogLevel.Critical,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<IOException>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));
            ErrorController controller = new(logger.Object);
            DefaultHttpContext httpContext = new();
            httpContext.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature()
            {
                Error = new IOException(),
                Path = "/test/path"
            });
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            IActionResult result = controller.Error();

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestResult>());
            
            logger.Verify(l => l.Log(
                LogLevel.Critical,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<IOException>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void ErrorControllerTestNoErrorStillReturnBadResponseAndLogsError()
        {
            // Arrange
            Mock<ILogger<ErrorController>> logger = new();
            logger.Setup(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));
            ErrorController controller = new(logger.Object);
            DefaultHttpContext httpContext = new();
            httpContext.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature()
            {
                Error = null,
                Path = "/test/path"
            });
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            IActionResult result = controller.Error();

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestResult>());
            
            logger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void ErrorControllerTestExceptionReturnsBadRequestAndLogsError()
        {
            // Arrange
            Mock<ILogger<ErrorController>> logger = new();
            logger.Setup(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));
            ErrorController controller = new(logger.Object);
            DefaultHttpContext httpContext = new();
            httpContext.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature()
            {
                Error = new DivideByZeroException(),
                Path = "/test/path"
            });
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            IActionResult result = controller.Error();

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestResult>());

            logger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
