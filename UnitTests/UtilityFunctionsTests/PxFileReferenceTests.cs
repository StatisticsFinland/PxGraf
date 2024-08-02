using NUnit.Framework;
using PxGraf.Models.Queries;
using System.Collections.Generic;

namespace UtilityFunctionsTests
{
    internal class PxFileReferenceTests
    {
        [Test]
        public void ToPathTest()
        {
            List<string> hierarchy = ["DB", "folder1", "folder2"];
            PxTableReference sample = new(hierarchy, "foobar");

            Assert.That(sample.ToPath(), Is.EqualTo("DB/folder1/folder2/foobar"));
        }
    }
}
