#nullable enable
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.MatrixMetadataTests
{
    [TestFixture]
    public class MatrixMetadataExpansionTrimTests
    {
        /// <summary>
        /// Builds a single-dimension MatrixMetadata with the given value codes.
        /// </summary>
        private static MatrixMetadata BuildMeta(string dimensionCode, string[] valueCodes)
        {
            MultilanguageString dimName = new(new Dictionary<string, string> { { "fi", dimensionCode } });
            List<DimensionValue> dimValues = valueCodes
                .Select(code => new DimensionValue(code, new MultilanguageString(new Dictionary<string, string> { { "fi", code } })))
                .ToList();
            Dictionary<string, MetaProperty> noProperties = [];
            Dimension dim = new(dimensionCode, dimName, noProperties, dimValues, DimensionType.Nominal);
            return new MatrixMetadata("fi", ["fi"], [dim], []);
        }

        /// <summary>
        /// Builds a 1-dimensional Matrix with the given value codes and decimal data values.
        /// </summary>
        private static Matrix<DecimalDataValue> BuildMatrix(string dimensionCode, string[] valueCodes, decimal[] values)
        {
            MatrixMetadata meta = BuildMeta(dimensionCode, valueCodes);
            DecimalDataValue[] data = values.Select(v => new DecimalDataValue(v, DataValueType.Exists)).ToArray();
            return new Matrix<DecimalDataValue>(meta, data);
        }

        /// <summary>
        /// Builds a MatrixQuery with a single dimension entry for the given dimension code,
        /// filtering to the specified value codes and optionally including virtual value definitions.
        /// </summary>
        private static MatrixQuery BuildQuery(
            string dimensionCode,
            List<string> filterCodes,
            List<VirtualValueDefinition>? virtualDefs = null)
        {
            DimensionQuery dimQuery = new()
            {
                ValueFilter = new ItemFilter(filterCodes),
                VirtualValueDefinitions = virtualDefs ?? []
            };
            return new MatrixQuery
            {
                TableReference = new PxTableReference { Name = "test.px", Hierarchy = [] },
                DimensionQueries = new Dictionary<string, DimensionQuery>
                {
                    [dimensionCode] = dimQuery
                }
            };
        }

        // ---- ExpandForVirtualValueComputation ----

        [Test]
        public void ExpandForVirtualValueComputation_NoVirtualDefinitions_ReturnsSameValueCodes()
        {
            // Arrange
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            MatrixQuery query = BuildQuery("dim1", ["a"]);

            // Act
            var (fetchMeta, _) = completeMeta.BuildVirtualValueMaps(query);

            // Assert
            List<string> resultCodes = fetchMeta.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            Assert.That(resultCodes, Is.EqualTo(new[] { "a" }));
        }

        [Test]
        public void ExpandForVirtualValueComputation_WithVirtualDefinitions_AddsOperandCodes()
        {
            // Arrange: virtual_1 = sum(a, b); "b" must be added, "c" must not
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];
            MatrixQuery query = BuildQuery("dim1", ["a"], defs);

            // Act
            var (fetchMeta, _) = completeMeta.BuildVirtualValueMaps(query);

            // Assert
            List<string> resultCodes = fetchMeta.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Contain("a"));
                Assert.That(resultCodes, Does.Contain("b"));
                Assert.That(resultCodes, Does.Not.Contain("c"));
                Assert.That(resultCodes, Does.Not.Contain("virtual_1"));
            }
        }

        [Test]
        public void ExpandForVirtualValueComputation_OperandAlreadySelected_NoDuplicates()
        {
            // Arrange: virtual_1 = sum(a, b); "b" is already present — no duplicate
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];
            MatrixQuery query = BuildQuery("dim1", ["a", "b"], defs);

            // Act
            var (fetchMeta, _) = completeMeta.BuildVirtualValueMaps(query);

            // Assert
            List<string> resultCodes = fetchMeta.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes.Count(c => c == "b"), Is.EqualTo(1));
                Assert.That(resultCodes, Does.Contain("a"));
                Assert.That(resultCodes, Does.Contain("b"));
            }
        }

        [Test]
        public void ExpandForVirtualValueComputation_NonExistentOperandCode_Ignored()
        {
            // Arrange: virtual_1 = sum(a, does_not_exist); "does_not_exist" is not in completeMeta → ignored
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "does_not_exist"] }
            ];
            MatrixQuery query = BuildQuery("dim1", ["a"], defs);

            // Act
            var (fetchMeta, _) = completeMeta.BuildVirtualValueMaps(query);

            // Assert
            List<string> resultCodes = fetchMeta.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Is.EqualTo(["a"]));
                Assert.That(resultCodes, Does.Not.Contain("does_not_exist"));
            }
        }

        [Test]
        public void ExpandForVirtualValueComputation_VirtualOperandCode_NotAdded()
        {
            // Arrange: virtual_2 = sum(virtual_1, a); "virtual_1" is itself a virtual code → must not be added to expanded meta
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] },
                new SumDefinition { Code = "virtual_2", OperandCodes = ["virtual_1", "a"] }
            ];
            MatrixQuery query = BuildQuery("dim1", ["a"], defs);

            // Act
            var (fetchMeta, _) = completeMeta.BuildVirtualValueMaps(query);

            // Assert
            List<string> resultCodes = fetchMeta.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Contain("a"));
                Assert.That(resultCodes, Does.Contain("b"));
                Assert.That(resultCodes, Does.Not.Contain("virtual_1"));
                Assert.That(resultCodes, Does.Not.Contain("virtual_2"));
            }
        }

        [Test]
        public void ExpandForVirtualValueComputation_OperandComesBeforeSelectedInCompleteMeta_PreservesOrder()
        {
            // completeMeta has codes ["a", "b", "c"]
            // query selects only ["c"] (user selected "c")
            // virtual_1 = sum(c, a) — operand "a" is before "c" in completeMeta
            // Expected: expanded codes are ["a", "c"] (NOT ["c", "a"])
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["c", "a"] }
            ];
            MatrixQuery query = BuildQuery("dim1", ["c"], defs);

            // Act
            var (fetchMeta, _) = completeMeta.BuildVirtualValueMaps(query);

            // Assert
            List<string> resultCodes = fetchMeta.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            Assert.That(resultCodes, Is.EqualTo(["a", "c"]));
        }

        // ---- TrimOperandOnlyValues ----

        [Test]
        public void TrimOperandOnlyValues_NoVirtualDefinitions_MatrixUnchanged()
        {
            // Arrange: matrix has ["a", "b"], no virtual defs
            Matrix<DecimalDataValue> matrix = BuildMatrix("dim1", ["a", "b"], [1m, 2m]);
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            MatrixQuery query = BuildQuery("dim1", ["a", "b"]);

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            Matrix<DecimalDataValue> result = matrix.GetTransform(outputMap);

            // Assert
            List<string> resultCodes = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            Assert.That(resultCodes, Is.EqualTo(["a", "b"]));
        }

        [Test]
        public void TrimOperandOnlyValues_RemovesOperandOnlyValues()
        {
            // Arrange: matrix has ["a", "b", "virtual_1"] (b was operand-only),
            // user explicitly selected virtual_1 in the ItemFilter
            Matrix<DecimalDataValue> matrix = BuildMatrix("dim1", ["a", "b", "virtual_1"], [1m, 2m, 3m]);
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];
            MatrixQuery query = BuildQuery("dim1", ["a", "virtual_1"], defs);

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            Matrix<DecimalDataValue> result = matrix.GetTransform(outputMap);

            // Assert
            List<string> resultCodes = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Contain("a"));
                Assert.That(resultCodes, Does.Not.Contain("b"));
                Assert.That(resultCodes, Does.Contain("virtual_1"));
            }
        }

        [Test]
        public void TrimOperandOnlyValues_KeepsSelectedOperandValues()
        {
            // Arrange: user explicitly selected "b" and "virtual_1"; matrix has ["a", "b", "virtual_1"]
            Matrix<DecimalDataValue> matrix = BuildMatrix("dim1", ["a", "b", "virtual_1"], [1m, 2m, 3m]);
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];
            MatrixQuery query = BuildQuery("dim1", ["a", "b", "virtual_1"], defs);

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            Matrix<DecimalDataValue> result = matrix.GetTransform(outputMap);

            // Assert
            List<string> resultCodes = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Contain("a"));
                Assert.That(resultCodes, Does.Contain("b"));
                Assert.That(resultCodes, Does.Contain("virtual_1"));
            }
        }

        [Test]
        public void TrimOperandOnlyValues_KeepsVirtualValues()
        {
            // Arrange: matrix has ["a", "b", "virtual_1", "virtual_2"]; b is operand-only;
            // user explicitly selected both virtual values in the ItemFilter
            Matrix<DecimalDataValue> matrix = BuildMatrix("dim1", ["a", "b", "virtual_1", "virtual_2"], [1m, 2m, 3m, 4m]);
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] },
                new MultiplicationOfTwoDefinition { Code = "virtual_2", LeftOperand = "a", RightOperand = "b" }
            ];
            MatrixQuery query = BuildQuery("dim1", ["a", "virtual_1", "virtual_2"], defs);

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            Matrix<DecimalDataValue> result = matrix.GetTransform(outputMap);

            // Assert
            List<string> resultCodes = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Contain("a"));
                Assert.That(resultCodes, Does.Not.Contain("b"));
                Assert.That(resultCodes, Does.Contain("virtual_1"));
                Assert.That(resultCodes, Does.Contain("virtual_2"));
            }
        }

        [Test]
        public void TrimOperandOnlyValues_ItemFilter_ExcludesUnselectedVirtualValues()
        {
            // Arrange: matrix has ["a", "b", "virtual_1"]; ItemFilter has only "a" — virtual_1 was NOT selected
            Matrix<DecimalDataValue> matrix = BuildMatrix("dim1", ["a", "b", "virtual_1"], [1m, 2m, 3m]);
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];
            MatrixQuery query = BuildQuery("dim1", ["a"], defs);

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            Matrix<DecimalDataValue> result = matrix.GetTransform(outputMap);

            // Assert: virtual_1 was not in the ItemFilter's Codes list, so it must be trimmed out
            List<string> resultCodes = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Contain("a"));
                Assert.That(resultCodes, Does.Not.Contain("b"));
                Assert.That(resultCodes, Does.Not.Contain("virtual_1"));
            }
        }

        [Test]
        public void TrimOperandOnlyValues_ItemFilter_KeepsSelectedVirtualValues()
        {
            // Arrange: matrix has ["a", "b", "virtual_1"]; ItemFilter explicitly includes "virtual_1"
            Matrix<DecimalDataValue> matrix = BuildMatrix("dim1", ["a", "b", "virtual_1"], [1m, 2m, 3m]);
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];
            // ItemFilter codes include the virtual code explicitly
            DimensionQuery dimQuery = new()
            {
                ValueFilter = new ItemFilter(["a", "virtual_1"]),
                VirtualValueDefinitions = defs
            };
            MatrixQuery query = new()
            {
                TableReference = new PxTableReference { Name = "test.px", Hierarchy = [] },
                DimensionQueries = new Dictionary<string, DimensionQuery>
                {
                    ["dim1"] = dimQuery
                }
            };

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            Matrix<DecimalDataValue> result = matrix.GetTransform(outputMap);

            // Assert: virtual_1 is in the ItemFilter's Codes list, so it must be kept
            List<string> resultCodes = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Contain("a"));
                Assert.That(resultCodes, Does.Not.Contain("b"));
                Assert.That(resultCodes, Does.Contain("virtual_1"));
            }
        }

        [Test]
        public void TrimOperandOnlyValues_AllFilter_KeepsAllVirtualValues()
        {
            // Arrange: matrix has ["a", "b", "virtual_1"]; AllFilter is used (not ItemFilter)
            // even though "virtual_1" was not explicitly in user selection, it should be kept
            Matrix<DecimalDataValue> matrix = BuildMatrix("dim1", ["a", "b", "virtual_1"], [1m, 2m, 3m]);
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];
            // Use AllFilter instead of ItemFilter
            DimensionQuery dimQuery = new()
            {
                ValueFilter = new AllFilter(),
                VirtualValueDefinitions = defs
            };
            MatrixQuery query = new()
            {
                TableReference = new PxTableReference { Name = "test.px", Hierarchy = [] },
                DimensionQueries = new Dictionary<string, DimensionQuery>
                {
                    ["dim1"] = dimQuery
                }
            };

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            Matrix<DecimalDataValue> result = matrix.GetTransform(outputMap);

            // Assert: with AllFilter, all virtual values are kept even without explicit selection
            List<string> resultCodes = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Contain("a"));
                Assert.That(resultCodes, Does.Contain("b"));
                Assert.That(resultCodes, Does.Contain("virtual_1"));
            }
        }

        [Test]
        public void TrimOperandOnlyValues_TopFilter_VirtualInWindow_IsIncluded()
        {
            // Arrange: 3 reals [a, b, c] + 1 virtual [virtual_1].
            // TopFilter(2) on combined [a, b, c, virtual_1] => last 2 => [c, virtual_1].
            // matrix has [a, b, c, virtual_1] (b was operand-only, a was not in window).
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            Matrix<DecimalDataValue> matrix = BuildMatrix("dim1", ["a", "b", "c", "virtual_1"], [1m, 2m, 3m, 4m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["b", "c"] }
            ];
            DimensionQuery dimQuery = new()
            {
                ValueFilter = new TopFilter(2),
                VirtualValueDefinitions = defs
            };
            MatrixQuery query = new()
            {
                TableReference = new PxTableReference { Name = "test.px", Hierarchy = [] },
                DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = dimQuery }
            };

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            Matrix<DecimalDataValue> result = matrix.GetTransform(outputMap);

            // Assert: c and virtual_1 are kept; a and b are trimmed
            List<string> resultCodes = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Contain("c"));
                Assert.That(resultCodes, Does.Contain("virtual_1"));
                Assert.That(resultCodes, Does.Not.Contain("a"));
                Assert.That(resultCodes, Does.Not.Contain("b"));
                Assert.That(resultCodes.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void TrimOperandOnlyValues_TopFilter_VirtualOutsideWindow_IsExcluded()
        {
            // Arrange: 3 reals [a, b, c] + 2 virtuals [virtual_1, virtual_2].
            // Combined = [a, b, c, virtual_1, virtual_2]. TopFilter(1) => last 1 => [virtual_2].
            // virtual_1 is NOT in the window. The window has no real values (window is pure virtual).
            // matrix has [a, b, c, virtual_1, virtual_2].
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            Matrix<DecimalDataValue> matrix = BuildMatrix("dim1", ["a", "b", "c", "virtual_1", "virtual_2"], [1m, 2m, 3m, 4m, 5m]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] },
                new SumDefinition { Code = "virtual_2", OperandCodes = ["b", "c"] }
            ];
            DimensionQuery dimQuery = new()
            {
                ValueFilter = new TopFilter(1),
                VirtualValueDefinitions = defs
            };
            MatrixQuery query = new()
            {
                TableReference = new PxTableReference { Name = "test.px", Hierarchy = [] },
                DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = dimQuery }
            };

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            Matrix<DecimalDataValue> result = matrix.GetTransform(outputMap);

            // Assert: only virtual_2 is in scope; virtual_1 is excluded
            List<string> resultCodes = result.Metadata.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(resultCodes, Does.Not.Contain("c"));
                Assert.That(resultCodes, Does.Contain("virtual_2"));
                Assert.That(resultCodes, Does.Not.Contain("virtual_1"));
                Assert.That(resultCodes, Does.Not.Contain("a"));
                Assert.That(resultCodes, Does.Not.Contain("b"));
            }
        }

        [Test]
        public void FilterDimensionValues_TopFilter_ReturnsRealValuesFromCombinedWindow()
        {
            // Arrange: dimension has [a, b, c]. Query has TopFilter(2) + virtual_1 def.
            // Combined list = [a, b, c, virtual_1]. TopFilter(2) => last 2 => [c, virtual_1].
            // FilterDimensionValues must return only the real codes from the window: [c].
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["b", "c"] }
            ];
            DimensionQuery dimQuery = new()
            {
                ValueFilter = new TopFilter(2),
                VirtualValueDefinitions = defs
            };
            MatrixQuery query = new()
            {
                TableReference = new PxTableReference { Name = "test.px", Hierarchy = [] },
                DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = dimQuery }
            };

            // Act
            IReadOnlyMatrixMetadata filteredMeta = completeMeta.FilterDimensionValues(query);

            // Assert: only [c] returned as real code — virtual_1 is not in the metadata
            List<string> resultCodes = filteredMeta.Dimensions.First(d => d.Code == "dim1").Values.Select(v => v.Code).ToList();
            Assert.That(resultCodes, Is.EqualTo(new[] { "c" }));
        }

        // ---- HasEmptyOutputDimension ----

        [Test]
        public void HasEmptyOutputDimension_NoVirtualDefs_ZeroRealValues_ReturnsTrue()
        {
            // Arrange: 0 real values and no virtual defs → dimension is empty
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            MatrixQuery query = BuildQuery("dim1", []);

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            bool result = outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasEmptyOutputDimension_WithRealValues_ReturnsFalse()
        {
            // Arrange: real values selected → dimension is not empty
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            MatrixQuery query = BuildQuery("dim1", ["a"]);

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            bool result = outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void HasEmptyOutputDimension_TopFilter_VirtualInWindow_ZeroReals_ReturnsFalse()
        {
            // Arrange: 3 reals [a, b, c] + 1 virtual [virtual_1].
            // TopFilter(1) on combined [a, b, c, virtual_1] => last 1 => [virtual_1].
            // HasEmptyOutputDimension must return false because virtual_1 is in scope.
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["b", "c"] }
            ];
            DimensionQuery dimQuery = new()
            {
                ValueFilter = new TopFilter(1),
                VirtualValueDefinitions = defs
            };
            MatrixQuery query = new()
            {
                TableReference = new PxTableReference { Name = "test.px", Hierarchy = [] },
                DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = dimQuery }
            };

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            bool result = outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0);

            // Assert: virtual_1 is in scope so total count = 1 → not empty
            Assert.That(result, Is.False);
        }

        [Test]
        public void HasEmptyOutputDimension_TopFilter_VirtualOutsideWindow_ZeroReals_ReturnsTrue()
        {
            // Arrange: 3 reals [a, b, c] + 2 virtuals [virtual_1, virtual_2].
            // TopFilter(0) on combined => 0 values selected.
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b", "c"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] },
                new SumDefinition { Code = "virtual_2", OperandCodes = ["b", "c"] }
            ];
            DimensionQuery dimQuery = new()
            {
                ValueFilter = new TopFilter(0),
                VirtualValueDefinitions = defs
            };
            MatrixQuery query = new()
            {
                TableReference = new PxTableReference { Name = "test.px", Hierarchy = [] },
                DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = dimQuery }
            };

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            bool result = outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0);

            // Assert: TopFilter(0) selects nothing → empty
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasEmptyOutputDimension_ItemFilter_VirtualNotSelected_ZeroReals_ReturnsTrue()
        {
            // Arrange: ItemFilter with only real codes but 0 real values.
            // virtual_1 is defined but not in ItemFilter.Codes → not in scope.
            MatrixMetadata completeMeta = BuildMeta("dim1", ["a", "b"]);
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["a", "b"] }
            ];
            DimensionQuery dimQuery = new()
            {
                ValueFilter = new ItemFilter([]),
                VirtualValueDefinitions = defs
            };
            MatrixQuery query = new()
            {
                TableReference = new PxTableReference { Name = "test.px", Hierarchy = [] },
                DimensionQueries = new Dictionary<string, DimensionQuery> { ["dim1"] = dimQuery }
            };

            // Act
            var (_, outputMap) = completeMeta.BuildVirtualValueMaps(query);
            bool result = outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0);

            // Assert: no reals and virtual not selected → empty
            Assert.That(result, Is.True);
        }
    }
}
