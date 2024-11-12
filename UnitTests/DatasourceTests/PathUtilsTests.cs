using NUnit.Framework;
using PxGraf.Datasource.FileDatasource;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;

namespace UnitTests.DatasourceTests
{
    public class PathUtilsTests
    {
        [Test]
        public void ValidHierarcyWithRoothEndingWithSlashReturnsValidPath()
        {
            string rootPath = "C:\\Foo\\";
            IReadOnlyList<string> groupHierarcy = ["Users", "Public"];
            string expected = "C:\\Foo\\Users\\Public";
            string actual = PathUtils.BuildAndSanitizePath(rootPath, groupHierarcy);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ValidHierarcyWithReturnsValidPath()
        {
            string rootPath = "C:\\Foo";
            IReadOnlyList<string> groupHierarcy = ["Users", "Public"];
            string expected = "C:\\Foo\\Users\\Public";
            string actual = PathUtils.BuildAndSanitizePath(rootPath, groupHierarcy);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TraversalAttemptThrowsUnauthorizedAccessException()
        {
            string rootPath = "C:\\Foo";
            IReadOnlyList<string> groupHierarcy = ["..", "Users", "Public"];
            Assert.Throws<UnauthorizedAccessException>(() => PathUtils.BuildAndSanitizePath(rootPath, groupHierarcy));
        }

        [Test]
        public void ValidReferenceWithRoothEndingWithSlashReturnsValidPath()
        {
            string rootPath = "C:\\Foo\\";
            PxTableReference reference = new(["Users", "Public"], "file.px");
            string expected = "C:\\Foo\\Users\\Public\\file.px";
            string actual = PathUtils.BuildAndSanitizePath(rootPath, reference);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ValidReferenceWithReturnsValidPath()
        {
            string rootPath = "C:\\Foo";
            PxTableReference reference = new(["Users", "Public"], "file.px");
            string expected = "C:\\Foo\\Users\\Public\\file.px";
            string actual = PathUtils.BuildAndSanitizePath(rootPath, reference);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TraversalAttemptWithReferenceThrowsUnauthorizedAccessException()
        {
            string rootPath = "C:\\Foo";
            PxTableReference reference = new(["..", "Users", "Public"], "file.px");
            Assert.Throws<UnauthorizedAccessException>(() => PathUtils.BuildAndSanitizePath(rootPath, reference));
        }
    }
}
