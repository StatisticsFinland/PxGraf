using NUnit.Framework;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.IO;

namespace UnitTests.UtilityFunctionsTests
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

        [Test]
        public void FromPathConstructorTest()
        {
            string path = Path.Combine("DB", "folder1", "folder2", "foobar.px");
            PxTableReference sample = new(path);

            Assert.That(sample.Hierarchy, Is.EqualTo(new List<string> { "DB", "folder1", "folder2" }));
            Assert.That(sample.Name, Is.EqualTo("foobar.px"));
        }

        [Test]
        public void FromPathWithLeadingSeparatorConstructorTest()
        {
            string path = Path.Combine("DB", "folder1", "folder2", "foobar.px");
            path = Path.DirectorySeparatorChar + path;
            PxTableReference sample = new(path);

            Assert.That(sample.Hierarchy, Is.EqualTo(new List<string> { "DB", "folder1", "folder2" }));
            Assert.That(sample.Name, Is.EqualTo("foobar.px"));
        }
    }
}
