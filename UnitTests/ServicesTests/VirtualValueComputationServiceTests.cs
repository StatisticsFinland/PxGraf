using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Services;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Fixtures;

namespace UnitTests.ServicesTests
{
    [TestFixture]
    internal class VirtualValueComputationServiceTests
    {
        private VirtualValueComputationService _service;

        [SetUp]
        public void SetUp()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
            _service = new VirtualValueComputationService(NullLogger<VirtualValueComputationService>.Instance);
        }

        /// <summary>
        /// Builds a 1-dimensional test matrix with the given value codes and corresponding data values.
        /// Data layout: data[i] corresponds to dimValues[i].
        /// </summary>
        private static Matrix<DecimalDataValue> Build1DMatrix(
            string dimensionCode,
            string[] valueCodes,
            decimal[] values)
        {
            MultilanguageString dimName = new(new Dictionary<string, string> { { "fi", dimensionCode } });
            List<DimensionValue> dimValues = valueCodes
                .Select(code => new DimensionValue(code, new MultilanguageString(new Dictionary<string, string> { { "fi", code } })))
                .ToList();
            Dictionary<string, MetaProperty> noProperties = [];
            Dimension dim = new(dimensionCode, dimName, noProperties, dimValues, DimensionType.Nominal);
            MatrixMetadata meta = new("fi", ["fi"], [dim], []);
            DecimalDataValue[] data = values.Select(v => new DecimalDataValue(v, DataValueType.Exists)).ToArray();
            return new Matrix<DecimalDataValue>(meta, data);
        }

        /// <summary>
        /// Builds a 2-dimensional test matrix. Data is row-major: last dimension varies fastest.
        /// Index = i * dim2Count + j for dim1[i], dim2[j].
        /// </summary>
        private static Matrix<DecimalDataValue> Build2DMatrix(
            string dim1Code,
            string[] dim1ValueCodes,
            string dim2Code,
            string[] dim2ValueCodes,
            decimal[] values)
        {
            MultilanguageString dim1Name = new(new Dictionary<string, string> { { "fi", dim1Code } });
            List<DimensionValue> dim1Values = dim1ValueCodes
                .Select(code => new DimensionValue(code, new MultilanguageString(new Dictionary<string, string> { { "fi", code } })))
                .ToList();
            Dictionary<string, MetaProperty> noProperties1 = [];
            Dimension dim1 = new(dim1Code, dim1Name, noProperties1, dim1Values, DimensionType.Nominal);

            MultilanguageString dim2Name = new(new Dictionary<string, string> { { "fi", dim2Code } });
            List<DimensionValue> dim2Values = dim2ValueCodes
                .Select(code => new DimensionValue(code, new MultilanguageString(new Dictionary<string, string> { { "fi", code } })))
                .ToList();
            Dictionary<string, MetaProperty> noProperties2 = [];
            Dimension dim2 = new(dim2Code, dim2Name, noProperties2, dim2Values, DimensionType.Nominal);

            MatrixMetadata meta = new("fi", ["fi"], [dim1, dim2], []);
            DecimalDataValue[] data = values.Select(v => new DecimalDataValue(v, DataValueType.Exists)).ToArray();
            return new Matrix<DecimalDataValue>(meta, data);
        }

        // ---- Sum ----

