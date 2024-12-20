using NUnit.Framework;
using PxGraf.Datasource.ApiDatasource.SerializationModels;
using PxGraf.Datasource.PxWebInterface.SerializationModels;
using System.Text.Json;
using UnitTests.PxWebInterfaceTests.Fixtures;

namespace UnitTests.PxWebInterfaceTests
{
    internal static class ModelExtensionsTests
    {
        [Test]
        public static void VarHasOrdinalScaleTypeTest_Fi()
        {
            JsonStat2 jsonStat = JsonSerializer.Deserialize<JsonStat2>(JsonStat2Fixtures.TEST_JSONSTAT2_FI);

            Assert.That(jsonStat.DimHasOrdinalScaleType("Vuosi"), Is.False, "Var: Vuosi");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Tiedot"), Is.False, "Var: Tiedot");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Maakunta"), Is.False, "Var: Maakunta");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Viitehenkilön ikä"), Is.True, "Var: Viitehenkilön ikä");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Asuntokunnat"), Is.False, "Var: Asuntokunnat");
        }

        [Test]
        public static void VarHasOrdinalScaleTypeTest_Sv()
        {
            JsonStat2 jsonStat = JsonSerializer.Deserialize<JsonStat2>(JsonStat2Fixtures.TEST_JSONSTAT2_SV);

            Assert.That(jsonStat.DimHasOrdinalScaleType("Vuosi"), Is.False, "Var: Vuosi");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Tiedot"), Is.False, "Var: Tiedot");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Maakunta"), Is.False, "Var: Maakunta");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Viitehenkilön ikä"), Is.True, "Var: Viitehenkilön ikä");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Asuntokunnat"), Is.False, "Var: Asuntokunnat");
        }

        [Test]
        public static void VarHasOrdinalScaleTypeTest_En()
        {
            JsonStat2 jsonStat = JsonSerializer.Deserialize<JsonStat2>(JsonStat2Fixtures.TEST_JSONSTAT2_EN);

            Assert.That(jsonStat.DimHasOrdinalScaleType("Vuosi"), Is.False, "Var: Vuosi");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Tiedot"), Is.False, "Var: Tiedot");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Maakunta"), Is.False, "Var: Maakunta");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Viitehenkilön ikä"), Is.True, "Var: Viitehenkilön ikä");
            Assert.That(jsonStat.DimHasOrdinalScaleType("Asuntokunnat"), Is.False, "Var: Asuntokunnat");
        }

        [Test]
        public static void IsSumValueTest_Fi()
        {
            PxMetaResponse pxMetaResponse = JsonSerializer.Deserialize<PxMetaResponse>(PxMetaResponseFixture.TEST_META_FI);

            Assert.That(pxMetaResponse.Dimensions[0].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Dimensions[0].Code} Value: {pxMetaResponse.Dimensions[0].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[1].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Dimensions[1].Code} Value: {pxMetaResponse.Dimensions[1].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[2].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Dimensions[2].Code} Value: {pxMetaResponse.Dimensions[2].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[3].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Dimensions[3].Code} Value: {pxMetaResponse.Dimensions[3].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[4].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Dimensions[4].Code} Value: {pxMetaResponse.Dimensions[4].Values[0]}");
        }

        [Test]
        public static void IsSumValueTest_Sv()
        {
            PxMetaResponse pxMetaResponse = JsonSerializer.Deserialize<PxMetaResponse>(PxMetaResponseFixture.TEST_META_SV);

            Assert.That(pxMetaResponse.Dimensions[0].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Dimensions[0].Code} Value: {pxMetaResponse.Dimensions[0].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[1].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Dimensions[1].Code} Value: {pxMetaResponse.Dimensions[1].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[2].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Dimensions[2].Code} Value: {pxMetaResponse.Dimensions[2].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[3].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Dimensions[3].Code} Value: {pxMetaResponse.Dimensions[3].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[4].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Dimensions[4].Code} Value: {pxMetaResponse.Dimensions[4].Values[0]}");
        }

        [Test]
        public static void IsSumValueTest_En()
        {
            PxMetaResponse pxMetaResponse = JsonSerializer.Deserialize<PxMetaResponse>(PxMetaResponseFixture.TEST_META_EN);

            Assert.That(pxMetaResponse.Dimensions[0].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Dimensions[0].Code} Value: {pxMetaResponse.Dimensions[0].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[1].IsSumValue(0), Is.False, $"Var: {pxMetaResponse.Dimensions[1].Code} Value: {pxMetaResponse.Dimensions[1].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[2].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Dimensions[2].Code} Value: {pxMetaResponse.Dimensions[2].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[3].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Dimensions[3].Code} Value: {pxMetaResponse.Dimensions[3].Values[0]}");
            Assert.That(pxMetaResponse.Dimensions[4].IsSumValue(0), Is.True, $"Var: {pxMetaResponse.Dimensions[4].Code} Value: {pxMetaResponse.Dimensions[4].Values[0]}");
        }
    }
}
