using NUnit.Framework;
using PxGraf.Utility;

namespace UtilityFunctionsTests
{
    internal class InputValidationTests
    {
        [Test]
        public void ValidSqIdTest()
        {
            Assert.True(InputValidation.ValidateSqIdString("a3af0d90-eeeb-4840-bc14-87f53bc7c8fe"));
        }

        [Test]
        public void ValidSqIdTestWithoutHyphens()
        {
            Assert.True(InputValidation.ValidateSqIdString("a3af0d90eeeb4840bc1487f53bc7c8fe"));
        }

        [Test]
        public void InvalidSqIdWithSlashesTest()
        {
            Assert.False(InputValidation.ValidateSqIdString("a3af0d90/eeeb/4840/bc14/87f53bc7c8fe"));
        }

        [Test]
        public void InvalidSqIdWithDoubleSlashesTest()
        {
            Assert.False(InputValidation.ValidateSqIdString("a3af0d90//eeeb//4840//bc14//787f53bc7c8fe"));
        }

        [Test]
        public void InvalidSqIdWithDoubleBackslashesTest()
        {
            Assert.False(InputValidation.ValidateSqIdString("a3af0d90\\eeeb\\4840\\bc14\\787f53bc7c8fe"));
        }

        [Test]
        public void InvalidSqIdWithQuotationsTest()
        {
            Assert.False(InputValidation.ValidateSqIdString("a3af0d90\"eeeb\"4840\"bc14\"787f53bc7c8fe"));
        }

        [Test]
        public void InvalidSqIdWithMultipleBadCharactersTest()
        {
            Assert.False(InputValidation.ValidateSqIdString("\\eeeb//4840\"bc14-787f53bc7c8fe"));
        }

        [Test]
        public void InvalidSqIdWithAsteriskTest()
        {
            Assert.False(InputValidation.ValidateSqIdString("eeeb-4840-bc14-787f53bc7c8fe*"));
        }

        [Test]
        public void InvalidSqIdWithEmptyStringTest()
        {
            Assert.False(InputValidation.ValidateSqIdString(""));
        }

        [Test]
        public void InvalidSqIdWithNullTest()
        {
            Assert.False(InputValidation.ValidateSqIdString(null));
        }
    }
}
