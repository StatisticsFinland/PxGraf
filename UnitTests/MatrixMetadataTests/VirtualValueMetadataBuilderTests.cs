using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Language;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Fixtures;

namespace UnitTests.MatrixMetadataTests
{
    internal class VirtualValueMetadataBuilderTests
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
        public void Build_AddsNamedVirtualValueWithoutChangingSourceMetadata()
        {
            MatrixMetadata metadata = TestDataCubeBuilder.BuildTestMeta(
                [new DimensionParameters(DimensionType.Other, 3)]);
            DimensionQuery dimensionQuery = TestDataCubeBuilder.BuildTestVariableQuery(
                new DimensionParameters(DimensionType.Other, 3));
            dimensionQuery.VirtualValueDefinitions =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["value-0", "value-1"] }
            ];
            dimensionQuery.ValueEdits["virtual_1"] = new DimensionQuery.DimensionValueEdition
            {
                NameEdit = new MultilanguageString(new Dictionary<string, string> { ["fi"] = "Laskettu arvo" })
            };

            IReadOnlyMatrixMetadata result = VirtualValueMetadataBuilder.Build(
                metadata,
                new MatrixQuery
                {
                    DimensionQueries = new Dictionary<string, DimensionQuery>
                    {
                        ["variable-0"] = dimensionQuery
                    }
                });

            Assert.Multiple(() =>
            {
                Assert.That(metadata.Dimensions[0].Values, Has.Count.EqualTo(3));
                Assert.That(result.Dimensions[0].Values, Has.Count.EqualTo(4));
                Assert.That(result.Dimensions[0].Values[^1].Code, Is.EqualTo("virtual_1"));
                Assert.That(result.Dimensions[0].Values[^1].Name["fi"], Is.EqualTo("Laskettu arvo"));
            });
        }

        [Test]
        public void Build_ChainedContentValues_MatchesComputedMatrixMetadata()
        {
            DimensionParameters parameters = new(DimensionType.Content, 3);
            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix([parameters]);
            DimensionQuery dimensionQuery = TestDataCubeBuilder.BuildTestVariableQuery(parameters);
            dimensionQuery.VirtualValueDefinitions =
            [
                new DivisionOfTwoDefinition { Code = "virtual_2", Dividend = "virtual_1", Divisor = "value-2" },
                new SumDefinition { Code = "virtual_1", OperandCodes = ["value-0", "value-1"] }
            ];
            MatrixQuery query = new()
            {
                DimensionQueries = new Dictionary<string, DimensionQuery>
                {
                    ["variable-0"] = dimensionQuery
                }
            };

            IReadOnlyMatrixMetadata metadataOnly = VirtualValueMetadataBuilder.Build(matrix.Metadata, query);
            Matrix<DecimalDataValue> computed = new VirtualValueComputationService(
                NullLogger<VirtualValueComputationService>.Instance).ApplyVirtualValues(matrix, query);

            foreach (string code in new[] { "virtual_1", "virtual_2" })
            {
                ContentDimensionValue metadataValue = (ContentDimensionValue)metadataOnly.Dimensions[0].Values
                    .Single(value => value.Code == code);
                ContentDimensionValue computedValue = (ContentDimensionValue)computed.Metadata.Dimensions[0].Values
                    .Single(value => value.Code == code);
                MultilanguageStringProperty metadataSource = (MultilanguageStringProperty)metadataValue.AdditionalProperties[PxSyntaxConstants.SOURCE_KEY];
                MultilanguageStringProperty computedSource = (MultilanguageStringProperty)computedValue.AdditionalProperties[PxSyntaxConstants.SOURCE_KEY];

                Assert.Multiple(() =>
                {
                    Assert.That(metadataValue.Name, Is.EqualTo(computedValue.Name));
                    Assert.That(metadataValue.Unit, Is.EqualTo(computedValue.Unit));
                    Assert.That(metadataValue.LastUpdated, Is.EqualTo(computedValue.LastUpdated));
                    Assert.That(metadataValue.Precision, Is.EqualTo(computedValue.Precision));
                    Assert.That(metadataSource.Value, Is.EqualTo(computedSource.Value));
                });
            }
        }

        [Test]
        public void Build_ItemFilter_MatchesDataPathAndIncludesOnlyQueriedValues()
        {
            DimensionParameters parameters = new(DimensionType.Other, 3);
            Matrix<DecimalDataValue> completeMatrix = TestDataCubeBuilder.BuildTestMatrix([parameters]);
            DimensionQuery dimensionQuery = new()
            {
                ValueFilter = new ItemFilter(["value-0", "virtual_selected"]),
                VirtualValueDefinitions =
                [
                    new SumDefinition { Code = "virtual_selected", OperandCodes = ["value-0", "value-1"] },
                    new SumDefinition { Code = "virtual_unselected", OperandCodes = ["value-1", "value-2"] }
                ]
            };
            MatrixQuery query = new()
            {
                DimensionQueries = new Dictionary<string, DimensionQuery>
                {
                    ["variable-0"] = dimensionQuery
                }
            };
            (IReadOnlyMatrixMetadata fetchMetadata, MatrixMap outputMap) = completeMatrix.Metadata
                .BuildVirtualValueMaps(query);

            IReadOnlyMatrixMetadata metadataResult = VirtualValueMetadataBuilder.Build(fetchMetadata, query)
                .GetTransform(outputMap);
            Matrix<DecimalDataValue> dataResult = new VirtualValueComputationService(
                NullLogger<VirtualValueComputationService>.Instance)
                .ApplyVirtualValues(completeMatrix.GetTransform(new MatrixMap([.. fetchMetadata.DimensionMaps])), query)
                .GetTransform(outputMap);

            IReadOnlyList<string> metadataCodes = metadataResult.Dimensions[0].ValueCodes;
            IReadOnlyList<string> dataCodes = dataResult.Metadata.Dimensions[0].ValueCodes;
            Assert.Multiple(() =>
            {
                Assert.That(metadataCodes, Is.EqualTo(["value-0", "virtual_selected"]));
                Assert.That(dataCodes, Is.EqualTo(metadataCodes));
                Assert.That(metadataCodes, Does.Not.Contain("value-1"));
                Assert.That(metadataCodes, Does.Not.Contain("value-2"));
                Assert.That(metadataCodes, Does.Not.Contain("virtual_unselected"));
            });
        }
    }
}