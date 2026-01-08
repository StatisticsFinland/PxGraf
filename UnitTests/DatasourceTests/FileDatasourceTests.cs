using NUnit.Framework;
using Moq;
using PxGraf.Datasource.FileDatasource;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using PxGraf.Models.Responses.DatabaseItems;

namespace UnitTests.DatasourceTests
{
    /// <summary>
    /// Unit tests for FileDatasource.
    /// </summary>
    [TestFixture]
    public class FileDatasourceTests
    {
        private Mock<IFileSystem> mockFileSystem;
        private FileDatasource datasource;
        private const string testRootPath = "/test/root";

        [SetUp]
        public void Setup()
        {
            mockFileSystem = new Mock<IFileSystem>();
            datasource = new FileDatasource(mockFileSystem.Object, testRootPath);
        }

        [Test]
        public void Constructor_WithValidParameters_DoesNotThrow()
        {
            // Arrange & Act & Assert
            Assert.That(() => new FileDatasource(mockFileSystem.Object, testRootPath), Throws.Nothing);
        }

        [Test]
        public void Constructor_WithNullFileSystem_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.That(() => new FileDatasource(null, testRootPath), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_WithNullRootPath_DoesNotThrow()
        {
            // Arrange, Act & Assert - null rootPath should be converted to empty string
            Assert.That(() => new FileDatasource(mockFileSystem.Object, null), Throws.Nothing);
        }

        [Test]
        public void Constructor_WithEmptyRootPath_DoesNotThrow()
        {
            // Arrange, Act & Assert
            Assert.That(() => new FileDatasource(mockFileSystem.Object, ""), Throws.Nothing);
        }

        [Test]
        public async Task GetTablesAsync_WithValidHierarchy_ReturnsExpectedTables()
        {
            // Arrange
            List<string> groupHierarchy = ["level1", "level2"];
            List<string> mockFiles = ["/test/root/level1/level2/table1.px", "/test/root/level1/level2/table2.px"];

            mockFileSystem.Setup(fs => fs.EnumerateFilesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockFiles);
            mockFileSystem.Setup(fs => fs.GetRelativePath(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((basePath, targetPath) => Path.GetRelativePath(basePath, targetPath));

            // Act
            List<PxTableReference> result = await datasource.GetTablesAsync(groupHierarchy);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetGroupHeadersAsync_WithValidHierarchy_ReturnsExpectedGroups()
        {
            // Arrange
            List<string> groupHierarchy = ["level1"];
            List<string> mockDirectories = ["/test/root/level1/subgroup1", "/test/root/level1/subgroup2"];
            List<string> mockAliasFiles = ["Alias_en.txt", "Alias_fi.txt"];

            mockFileSystem.Setup(fs => fs.EnumerateDirectoriesAsync(It.IsAny<string>()))
                .ReturnsAsync(mockDirectories);
            mockFileSystem.Setup(fs => fs.GetDirectoryName(It.IsAny<string>()))
                .Returns<string>(path => Path.GetFileName(path));
            mockFileSystem.Setup(fs => fs.EnumerateFilesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockAliasFiles);
            mockFileSystem.Setup(fs => fs.GetFileName(It.IsAny<string>()))
                .Returns<string>(path => Path.GetFileName(path));
            mockFileSystem.Setup(fs => fs.ReadAllTextAsync(It.IsAny<string>()))
                .ReturnsAsync("Test Group");

            // Act
            List<DatabaseGroupHeader> result = await datasource.GetGroupHeadersAsync(groupHierarchy);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithRootPath_SetsRootPathCorrectly()
        {
            // Act
            FileDatasource datasourceWithRoot = new(mockFileSystem.Object, testRootPath);

            // Assert - Verify the datasource works with the root path
            Assert.That(() => datasourceWithRoot.GetTablesAsync([]), Throws.Nothing);
        }
    }

    /// <summary>
    /// Unit tests for BlobContainerDatabaseConfig.
    /// </summary>
    [TestFixture]
    public class BlobContainerDatabaseConfigTests
    {
        [Test]
        public void Constructor_WithValidParameters_SetsProperties()
        {
            // Arrange
            const bool enabled = true;
            const string storageAccountName = "teststorageaccount";
            const string containerName = "testcontainer";
            const string rootPath = "database/";

            // Act
            BlobContainerDatabaseConfig config = new(enabled, storageAccountName, containerName, rootPath);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.Enabled, Is.EqualTo(enabled));
                Assert.That(config.StorageAccountName, Is.EqualTo(storageAccountName));
                Assert.That(config.ContainerName, Is.EqualTo(containerName));
                Assert.That(config.RootPath, Is.EqualTo(rootPath));
            });
        }

        [Test]
        public void Constructor_WithoutRootPath_SetsEmptyRootPath()
        {
            // Arrange
            const bool enabled = true;
            const string storageAccountName = "teststorageaccount";
            const string containerName = "testcontainer";

            // Act
            BlobContainerDatabaseConfig config = new(enabled, storageAccountName, containerName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.Enabled, Is.EqualTo(enabled));
                Assert.That(config.StorageAccountName, Is.EqualTo(storageAccountName));
                Assert.That(config.ContainerName, Is.EqualTo(containerName));
                Assert.That(config.RootPath, Is.EqualTo(""));
            });
        }

