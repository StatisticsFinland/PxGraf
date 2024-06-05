using NUnit.Framework;
using PxGraf.Data.MetaData;
using System.Collections.Generic;
using PxGraf.Models.Queries;
using System.Linq;

namespace UnitTests.ModelTests
{
    internal class ValueFilterTests
    {
        private readonly List<IReadOnlyVariableValue> values =
        [
            new VariableValue("val0", null, null, false),
            new VariableValue("val1", null, null, false),
            new VariableValue("val2", null, null, false),
            new VariableValue("val3", null, null, false),
            new VariableValue("val4", null, null, false)
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
