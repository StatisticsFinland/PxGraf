using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Language;
using PxGraf.Services;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using UnitTests.Fixtures;

namespace UnitTests.ServicesTests
{
    [TestFixture]
    internal class AuditLogServiceTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
            Dictionary<string, string> config = TestInMemoryConfiguration.Get();
            config.Add("LogOptions:Folder", "\"foo/bar\"");
            config.Add("LogOptions:AuditLog:Enabled", "true");
            config.Add("LogOptions:AuditLog:IncludedHeaders:0", "foo");
            config.Add("LogOptions:AuditLog:IncludedHeaders:1", "bar");

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public void LogAuditEvent_WithConfiguredIncludedHeaders_LogsWithIncludedHeaders()
        {
            // Arrange
            Mock<HttpContext> httpContext = new();
            Mock<HttpRequest> request = new();
            HeaderDictionary headers = new()
            {
                { "foo", "fooValue" },
                { "bar", "barValue" },
                { "baz", "bazValue" }
            };

            request.Setup(r => r.Headers).Returns(headers);
            httpContext.Setup(c => c.Request).Returns(request.Object);
            httpContext.Setup(c => c.User.Identity.Name).Returns("TestUser");
            Mock<ConnectionInfo> connection = new();
            connection.Setup(c => c.RemoteIpAddress).Returns(System.Net.IPAddress.Parse("127.0.0.1"));
            httpContext.Setup(c => c.Connection).Returns(connection.Object);
            Mock<IHttpContextAccessor> httpContextAccessorMock = new();
            httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext.Object);
            Mock<ILogger<AuditLogService>> loggerMock = new();
            Dictionary<string, string> capturedScope = null;
            loggerMock
                .Setup(l => l.BeginScope(It.IsAny<IDictionary<string, string>>()))
                .Callback<object>(scope =>
                {
                    capturedScope = new Dictionary<string, string>((IDictionary<string, string>)scope);
                })
                .Returns(Mock.Of<IDisposable>());

            AuditLogService auditLogService = new (httpContextAccessorMock.Object, loggerMock.Object);

            // Act
            auditLogService.LogAuditEvent("TestAction", "TestResource");

            // Assert
            Assert.That(capturedScope, Is.Not.Null);
            Assert.That(capturedScope, Contains.Key("foo").WithValue("fooValue"));
            Assert.That(capturedScope, Contains.Key("bar").WithValue("barValue"));
            Assert.That(capturedScope, Does.Not.ContainKey("baz"));
            Assert.That(capturedScope, Contains.Key("Category").WithValue("Audit"));
            loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Audit Event: action=TestAction, resource=TestResource, user=TestUser, clientIP=127.0.0.1")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
