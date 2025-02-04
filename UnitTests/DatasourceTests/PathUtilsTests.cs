using NUnit.Framework;
using PxGraf.Datasource.DatabaseConnection;
using PxGraf.Datasource.FileDatasource;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public void TraversalAttemptWithReferenceThrowsUnauthorizedAccessException()
        {
            string rootPath = Path.Combine("C:", "Foo");
            PxTableReference reference = new ([ "..", "Users", "Public" ], "file.px");
            Assert.Throws<UnauthorizedAccessException>(() => PathUtils.BuildAndSanitizePath(rootPath, reference));
        }

        [Test]
        public void DatabaseIsWhitelistedWithEmptyWhitelistReturnsTrue()
        {
            string[] databaseWhitelist = [];
            LocalFilesystemDatabaseConfig config = new(true, $"path{Path.DirectorySeparatorChar}to{Path.DirectorySeparatorChar}database", Encoding.Default, databaseWhitelist);
            Assert.That(PathUtils.IsDatabaseWhitelisted(["foo", "bar"], config), Is.True);
        }

        [Test]
        public void DatabaseIsWhitelistedWithWhitelistedDatabaseReturnsTrue()
        {
            string[] databaseWhitelist = ["foo"];
            LocalFilesystemDatabaseConfig config = new(true, $"path{Path.DirectorySeparatorChar}to{Path.DirectorySeparatorChar}database", Encoding.Default, databaseWhitelist);
            Assert.That(PathUtils.IsDatabaseWhitelisted(["foo", "bar"], config), Is.True);
        }

        [Test]
        public void DatabaseIsWhitelistedWithNonWhitelistedDatabaseThrowsDirectoryNotFoundException()
        {
            string[] databaseWhitelist = ["baz"];
            LocalFilesystemDatabaseConfig config = new(true, $"path{Path.DirectorySeparatorChar}to{Path.DirectorySeparatorChar}database", Encoding.Default, databaseWhitelist);
            Assert.Throws<DirectoryNotFoundException>(() => PathUtils.IsDatabaseWhitelisted(["foo", "bar"], config));
        }

        [Test]
        public void DatabaseIsWhitelistedWithSubgroupNameAsWhitelisteddDatabaseThrowsDirectoryNotFoundException()
        {
            string[] databaseWhitelist = ["foo"];
            LocalFilesystemDatabaseConfig config = new(true, $"path{Path.DirectorySeparatorChar}to{Path.DirectorySeparatorChar}database", Encoding.Default, databaseWhitelist);
            Assert.Throws<DirectoryNotFoundException>(() => PathUtils.IsDatabaseWhitelisted(["bar", "foo"], config));
        }

        [Test]
        public void DatabaseIsWhitelistedWithOnlyInvalidDatabaseNameReturnsFalse()
        {
            string[] databaseWhitelist = ["foo"];
            LocalFilesystemDatabaseConfig config = new(true, $"path{Path.DirectorySeparatorChar}to{Path.DirectorySeparatorChar}database", Encoding.Default, databaseWhitelist);
            Assert.That(PathUtils.IsDatabaseWhitelisted("bar", config, false), Is.False);
        }
    }
}
