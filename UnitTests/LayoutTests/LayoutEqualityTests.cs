using NUnit.Framework;
using PxGraf.Models.Queries;
using System.Collections.Generic;

namespace UnitTests.LayoutTests
{
    internal class LayoutEqualityTests
    {
        [Test]
        public void Equals_IdenticalLayouts_ReturnsTrue()
        {
            Layout layout1 = new()
            {
                RowDimensionCodes = ["foo", "bar"],
                ColumnDimensionCodes = ["foo", "bar"],
            };

            Layout layout2 = new()
            {
                RowDimensionCodes = ["foo", "bar"],
                ColumnDimensionCodes = ["foo", "bar"],
            };

            Assert.That(layout1, Is.EqualTo(layout2));
        }

        [Test]
        public void Equals_DifferentOrder_ReturnsFalse()
        {
            Layout layout1 = new()
            {
                RowDimensionCodes = ["foo", "bar"],
                ColumnDimensionCodes = ["foo", "bar"]
            };

            Layout layout2 = new()
            {
                RowDimensionCodes = ["bar", "foo"],
                ColumnDimensionCodes = ["foo", "bar"]
            };

            Assert.That(layout1, Is.Not.EqualTo(layout2));
        }

        [Test]
        public void Equals_DifferentData_ReturnsFalse()
        {
            Layout layout1 = new()
            {
                RowDimensionCodes = ["foo", "bar"],
                ColumnDimensionCodes = ["foo", "bar"]
            };

            Layout layout2 = new()
            {
                RowDimensionCodes = ["xxx", "bar"],
                ColumnDimensionCodes = ["foo", "bar"]
            };

            Assert.That(layout1, Is.Not.EqualTo(layout2));
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            Layout layout1 = new()
            {
                RowDimensionCodes = ["foo", "bar"],
                ColumnDimensionCodes = ["foo", "bar"]
            };

            List<string> layout2 = ["foo", "bar"];

            Assert.That(layout1, Is.Not.EqualTo(layout2));
        }

        [Test]
        public void Equals_OtherParameterIsNull_ReturnsFalse()
        {
            Layout layout1 = new()
            {
                RowDimensionCodes = ["foo", "bar"],
                ColumnDimensionCodes = ["foo", "bar"]
            };

            Layout layout2 = null;

            Assert.That(layout1, Is.Not.EqualTo(layout2));
        }
    }
}
