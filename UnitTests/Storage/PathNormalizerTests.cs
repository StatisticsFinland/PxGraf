using NUnit.Framework;
using PxGraf.Storage;
using System;

namespace UnitTests.Storage
{
    [TestFixture]
    public class PathNormalizerTests
    {
        #region NormalizeFileExtension Tests

        [Test]
        public void NormalizeFileExtension_WithNull_ReturnsEmptyString()
        {
            // Act
            string result = PathNormalizer.NormalizeFileExtension(null);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void NormalizeFileExtension_WithEmptyString_ReturnsEmptyString()
        {
            // Act
            string result = PathNormalizer.NormalizeFileExtension(string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void NormalizeFileExtension_WithExtensionStartingWithDot_ReturnsSame()
        {
            // Arrange
            string extension = ".txt";

            // Act
            string result = PathNormalizer.NormalizeFileExtension(extension);

            // Assert
            Assert.That(result, Is.EqualTo(".txt"));
        }

        [Test]
        public void NormalizeFileExtension_WithExtensionWithoutDot_AddsDot()
        {
            // Arrange
            string extension = "txt";

            // Act
            string result = PathNormalizer.NormalizeFileExtension(extension);

            // Assert
            Assert.That(result, Is.EqualTo(".txt"));
        }

        [Test]
        public void NormalizeFileExtension_WithPxExtension_WorksCorrectly()
        {
            // Arrange
            string extension = "px";

            // Act
            string result = PathNormalizer.NormalizeFileExtension(extension);

            // Assert
            Assert.That(result, Is.EqualTo(".px"));
        }

        #endregion

        #region CombinePaths Tests

        [Test]
        public void CombinePaths_BothEmpty_ReturnsEmpty()
        {
            // Act
            string result = PathNormalizer.CombinePaths(string.Empty, string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void CombinePaths_RootEmpty_ReturnsUserPath()
        {
            // Act
            string result = PathNormalizer.CombinePaths(string.Empty, "user/path");

            // Assert
            Assert.That(result, Is.EqualTo("user/path"));
        }

        [Test]
        public void CombinePaths_UserPathEmpty_ReturnsRootPath()
        {
            // Act
            string result = PathNormalizer.CombinePaths("root/path", string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo("root/path"));
        }

        [Test]
        public void CombinePaths_BothProvided_CombinesWithSlash()
        {
            // Act
            string result = PathNormalizer.CombinePaths("root/path", "user/path");

            // Assert
            Assert.That(result, Is.EqualTo("root/path/user/path"));
        }

        [Test]
        public void CombinePaths_RootWithTrailingSlash_RemovesExtraSlash()
        {
            // Act
            string result = PathNormalizer.CombinePaths("root/path/", "user/path");

            // Assert
            Assert.That(result, Is.EqualTo("root/path/user/path"));
        }

        [Test]
        public void CombinePaths_MultipleTrailingSlashes_NormalizesCorrectly()
        {
            // Act
            string result = PathNormalizer.CombinePaths("root/path///", "user/path");

            // Assert
            Assert.That(result, Is.EqualTo("root/path/user/path"));
        }

        #endregion

        #region GetPathDepth Tests

        [Test]
        public void GetPathDepth_WithNull_ReturnsZero()
        {
            // Act
            int result = PathNormalizer.GetPathDepth(null);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetPathDepth_WithEmptyString_ReturnsZero()
        {
            // Act
            int result = PathNormalizer.GetPathDepth(string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetPathDepth_WithSingleSegment_ReturnsOne()
        {
            // Act
            int result = PathNormalizer.GetPathDepth("folder");

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void GetPathDepth_WithTwoSegments_ReturnsTwo()
        {
            // Act
            int result = PathNormalizer.GetPathDepth("folder/subfolder");

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void GetPathDepth_WithThreeSegments_ReturnsThree()
        {
            // Act
            int result = PathNormalizer.GetPathDepth("root/folder/subfolder");

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void GetPathDepth_WithTrailingSlash_IgnoresEmptySegment()
        {
            // Act
            int result = PathNormalizer.GetPathDepth("root/folder/");

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        #endregion

        #region ValidatePathSecurity Tests

        [Test]
        public void ValidatePathSecurity_WithValidPath_DoesNotThrow()
        {
            // Arrange
            string path = "root/folder/subfolder";
            int rootDepth = 1;

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(path, rootDepth));
        }

        [Test]
        public void ValidatePathSecurity_WithDotDotWithinBounds_DoesNotThrow()
        {
            // Arrange
            string path = "root/folder/../other";
            int rootDepth = 1;

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(path, rootDepth));
        }

        [Test]
        public void ValidatePathSecurity_WithDotDotEscapingRoot_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            string path = "root/../..";
            int rootDepth = 1;

            // Act & Assert
            UnauthorizedAccessException exception = Assert.Throws<UnauthorizedAccessException>(
                () => PathNormalizer.ValidatePathSecurity(path, rootDepth)
            );
            Assert.That(exception.Message, Is.EqualTo("Access to the path is denied."));
        }

        [Test]
        public void ValidatePathSecurity_WithDotSegments_DoesNotAffectValidation()
        {
            // Arrange
            string path = "root/./folder/./subfolder";
            int rootDepth = 1;

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(path, rootDepth));
        }

        [Test]
        public void ValidatePathSecurity_WithZeroRootDepth_AllowsAnyPath()
        {
            // Arrange
            string path = "folder/subfolder";
            int rootDepth = 0;

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(path, rootDepth));
        }

        [Test]
        public void ValidatePathSecurity_WithMultipleDotDotsEscaping_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            string path = "root/folder/../../..";
            int rootDepth = 1;

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(
                () => PathNormalizer.ValidatePathSecurity(path, rootDepth)
            );
        }

        #endregion

        #region NormalizePath Tests

        [Test]
        public void NormalizePath_WithNull_ReturnsEmptyString()
        {
            // Act
            string result = PathNormalizer.NormalizePath(null);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void NormalizePath_WithEmptyString_ReturnsEmptyString()
        {
            // Act
            string result = PathNormalizer.NormalizePath(string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void NormalizePath_WithSimplePath_ReturnsSame()
        {
            // Act
            string result = PathNormalizer.NormalizePath("root/folder/subfolder");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder/subfolder"));
        }

        [Test]
        public void NormalizePath_WithDotSegments_RemovesDots()
        {
            // Act
            string result = PathNormalizer.NormalizePath("root/./folder/./subfolder");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder/subfolder"));
        }

        [Test]
        public void NormalizePath_WithDotDotSegments_ResolvesProperly()
        {
            // Act
            string result = PathNormalizer.NormalizePath("root/folder/../subfolder");

            // Assert
            Assert.That(result, Is.EqualTo("root/subfolder"));
        }

        [Test]
        public void NormalizePath_WithMultipleDotDots_ResolvesAll()
        {
            // Act
            string result = PathNormalizer.NormalizePath("root/a/b/c/../../d");

            // Assert
            Assert.That(result, Is.EqualTo("root/a/d"));
        }

        [Test]
        public void NormalizePath_WithDotDotAtStart_IgnoresWhenNothingToGoUp()
        {
            // Act
            string result = PathNormalizer.NormalizePath("../folder");

            // Assert
            Assert.That(result, Is.EqualTo("folder"));
        }

        [Test]
        public void NormalizePath_WithMixedDotAndDotDot_NormalizesCorrectly()
        {
            // Act
            string result = PathNormalizer.NormalizePath("root/./folder/../other/./final");

            // Assert
            Assert.That(result, Is.EqualTo("root/other/final"));
        }

        [Test]
        public void NormalizePath_WithTrailingSlash_RemovesIt()
        {
            // Act
            string result = PathNormalizer.NormalizePath("root/folder/");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder"));
        }

        [Test]
        public void NormalizePath_WithLeadingSlash_RemovesIt()
        {
            // Act
            string result = PathNormalizer.NormalizePath("/root/folder");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder"));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void IntegrationTest_CombineAndNormalize_WorksTogether()
        {
            // Arrange
            string rootPath = "root/base";
            string userPath = "./folder/../other";

            // Act
            string combined = PathNormalizer.CombinePaths(rootPath, userPath);
            string normalized = PathNormalizer.NormalizePath(combined);

            // Assert
            Assert.That(normalized, Is.EqualTo("root/base/other"));
        }

        [Test]
        public void IntegrationTest_SecurityValidationAndNormalization_WorksTogether()
        {
            // Arrange
            string path = "root/folder/./subfolder";
            int rootDepth = 1;

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(path, rootDepth));
            string normalized = PathNormalizer.NormalizePath(path);
            Assert.That(normalized, Is.EqualTo("root/folder/subfolder"));
        }

        #endregion
    }
}
