using NUnit.Framework;
using PxGraf.Datasource.FileDatasource;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTests.DatasourceTests
{
    public class PathUtilsTests
    {
        [Test]
        public void ValidHierarcyWithRoothEndingWithSlashReturnsValidPath()
        {
            string rootPath = Path.Combine("C:", "Foo", "");
            IReadOnlyList<string> groupHierarcy = [ "Users", "Public" ];
            string expected = Path.GetFullPath(Path.Combine("C:", "Foo", "Users", "Public"));
            string actual = PathUtils.BuildAndSanitizePath(rootPath, groupHierarcy);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ValidHierarcyWithReturnsValidPath()
        {
            string rootPath = Path.Combine("C:", "Foo");
            IReadOnlyList<string> groupHierarcy = [ "Users", "Public" ];
            string expected = Path.GetFullPath(Path.Combine("C:", "Foo", "Users", "Public"));
            string actual = PathUtils.BuildAndSanitizePath(rootPath, groupHierarcy);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TraversalAttemptThrowsUnauthorizedAccessException()
        {
            string rootPath = Path.Combine("C:", "Foo");
            IReadOnlyList<string> groupHierarcy = [ "..", "Users", "Public" ];
            Assert.Throws<UnauthorizedAccessException>(() => PathUtils.BuildAndSanitizePath(rootPath, groupHierarcy));
        }

        [Test]
        public void ValidReferenceWithRoothEndingWithSlashReturnsValidPath()
        {
            string rootPath = Path.Combine("C:", "Foo", "");
            PxTableReference reference = new (["Users", "Public" ], "file.px");
            string expected = Path.GetFullPath(Path.Combine("C:", "Foo", "Users", "Public", "file.px"));
            string actual = PathUtils.BuildAndSanitizePath(rootPath, reference);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ValidReferenceWithReturnsValidPath()
        {
            string rootPath = Path.Combine("C:", "Foo");
            PxTableReference reference = new ([ "Users", "Public" ], "file.px");
            string expected = Path.GetFullPath(Path.Combine("C:", "Foo", "Users", "Public", "file.px"));
            string actual = PathUtils.BuildAndSanitizePath(rootPath, reference);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TraversalAttemptWithReferenceThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => { 
                PxTableReference _ = new ([ "..", "Users", "Public" ], "file.px");
            });
        }

        [Test]
        public void DatabaseIsWhitelistedWithEmptyWhitelistReturnsTrue()
        {
            string[] databaseWhitelist = [];
            Assert.That(PathUtils.IsDatabaseWhitelisted(["foo", "bar"], databaseWhitelist), Is.True);
        }

        [Test]
        public void DatabaseIsWhitelistedWithWhitelistedDatabaseReturnsTrue()
        {
            string[] databaseWhitelist = ["foo"];
            Assert.That(PathUtils.IsDatabaseWhitelisted(["foo", "bar"], databaseWhitelist), Is.True);
        }

        [Test]
        public void DatabaseIsWhitelistedWithNonWhitelistedDatabaseReturnsFalse()
        {
            string[] databaseWhitelist = ["baz"];
            Assert.That(PathUtils.IsDatabaseWhitelisted(["foo", "bar"], databaseWhitelist), Is.False);
        }

        [Test]
        public void DatabaseIsWhitelistedWithSubgroupNameAsWhitelisteddDatabaseReturnsFalse()
        {
            string[] databaseWhitelist = ["foo"];
            Assert.That(PathUtils.IsDatabaseWhitelisted(["bar", "foo"], databaseWhitelist), Is.False);
        }

        [Test]
        public void DatabaseIsWhitelistedWithOnlyInvalidDatabaseNameReturnsFalse()
        {
            string[] databaseWhitelist = ["foo"];
            Assert.That(PathUtils.IsDatabaseWhitelisted("bar", databaseWhitelist), Is.False);
        }
    }
}
