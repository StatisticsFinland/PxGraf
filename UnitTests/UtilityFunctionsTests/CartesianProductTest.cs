using NUnit.Framework;
using PxGraf.Utility;
using System.Collections.Generic;

namespace UtilityFunctionsTests
{
    class CartesianProductTest
    {
        [Test]
        public void CartesianProductBasicCaseTest()
        {
            List<string> first = new() { "A", "B" };
            List<string> second = new() { "1", "2" };
            List<List<string>> set = new() { first, second };

            List<List<string>> product = new()
            {
                new() { "A", "1" },
                new() { "A", "2" },
                new() { "B", "1" },
                new() { "B", "2" }
            };

            Assert.AreEqual(product, UtilityFunctions.CartesianProduct(set));
        }

        [Test]
        public void CartesianProductEmptyTest()
        {
            List<List<string>> empty = new();
            List<List<string>> product = new() { new() };

            Assert.AreEqual(product, UtilityFunctions.CartesianProduct(empty));
        }

        [Test]
        public void CartesianProductOneTest()
        {
            List<List<string>> test = new() { new() { "test" } };

            Assert.AreEqual(test, UtilityFunctions.CartesianProduct(test));
        }

        [Test]
        public void CartesianProductSetTest()
        {
            List<List<string>> set = new()
            {
                new() { "A", "B" },
                new() { "1", "2" },
                new() { "?", "!" }
            };


            List<List<string>> product = new()
            {
                new() { "A", "1", "?" },
                new() { "A", "1", "!" },
                new() { "A", "2", "?" },
                new() { "A", "2", "!" },
                new() { "B", "1", "?" },
                new() { "B", "1", "!" },
                new() { "B", "2", "?" },
                new() { "B", "2", "!" }
            };

            Assert.AreEqual(product, UtilityFunctions.CartesianProduct(set));
        }
    }
}