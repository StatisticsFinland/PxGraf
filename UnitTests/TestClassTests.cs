using NUnit.Framework;

namespace UnitTests
{
    public class TestClassTests
    {
        [Test]
        public void TestMethodATest()
        {
            var testClass = new PxGraf.TestClass();
            Assert.That(testClass.TestMethodA(1), Is.EqualTo(2));
        }
    }
}