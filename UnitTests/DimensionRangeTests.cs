using NUnit.Framework;
using PxGraf.ChartTypeSelection;
using System;

namespace UnitTests
{
    [TestFixture]
    public class DimensionRangeTests
    {
        [Test]
        public void IgnoreTest()
        {
            var r = new DimensionRange("Ignore");
            Assert.That(r.Min, Is.EqualTo(0));
            Assert.That(r.Max, Is.GreaterThan(1000000));
        }

        [Test]
        public void NotAllowedTest()
        {
            var r = new DimensionRange("Not allowed");
            Assert.That(r.DimensionNotAllowed, Is.True);
        }

        [Test]
        public void TestValue0Min()
        {
            var r = new DimensionRange("0");
            Assert.That(r.Min, Is.EqualTo(0));
        }

        [Test]
        public void TestValue0Max()
        {
            var r = new DimensionRange("0");
            Assert.That(r.Max, Is.EqualTo(0));
        }

        [Test]
        public void TestValue1Min()
        {
            var r = new DimensionRange("1");
            Assert.That(r.Min, Is.EqualTo(1));
        }

        [Test]
        public void TestValue1Max()
        {
            var r = new DimensionRange("1");
            Assert.That(r.Max, Is.EqualTo(1));
        }

        [Test]
        public void TestValue2Min()
        {
            var r = new DimensionRange("2");
            Assert.That(r.Min, Is.EqualTo(2));
        }

        [Test]
        public void TestValue2Max()
        {
            var r = new DimensionRange("2");
            Assert.That(r.Max, Is.EqualTo(2));
        }

        [Test]
        public void TestRange0To1()
        {
            var r = new DimensionRange("0-1");
            Assert.That(r.Min, Is.EqualTo(0));
            Assert.That(r.Max, Is.EqualTo(1));
            Assert.That(r.Ignore, Is.False);
        }

        [Test]
        public void TestRange0To2()
        {
            var r = new DimensionRange("0-2");
            Assert.That(r.Min, Is.EqualTo(0));
            Assert.That(r.Max, Is.EqualTo(2));
            Assert.That(r.Ignore, Is.False);
        }

        [Test]
        public void TestRange1To10()
        {
            var r = new DimensionRange("1-10");
            Assert.That(r.Min, Is.EqualTo(1));
            Assert.That(r.Max, Is.EqualTo(10));
            Assert.That(r.Ignore, Is.False);
        }

        [Test]
        public void TestRange5To10()
        {
            var r = new DimensionRange("5-10");
            Assert.That(r.Min, Is.EqualTo(5));
            Assert.That(r.Max, Is.EqualTo(10));
            Assert.That(r.Ignore, Is.False);
        }

        [Test]
        public void TestRange1To999()
        {
            var r = new DimensionRange("1-999");
            Assert.That(r.Min, Is.EqualTo(1));
            Assert.That(r.Max, Is.EqualTo(999));
            Assert.That(r.Ignore, Is.False);
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