        [Test]
        public void Constructor_WithNullRootPath_SetsEmptyRootPath()
        {
            // Arrange
            const bool enabled = true;
            const string storageAccountName = "teststorageaccount";
            const string containerName = "testcontainer";

            // Act
            BlobContainerDatabaseConfig config = new(enabled, storageAccountName, containerName, null);

            // Assert
            Assert.That(config.RootPath, Is.EqualTo(""));
        }

        [Test]
        public void Constructor_WithDisabledFlag_SetsEnabledToFalse()
        {
            // Arrange
            const bool enabled = false;
            const string storageAccountName = "teststorageaccount";
            const string containerName = "testcontainer";

            // Act
            BlobContainerDatabaseConfig config = new(enabled, storageAccountName, containerName);

            // Assert
            Assert.That(config.Enabled, Is.False);
        }

        [Test]
        public void Constructor_WithEmptyStorageAccountName_SetsProperty()
        {
            // Arrange
            const bool enabled = true;
            const string storageAccountName = "";
            const string containerName = "testcontainer";

            // Act
            BlobContainerDatabaseConfig config = new(enabled, storageAccountName, containerName);

            // Assert
            Assert.That(config.StorageAccountName, Is.EqualTo(storageAccountName));
        }

        [Test]
        public void Constructor_WithEmptyContainerName_SetsProperty()
        {
            // Arrange
            const bool enabled = true;
            const string storageAccountName = "teststorageaccount";
            const string containerName = "";

            // Act
            BlobContainerDatabaseConfig config = new(enabled, storageAccountName, containerName);

            // Assert
            Assert.That(config.ContainerName, Is.EqualTo(containerName));
        }
    }

    /// <summary>
    /// Unit tests for LocalFilesystemDatabaseConfig.
    /// </summary>
    [TestFixture]
    public class LocalFilesystemDatabaseConfigTests
    {
        [Test]
        public void Constructor_WithValidParameters_SetsProperties()
        {
            // Arrange
            const bool enabled = true;
            const string databaseRootPath = "/test/path";
            Encoding encoding = Encoding.UTF8;

            // Act
            LocalFilesystemDatabaseConfig config = new(enabled, databaseRootPath, encoding);

            // Assert
            Assert.Multiple(() =>
           {
               Assert.That(config.Enabled, Is.EqualTo(enabled));
               Assert.That(config.DatabaseRootPath, Is.EqualTo(databaseRootPath));
               Assert.That(config.Encoding, Is.EqualTo(encoding));
           });
        }

        [Test]
        public void Constructor_WithDisabledFlag_SetsEnabledToFalse()
        {
            // Arrange
            const bool enabled = false;
            const string databaseRootPath = "/test/path";
            Encoding encoding = Encoding.UTF8;

            // Act
            LocalFilesystemDatabaseConfig config = new(enabled, databaseRootPath, encoding);

            // Assert
            Assert.That(config.Enabled, Is.False);
        }
    }
}