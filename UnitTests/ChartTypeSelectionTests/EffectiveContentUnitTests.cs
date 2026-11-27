using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Fixtures;

namespace UnitTests.ChartTypeSelectionTests
{
    public class EffectiveContentUnitTests
    {
        private static DimensionQuery.DimensionValueEdition UnitEdit(params KeyValuePair<string, string>[] translations) =>
            new() { ContentComponent = new ContentComponentEdition { UnitEdit = new MultilanguageString(translations) } };

        private static int? UnitCount(MatrixQuery query, Matrix<DecimalDataValue> matrix) =>
            VisualizationTypeSelectionObject.FromQueryAndMatrix(query, matrix).Dimensions
                .Single(dimension => dimension.Type == DimensionType.Content).NumberOfUnits;

        [Test]
        public void DifferentSourceUnitsEditedEqual_EnableLineChartWithoutMutatingMetadata()
        {
            List<DimensionParameters> dimensions =
            [
                new(DimensionType.Content, 3),
                new(DimensionType.Time, 3)
            ];
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions);
            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, languages: ["fi", "en", "sv"]);
            ContentDimension content = (ContentDimension)matrix.Metadata.Dimensions[0];
            string originalUnit = content.Values[0].Unit["fi"];
            DimensionQuery edits = query.DimensionQueries[content.Code];
            edits.ValueEdits["value-0"] = UnitEdit(new("fi", "u"), new("en", "unit"), new("sv", "enhet"));
            edits.ValueEdits["value-1"] = UnitEdit(new("sv", "enhet"), new("fi", "u"), new("en", "unit"));
            edits.ValueEdits["value-2"] = UnitEdit(new("en", "unit"), new("sv", "enhet"), new("fi", "u"));

            Assert.That(UnitCount(query, matrix), Is.EqualTo(1));
            Assert.That(ChartTypeSelector.Selector.GetValidChartTypes(query, matrix), Does.Contain(VisualizationType.LineChart));
            Assert.That(content.Values[0].Unit["fi"], Is.EqualTo(originalUnit));

            edits.ValueEdits["value-2"] = UnitEdit(new("fi", "u"), new("en", "unit"), new("sv", "different"));
            Assert.That(UnitCount(query, matrix), Is.EqualTo(2));
            Assert.That(ChartTypeSelector.Selector.GetRejectionReasons(query, matrix)[VisualizationType.LineChart]
                .Select(reason => reason.Reason), Does.Contain(RejectionReason.UnambiguousContentUnitRequired));
        }

        [Test]
        public void PartialAndEmptyEditsUseSourceTranslations()
        {
            List<DimensionParameters> dimensions = [new(DimensionType.Content, 2) { SameUnit = true }];
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions);
            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions);
            DimensionQuery content = query.DimensionQueries["variable-0"];

            Assert.That(UnitCount(query, matrix), Is.EqualTo(1));
            content.ValueEdits["value-0"] = UnitEdit(new("fi", "changed"), new("en", ""));
            Assert.That(UnitCount(query, matrix), Is.EqualTo(2));
            content.ValueEdits["value-1"] = UnitEdit(new KeyValuePair<string, string>("fi", "changed"));
            Assert.That(UnitCount(query, matrix), Is.EqualTo(1));
            content.ValueEdits["value-1"] = UnitEdit(new("fi", "changed"), new("en", "other"));
            Assert.That(UnitCount(query, matrix), Is.EqualTo(2));
        }

        [Test]
        public void SourceUnitLanguageOrderDoesNotAffectEquality()
        {
            MultilanguageString name = new("fi", "name");
            ContentDimension dimension = new("content", name, [],
            [
                new ContentDimensionValue("a", name, new MultilanguageString(
                    new Dictionary<string, string> { ["fi"] = "u", ["en"] = "unit" }), DateTime.MinValue, 0),
                new ContentDimensionValue("b", name, new MultilanguageString(
                    new Dictionary<string, string> { ["en"] = "unit", ["fi"] = "u" }), DateTime.MinValue, 0)
            ]);

            var result = VisualizationTypeSelectionObject.DimensionInfo.FromQueryAndDimension(new DimensionQuery(), dimension);
            Assert.That(result.NumberOfUnits, Is.EqualTo(1));
        }

        [Test]
        public void UnselectedEditsAndSelectableValuesDoNotChangeUnitCount()
        {
            List<DimensionParameters> dimensions = [new(DimensionType.Content, 2) { SameUnit = true }];
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions);
            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions);
            DimensionQuery content = query.DimensionQueries["variable-0"];
            content.ValueEdits["value-2"] = UnitEdit(new("fi", "other"), new("en", "other"));
            Assert.That(UnitCount(query, matrix), Is.EqualTo(1));

            content.ValueEdits["value-0"] = UnitEdit(new KeyValuePair<string, string>("fi", "different"));
            content.Selectable = true;
            Assert.That(UnitCount(query, matrix), Is.EqualTo(1));
        }

        [Test]
        public void VirtualContentUnitUsesItsOwnEdits()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
            List<DimensionParameters> dimensions = [new(DimensionType.Content, 1)];
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions);
            DimensionQuery content = query.DimensionQueries["variable-0"];
            content.VirtualValueDefinitions = [new SumDefinition { Code = "virtual", OperandCodes = ["value-0"], Constant = 0 }];
            content.ValueEdits["value-0"] = UnitEdit(new("fi", "u"), new("en", "unit"));
            content.ValueEdits["virtual"] = UnitEdit(new("fi", "u"), new("en", "unit"));
            VirtualValueComputationService service = new(NullLogger<VirtualValueComputationService>.Instance);
            Matrix<DecimalDataValue> matrix = service.ApplyVirtualValues(TestDataCubeBuilder.BuildTestMatrix(dimensions), query);

            Assert.That(UnitCount(query, matrix), Is.EqualTo(1));
            content.ValueEdits["virtual"] = UnitEdit(new("fi", "u"), new("en", "different"));
            Assert.That(UnitCount(query, matrix), Is.EqualTo(2));
        }
    }
}
