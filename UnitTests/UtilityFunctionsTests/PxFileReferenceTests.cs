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
            List<string> hierarchy = new() { "DB", "folder1", "folder2" };
            PxFileReference sample = new(hierarchy, "foobar");

            Assert.AreEqual("DB/folder1/folder2/foobar", sample.ToPath());
        }
    }
}
