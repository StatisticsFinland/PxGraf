using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.PxWebInterfaceTests.Fixtures
{
    internal static class PxMetaResponseFixture
    {
        internal const string TEST_META_FI = @"
        {
            ""title"": ""Asuntokuntien velat, velkaantumisaste ja tulot muuttujina Vuosi, Tiedot, Maakunta, Viitehenkilön ikä ja Asuntokunnat"",
            ""variables"": [
                {
                    ""code"": ""Vuosi"",
                    ""text"": ""Vuosi"",
                    ""values"": [
                        ""2002"",
                        ""2003"",
                        ""2004"",
                        ""2005"",
                        ""2006"",
                        ""2007"",
                        ""2008"",
                        ""2009"",
                        ""2010"",
                        ""2011"",
                        ""2012"",
                        ""2013"",
                        ""2014"",
                        ""2015"",
                        ""2016"",
                        ""2017"",
                        ""2018"",
                        ""2019"",
                        ""2020"",
                        ""2021""
                    ],
                    ""valueTexts"": [
                        ""2002"",
                        ""2003"",
                        ""2004"",
                        ""2005"",
                        ""2006"",
                        ""2007"",
                        ""2008"",
                        ""2009"",
                        ""2010"",
                        ""2011"",
                        ""2012"",
                        ""2013"",
                        ""2014"",
                        ""2015"",
                        ""2016"",
                        ""2017"",
                        ""2018"",
                        ""2019"",
                        ""2020"",
                        ""2021""
                    ],
                    ""time"": true
                },
                {
                    ""code"": ""Tiedot"",
                    ""text"": ""Tiedot"",
                    ""values"": [
                        ""velat_yht"",
                        ""vaste"",
                        ""kturaha_summa"",
                        ""kturaha_ka"",
                        ""kturaha_med"",
                        ""asuntokunta_lkm""
                    ],
                    ""valueTexts"": [
                        ""Velat yhteensä (euroa)"",
                        ""Velkaantumisaste (%)"",
                        ""Käytettävissä oleva rahatulo (euroa)"",
                        ""Käytettävissä oleva rahatulo, keskiarvo (euroa)"",
                        ""Käytettävissä oleva rahatulo, mediaani (euroa)"",
                        ""Asuntokuntien lukumäärä""
                    ]
                },
                {
                    ""code"": ""Maakunta"",
                    ""text"": ""Maakunta"",
                    ""values"": [
                        ""SSS"",
                        ""MK01"",
                        ""MK02"",
                        ""MK05"",
                        ""MK07"",
                        ""MK08"",
                        ""MK09"",
                        ""MK04"",
                        ""MK06"",
                        ""MK13"",
                        ""MK14"",
                        ""MK15"",
                        ""MK10"",
                        ""MK11"",
                        ""MK12"",
                        ""MK16"",
                        ""MK17"",
                        ""MK18"",
                        ""MK19"",
                        ""MK21""
                    ],
                    ""valueTexts"": [
                        ""KOKO MAA"",
                        ""MK01 Uusimaa"",
                        ""MK02 Varsinais-Suomi"",
                        ""MK05 Kanta-Häme"",
                        ""MK07 Päijät-Häme"",
                        ""MK08 Kymenlaakso"",
                        ""MK09 Etelä-Karjala"",
                        ""MK04 Satakunta"",
                        ""MK06 Pirkanmaa"",
                        ""MK13 Keski-Suomi"",
                        ""MK14 Etelä-Pohjanmaa"",
                        ""MK15 Pohjanmaa"",
                        ""MK10 Etelä-Savo"",
                        ""MK11 Pohjois-Savo"",
                        ""MK12 Pohjois-Karjala"",
                        ""MK16 Keski-Pohjanmaa"",
                        ""MK17 Pohjois-Pohjanmaa"",
                        ""MK18 Kainuu"",
                        ""MK19 Lappi"",
                        ""MK21 Ahvenanmaa - Åland""
                    ],
                    ""elimination"": true
                },
                {
                    ""code"": ""Viitehenkilön ikä"",
                    ""text"": ""Viitehenkilön ikä"",
                    ""values"": [
                        ""SSS"",
                        ""1"",
                        ""2"",
                        ""3"",
                        ""4"",
                        ""5"",
                        ""6"",
                        ""7""
                    ],
                    ""valueTexts"": [
                        ""Yhteensä"",
                        ""0-24"",
                        ""25-34"",
                        ""35-44"",
                        ""45-54"",
                        ""55-64"",
                        ""65-74"",
                        ""75-""
                    ],
                    ""elimination"": true
                },
                {
                    ""code"": ""Asuntokunnat"",
                    ""text"": ""Asuntokunnat"",
                    ""values"": [
                        ""SSS"",
                        ""L5"",
                        ""L1"",
                        ""L2"",
                        ""L3"",
                        ""L4"",
                        ""L6"",
                        ""L7"",
                        ""L8"",
                        ""L9""
                    ],
                    ""valueTexts"": [
                        ""Kaikki asuntokunnat"",
                        ""Velalliset asuntokunnat"",
                        ""Asuntovelalliset asuntokunnat"",
                        ""Asuntokunnat, joilla on muuta velkaa"",
                        ""Asuntokunnat, joilla on elinkeinotoiminnan ja tulolähteen velkaa"",
                        ""Asuntokunnat, joilla on opintovelkaa"",
                        ""Asuntokunnat, joilla ei ole asuntovelkaa"",
                        ""Asuntokunnat, joilla ei ole elinkeinotoiminnan ja tulolähteen velkaa"",
                        ""Asuntokunnat, joilla ei ole muuta velkaa"",
                        ""Asuntokunnat, joilla ei ole opintovelkaa""
                    ]
                }
            ]
        }";

        internal const string TEST_META_SV = @"
        {
            ""title"": ""Bostadshushållernas skulder, skuldsättningsgrad och inkomst efter År, Uppgifter, Landskap, Referenspersons ålder och Bostadsushåll"",
            ""variables"": [
                {
                    ""code"": ""Vuosi"",
                    ""text"": ""År"",
                    ""values"": [
                        ""2002"",
                        ""2003"",
                        ""2004"",
                        ""2005"",
                        ""2006"",
                        ""2007"",
                        ""2008"",
                        ""2009"",
                        ""2010"",
                        ""2011"",
                        ""2012"",
                        ""2013"",
                        ""2014"",
                        ""2015"",
                        ""2016"",
                        ""2017"",
                        ""2018"",
                        ""2019"",
                        ""2020"",
                        ""2021""
                    ],
                    ""valueTexts"": [
                        ""2002"",
                        ""2003"",
                        ""2004"",
                        ""2005"",
                        ""2006"",
                        ""2007"",
                        ""2008"",
                        ""2009"",
                        ""2010"",
                        ""2011"",
                        ""2012"",
                        ""2013"",
                        ""2014"",
                        ""2015"",
                        ""2016"",
                        ""2017"",
                        ""2018"",
                        ""2019"",
                        ""2020"",
                        ""2021""
                    ],
                    ""time"": true
                },
                {
                    ""code"": ""Tiedot"",
                    ""text"": ""Uppgifter"",
                    ""values"": [
                        ""velat_yht"",
                        ""vaste"",
                        ""kturaha_summa"",
                        ""kturaha_ka"",
                        ""kturaha_med"",
                        ""asuntokunta_lkm""
                    ],
                    ""valueTexts"": [
                        ""Skulderna sammanlagt (euro)"",
                        ""Skuldsättningsgrad (procent)"",
                        ""Disponibla penninginkomster (euro)"",
                        ""Disponibla penninginkomster, medeltal (euro)"",
                        ""Disponibla penninginkomster, median (euro)"",
                        ""Antal bostadshushåll""
                    ]
                },
                {
                    ""code"": ""Maakunta"",
                    ""text"": ""Landskap"",
                    ""values"": [
                        ""SSS"",
                        ""MK01"",
                        ""MK02"",
                        ""MK05"",
                        ""MK07"",
                        ""MK08"",
                        ""MK09"",
                        ""MK04"",
                        ""MK06"",
                        ""MK13"",
                        ""MK14"",
                        ""MK15"",
                        ""MK10"",
                        ""MK11"",
                        ""MK12"",
                        ""MK16"",
                        ""MK17"",
                        ""MK18"",
                        ""MK19"",
                        ""MK21""
                    ],
                    ""valueTexts"": [
                        ""HELA LANDET"",
                        ""MK01 Nyland"",
                        ""MK02 Egentliga Finland"",
                        ""MK05 Egentliga Tavastland"",
                        ""MK07 Päijänne-Tavastland"",
                        ""MK08 Kymmenedalen"",
                        ""MK09 Södra Karelen"",
                        ""MK04 Satakunta"",
                        ""MK06 Birkaland"",
                        ""MK13 Mellersta Finland"",
                        ""MK14 Södra Österbotten"",
                        ""MK15 Österbotten"",
                        ""MK10 Södra Savolax"",
                        ""MK11 Norra Savolax"",
                        ""MK12 Norra Karelen"",
                        ""MK16 Mellersta Österbotten"",
                        ""MK17 Norra Österbotten"",
                        ""MK18 Kajanaland"",
                        ""MK19 Lappland"",
                        ""MK21 Åland""
                    ],
                    ""elimination"": true
                },
                {
                    ""code"": ""Viitehenkilön ikä"",
                    ""text"": ""Referenspersons ålder"",
                    ""values"": [
                        ""SSS"",
                        ""1"",
                        ""2"",
                        ""3"",
                        ""4"",
                        ""5"",
                        ""6"",
                        ""7""
                    ],
                    ""valueTexts"": [
                        ""Sammanlagt"",
                        ""0-24"",
                        ""24-34"",
                        ""35-44"",
                        ""45-54"",
                        ""55-64"",
                        ""65-74"",
                        ""75-""
                    ],
                    ""elimination"": true
                },
                {
                    ""code"": ""Asuntokunnat"",
                    ""text"": ""Bostadsushåll"",
                    ""values"": [
                        ""SSS"",
                        ""L5"",
                        ""L1"",
                        ""L2"",
                        ""L3"",
                        ""L4"",
                        ""L6"",
                        ""L7"",
                        ""L8"",
                        ""L9""
                    ],
                    ""valueTexts"": [
                        ""Alla bostadshushåll"",
                        ""Bostadshushåll med skulder"",
                        ""Bostadshushåll med bostadsskuld"",
                        ""Bostadshushåll med övriga skulder"",
                        ""Bostadshushåll med skulder för näringsverksamhet och inkomstkälla"",
                        ""Bostadshushåll med studieskulder"",
                        ""Bostadshushåll utan bostadsskuld"",
                        ""Bostadshushåll utan skulder för näringsverksamhet och inkomstkälla"",
                        ""Bostadshushåll utan övriga skulder"",
                        ""Bostadshushåll utan studieskulder""
                    ]
                }
            ]
        }";

        internal const string TEST_META_EN = @"
        {
            ""title"": ""Debts, rate of indebtedness and income of household-dwelling units by Year, Information, Region, Age of reference person and Household-dwelling units"",
            ""variables"": [
                {
                    ""code"": ""Vuosi"",
                    ""text"": ""Year"",
                    ""values"": [
                        ""2002"",
                        ""2003"",
                        ""2004"",
                        ""2005"",
                        ""2006"",
                        ""2007"",
                        ""2008"",
                        ""2009"",
                        ""2010"",
                        ""2011"",
                        ""2012"",
                        ""2013"",
                        ""2014"",
                        ""2015"",
                        ""2016"",
                        ""2017"",
                        ""2018"",
                        ""2019"",
                        ""2020"",
                        ""2021""
                    ],
                    ""valueTexts"": [
                        ""2002"",
                        ""2003"",
                        ""2004"",
                        ""2005"",
                        ""2006"",
                        ""2007"",
                        ""2008"",
                        ""2009"",
                        ""2010"",
                        ""2011"",
                        ""2012"",
                        ""2013"",
                        ""2014"",
                        ""2015"",
                        ""2016"",
                        ""2017"",
                        ""2018"",
                        ""2019"",
                        ""2020"",
                        ""2021""
                    ],
                    ""time"": true
                },
                {
                    ""code"": ""Tiedot"",
                    ""text"": ""Information"",
                    ""values"": [
                        ""velat_yht"",
                        ""vaste"",
                        ""kturaha_summa"",
                        ""kturaha_ka"",
                        ""kturaha_med"",
                        ""asuntokunta_lkm""
                    ],
                    ""valueTexts"": [
                        ""Debts total (euros)"",
                        ""Rate of indebtedness (per cent)"",
                        ""Disposable monetary income (euros)"",
                        ""Disposable monetary income, mean (euros)"",
                        ""Disposable monetary income, median (euros)"",
                        ""Number of household-dwelling units""
                    ]
                },
                {
                    ""code"": ""Maakunta"",
                    ""text"": ""Region"",
                    ""values"": [
                        ""SSS"",
                        ""MK01"",
                        ""MK02"",
                        ""MK05"",
                        ""MK07"",
                        ""MK08"",
                        ""MK09"",
                        ""MK04"",
                        ""MK06"",
                        ""MK13"",
                        ""MK14"",
                        ""MK15"",
                        ""MK10"",
                        ""MK11"",
                        ""MK12"",
                        ""MK16"",
                        ""MK17"",
                        ""MK18"",
                        ""MK19"",
                        ""MK21""
                    ],
                    ""valueTexts"": [
                        ""WHOLE COUNTRY"",
                        ""MK01 Uusimaa"",
                        ""MK02 Varsinais-Suomi"",
                        ""MK05 Kanta-Häme"",
                        ""MK07 Päijät-Häme"",
                        ""MK08 Kymenlaakso"",
                        ""MK09 South Karelia"",
                        ""MK04 Satakunta"",
                        ""MK06 Pirkanmaa"",
                        ""MK13 Central Finland"",
                        ""MK14 South Ostrobothnia"",
                        ""MK15 Ostrobothnia"",
                        ""MK10 Etelä-Savo"",
                        ""MK11 Pohjois-Savo"",
                        ""MK12 North Karelia"",
                        ""MK16 Central Ostrobothnia"",
                        ""MK17 North Ostrobothnia"",
                        ""MK18 Kainuu"",
                        ""MK19 Lapland"",
                        ""MK21 Åland""
                    ],
                    ""elimination"": true
                },
                {
                    ""code"": ""Viitehenkilön ikä"",
                    ""text"": ""Age of reference person"",
                    ""values"": [
                        ""SSS"",
                        ""1"",
                        ""2"",
                        ""3"",
                        ""4"",
                        ""5"",
                        ""6"",
                        ""7""
                    ],
                    ""valueTexts"": [
                        ""Total"",
                        ""0-24"",
                        ""24-34"",
                        ""35-44"",
                        ""45-54"",
                        ""55-64"",
                        ""65-74"",
                        ""75-""
                    ],
                    ""elimination"": true
                },
                {
                    ""code"": ""Asuntokunnat"",
                    ""text"": ""Household-dwelling units"",
                    ""values"": [
                        ""SSS"",
                        ""L5"",
                        ""L1"",
                        ""L2"",
                        ""L3"",
                        ""L4"",
                        ""L6"",
                        ""L7"",
                        ""L8"",
                        ""L9""
                    ],
                    ""valueTexts"": [
                        ""All household-dwelling units"",
                        ""Household-dwelling units with debts"",
                        ""Household-dwelling units with housing loans"",
                        ""Household-dwelling units with other debts"",
                        ""Household-dwelling units with loans charged on business activities or a source of income"",
                        ""Household-dwelling units with study loans"",
                        ""Household-dwelling units without housing debts"",
                        ""Household-dwelling units without loans charged on business activities or a source of income"",
                        ""Household-dwelling units without other debts"",
                        ""Household-dwelling units without study loans""
                    ]
                }
            ]
        }";
    }
}
