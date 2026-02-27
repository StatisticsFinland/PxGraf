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

        #region CombineAndNormalizeAzurePaths Tests

        [Test]
        public void CombineAndNormalizeAzurePaths_WithNull_ReturnsEmpty()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths(null);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void CombineAndNormalizeAzurePaths_WithNoArguments_ReturnsEmpty()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths();

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void CombineAndNormalizeAzurePaths_BothEmpty_ReturnsEmpty()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths(string.Empty, string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void CombineAndNormalizeAzurePaths_RootEmpty_ReturnsUserPath()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths(string.Empty, "user/path");

            // Assert
            Assert.That(result, Is.EqualTo("user/path"));
        }

        [Test]
        public void CombineAndNormalizeAzurePaths_UserPathEmpty_ReturnsRootPath()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths("root/path", string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo("root/path"));
        }

        [Test]
        public void CombineAndNormalizeAzurePaths_BothProvided_CombinesWithSlash()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths("root/path", "user/path");

            // Assert
            Assert.That(result, Is.EqualTo("root/path/user/path"));
        }

        [Test]
        public void CombineAndNormalizeAzurePaths_RootWithTrailingSlash_RemovesExtraSlash()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths("root/path/", "user/path");

            // Assert
            Assert.That(result, Is.EqualTo("root/path/user/path"));
        }

        [Test]
        public void CombineAndNormalizeAzurePaths_MultipleTrailingSlashes_NormalizesCorrectly()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths("root/path///", "user/path");

            // Assert
            Assert.That(result, Is.EqualTo("root/path/user/path"));
        }

        [Test]
        public void CombineAndNormalizeAzurePaths_MoreThanTwoSegments_CombinesAll()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths("root", "middle", "leaf");

            // Assert
            Assert.That(result, Is.EqualTo("root/middle/leaf"));
        }

        [Test]
        public void CombineAndNormalizeAzurePaths_WithBackslashes_NormalizesToForwardSlashes()
        {
            // Act
            string result = PathNormalizer.CombineAndNormalizeAzurePaths("root\\path", "user\\path");

            // Assert
            Assert.That(result, Is.EqualTo("root/path/user/path"));
        }

        #endregion

        #region ValidatePathSecurity Tests

        [Test]
        public void ValidatePathSecurity_WithValidPath_DoesNotThrow()
        {
            // Arrange
            string combinedPath = "root/folder/subfolder";
            string rootPath = "root";

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath));
        }

        [Test]
        public void ValidatePathSecurity_WithDotDotWithinBounds_DoesNotThrow()
        {
            // Arrange
            string combinedPath = "root/folder/../other";
            string rootPath = "root";

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath));
        }

        [Test]
        public void ValidatePathSecurity_WithDotDotEscapingRoot_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            string combinedPath = "root/../..";
            string rootPath = "root";

            // Act & Assert
            UnauthorizedAccessException exception = Assert.Throws<UnauthorizedAccessException>(
                () => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath)
            );
            Assert.That(exception.Message, Is.EqualTo("Access to the path is denied."));
        }

        [Test]
        public void ValidatePathSecurity_WithDotSegments_DoesNotAffectValidation()
        {
            // Arrange
            string combinedPath = "root/./folder/./subfolder";
            string rootPath = "root";

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath));
        }

        [Test]
        public void ValidatePathSecurity_WithEmptyRootPath_AllowsAnyPath()
        {
            // Arrange
            string combinedPath = "folder/subfolder";
            string rootPath = "";

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath));
        }

        [Test]
        public void ValidatePathSecurity_WithMultipleDotDotsEscaping_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            string combinedPath = "root/folder/../../..";
            string rootPath = "root";

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(
                () => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath)
            );
        }

        [Test]
        public void ValidatePathSecurity_CaseInsensitive_DoesNotThrow()
        {
            // Arrange
            string combinedPath = "Root/Folder/SubFolder";
            string rootPath = "root";

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath));
        }

        [Test]
        public void ValidatePathSecurity_CombinedPathEqualsRoot_DoesNotThrow()
        {
            // Arrange
            string combinedPath = "root";
            string rootPath = "root";

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath));
        }

        [Test]
        public void ValidatePathSecurity_WithBackslashes_NormalizesBeforeValidation()
        {
            // Arrange
            string combinedPath = "root\\folder\\subfolder";
            string rootPath = "root";

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath));
        }

        [Test]
        public void ValidatePathSecurity_CompletelyDifferentPath_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            string combinedPath = "other/folder";
            string rootPath = "root";

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(
                () => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath)
            );
        }

        [Test]
        public void ValidatePathSecurity_PrefixCollision_ThrowsUnauthorizedAccessException()
        {
            // Arrange - "rootother" starts with "root" but is not under the "root" directory
            string combinedPath = "rootother/file";
            string rootPath = "root";

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(
                () => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath)
            );
        }

        #endregion

        #region NormalizePath Tests

        [Test]
        public void NormalizePath_WithNull_ReturnsEmptyString()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath(null);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void NormalizePath_WithEmptyString_ReturnsEmptyString()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath(string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void NormalizePath_WithSimplePath_ReturnsSame()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("root/folder/subfolder");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder/subfolder"));
        }

        [Test]
        public void NormalizePath_WithDotSegments_RemovesDots()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("root/./folder/./subfolder");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder/subfolder"));
        }

        [Test]
        public void NormalizePath_WithDotDotSegments_ResolvesProperly()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("root/folder/../subfolder");

            // Assert
            Assert.That(result, Is.EqualTo("root/subfolder"));
        }

        [Test]
        public void NormalizePath_WithMultipleDotDots_ResolvesAll()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("root/a/b/c/../../d");

            // Assert
            Assert.That(result, Is.EqualTo("root/a/d"));
        }

        [Test]
        public void NormalizePath_WithDotDotAtStart_IgnoresWhenNothingToGoUp()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("../folder");

            // Assert
            Assert.That(result, Is.EqualTo("folder"));
        }

        [Test]
        public void NormalizePath_WithMixedDotAndDotDot_NormalizesCorrectly()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("root/./folder/../other/./final");

            // Assert
            Assert.That(result, Is.EqualTo("root/other/final"));
        }

        [Test]
        public void NormalizePath_WithTrailingSlash_RemovesIt()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("root/folder/");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder"));
        }

        [Test]
        public void NormalizePath_WithLeadingSlash_RemovesIt()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("/root/folder");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder"));
        }

        [Test]
        public void NormalizePath_WithBackslashes_ConvertedToForwardSlashes()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("root\\folder\\subfolder");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder/subfolder"));
        }

        [Test]
        public void NormalizePath_AllSegmentsCollapse_ReturnsEmpty()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("a/b/../../");

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void NormalizePath_WithDuplicateSlashes_RemovesThem()
        {
            // Act
            string result = PathNormalizer.NormalizeToAzurePath("root//folder///subfolder");

            // Assert
            Assert.That(result, Is.EqualTo("root/folder/subfolder"));
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
            string combined = PathNormalizer.CombineAndNormalizeAzurePaths(rootPath, userPath);
            string normalized = PathNormalizer.NormalizeToAzurePath(combined);

            // Assert
            Assert.That(normalized, Is.EqualTo("root/base/other"));
        }

        [Test]
        public void IntegrationTest_SecurityValidationAndNormalization_WorksTogether()
        {
            // Arrange
            string combinedPath = "root/folder/./subfolder";
            string rootPath = "root";

            // Act & Assert
            Assert.DoesNotThrow(() => PathNormalizer.ValidatePathSecurity(combinedPath, rootPath));
            string normalized = PathNormalizer.NormalizeToAzurePath(combinedPath);
            Assert.That(normalized, Is.EqualTo("root/folder/subfolder"));
        }

        #endregion
    }
}
