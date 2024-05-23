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
            List<string> first = ["A", "B"];
            List<string> second = ["1", "2"];
            List<List<string>> set = [first, second];

            List<List<string>> product =
            [
                ["A", "1"],
                ["A", "2"],
                ["B", "1"],
                ["B", "2"]
            ];

            Assert.That(UtilityFunctions.CartesianProduct(set), Is.EqualTo(product));
        }

        [Test]
        public void CartesianProductEmptyTest()
        {
            List<List<string>> empty = [];
            List<List<string>> product = [[]];

            Assert.That(UtilityFunctions.CartesianProduct(empty), Is.EqualTo(product));
        }

        [Test]
        public void CartesianProductOneTest()
        {
            List<List<string>> test = [["test"]];

            Assert.That(UtilityFunctions.CartesianProduct(test), Is.EqualTo(test));
        }

        [Test]
        public void CartesianProductSetTest()
        {
            List<List<string>> set =
            [
                ["A", "B"],
                ["1", "2"],
                ["?", "!"]
            ];


            List<List<string>> product =
            [
                ["A", "1", "?"],
                ["A", "1", "!"],
                ["A", "2", "?"],
                ["A", "2", "!"],
                ["B", "1", "?"],
                ["B", "1", "!"],
                ["B", "2", "?"],
                ["B", "2", "!"]
            ];

            Assert.That(UtilityFunctions.CartesianProduct(set), Is.EqualTo(product));
        }
    }
}