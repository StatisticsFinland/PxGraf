using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement.Mvc;
using NUnit.Framework;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTests.ConfigurationTests
{
    [TestFixture]
    internal class ApiExplorerConventionsTests
    {
        private ApiExplorerConventions _conventions;

        [SetUp]
        public void Setup()
        {
            _conventions = new ApiExplorerConventions();
        }

        private static ActionModel CreateActionModel(Type controllerType, string methodName = "FakeAction", bool? initialVisibility = true)
        {
            ControllerModel controllerModel = new(controllerType.GetTypeInfo(), controllerType.GetCustomAttributes(inherit: true))
            {
                ControllerName = controllerType.Name
            };

            MethodInfo methodInfo = controllerType.GetMethod(methodName) ?? typeof(ApiExplorerConventionsTests).GetMethod(nameof(FakeAction));
            ActionModel action = new(methodInfo, methodInfo.GetCustomAttributes(inherit: true))
            {
                ActionName = methodName,
                Controller = controllerModel
            };

            action.ApiExplorer.IsVisible = initialVisibility;
            return action;
        }

        // Dummy action method used for reflection when the controller type has no matching method
        public static void FakeAction()
        {
            // Method intentionally left empty.
        }

        [Test]
        public void Apply_NullVisibility_SetsHidden()
        {
            ActionModel action = CreateActionModel(typeof(UngatedController), initialVisibility: null);

            _conventions.Apply(action);

            Assert.That(action.ApiExplorer.IsVisible, Is.False);
        }

        [Test]
        public void Apply_UngatedController_StaysVisible()
        {
            ActionModel action = CreateActionModel(typeof(UngatedController));

            _conventions.Apply(action);

            Assert.That(action.ApiExplorer.IsVisible, Is.True);
        }

        [Test]
        public void Apply_GatedControllerWithFeatureEnabled_StaysVisible()
        {
            LoadConfigWithCreationAPI(true);
            ActionModel action = CreateActionModel(typeof(GatedController));

            _conventions.Apply(action);

            Assert.That(action.ApiExplorer.IsVisible, Is.True);
        }

        [Test]
        public void Apply_GatedControllerWithFeatureDisabled_SetsHidden()
        {
            LoadConfigWithCreationAPI(false);
            ActionModel action = CreateActionModel(typeof(GatedController));

            _conventions.Apply(action);

            Assert.That(action.ApiExplorer.IsVisible, Is.False);
        }

        private static void LoadConfigWithCreationAPI(bool enabled)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "DatabaseConfig:Type", "PxWeb" },
                    { "DatabaseConfig:PxWebUrl", "http://test:1234/" },
                    { "FeatureManagement:CreationAPI", enabled.ToString() }
                })
                .Build();

            Configuration.Load(configuration);
        }

        // Test controller types for attribute detection
        private class UngatedController
        {
#pragma warning disable S1144,S3218,CA1822 // Mark members as static
            public void FakeAction()
            {
                // Method intentionally left empty.
            }
#pragma warning restore S1144,S3218,CA1822 // Mark members as static
        }

        [FeatureGate("CreationAPI")]
        private class GatedController
        {
#pragma warning disable S1144,S3218,CA1822 // Mark members as static
            public void FakeAction()
            {
                // Method intentionally left empty.
            }
#pragma warning restore S1144,S3218,CA1822 // Mark members as static
        }
    }
}
