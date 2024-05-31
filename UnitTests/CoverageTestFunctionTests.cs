using NUnit.Framework;

namespace UnitTests
{
    internal class CoverageTestFunctionTests
    {
        [Test]
        public void TestCoverageTestFunctionA()
        {
            int input = 5;
            int expected = 10;
            int actual = PxGraf.CoverageTestFunctions.CoveredTestFunction(input);
            Assert.That(expected, Is.EqualTo(actual));
        }
    }
}
