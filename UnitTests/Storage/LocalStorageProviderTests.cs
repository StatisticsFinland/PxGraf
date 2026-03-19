using NUnit.Framework;
using NUnit.Framework.Legacy;
using PxGraf.Storage;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Storage
{
    [TestFixture]
    public class LocalStorageProviderTests
    {
        private string tempRoot;
        private LocalStorageProvider provider;

        [SetUp]
        public void SetUp()
        {
            tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);
            provider = new LocalStorageProvider(Encoding.UTF8);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, true);
        }

        [Test]
        public void BuildPath_TraversalOutsideRoot_ThrowsUnauthorizedAccessException()
        {
            Assert.Throws<UnauthorizedAccessException>(() =>
                provider.BuildPath(tempRoot, "../outside.txt")
            );
        }

        [Test]
        public void BuildPath_PrefixBypass_ThrowsUnauthorizedAccessException()
        {
            string sneaky = tempRoot + "_notroot";
            Assert.Throws<UnauthorizedAccessException>(() =>
                provider.BuildPath(tempRoot, Path.Combine("..", Path.GetFileName(sneaky)))
            );
        }

        [Test]
        public void BuildPath_PrefixBypassWithExistingDir_ThrowsUnauthorizedAccessException()
        {
            string prefixDir = tempRoot + "bar";
            Directory.CreateDirectory(prefixDir);
            string userPath = Path.Combine(prefixDir, "file.txt");
            Assert.Throws<UnauthorizedAccessException>(() =>
                provider.BuildPath(tempRoot, userPath)
            );
        }

        [Test]
        public void BuildPath_ValidPath_ReturnsFullPath()
        {
            string userPath = "subdir/file.txt";
            string fullPath = provider.BuildPath(tempRoot, userPath);
            Assert.That(fullPath, Does.StartWith(tempRoot));
            Assert.That(fullPath, Does.EndWith("file.txt"));
        }

        [Test]
        public async Task FileExistsAsync_ReturnsCorrectResult()
        {
            string file = Path.Combine(tempRoot, "exists.txt");
            await File.WriteAllTextAsync(file, "test");
            bool exists = await provider.FileExistsAsync(file);
            Assert.IsTrue(exists);
            bool notExists = await provider.FileExistsAsync(Path.Combine(tempRoot, "missing.txt"));
            Assert.IsFalse(notExists);
        }

        [Test]
        public async Task WriteAllTextAsync_And_ReadAllTextAsync_Works()
        {
            string file = Path.Combine(tempRoot, "write.txt");
            await provider.WriteAllTextAsync(file, "hello world");
            string content = await provider.ReadAllTextAsync(file);
            Assert.AreEqual("hello world", content);
        }

        [Test]
        public async Task OpenReadAsync_ReturnsStream()
        {
            string file = Path.Combine(tempRoot, "stream.txt");
            await File.WriteAllTextAsync(file, "streamdata");
            using var stream = await provider.OpenReadAsync(file);
            using var reader = new StreamReader(stream);
            string data = await reader.ReadToEndAsync();
            Assert.AreEqual("streamdata", data);
        }

        [Test]
        public async Task EnumerateFilesAsync_ReturnsFiles()
        {
            string dir = Path.Combine(tempRoot, "files");
            Directory.CreateDirectory(dir);
            string file1 = Path.Combine(dir, "a.txt");
            string file2 = Path.Combine(dir, "b.txt");
            await File.WriteAllTextAsync(file1, "a");
            await File.WriteAllTextAsync(file2, "b");
            var files = await provider.EnumerateFilesAsync(dir, ".txt");
            CollectionAssert.AreEquivalent(new[] { file1, file2 }, files);
        }

        [Test]
        public async Task EnumerateDirectoriesAsync_ReturnsDirectories()
        {
            string dir1 = Path.Combine(tempRoot, "d1");
            string dir2 = Path.Combine(tempRoot, "d2");
            Directory.CreateDirectory(dir1);
            Directory.CreateDirectory(dir2);
            var dirs = await provider.EnumerateDirectoriesAsync(tempRoot);
            CollectionAssert.IsSubsetOf(new[] { dir1, dir2 }, dirs);
        }

        [Test]
        public async Task GetLastWriteTimeAsync_ReturnsCorrectTime()
        {
            string file = Path.Combine(tempRoot, "time.txt");
            await File.WriteAllTextAsync(file, "t");
            DateTime expected = File.GetLastWriteTime(file);
            DateTime actual = await provider.GetLastWriteTimeAsync(file);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetDirectoryName_ReturnsName()
        {
            string dir = Path.Combine(tempRoot, "dir");
            Directory.CreateDirectory(dir);
            string name = provider.GetDirectoryName(dir);
            Assert.AreEqual("dir", name);
        }

        [Test]
        public void GetFileName_ReturnsName()
        {
            string file = Path.Combine(tempRoot, "file.txt");
            string name = provider.GetFileName(file);
            Assert.AreEqual("file.txt", name);
        }

        [Test]
        public void CombinePath_ReturnsCombined()
        {
            string combined = provider.CombinePath(tempRoot, "a", "b.txt");
            Assert.That(combined, Does.EndWith(Path.Combine("a", "b.txt")));
        }

        [Test]
        public void GetRelativePath_ReturnsNormalized()
        {
            string basePath = tempRoot;
            string targetPath = Path.Combine(tempRoot, "sub", "file.txt");
            string rel = provider.GetRelativePath(basePath, targetPath);
            Assert.That(rel, Is.EqualTo("sub/file.txt"));
        }
    }
}
