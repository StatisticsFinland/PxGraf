using NUnit.Framework;
using PxGraf.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DataValueTests
{
    public class DataValueTests
    {
        private const double NUMBER = 3141592.6535897931; //Pi x 1M, note that correct rounding is also tested

        [Test]
        public void ToStringMachineReadableTest()
        {
            List<DataValueType> allDataValueTypes = Enum.GetValues(typeof(DataValueType)).Cast<DataValueType>().ToList();

            string[] dataValues = allDataValueTypes.Select(type =>
            {
                var dataValue = new DataValue(NUMBER, type);
                return dataValue.ToMachineReadableString(5);
            })
            .ToArray();

            string[] expected =
            [
                "3141592.65359",
                ".",
                "..",
                "...",
                "....",
                ".....",
                "......",
                "-",
            ];
            Assert.That(dataValues, Is.EqualTo(expected));
        }

        [Test]
        public void ToStringEnglishTest()
        {
            List<DataValueType> allDataValueTypes = Enum.GetValues(typeof(DataValueType)).Cast<DataValueType>().ToList();

            CultureInfo culture = new("en");
            string[] dataValues = allDataValueTypes.Select(type =>
            {
                DataValue dataValue = new(NUMBER, type);
                return dataValue.ToHumanReadableString(5, culture);
            })
                .ToArray();

            string[] expected =
            [
                "3,141,592.65359",
                ".",
                "..",
                "...",
                "....",
                ".....",
                "......",
                "-",
            ];
            Assert.That(dataValues, Is.EqualTo(expected));
        }

        [Test]
        public void ToStringFinnishTest()
        {
            List<DataValueType> allDataValueTypes = Enum.GetValues(typeof(DataValueType)).Cast<DataValueType>().ToList();

            CultureInfo culture = new("fi");
            string[] dataValues = allDataValueTypes.Select(type =>
            {
                DataValue dataValue = new(NUMBER, type);
                return dataValue.ToHumanReadableString(5, culture);
            })
                .ToArray();

            // \u00a0 = NO-BREAK SPACE
            string[] expected =
            [
                "3\u00a0141\u00a0592,65359",
                ".",
                "..",
                "...",
                "....",
                ".....",
                "......",
                "-",
            ];
            Assert.That(dataValues, Is.EqualTo(expected));
        }

        /// <summary>
        /// Point of these test is to ensure that MidpointRounding.AwayFromZero behaviour
        /// Default "N#" format round like MidpointRounding.AwayFromZero on .Net Framework but
        /// like MidpointRounding.ToEven on .Net Core 3.1
        /// </summary>
        [Test]
        public void RoundingTest()
        {
            var culture = new CultureInfo("en");

            var dataValue = DataValue.FromRaw(10.5);
            var humanString = dataValue.ToHumanReadableString(0, culture);
            var machineString = dataValue.ToMachineReadableString(0);

            Assert.That(humanString, Is.EqualTo("11"));
            Assert.That(machineString, Is.EqualTo("11"));
        }

        [Test]
        public void NegativeRoundingTest()
        {
            var culture = new CultureInfo("en");

            var dataValue = DataValue.FromRaw(-10.5);
            var humanString = dataValue.ToHumanReadableString(0, culture);
            var machineString = dataValue.ToMachineReadableString(0);

            Assert.That(humanString, Is.EqualTo("-11"));
            Assert.That(machineString, Is.EqualTo("-11"));
        }

        [Test]
        public void DecimalRoundingTest()
        {
            var culture = new CultureInfo("en");

            var dataValue = DataValue.FromRaw(10.45);
            var humanString = dataValue.ToHumanReadableString(1, culture);
            var machineString = dataValue.ToMachineReadableString(1);

            Assert.That(humanString, Is.EqualTo("10.5"));
            Assert.That(machineString, Is.EqualTo("10.5"));
        }

        [Test]
        public void TrailingZeoresTest()
        {
            var culture = new CultureInfo("en");

            var dataValue = DataValue.FromRaw(10);
            var humanString = dataValue.ToHumanReadableString(1, culture);
            var machineString = dataValue.ToMachineReadableString(1);

            Assert.That(humanString, Is.EqualTo("10.0"));
            Assert.That(machineString, Is.EqualTo("10.0"));
        }

        [Test]
        public void CurrentCultureTest()
        {
            // Snapshot original culture settings so that test does not have side effects
            CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            CultureInfo currentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

            try
            {
                DataValue dataValue = DataValue.FromRaw(10.45);

                CultureInfo enCulture = new("en");
                CultureInfo fiCulture = new("fi");
                
                System.Threading.Thread.CurrentThread.CurrentCulture = enCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = enCulture;

                Assert.That(dataValue.Value.ToString("0.0"), Is.EqualTo("10.5"));               // By default current culture: en
                Assert.That(dataValue.ToHumanReadableString(1, fiCulture), Is.EqualTo("10,5")); // Parameter: fi
                Assert.That(dataValue.ToMachineReadableString(1), Is.EqualTo("10.5"));          // Always en
                
                System.Threading.Thread.CurrentThread.CurrentCulture = fiCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = fiCulture;

                Assert.That(dataValue.Value.ToString("0.0"), Is.EqualTo("10,5"));               // By default current culture: fi
                Assert.That(dataValue.ToHumanReadableString(1, enCulture), Is.EqualTo("10.5")); // Parameter: en
                Assert.That(dataValue.ToMachineReadableString(1), Is.EqualTo("10.5"));          // Always en
            }
            finally
            {
                // Restore original
                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = currentUICulture;
            }
        }
    }
}
