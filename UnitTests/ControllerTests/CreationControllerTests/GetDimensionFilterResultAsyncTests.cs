using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Settings;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class GetVariableFilterResultAsyncTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task GetVariableFilterResultAsyncTest()
        {
            // Arrange
            List<DimensionParameters> cubeParams =
            [
                new (DimensionType.Content, 3),
                new (DimensionType.Time, 12),
                new (DimensionType.Nominal, 5),
                new (DimensionType.Other, 3)
            ];
            string path = Path.Combine("foo", "bar", "baz");
            FilterRequest filter = new()
            {
                TableReference = new(path),
                Filters = new Dictionary<string, IValueFilter>
                {
                    { "variable-0", new ItemFilter(["value-0"]) },
                    { "variable-1", new AllFilter() },
                    { "variable-2", new FromFilter("value-3") },
                    { "variable-3", new TopFilter(2) },
                    { "foo", new AllFilter() }
                }
            };
            CreationController controller = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams, null);

            // Act
            ActionResult<Dictionary<string, List<string>>> result = await controller.GetDimensionFilterResultAsync(filter);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Count, Is.EqualTo(5));
            Assert.That(result.Value["variable-0"].Count, Is.EqualTo(1));
            Assert.That(result.Value["variable-1"].Count, Is.EqualTo(12));
            Assert.That(result.Value["variable-2"].Count, Is.EqualTo(2));
            Assert.That(result.Value["variable-3"].Count, Is.EqualTo(2));
            Assert.That(result.Value["foo"], Is.Not.Null);
            Assert.That(result.Value["foo"].Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetDimensionFilterResult_ItemFilter_DoesNotIncludeUnselectedVirtualValues()
        {
            // Arrange: dimension has values ["value-0", "value-1", "value-2"]; user picks only "value-0";
            // virtual "virtual_sum" is defined but NOT listed in ItemFilter.Codes
            List<DimensionParameters> cubeParams =
            [
                new(DimensionType.Nominal, 3)
            ];
            string path = Path.Combine("foo", "bar", "test");
            FilterRequest filterRequest = new()
            {
                TableReference = new(path),
                Filters = new Dictionary<string, IValueFilter>
                {
                    { "variable-0", new ItemFilter(["value-0"]) }
                },
                VirtualValueDefinitions = new Dictionary<string, List<string>>
                {
                    { "variable-0", ["virtual_sum"] }
                }
            };
            CreationController controller = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams, null);

            // Act
            ActionResult<Dictionary<string, List<string>>> result = await controller.GetDimensionFilterResultAsync(filterRequest);

            // Assert: virtual_sum was not selected in the ItemFilter — must not appear in the response
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value["variable-0"], Does.Not.Contain("virtual_sum"));
        }

        [Test]
        public async Task GetDimensionFilterResult_ItemFilter_IncludesSelectedVirtualValues()
        {
            // Arrange: user explicitly adds "virtual_sum" to ItemFilter.Codes
            List<DimensionParameters> cubeParams =
            [
                new(DimensionType.Nominal, 3)
            ];
            string path = Path.Combine("foo", "bar", "test");
            FilterRequest filterRequest = new()
            {
                TableReference = new(path),
                Filters = new Dictionary<string, IValueFilter>
                {
                    { "variable-0", new ItemFilter(["value-0", "virtual_sum"]) }
                },
                VirtualValueDefinitions = new Dictionary<string, List<string>>
                {
                    { "variable-0", ["virtual_sum"] }
                }
            };
            CreationController controller = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams, null);

            // Act
            ActionResult<Dictionary<string, List<string>>> result = await controller.GetDimensionFilterResultAsync(filterRequest);

            // Assert: virtual_sum was explicitly listed in ItemFilter.Codes — it must appear in the response
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value["variable-0"], Does.Contain("virtual_sum"));
        }

        [Test]
        public async Task GetDimensionFilterResult_AllFilter_IncludesAllVirtualValues()
        {
            // Arrange: AllFilter — all virtual values should be included regardless
            List<DimensionParameters> cubeParams =
            [
                new(DimensionType.Nominal, 3)
            ];
            string path = Path.Combine("foo", "bar", "test");
            FilterRequest filterRequest = new()
            {
                TableReference = new(path),
                Filters = new Dictionary<string, IValueFilter>
                {
                    { "variable-0", new AllFilter() }
                },
                VirtualValueDefinitions = new Dictionary<string, List<string>>
                {
                    { "variable-0", ["virtual_sum"] }
                }
            };
            CreationController controller = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams, null);

            // Act
            ActionResult<Dictionary<string, List<string>>> result = await controller.GetDimensionFilterResultAsync(filterRequest);

            // Assert: AllFilter applied to the combined real+virtual list includes all virtual codes
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value["variable-0"], Does.Contain("virtual_sum"));
        }

        [Test]
        public async Task GetDimensionFilterResult_TopFilter_VirtualWithinWindow_IsIncluded()
        {
            // Arrange: dimension has 3 real values [value-0, value-1, value-2].
            // TopFilter(2) on combined [value-0, value-1, value-2, virtual_sum] => last 2 => [value-2, virtual_sum].
            // Old (broken) behavior: [value-1, value-2, virtual_sum] = 3 values.
            // New (correct) behavior: [value-2, virtual_sum] = 2 values.
            List<DimensionParameters> cubeParams =
            [
                new(DimensionType.Nominal, 3)
            ];
            string path = Path.Combine("foo", "bar", "test");
            FilterRequest filterRequest = new()
            {
                TableReference = new(path),
                Filters = new Dictionary<string, IValueFilter>
                {
                    { "variable-0", new TopFilter(2) }
                },
                VirtualValueDefinitions = new Dictionary<string, List<string>>
                {
                    { "variable-0", ["virtual_sum"] }
                }
            };
            CreationController controller = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams, null);

            // Act
            ActionResult<Dictionary<string, List<string>>> result = await controller.GetDimensionFilterResultAsync(filterRequest);

            // Assert: exactly 2 values returned (not 3); virtual_sum is included within the window
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value["variable-0"].Count, Is.EqualTo(2));
            Assert.That(result.Value["variable-0"], Does.Contain("virtual_sum"));
            Assert.That(result.Value["variable-0"], Does.Contain("value-2"));
            Assert.That(result.Value["variable-0"], Does.Not.Contain("value-0"));
            Assert.That(result.Value["variable-0"], Does.Not.Contain("value-1"));
        }

        [Test]
        public async Task GetDimensionFilterResult_TopFilter_MultipleVirtuals_OnlyWindowVirtualsIncluded()
        {
            // Arrange: dimension has 3 real values [value-0, value-1, value-2] and 2 virtual values.
            // Combined = [value-0, value-1, value-2, virtual_1, virtual_2].
            // TopFilter(1) => last 1 => [virtual_2]. virtual_1 is outside the window.
            List<DimensionParameters> cubeParams =
            [
                new(DimensionType.Nominal, 3)
            ];
            string path = Path.Combine("foo", "bar", "test");
            FilterRequest filterRequest = new()
            {
                TableReference = new(path),
                Filters = new Dictionary<string, IValueFilter>
                {
                    { "variable-0", new TopFilter(1) }
                },
                VirtualValueDefinitions = new Dictionary<string, List<string>>
                {
                    { "variable-0", ["virtual_1", "virtual_2"] }
                }
            };
            CreationController controller = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams, null);

            // Act
            ActionResult<Dictionary<string, List<string>>> result = await controller.GetDimensionFilterResultAsync(filterRequest);

            // Assert: exactly 1 value returned; only virtual_2 is in scope
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value["variable-0"].Count, Is.EqualTo(1));
            Assert.That(result.Value["variable-0"], Does.Contain("virtual_2"));
            Assert.That(result.Value["variable-0"], Does.Not.Contain("virtual_1"));
        }
    }
}
