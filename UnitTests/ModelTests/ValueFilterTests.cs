using NUnit.Framework;
using System.Collections.Generic;
using PxGraf.Models.Queries;
using System.Linq;
using Px.Utils.Models.Metadata.Dimensions;

namespace UnitTests.ModelTests
{
    internal class ValueFilterTests
    {
        private readonly List<IReadOnlyDimensionValue> values =
        [
            new DimensionValue("val0", null),
            new DimensionValue("val1", null),
            new DimensionValue("val2", null),
            new DimensionValue("val3", null),
            new DimensionValue("val4", null)
        ];

        [Test]
        public void TopFilterTest()
        {
            TopFilter filter = new(2);
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            string[] expected = ["val3", "val4"];
            Assert.That(output, Is.EquivalentTo(expected));
        }

        [Test]
        public void FromFilterTest()
        {
            FromFilter filter = new("val2");
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            string[] expected = ["val2", "val3", "val4"];
            Assert.That(output, Is.EquivalentTo(expected));
        }

        [Test]
        public void FromFilterTest_WithUnmatchingCode_ReturnsEmpty()
        {
            FromFilter filter = new("val5");
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            string[] expected = [];
            Assert.That(output, Is.EquivalentTo(expected));
        }

        [Test]
        public void AllFilterTest()
        {
            AllFilter filter = new();
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            string[] expected = ["val0", "val1", "val2", "val3", "val4"];
            Assert.That(output, Is.EquivalentTo(expected));
        }

        [Test]
        public void ItemFilterTest()
        {
            ItemFilter filter = new(["val1", "val2"]);
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            string[] expected = ["val1", "val2"];
            Assert.That(output, Is.EquivalentTo(expected));
        }

        private readonly List<string> codes = ["val0", "val1", "val2", "val3", "val4"];

        [Test]
        public void TopFilter_FilterCodes_ReturnsLastN()
        {
            TopFilter filter = new(2);
            IEnumerable<string> output = filter.Filter(codes);
            string[] expected = ["val3", "val4"];
            Assert.That(output, Is.EquivalentTo(expected));
        }

        [Test]
        public void FromFilter_FilterCodes_ReturnsFromCode()
        {
            FromFilter filter = new("val2");
            IEnumerable<string> output = filter.Filter(codes);
            string[] expected = ["val2", "val3", "val4"];
            Assert.That(output, Is.EquivalentTo(expected));
        }

        [Test]
        public void FromFilter_FilterCodes_WithUnmatchingCode_ReturnsEmpty()
        {
            FromFilter filter = new("val5");
            IEnumerable<string> output = filter.Filter(codes);
            Assert.That(output, Is.Empty);
        }

        [Test]
        public void AllFilter_FilterCodes_ReturnsAll()
        {
            AllFilter filter = new();
            IEnumerable<string> output = filter.Filter(codes);
            string[] expected = ["val0", "val1", "val2", "val3", "val4"];
            Assert.That(output, Is.EquivalentTo(expected));
        }

        [Test]
        public void ItemFilter_FilterCodes_ReturnsMatchingCodes()
        {
            ItemFilter filter = new(["val1", "val2"]);
            IEnumerable<string> output = filter.Filter(codes);
            string[] expected = ["val1", "val2"];
            Assert.That(output, Is.EquivalentTo(expected));
        }

        [Test]
        public void InverseItemFilterTest()
        {
            InverseItemFilter filter = new(["val1", "val2"]);
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            string[] expected = ["val0", "val3", "val4"];
            Assert.That(output, Is.EqualTo(expected));
        }

        [Test]
        public void InverseItemFilterTest_WithEmptyCodes_ReturnsAll()
        {
            InverseItemFilter filter = new([]);
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            string[] expected = ["val0", "val1", "val2", "val3", "val4"];
            Assert.That(output, Is.EqualTo(expected));
        }

        [Test]
        public void InverseItemFilter_FilterCodes_ReturnsNonMatchingCodes()
        {
            InverseItemFilter filter = new(["val1", "val2"]);
            IEnumerable<string> output = filter.Filter(codes);
            string[] expected = ["val0", "val3", "val4"];
            Assert.That(output, Is.EqualTo(expected));
        }

        [Test]
        public void InverseItemFilter_FilterCodes_WithEmptyCodes_ReturnsAll()
        {
            InverseItemFilter filter = new([]);
            IEnumerable<string> output = filter.Filter(codes);
            Assert.That(output, Is.EqualTo(codes));
        }

        [Test]
        public void RegexFilterTest_SubstringMatch()
        {
            RegexFilter filter = new("al[1-3]");
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            string[] expected = ["val1", "val2", "val3"];
            Assert.That(output, Is.EqualTo(expected));
        }

        [Test]
        public void RegexFilterTest_IsCaseSensitive()
        {
            RegexFilter filter = new("VAL1");
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            Assert.That(output, Is.Empty);
        }

        [Test]
        public void RegexFilterTest_WithAnchors_MatchesWholeCodeOnly()
        {
            RegexFilter filter = new("^val1$");
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            string[] expected = ["val1"];
            Assert.That(output, Is.EqualTo(expected));
        }

        [Test]
        public void RegexFilterTest_WithInvalidPattern_ReturnsEmpty()
        {
            RegexFilter filter = new("[invalid");
            IEnumerable<string> output = filter.Filter(values).Select(v => v.Code);
            Assert.That(output, Is.Empty);
        }

        [Test]
        public void RegexFilter_FilterCodes_SubstringMatch()
        {
            RegexFilter filter = new("al[1-3]");
            IEnumerable<string> output = filter.Filter(codes);
            string[] expected = ["val1", "val2", "val3"];
            Assert.That(output, Is.EqualTo(expected));
        }

        [Test]
        public void RegexFilter_FilterCodes_IsCaseSensitive()
        {
            RegexFilter filter = new("VAL1");
            IEnumerable<string> output = filter.Filter(codes);
            Assert.That(output, Is.Empty);
        }

        [Test]
        public void RegexFilter_FilterCodes_WithInvalidPattern_ReturnsEmpty()
        {
            RegexFilter filter = new("[invalid");
            IEnumerable<string> output = filter.Filter(codes);
            Assert.That(output, Is.Empty);
        }
    }
}
