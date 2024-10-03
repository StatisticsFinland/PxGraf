using Newtonsoft.Json;
using NUnit.Framework;
using PxGraf.Datasource.PxWebInterface.SerializationModels;
using UnitTests.PxWebInterfaceTests.Fixtures;

namespace PxWebInterfaceTests
{
    internal static class ModelExtensionsTests
    {
        [Test]
        public static void VarHasOrdinalScaleTypeTest_Fi()
        {
            JsonStat2 jsonStat = JsonConvert.DeserializeObject<JsonStat2>(JsonStat2Fixtures.TEST_JSONSTAT2_FI);
            
            Assert.That(jsonStat.VarHasOrdinalScaleType("Vuosi"), Is.False, "Var: Vuosi");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Tiedot"), Is.False, "Var: Tiedot");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Maakunta"), Is.False, "Var: Maakunta");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Viitehenkilön ikä"), Is.True, "Var: Viitehenkilön ikä");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Asuntokunnat"), Is.False, "Var: Asuntokunnat");
        }

        [Test]
        public static void VarHasOrdinalScaleTypeTest_Sv()
        {
            JsonStat2 jsonStat = JsonConvert.DeserializeObject<JsonStat2>(JsonStat2Fixtures.TEST_JSONSTAT2_SV);

            Assert.That(jsonStat.VarHasOrdinalScaleType("Vuosi"), Is.False, "Var: Vuosi");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Tiedot"), Is.False, "Var: Tiedot");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Maakunta"), Is.False, "Var: Maakunta");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Viitehenkilön ikä"), Is.True, "Var: Viitehenkilön ikä");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Asuntokunnat"), Is.False, "Var: Asuntokunnat");
        }

        [Test]
        public static void VarHasOrdinalScaleTypeTest_En()
        {
            JsonStat2 jsonStat = JsonConvert.DeserializeObject<JsonStat2>(JsonStat2Fixtures.TEST_JSONSTAT2_EN);

            Assert.That(jsonStat.VarHasOrdinalScaleType("Vuosi"), Is.False, "Var: Vuosi");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Tiedot"), Is.False, "Var: Tiedot");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Maakunta"), Is.False, "Var: Maakunta");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Viitehenkilön ikä"), Is.True, "Var: Viitehenkilön ikä");
            Assert.That(jsonStat.VarHasOrdinalScaleType("Asuntokunnat"), Is.False, "Var: Asuntokunnat");
        }

        [Test]
        public static void IsSumValueTest_Fi()
        {
            PxMetaResponse pxMetaResponse = JsonConvert.DeserializeObject<PxMetaResponse>(PxMetaResponseFixture.TEST_META_FI);

            Assert.That(pxMetaResponse.Variables[0].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Variables[0].Code} Value: {pxMetaResponse.Variables[0].Values[0]}");
            Assert.That(pxMetaResponse.Variables[1].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Variables[1].Code} Value: {pxMetaResponse.Variables[1].Values[0]}");
            Assert.That(pxMetaResponse.Variables[2].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Variables[2].Code} Value: {pxMetaResponse.Variables[2].Values[0]}");
            Assert.That(pxMetaResponse.Variables[3].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Variables[3].Code} Value: {pxMetaResponse.Variables[3].Values[0]}");
            Assert.That(pxMetaResponse.Variables[4].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Variables[4].Code} Value: {pxMetaResponse.Variables[4].Values[0]}");
        }

        [Test]
        public static void IsSumValueTest_Sv()
        {
            PxMetaResponse pxMetaResponse = JsonConvert.DeserializeObject<PxMetaResponse>(PxMetaResponseFixture.TEST_META_SV);

            Assert.That(pxMetaResponse.Variables[0].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Variables[0].Code} Value: {pxMetaResponse.Variables[0].Values[0]}");
            Assert.That(pxMetaResponse.Variables[1].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Variables[1].Code} Value: {pxMetaResponse.Variables[1].Values[0]}");
            Assert.That(pxMetaResponse.Variables[2].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Variables[2].Code} Value: {pxMetaResponse.Variables[2].Values[0]}");
            Assert.That(pxMetaResponse.Variables[3].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Variables[3].Code} Value: {pxMetaResponse.Variables[3].Values[0]}");
            Assert.That(pxMetaResponse.Variables[4].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Variables[4].Code} Value: {pxMetaResponse.Variables[4].Values[0]}");
        }

        [Test]
        public static void IsSumValueTest_En()
        {
            PxMetaResponse pxMetaResponse = JsonConvert.DeserializeObject<PxMetaResponse>(PxMetaResponseFixture.TEST_META_EN);

            Assert.That(pxMetaResponse.Variables[0].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Variables[0].Code} Value: {pxMetaResponse.Variables[0].Values[0]}");
            Assert.That(pxMetaResponse.Variables[1].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Variables[1].Code} Value: {pxMetaResponse.Variables[1].Values[0]}");
            Assert.That(pxMetaResponse.Variables[2].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Variables[2].Code} Value: {pxMetaResponse.Variables[2].Values[0]}");
            Assert.That(pxMetaResponse.Variables[3].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Variables[3].Code} Value: {pxMetaResponse.Variables[3].Values[0]}");
            Assert.That(pxMetaResponse.Variables[4].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Variables[4].Code} Value: {pxMetaResponse.Variables[4].Values[0]}");
        }
    }
}
