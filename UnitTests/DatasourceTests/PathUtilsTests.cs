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
