﻿using NUnit.Framework;
using PxGraf.Utility;

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
    }
}
