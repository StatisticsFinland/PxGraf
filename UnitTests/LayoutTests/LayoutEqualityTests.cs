using NUnit.Framework;
using PxGraf.Models.Queries;
using System.Collections.Generic;

namespace LayoutTests
{
    internal class LayoutEqualityTests
    {
        [Test]
        public void Equals_IdenticalLayouts_ReturnsTrue()
        {
            Layout layout1 = new()
            {
                RowVariableCodes = ["foo", "bar"],
                ColumnVariableCodes = ["foo", "bar"],
            };

            Layout layout2 = new()
            {
                RowVariableCodes = ["foo", "bar"],
                ColumnVariableCodes = ["foo", "bar"],
            };

            Assert.IsTrue(layout1.Equals(layout2));
        }

        [Test]
        public void Equals_DifferentOrder_ReturnsFalse()
        {
            Layout layout1 = new()
            {
                RowVariableCodes = ["foo", "bar"],
                ColumnVariableCodes = ["foo", "bar"]
            };

            Layout layout2 = new()
            {
                RowVariableCodes = ["bar", "foo"],
                ColumnVariableCodes = ["foo", "bar"]
            };

            Assert.IsFalse(layout1.Equals(layout2));
        }

        [Test]
        public void Equals_DifferentData_ReturnsFalse()
        {
            Layout layout1 = new()
            {
                RowVariableCodes = ["foo", "bar"],
                ColumnVariableCodes = ["foo", "bar"]
            };

            Layout layout2 = new()
            {
                RowVariableCodes = ["xxx", "bar"],
                ColumnVariableCodes = ["foo", "bar"]
            };

            Assert.IsFalse(layout1.Equals(layout2));
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            Layout layout1 = new()
            {
                RowVariableCodes = ["foo", "bar"],
                ColumnVariableCodes = ["foo", "bar"]
            };

            List<string> layout2 = ["foo", "bar"];

            Assert.IsFalse(layout1.Equals(layout2));
        }

        [Test]
        public void Equals_OtherParameterIsNull_ReturnsFalse()
        {
            Layout layout1 = new()
            {
                RowVariableCodes = ["foo", "bar"],
                ColumnVariableCodes = ["foo", "bar"]
            };

            Layout layout2 = null;

            Assert.IsFalse(layout1.Equals(layout2));
        }
    }
}