        [Test]
        public void ApplyVirtualValues_Sum2Values_CorrectSum()
        {
            // a=10, b=20, c=30  →  virtual_1 = a+b = 30
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Count, Is.EqualTo(4));
                Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(30m));
            }
        }

        [Test]
        public void ApplyVirtualValues_Sum3ValuesWithConstant_CorrectSum()
        {
            // a=10, b=20, c=30  →  virtual_1 = a+b+c+10 = 70
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b", "c"], Constant = 10.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(70m));
        }

        // ---- Subtraction ----

        [Test]
        public void ApplyVirtualValues_Subtraction2Operands_CorrectDifference()
        {
            // a=10, b=20  →  virtual_1 = a-b = -10
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SubtractionOfTwoDefinition { Code = "virtual_1", Minuend = "a", Subtrahend = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(-10m));
        }

        [Test]
        public void ApplyVirtualValues_Subtraction1OperandPlusConstant_CorrectResult()
        {
            // a=10  →  virtual_1 = a-5 = 5
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SubtractionByConstantDefinition { Code = "virtual_1", Operand = "a", Constant = 5.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(5m));
        }

        // ---- Multiplication ----

        [Test]
        public void ApplyVirtualValues_Multiplication2Operands_CorrectProduct()
        {
            // a=10, b=20  →  virtual_1 = a*b = 200
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new MultiplicationOfTwoDefinition { Code = "virtual_1", LeftOperand = "a", RightOperand = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(200m));
        }

        [Test]
        public void ApplyVirtualValues_Multiplication1OperandPlusConstant_CorrectResult()
        {
            // a=10  →  virtual_1 = a*3 = 30
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new MultiplicationByConstantDefinition { Code = "virtual_1", Operand = "a", Constant = 3.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(30m));
        }

        // ---- Division ----

        [Test]
        public void ApplyVirtualValues_Division2Operands_CorrectQuotient()
        {
            // a=10, b=20  →  virtual_1 = a/b = 0.5
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "virtual_1", Dividend = "a", Divisor = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(0.5m));
        }

        [Test]
        public void ApplyVirtualValues_Division1OperandPlusConstant_CorrectResult()
        {
            // a=10  →  virtual_1 = a/2 = 5
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionByConstantDefinition { Code = "virtual_1", Operand = "a", Constant = 2.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(5m));
        }

        // ---- Chaining ----

        [Test]
        public void ApplyVirtualValues_ChainedDefinitions_CorrectResults()
        {
            // a=10, b=20, c=30  →  v1 = a+b = 30, v2 = v1+c = 60
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["a", "b"] },
                new SumDefinition { Code = "v2", OperandCodes = ["v1", "c"] }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Count, Is.EqualTo(5));
                Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(30m)); // v1 = a+b
                Assert.That(result.Data[4].UnsafeValue, Is.EqualTo(60m)); // v2 = v1+c
            }
        }

        /// <summary>
        /// Builds a 1-dimensional test matrix where the single dimension is a ContentDimension.
        /// Each value is a ContentDimensionValue with a given unit and precision 2.
        /// </summary>
        private static Matrix<DecimalDataValue> Build1DContentDimensionMatrix(
            string dimensionCode,
            string[] valueCodes,
            decimal[] values,
            string unit = "testUnit")
        {
            MultilanguageString dimName = new(new Dictionary<string, string> { { "fi", dimensionCode } });
            MultilanguageString unitMs = new(new Dictionary<string, string> { { "fi", unit } });
            DateTime lastUpdated = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            List<ContentDimensionValue> dimValues = valueCodes
                .Select(code => new ContentDimensionValue(
                    code,
                    new MultilanguageString(new Dictionary<string, string> { { "fi", code } }),
                    unitMs,
                    lastUpdated,
                    2))
                .ToList();
            Dictionary<string, MetaProperty> noProperties = [];
            ContentDimension dim = new(dimensionCode, dimName, noProperties, dimValues);
            MatrixMetadata meta = new("fi", ["fi"], [dim], []);
            DecimalDataValue[] data = values.Select(v => new DecimalDataValue(v, DataValueType.Exists)).ToArray();
            return new Matrix<DecimalDataValue>(meta, data);
        }

        // ---- Content dimension ----

        [Test]
        public void ApplyVirtualValues_ContentDimension_Division2Operands_DoesNotThrow()
        {
            // a=10, b=2  →  virtual_1 = a/b = 5
            Matrix<DecimalDataValue> matrix = Build1DContentDimensionMatrix("dim1", ["a", "b", "c"], [10m, 2m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "virtual_1", Dividend = "a", Divisor = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(5m));
        }

        [Test]
        public void ApplyVirtualValues_ContentDimension_Subtraction2Operands_DoesNotThrow()
        {
            // a=10, b=3  →  virtual_1 = a-b = 7
            Matrix<DecimalDataValue> matrix = Build1DContentDimensionMatrix("dim1", ["a", "b", "c"], [10m, 3m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SubtractionOfTwoDefinition { Code = "virtual_1", Minuend = "a", Subtrahend = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(7m));
        }

        [Test]
        public void ApplyVirtualValues_ContentDimension_VirtualValueIsContentDimensionValue_HasComputedPlaceholderUnitAndSource()
        {
            // a=10, b=2  →  virtual_1 = a/b; virtual_1 should be ContentDimensionValue with localized placeholder unit and source
            Matrix<DecimalDataValue> matrix = Build1DContentDimensionMatrix("dim1", ["a", "b", "c"], [10m, 2m, 30m], unit: "units");
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "virtual_1", Dividend = "a", Divisor = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            IReadOnlyDimensionValue virtualValue = result.Metadata.Dimensions
                .First(d => d.Code == "dim1").Values.First(v => v.Code == "virtual_1");

            string expectedPlaceholder = TranslationFixture.Translations["fi"].ComputedValuePlaceholder;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(virtualValue, Is.InstanceOf<ContentDimensionValue>());
                ContentDimensionValue cdv = (ContentDimensionValue)virtualValue;
                Assert.That(cdv.Unit["fi"], Is.EqualTo(expectedPlaceholder));
                MultilanguageStringProperty sourceProp = (MultilanguageStringProperty)cdv.AdditionalProperties[PxSyntaxConstants.SOURCE_KEY];
                Assert.That(sourceProp.Value["fi"], Is.EqualTo(expectedPlaceholder));
                Assert.That(cdv.Precision, Is.EqualTo(2));
                Assert.That(cdv.LastUpdated, Is.EqualTo(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
            }
        }

        [Test]
        public void ApplyVirtualValues_ContentDimension_ChainedDivision_CorrectResults()
        {
            // a=100, b=10, c=2
            // v1 = a/b = 10
            // v2 = v1/c = 5
            Matrix<DecimalDataValue> matrix = Build1DContentDimensionMatrix("dim1", ["a", "b", "c"], [100m, 10m, 2m]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "v1", Dividend = "a", Divisor = "b" },
                new DivisionOfTwoDefinition { Code = "v2", Dividend = "v1", Divisor = "c" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(10m)); // v1 = a/b
                Assert.That(result.Data[4].UnsafeValue, Is.EqualTo(5m));  // v2 = v1/c
            }
        }

        [Test]
        public void ApplyVirtualValues_ContentDimension_VirtualValuePrecision_IsMinOfOperandPrecisions()
        {
            // a (precision=3), b (precision=0) → virtual_1 = a/b; precision should be 0 (less accurate)
            MultilanguageString dimName = new(new Dictionary<string, string> { { "fi", "dim1" } });
            MultilanguageString unit = new(new Dictionary<string, string> { { "fi", "u" } });
            DateTime lastUpdated = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            List<ContentDimensionValue> dimValues =
            [
                new("a", new MultilanguageString(new Dictionary<string, string> { { "fi", "a" } }), unit, lastUpdated, 3),
                new("b", new MultilanguageString(new Dictionary<string, string> { { "fi", "b" } }), unit, lastUpdated, 0),
            ];
            ContentDimension dim = new("dim1", dimName, [], dimValues);
            MatrixMetadata meta = new("fi", ["fi"], [dim], []);
            Matrix<DecimalDataValue> matrix = new(meta,
            [
                new DecimalDataValue(10m, DataValueType.Exists),
                new DecimalDataValue(2m, DataValueType.Exists)
            ]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "virtual_1", Dividend = "a", Divisor = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            ContentDimensionValue cdv = (ContentDimensionValue)result.Metadata.Dimensions
                .First(d => d.Code == "dim1").Values.First(v => v.Code == "virtual_1");
            Assert.That(cdv.Precision, Is.EqualTo(0));
        }

        // ---- Error handling ----

        [Test]
        public void ApplyVirtualValues_CircularDependency_ThrowsInvalidOperationException()
        {
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b"], [10m, 20m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["v2", "a"] },
                new SumDefinition { Code = "v2", OperandCodes = ["v1", "b"] }
            ];

            Assert.That(
                () => _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } }),
                Throws.TypeOf<InvalidOperationException>());
        }

        // ---- Dimension preservation ----

        [Test]
        public void ApplyVirtualValues_2DMatrix_PreservesOtherDimensionAndOriginalValues()
        {
            // dim1=["a","b"], dim2=["x","y"]
            // data (row-major, dim2 varies fastest):
            //   [a,x]=10, [a,y]=20, [b,x]=30, [b,y]=40
            // After sum(a,b) on dim1 → v1:
            //   [v1,x] = 10+30 = 40, [v1,y] = 20+40 = 60
            Matrix<DecimalDataValue> matrix = Build2DMatrix(
                "dim1", ["a", "b"],
                "dim2", ["x", "y"],
                [10m, 20m, 30m, 40m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["a", "b"] }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Count, Is.EqualTo(3));
                Assert.That(result.Metadata.Dimensions.First(d => d.Code == "dim2").Values.Count, Is.EqualTo(2));
                Assert.That(result.Data[0].UnsafeValue, Is.EqualTo(10m)); // [a,x] unchanged
                Assert.That(result.Data[1].UnsafeValue, Is.EqualTo(20m)); // [a,y] unchanged
                Assert.That(result.Data[2].UnsafeValue, Is.EqualTo(30m)); // [b,x] unchanged
                Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(40m)); // [b,y] unchanged
                Assert.That(result.Data[4].UnsafeValue, Is.EqualTo(40m)); // [v1,x] = 10+30
                Assert.That(result.Data[5].UnsafeValue, Is.EqualTo(60m)); // [v1,y] = 20+40
            }
        }

        // ---- Name resolution ----

        [Test]
        public void ApplyVirtualValues_ValueEditsEmpty_NameFallsBackToPlaceholder()
        {
            // a=10, b=20, c=30  →  virtual_1 = sum(a,b)
            // With empty ValueEdits, the virtual value name should fall back to localized placeholder + sequence number
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            IReadOnlyDimensionValue virtualValue = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.First(v => v.Code == "virtual_1");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(virtualValue.Name["fi"], Is.EqualTo(TranslationFixture.Translations["fi"].SumPlaceholder + " 1"));
                Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(30m));
            }
        }

        [Test]
        public void ApplyVirtualValues_ValueEditsContainsNameEdit_UsesCustomName()
        {
            // a=10, b=20, c=30  →  virtual_1 = sum(a,b)
            // With ValueEdits containing a custom name, the virtual value should use that name
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];

            DimensionQuery dimQuery = new();
            dimQuery.ValueEdits["virtual_1"] = new DimensionQuery.DimensionValueEdition
            {
                NameEdit = new MultilanguageString(new Dictionary<string, string> { { "fi", "Laskettu arvo 1" } })
            };
            dimQuery.VirtualValueDefinitions = defs;

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = dimQuery } });

            IReadOnlyDimensionValue virtualValue = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.First(v => v.Code == "virtual_1");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(virtualValue.Name["fi"], Is.EqualTo("Laskettu arvo 1"));
                Assert.That(result.Data[3].UnsafeValue, Is.EqualTo(30m));
            }
        }

        [Test]
        public void ApplyVirtualValues_PartialNameEdit_EditedLanguageUsed_MissingLanguageFallsBackToPlaceholder()
        {
            // Matrix with both "fi" and "en" languages.
            // NameEdit contains only "en". "fi" should fall back to placeholder + sequence number.
            MultilanguageString dimName = new(new Dictionary<string, string> { { "fi", "dim1" }, { "en", "dim1" } });
            List<DimensionValue> dimValues = new string[] { "a", "b", "c" }
                .Select(code => new DimensionValue(code, new MultilanguageString(new Dictionary<string, string> { { "fi", code }, { "en", code } })))
                .ToList();
            Dimension dim = new("dim1", dimName, [], dimValues, DimensionType.Nominal);
            MatrixMetadata meta = new("fi", ["fi", "en"], [dim], []);
            Matrix<DecimalDataValue> matrix = new(meta, [
                new DecimalDataValue(10m, DataValueType.Exists),
                new DecimalDataValue(20m, DataValueType.Exists),
                new DecimalDataValue(30m, DataValueType.Exists),
            ]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];

            DimensionQuery dimQuery = new();
            dimQuery.ValueEdits["virtual_1"] = new DimensionQuery.DimensionValueEdition
            {
                NameEdit = new MultilanguageString(new Dictionary<string, string> { { "en", "Custom English name" } })
            };
            dimQuery.VirtualValueDefinitions = defs;

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = dimQuery } });

            IReadOnlyDimensionValue virtualValue = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.First(v => v.Code == "virtual_1");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(virtualValue.Name["en"], Is.EqualTo("Custom English name"));
                Assert.That(virtualValue.Name["fi"], Is.EqualTo(TranslationFixture.Translations["fi"].SumPlaceholder + " 1"));
            }
        }

        [Test]
        public void ApplyVirtualValues_TwoIndependentSameTypeDefs_NamesAssignedInInputListOrder()
        {
            // sum_a = a+b (no dependency on sum_b), sum_b = a+c (no dependency on sum_a)
            // Both are independent; topological sort must preserve input order so sum_a gets "Sum 1" and sum_b gets "Sum 2".
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "sum_a", OperandCodes = ["a", "b"] },
                new SumDefinition { Code = "sum_b", OperandCodes = ["a", "c"] }
            ];

            MatrixQuery query = new() { DimensionQueries = new() { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } };
            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, query);

            IReadOnlyDimension dim = result.Metadata.Dimensions.First(d => d.Code == "dim1");
            IReadOnlyDimensionValue sumA = dim.Values.First(v => v.Code == "sum_a");
            IReadOnlyDimensionValue sumB = dim.Values.First(v => v.Code == "sum_b");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(sumA.Name["fi"], Is.EqualTo(TranslationFixture.Translations["fi"].SumPlaceholder + " 1"));
                Assert.That(sumB.Name["fi"], Is.EqualTo(TranslationFixture.Translations["fi"].SumPlaceholder + " 2"));
            }
        }

        [Test]
        public void ApplyVirtualValues_SubtractionDefinition_DefaultNameUsesSubtractionPlaceholder()
        {
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new SubtractionOfTwoDefinition { Code = "sub_1", Minuend = "a", Subtrahend = "b" }
            ];

            MatrixQuery query = new() { DimensionQueries = new() { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } };
            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, query);

            IReadOnlyDimensionValue virtualValue = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.First(v => v.Code == "sub_1");
            Assert.That(virtualValue.Name["fi"], Is.EqualTo(TranslationFixture.Translations["fi"].SubtractionPlaceholder + " 1"));
        }

        [Test]
        public void ApplyVirtualValues_MultiplicationDefinition_DefaultNameUsesMultiplicationPlaceholder()
        {
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new MultiplicationOfTwoDefinition { Code = "mul_1", LeftOperand = "a", RightOperand = "b" }
            ];

            MatrixQuery query = new() { DimensionQueries = new() { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } };
            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, query);

            IReadOnlyDimensionValue virtualValue = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.First(v => v.Code == "mul_1");
            Assert.That(virtualValue.Name["fi"], Is.EqualTo(TranslationFixture.Translations["fi"].MultiplicationPlaceholder + " 1"));
        }

        [Test]
        public void ApplyVirtualValues_DivisionDefinition_DefaultNameUsesDivisionPlaceholder()
        {
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b", "c"], [10m, 20m, 30m]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "div_1", Dividend = "a", Divisor = "b" }
            ];

            MatrixQuery query = new() { DimensionQueries = new() { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } };
            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, query);

            IReadOnlyDimensionValue virtualValue = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.First(v => v.Code == "div_1");
            Assert.That(virtualValue.Name["fi"], Is.EqualTo(TranslationFixture.Translations["fi"].DivisionPlaceholder + " 1"));
        }

        // ---- 2D matrix + constant operations ----

        [Test]
        public void ApplyVirtualValues_2DMatrix_SumWithConstant_CorrectResults()
        {
            // dim1=["a","b"], dim2=["x","y"]
            // data (row-major): [a,x]=10, [a,y]=20, [b,x]=30, [b,y]=40
            // virtual_1 = sum(a,b) + 5 on dim1:
            //   [virtual_1,x] = (10+30)+5 = 45, [virtual_1,y] = (20+40)+5 = 65
            Matrix<DecimalDataValue> matrix = Build2DMatrix(
                "dim1", ["a", "b"],
                "dim2", ["x", "y"],
                [10m, 20m, 30m, 40m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"], Constant = 5.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Data[4].UnsafeValue, Is.EqualTo(45m)); // [virtual_1,x]
                Assert.That(result.Data[5].UnsafeValue, Is.EqualTo(65m)); // [virtual_1,y]
            }
        }

        [Test]
        public void ApplyVirtualValues_2DMatrix_SubtractionWithConstant_CorrectResults()
        {
            // dim1=["a","b"], dim2=["x","y"]
            // data: [a,x]=10, [a,y]=20, [b,x]=30, [b,y]=40
            // virtual_1 = a - 3 on dim1:
            //   [virtual_1,x] = 10-3 = 7, [virtual_1,y] = 20-3 = 17
            Matrix<DecimalDataValue> matrix = Build2DMatrix(
                "dim1", ["a", "b"],
                "dim2", ["x", "y"],
                [10m, 20m, 30m, 40m]);
            List<VirtualValueDefinition> defs =
            [
                new SubtractionByConstantDefinition { Code = "virtual_1", Operand = "a", Constant = 3.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Data[4].UnsafeValue, Is.EqualTo(7m));  // [virtual_1,x]
                Assert.That(result.Data[5].UnsafeValue, Is.EqualTo(17m)); // [virtual_1,y]
            }
        }

        [Test]
        public void ApplyVirtualValues_2DMatrix_MultiplicationWithConstant_CorrectResults()
        {
            // dim1=["a","b"], dim2=["x","y"]
            // data: [a,x]=10, [a,y]=20, [b,x]=30, [b,y]=40
            // virtual_1 = a * 3 on dim1:
            //   [virtual_1,x] = 10*3 = 30, [virtual_1,y] = 20*3 = 60
            Matrix<DecimalDataValue> matrix = Build2DMatrix(
                "dim1", ["a", "b"],
                "dim2", ["x", "y"],
                [10m, 20m, 30m, 40m]);
            List<VirtualValueDefinition> defs =
            [
                new MultiplicationByConstantDefinition { Code = "virtual_1", Operand = "a", Constant = 3.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Data[4].UnsafeValue, Is.EqualTo(30m)); // [virtual_1,x]
                Assert.That(result.Data[5].UnsafeValue, Is.EqualTo(60m)); // [virtual_1,y]
            }
        }

        [Test]
        public void ApplyVirtualValues_2DMatrix_DivisionWithConstant_CorrectResults()
        {
            // dim1=["a","b"], dim2=["x","y"]
            // data: [a,x]=10, [a,y]=20, [b,x]=30, [b,y]=40
            // virtual_1 = a / 2 on dim1:
            //   [virtual_1,x] = 10/2 = 5, [virtual_1,y] = 20/2 = 10
            Matrix<DecimalDataValue> matrix = Build2DMatrix(
                "dim1", ["a", "b"],
                "dim2", ["x", "y"],
                [10m, 20m, 30m, 40m]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionByConstantDefinition { Code = "virtual_1", Operand = "a", Constant = 2.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Data[4].UnsafeValue, Is.EqualTo(5m));  // [virtual_1,x]
                Assert.That(result.Data[5].UnsafeValue, Is.EqualTo(10m)); // [virtual_1,y]
            }
        }

        [Test]
        public void ApplyVirtualValues_2DMatrix_SumThenDivideByConstant_ReproducesUserScenario()
        {
            // Reproduces: sum(a,b) then divide by constant 2 (computing an average)
            // dim1=["a","b"], dim2=["x","y"]
            // data: [a,x]=10, [a,y]=20, [b,x]=30, [b,y]=40
            // v1 = sum(a,b) = [v1,x]=40, [v1,y]=60
            // v2 = v1 / 2   = [v2,x]=20, [v2,y]=30
            Matrix<DecimalDataValue> matrix = Build2DMatrix(
                "dim1", ["a", "b"],
                "dim2", ["x", "y"],
                [10m, 20m, 30m, 40m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["a", "b"] },
                new DivisionByConstantDefinition { Code = "v2", Operand = "v1", Constant = 2.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Data[4].UnsafeValue, Is.EqualTo(40m)); // [v1,x]
                Assert.That(result.Data[5].UnsafeValue, Is.EqualTo(60m)); // [v1,y]
                Assert.That(result.Data[6].UnsafeValue, Is.EqualTo(20m)); // [v2,x]
                Assert.That(result.Data[7].UnsafeValue, Is.EqualTo(30m)); // [v2,y]
            }
        }

        // ---- Safe division: zero and missing divisors ----

        [Test]
        public void ApplyVirtualValues_DivisionByZeroConstant_ReturnsCanNotRepresentVirtualValues()
        {
            // a=5, virtual_1 = a / constant(0) → virtual cell must be CanNotRepresent
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a"], [5m]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionByConstantDefinition { Code = "virtual_1", Operand = "a", Constant = 0.0 }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[1].Type, Is.EqualTo(DataValueType.CanNotRepresent));
        }

        [Test]
        public void ApplyVirtualValues_DivisionBySelectedZeroValue_ReturnsMissingVirtualValue()
        {
            // a=5, b=0, virtual_1 = a / b → virtual cell must be CanNotRepresent
            Matrix<DecimalDataValue> matrix = Build1DMatrix("dim1", ["a", "b"], [5m, 0m]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "virtual_1", Dividend = "a", Divisor = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[2].Type, Is.EqualTo(DataValueType.CanNotRepresent));
        }

        [Test]
        public void ApplyVirtualValues_2DMatrix_DivisionBySelectedZero_MarksOnlyAffectedCellsMissing()
        {
            // dim1=["a","b"] (operated on), dim2=["t0","t1"] (time periods)
            // data row-major (dim2 varies fastest):
            //   [a,t0]=10, [a,t1]=15, [b,t0]=0, [b,t1]=5
            // virtual = a / b:
            //   [virtual,t0] → divisor=0 → Missing
            //   [virtual,t1] → 15/5 = 3 → Exists
            Matrix<DecimalDataValue> matrix = Build2DMatrix(
                "dim1", ["a", "b"],
                "dim2", ["t0", "t1"],
                [10m, 15m, 0m, 5m]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "virtual_1", Dividend = "a", Divisor = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            // data indices: [a,t0]=0,[a,t1]=1,[b,t0]=2,[b,t1]=3,[virtual,t0]=4,[virtual,t1]=5
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Data[4].Type, Is.EqualTo(DataValueType.CanNotRepresent));
                Assert.That(result.Data[5].UnsafeValue, Is.EqualTo(3m));
                Assert.That(result.Data[5].Type, Is.EqualTo(DataValueType.Exists));
            }
        }

        [Test]
        public void ApplyVirtualValues_DivisionBySelectedMissingValue_ReturnsMissingVirtualValue()
        {
            // a=5, b=Missing, virtual_1 = a / b → virtual cell must be Missing
            MultilanguageString dimName = new(new Dictionary<string, string> { { "fi", "dim1" } });
            List<DimensionValue> dimValues =
            [
                new("a", new MultilanguageString(new Dictionary<string, string> { { "fi", "a" } })),
                new("b", new MultilanguageString(new Dictionary<string, string> { { "fi", "b" } }))
            ];
            Dimension dim = new("dim1", dimName, [], dimValues, DimensionType.Nominal);
            MatrixMetadata meta = new("fi", ["fi"], [dim], []);
            Matrix<DecimalDataValue> matrix = new(meta,
            [
                new DecimalDataValue(5m, DataValueType.Exists),
                new DecimalDataValue(0m, DataValueType.Missing)
            ]);
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "virtual_1", Dividend = "a", Divisor = "b" }
            ];

            Matrix<DecimalDataValue> result = _service.ApplyVirtualValues(matrix, new MatrixQuery { DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = new DimensionQuery { VirtualValueDefinitions = defs } } });

            Assert.That(result.Data[2].Type, Is.EqualTo(DataValueType.CanNotRepresent));
        }
    }
}
