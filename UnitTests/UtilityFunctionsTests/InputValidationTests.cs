using NUnit.Framework;
using PxGraf.Utility;
using System.IO;

namespace UnitTests.UtilityFunctionsTests
{
    internal class InputValidationTests
    {
        [Test]
        public void ValidSqIdTest()
        {
            Assert.That(InputValidation.ValidateSqIdString("a3af0d90-eeeb-4840-bc14-87f53bc7c8fe"), Is.True);
        }

        [Test]
        public void ValidSqIdTestWithoutHyphens()
        {
            Assert.That(InputValidation.ValidateSqIdString("a3af0d90eeeb4840bc1487f53bc7c8fe"), Is.True);
        }

        [Test]
        public void InvalidSqIdWithSlashesTest()
        {
            Assert.That(InputValidation.ValidateSqIdString("a3af0d90/eeeb/4840/bc14/87f53bc7c8fe"), Is.False);
        }

        [Test]
        public void InvalidSqIdWithDoubleSlashesTest()
        {
            Assert.That(InputValidation.ValidateSqIdString("a3af0d90//eeeb//4840//bc14//787f53bc7c8fe"), Is.False);
        }

        [Test]
        public void InvalidSqIdWithDoubleBackslashesTest()
        {
            Assert.That(InputValidation.ValidateSqIdString("a3af0d90\\eeeb\\4840\\bc14\\787f53bc7c8fe"), Is.False);
        }

        [Test]
        public void InvalidSqIdWithQuotationsTest()
        {
            Assert.That(InputValidation.ValidateSqIdString("a3af0d90\"eeeb\"4840\"bc14\"787f53bc7c8fe"), Is.False);
        }

        [Test]
        public void InvalidSqIdWithMultipleBadCharactersTest()
        {
            Assert.That(InputValidation.ValidateSqIdString("\\eeeb//4840\"bc14-787f53bc7c8fe"), Is.False);
        }

        [Test]
        public void InvalidSqIdWithAsteriskTest()
        {
            Assert.That(InputValidation.ValidateSqIdString("eeeb-4840-bc14-787f53bc7c8fe*"), Is.False);
        }

        [Test]
        public void InvalidSqIdWithEmptyStringTest()
        {
            Assert.That(InputValidation.ValidateSqIdString(""), Is.False);
        }

        [Test]
        public void InvalidSqIdWithNullTest()
        {
            Assert.That(InputValidation.ValidateSqIdString(null), Is.False);
        }

        [Test]
        public void ValidFilePathPartTest()
        {
            Assert.Multiple(() => {
                Assert.That(InputValidation.ValidateFilePathPart("Folder1"), Is.True);
                Assert.That(InputValidation.ValidateFilePathPart("abc123"), Is.True);
                Assert.That(InputValidation.ValidateFilePathPart("a.b.c"), Is.True);
                Assert.That(InputValidation.ValidateFilePathPart("A1_b2-C3"), Is.True);
            });
        }

        [Test]
        public void InvalidFilePathPart_EmptyOrNullTest()
        {
            Assert.Multiple(() => {
                Assert.That(InputValidation.ValidateFilePathPart(""), Is.False);
                Assert.That(InputValidation.ValidateFilePathPart(null), Is.False);
            });
        }

        [Test]
        public void InvalidFilePathPart_TooLongTest()
        {
            string longPart = new ('a', 101);
            Assert.That(InputValidation.ValidateFilePathPart(longPart), Is.False);
        }

        [Test]
        public void InvalidFilePathPart_AllDotsTest()
        {
            Assert.That(InputValidation.ValidateFilePathPart("..."), Is.False);
        }

        [Test]
        public void InvalidFileName_InvalidCharsTest()
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                Assert.That(InputValidation.ValidateFileName("file" + c + "name"), Is.False);
            }
        }

        [Test]
        public void InvalidFileName_NoLetterOrDigitTest()
        {
            Assert.That(InputValidation.ValidateFileName("_-__--"), Is.False);
        }

        [Test]
        public void ValidateFileName_ValidFileNameTest()
        {
            Assert.That(InputValidation.ValidateFileName("database_statisticalProgram-table.px"), Is.True);
        }
    }
}
