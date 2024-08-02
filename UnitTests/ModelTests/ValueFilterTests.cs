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
    }
}
