using NUnit.Framework;
using PxGraf.Data;
using System;
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
            var allDataValueTypes = Enum.GetValues(typeof(DataValueType)).Cast<DataValueType>().ToList();

            var dataValues = allDataValueTypes.Select(type =>
            {
                var dataValue = new DataValue(NUMBER, type);
                return dataValue.ToMachineReadableString(5);
            })
                .ToArray();

            Assert.AreEqual(new string[]
                {
                    "3141592.65359",
                    ".",
                    "..",
                    "...",
                    "....",
                    ".....",
                    "......",
                    "-",
                },
                dataValues
            );
        }

        [Test]
        public void ToStringEnglishTest()
        {
            var allDataValueTypes = Enum.GetValues(typeof(DataValueType)).Cast<DataValueType>().ToList();

            var culture = new CultureInfo("en");
            var dataValues = allDataValueTypes.Select(type =>
            {
                var dataValue = new DataValue(NUMBER, type);
                return dataValue.ToHumanReadableString(5, culture);
            })
                .ToArray();

            Assert.AreEqual(new string[]
                {
                    "3,141,592.65359",
                    ".",
                    "..",
                    "...",
                    "....",
                    ".....",
                    "......",
                    "-",
                },
                dataValues
            );
        }

        [Test]
        public void ToStringFinnishTest()
        {
            var allDataValueTypes = Enum.GetValues(typeof(DataValueType)).Cast<DataValueType>().ToList();

            var culture = new CultureInfo("fi");
            var dataValues = allDataValueTypes.Select(type =>
            {
                var dataValue = new DataValue(NUMBER, type);
                return dataValue.ToHumanReadableString(5, culture);
            })
                .ToArray();

            // \u00a0 = NO-BREAK SPACE
            Assert.AreEqual(new string[]
                {
                    "3\u00a0141\u00a0592,65359",
                    ".",
                    "..",
                    "...",
                    "....",
                    ".....",
                    "......",
                    "-",
                },
                dataValues
            );
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
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var currentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

            try
            {
                var dataValue = DataValue.FromRaw(10.45);

                var enCulture = new CultureInfo("en");
                var fiCulture = new CultureInfo("fi");

                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = enCulture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = enCulture;

                    var humanString = dataValue.ToHumanReadableString(1, fiCulture);
                    var machineString = dataValue.ToMachineReadableString(1);

                    Assert.AreEqual("10.5", dataValue.Value.ToString("0.0"));    //By default current culture: en
                    Assert.AreEqual("10,5", humanString);                   //Parameter: fi
                    Assert.AreEqual("10.5", machineString);                 //Always en
                }

                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = fiCulture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = fiCulture;

                    var humanString = dataValue.ToHumanReadableString(1, enCulture);
                    var machineString = dataValue.ToMachineReadableString(1);

                    Assert.AreEqual("10,5", dataValue.Value.ToString("0.0"));    //By default current culture: en
                    Assert.AreEqual("10.5", humanString);                   //Parameter: en
                    Assert.AreEqual("10.5", machineString);                 //Always en
                }
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
