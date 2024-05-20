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
            Assert.AreEqual(expected, dataValues);
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
            Assert.AreEqual(expected, dataValues);
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
            Assert.AreEqual(expected, dataValues);
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

            Assert.AreEqual("11", humanString);
            Assert.AreEqual("11", machineString);
        }

        [Test]
        public void NegativeRoundingTest()
        {
            var culture = new CultureInfo("en");

            var dataValue = DataValue.FromRaw(-10.5);
            var humanString = dataValue.ToHumanReadableString(0, culture);
            var machineString = dataValue.ToMachineReadableString(0);

            Assert.AreEqual("-11", humanString);
            Assert.AreEqual("-11", machineString);
        }

        [Test]
        public void DecimalRoundingTest()
        {
            var culture = new CultureInfo("en");

            var dataValue = DataValue.FromRaw(10.45);
            var humanString = dataValue.ToHumanReadableString(1, culture);
            var machineString = dataValue.ToMachineReadableString(1);

            Assert.AreEqual("10.5", humanString);
            Assert.AreEqual("10.5", machineString);
        }

        [Test]
        public void TrailingZeoresTest()
        {
            var culture = new CultureInfo("en");

            var dataValue = DataValue.FromRaw(10);
            var humanString = dataValue.ToHumanReadableString(1, culture);
            var machineString = dataValue.ToMachineReadableString(1);

            Assert.AreEqual("10.0", humanString);
            Assert.AreEqual("10.0", machineString);
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

                string humanStringEn = dataValue.ToHumanReadableString(1, fiCulture);
                string machineStringEn = dataValue.ToMachineReadableString(1);

                Assert.AreEqual("10.5", dataValue.Value.ToString("0.0"));    //By default current culture: en
                Assert.AreEqual("10,5", humanStringEn);                   //Parameter: fi
                Assert.AreEqual("10.5", machineStringEn);                 //Always en
                
                System.Threading.Thread.CurrentThread.CurrentCulture = fiCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = fiCulture;

                string humanStringFi = dataValue.ToHumanReadableString(1, enCulture);
                string machineStringFi = dataValue.ToMachineReadableString(1);

                Assert.AreEqual("10,5", dataValue.Value.ToString("0.0"));    //By default current culture: en
                Assert.AreEqual("10.5", humanStringFi);                   //Parameter: en
                Assert.AreEqual("10.5", machineStringFi);                 //Always en
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
