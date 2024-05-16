using Newtonsoft.Json;
using NUnit.Framework;
using PxGraf.PxWebInterface.SerializationModels;
using UnitTests.PxWebInterfaceTests.Fixtures;

namespace PxWebInterfaceTests
{
    internal static class ModelExtensionsTests
    {
        [Test]
        public static void VarHasOrdinalScaleTypeTest_Fi()
        {
            JsonStat2 jsonStat = JsonConvert.DeserializeObject<JsonStat2>(JsonStat2Fixtures.TEST_JSONSTAT2_FI);
            
            Assert.False(jsonStat.VarHasOrdinalScaleType("Vuosi"), "Var: Vuosi"); // Time var
            Assert.False(jsonStat.VarHasOrdinalScaleType("Tiedot"), "Var: Tiedot"); // Content var
            Assert.False(jsonStat.VarHasOrdinalScaleType("Maakunta"), "Var: Maakunta");
            Assert.True(jsonStat.VarHasOrdinalScaleType("Viitehenkilön ikä"), "Var: Viitehenkilön ikä");
            Assert.False(jsonStat.VarHasOrdinalScaleType("Asuntokunnat"), "Var: Asuntokunnat");
        }

        [Test]
        public static void VarHasOrdinalScaleTypeTest_Sv()
        {
            JsonStat2 jsonStat = JsonConvert.DeserializeObject<JsonStat2>(JsonStat2Fixtures.TEST_JSONSTAT2_SV);

            Assert.False(jsonStat.VarHasOrdinalScaleType("Vuosi"), "Var: Vuosi"); // Time var
            Assert.False(jsonStat.VarHasOrdinalScaleType("Tiedot"), "Var: Tiedot"); // Content var
            Assert.False(jsonStat.VarHasOrdinalScaleType("Maakunta"), "Var: Maakunta");
            Assert.True(jsonStat.VarHasOrdinalScaleType("Viitehenkilön ikä"), "Var: Viitehenkilön ikä");
            Assert.False(jsonStat.VarHasOrdinalScaleType("Asuntokunnat"), "Var: Asuntokunnat");
        }

        [Test]
        public static void VarHasOrdinalScaleTypeTest_En()
        {
            JsonStat2 jsonStat = JsonConvert.DeserializeObject<JsonStat2>(JsonStat2Fixtures.TEST_JSONSTAT2_EN);

            Assert.False(jsonStat.VarHasOrdinalScaleType("Vuosi"), "Var: Vuosi"); // Time var
            Assert.False(jsonStat.VarHasOrdinalScaleType("Tiedot"), "Var: Tiedot"); // Content var
            Assert.False(jsonStat.VarHasOrdinalScaleType("Maakunta"), "Var: Maakunta");
            Assert.True(jsonStat.VarHasOrdinalScaleType("Viitehenkilön ikä"), "Var: Viitehenkilön ikä");
            Assert.False(jsonStat.VarHasOrdinalScaleType("Asuntokunnat"), "Var: Asuntokunnat");
        }

        [Test]
        public static void IsSumValueTest_Fi()
        {
            PxMetaResponse pxMetaResponse = JsonConvert.DeserializeObject<PxMetaResponse>(PxMetaResponseFixture.TEST_META_FI);
            Assert.False(pxMetaResponse.Variables[0].IsSumValue(0), $"Var: {pxMetaResponse.Variables[0].Code} Value: {pxMetaResponse.Variables[0].Values[0]}");
            Assert.False(pxMetaResponse.Variables[1].IsSumValue(0), $"Var: {pxMetaResponse.Variables[1].Code} Value: {pxMetaResponse.Variables[1].Values[0]}");
            Assert.True(pxMetaResponse.Variables[2].IsSumValue(0), $"Var: {pxMetaResponse.Variables[2].Code} Value: {pxMetaResponse.Variables[2].Values[0]}");
            Assert.True(pxMetaResponse.Variables[3].IsSumValue(0), $"Var: {pxMetaResponse.Variables[3].Code} Value: {pxMetaResponse.Variables[3].Values[0]}");
            Assert.True(pxMetaResponse.Variables[4].IsSumValue(0), $"Var: {pxMetaResponse.Variables[4].Code} Value: {pxMetaResponse.Variables[4].Values[0]}");
        }

        [Test]
        public static void IsSumValueTest_Sv()
        {
            PxMetaResponse pxMetaResponse = JsonConvert.DeserializeObject<PxMetaResponse>(PxMetaResponseFixture.TEST_META_SV);
            Assert.False(pxMetaResponse.Variables[0].IsSumValue(0), $"Var: {pxMetaResponse.Variables[0].Code} Value: {pxMetaResponse.Variables[0].Values[0]}");
            Assert.False(pxMetaResponse.Variables[1].IsSumValue(0), $"Var: {pxMetaResponse.Variables[1].Code} Value: {pxMetaResponse.Variables[1].Values[0]}");
            Assert.True(pxMetaResponse.Variables[2].IsSumValue(0), $"Var: {pxMetaResponse.Variables[2].Code} Value: {pxMetaResponse.Variables[2].Values[0]}");
            Assert.True(pxMetaResponse.Variables[3].IsSumValue(0), $"Var: {pxMetaResponse.Variables[3].Code} Value: {pxMetaResponse.Variables[3].Values[0]}");
            Assert.True(pxMetaResponse.Variables[4].IsSumValue(0), $"Var: {pxMetaResponse.Variables[4].Code} Value: {pxMetaResponse.Variables[4].Values[0]}");
        }

        [Test]
        public static void IsSumValueTest_En()
        {
            PxMetaResponse pxMetaResponse = JsonConvert.DeserializeObject<PxMetaResponse>(PxMetaResponseFixture.TEST_META_EN);
            Assert.False(pxMetaResponse.Variables[0].IsSumValue(0), $"Var: {pxMetaResponse.Variables[0].Code} Value: {pxMetaResponse.Variables[0].Values[0]}");
            Assert.False(pxMetaResponse.Variables[1].IsSumValue(0), $"Var: {pxMetaResponse.Variables[1].Code} Value: {pxMetaResponse.Variables[1].Values[0]}");
            Assert.True(pxMetaResponse.Variables[2].IsSumValue(0), $"Var: {pxMetaResponse.Variables[2].Code} Value: {pxMetaResponse.Variables[2].Values[0]}");
            Assert.True(pxMetaResponse.Variables[3].IsSumValue(0), $"Var: {pxMetaResponse.Variables[3].Code} Value: {pxMetaResponse.Variables[3].Values[0]}");
            Assert.True(pxMetaResponse.Variables[4].IsSumValue(0), $"Var: {pxMetaResponse.Variables[4].Code} Value: {pxMetaResponse.Variables[4].Values[0]}");
        }
    }
}
