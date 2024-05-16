using NUnit.Framework;
using PxGraf.ChartTypeSelection;
using System;

namespace RangeClassTests
{
    [TestFixture]
    class DimensionRangeTests
    {
        [Test]
        public void IgnoreTest()
        {
            var r = new DimensionRange("Ignore");
            Assert.True(r.Min == 0 && r.Max > 1000000);
        }

        [Test]
        public void NotAllowedTest()
        {
            var r = new DimensionRange("Not allowed");
            Assert.IsTrue(r.DimensionNotAllowed);
        }

        [Test]
        public void TestValue0Min()
        {
            var r = new DimensionRange("0");
            Assert.AreEqual(0, r.Min);
        }

        [Test]
        public void TestValue0Max()
        {
            var r = new DimensionRange("0");
            Assert.AreEqual(0, r.Max);
        }

        [Test]
        public void TestValue1Min()
        {
            var r = new DimensionRange("1");
            Assert.AreEqual(1, r.Min);
        }

        [Test]
        public void TestValue1Max()
        {
            var r = new DimensionRange("1");
            Assert.AreEqual(1, r.Max);
        }

        [Test]
        public void TestValue2Min()
        {
            var r = new DimensionRange("2");
            Assert.AreEqual(2, r.Min);
        }

        [Test]
        public void TestValue2Max()
        {
            var r = new DimensionRange("2");
            Assert.AreEqual(2, r.Max);
        }

        [Test]
        public void TestRange0To1()
        {
            var r = new DimensionRange("0-1");
            Assert.True(r.Min == 0 && r.Max == 1 && !r.Ignore);
        }

        [Test]
        public void TestRange0To2()
        {
            var r = new DimensionRange("0-2");
            Assert.True(r.Min == 0 && r.Max == 2 && !r.Ignore);
        }

        [Test]
        public void TestRange1To10()
        {
            var r = new DimensionRange("1-10");
            Assert.True(r.Min == 1 && r.Max == 10 && !r.Ignore);
        }

        [Test]
        public void TestRange5To10()
        {
            var r = new DimensionRange("5-10");
            Assert.True(r.Min == 5 && r.Max == 10 && !r.Ignore);
        }

        [Test]
        public void TestRange1To999()
        {
            var r = new DimensionRange("1-999");
            Assert.True(r.Min == 1 && r.Max == 999 && !r.Ignore);
        }

        [Test]
        public void TestInvalidMinGreaterThanMax()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { new DimensionRange("10-5");});
        }

        [Test]
        public void TestInvalidTooManyValus()
        {
            Assert.Throws<ArgumentException>(() => { new DimensionRange("1-5-6"); });
        }

        [Test]
        public void TestInvalidInput()
        {
            Assert.Throws<ArgumentException>(() => { new DimensionRange("foobar"); });
        }
    }
}
