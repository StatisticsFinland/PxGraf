using PxGraf.Language;
using System.Collections.Generic;

namespace UnitTests.Fixtures
{
    internal static class TranslationFixture
    {
        public static string DefaultLanguage = "fi";
        public static Dictionary<string, Translation> Translations =
            new()
            {
                ["fi"] = new Translation()
                {
                    ChartTypes = new ChartTypeTranslation()
                    {
                        VerticalBar = "Pystypylväs",
                        GroupVerticalBar = "Pystypylväsryhmä",
                        StackedVerticalBar = "Pinottu pystypylväs",
                        PercentVerticalBar = "Pystypylväs 100 %",
                        HorizontalBar = "Vaakapylväs",
                        GroupHorizontalBar = "Vaakapylväsryhmä",
                        StackedHorizontalBar = "Pinottu vaakapylväs",
                        PercentHorizontalBar = "Vaakapylväs 100 %",
                        Pyramid = "Pyramidi",
                        Pie = "Piirakka",
                        Line = "Viiva",
                        ScatterPlot = "Hajonta",
                        Table = "Taulukko"
                    },
                    RejectionReasons = new RejectionReasonTranslation()
                    {
                        TimeRequired = "Poimintasi ei sisällä aikaulottuvuutta",
                        TimeNotAllowed = "Poimintasi ei voi sisältää aikaulottuvuutta ({0})",
                        TimeOverMax = "Aikaulottuvuudessa ({2}) on liikaa valintoja ({0} / {1})",
                        TimeBelowMin = "Aikaulottuvuudessa ({2}) on liian vähän valintoja ({0} / {1})",
                        ContentRequired = "Poimintasi ei sisällä sisältöulottuvuutta",
                        ContentNotAllowed = "Poimintasi ei voi sisältää sisältöulottuvuutta ({0})",
                        ContentOverMax = "Sisältöulottuvuudessa ({2}) on liikaa valintoja ({0} / {1})",
                        ContentBelowMin = "Sisältöulottuvuudessa ({2}) on liian vähän valintoja ({0} / {1})",
                        UnambiguousContentUnitRequired = "Sisältöulottuvuudessa ({1}) valituilla arvoilla ({0} kpl) tulee olla yhteinen yksikkö",
                        DimensionBelowMin = "Ulottuvuudessa ({2}) on liian vähän valintoja ({0} / {1})",
                        DimensionOverMax = "Ulottuvuudessa ({2}) on liian paljon valintoja ({0} / {1})",
                        MultiselectProductBelowMin = "Monivalintaulottuvuuksien tulo on liian pieni ({0} / {1})",
                        MultiselectProductOverMax = "Monivalintaulottuvuuksien tulo on liian suuri ({0} / {1})",
                        NotEnoughMultiselections = "Poiminnassa ei ole riittävästi monivalintaulottuvuuksia ({0} / {1})",
                        TooManyMultiselections = "Poiminnassa on liikaa monivalintaulottuvuuksia ({0} / {1})",
                        IrregularTimeNotAllowed = "Poiminta ei sisällä säännöllistä aikamuuttujaa",
                        IrregularTimeOverMax = "Valintoja on liikaa ({0} / {1}) valitussa epäsäännöllisessä aikamuuttujassa ({2})",
                        IrregularTimeBelowMin = "Valintoja on liian vähän ({0} / {1}) valitussa epäsäännöllisessä aikamuuttujassa ({2})",
                        TimeOrProgressiveRequired = "Poiminta ei sisällä aikaulottuvuutta tai asteikollista muuttujaa",
                        ProgressiveNotAllowed = "Poiminta ei voi sisältää asteikollista muuttujaa ({0})",
                        ProgressiveRequired = "Poiminta ei sisällä asteikollista muuttujaa",
                        CombinationValuesNotAllowed = "Valintaulottuvuus ({0}) ei voi sisältää arvoa: {1}",
                        NegativeDataNotAllowed = "Poiminta ei voi sisältää negatiivisia tietoja",
                        DataRequired = "Poiminta ei voi olla tyhjä"
                    },
                    SortingOptions = new SortingOptionTranslation()
                    {
                        Ascending = "Nouseva",
                        Descending = "Laskeva",
                        NoSorting = "Ei lajittelua",
                        Sum = "Summa"
                    },
                    MissingData = new MissingDataTranslation()
                    {
                        Missing = "Tieto on puuttuva",
                        CannotRepresent = "Tieto on epälooginen esitettäväksi",
                        Confidential = "Tieto on salassapitosäännön alainen",
                        NotAcquired = "Tietoa ei ole saatu",
                        NotAsked = "Tietoa ei ole kysytty",
                        Empty = "......",
                        Nill = "Ei yhtään"
                    },
                    HeaderTimeFormats = new HeaderTimeFormatsTranslation()
                    {
                        SingleTimeValue = "{0}",
                        TimeRange = "{0}-{1}"
                    },
                    NoteDescription = "Miksi kyseinen kuviotyyppi ei ole mahdollinen tällä valinnalla",
                    TitleVariable = "muuttujana",
                    TitleVariablePlural = "muuttujina",
                    Source = "Lähde",
                    Unit = "Yksikkö"
                },
                ["en"] = new Translation()
                {
                    ChartTypes = new ChartTypeTranslation()
                    {
                        VerticalBar = "Vertical barchart",
                        GroupVerticalBar = "Grouped vertical barchart",
                        StackedVerticalBar = "Stacked vertical barchart",
                        PercentVerticalBar = "Vertical barchart (100%)",
                        HorizontalBar = "Horizontal barchart",
                        GroupHorizontalBar = "Grouped horizontal barchart",
                        StackedHorizontalBar = "Stacked horizontal barchart",
                        PercentHorizontalBar = "Horizontal barchart (100%)",
                        Pyramid = "Pyramidi",
                        Pie = "Pie chart",
                        Line = "Line chart",
                        ScatterPlot = "Scatter chart",
                        Table = "Table"
                    },
                    RejectionReasons = new RejectionReasonTranslation()
                    {
                        TimeRequired = "Your selection does not include time dimension",
                        TimeNotAllowed = "Your selection cannot include time dimension ({0})",
                        TimeOverMax = "There are too many selections ({0} / {1}) in time dimension ({2})",
                        TimeBelowMin = "There are too few selections ({0} / {1}) in time dimension ({2})",
                        ContentRequired = "Your selection does not include content dimension",
                        ContentNotAllowed = "Your selection cannot include content dimension ({0})",
                        ContentOverMax = "There are too many selections ({0} / {1}) in content dimension ({2})",
                        ContentBelowMin = "There are too few selections ({0} / {1}) in content dimension ({2})",
                        UnambiguousContentUnitRequired = "The values ({0} item(s)) selected in content dimension ({1}) must have a common unit",
                        DimensionBelowMin = "The dimension ({2}) has too few selections ({0} / {1})",
                        DimensionOverMax = "The dimension ({2}) has too many selections ({0} / {1})",
                        MultiselectProductBelowMin = "The product of multiple choice dimensions is too low ({0} / {1})",
                        MultiselectProductOverMax = "The product of multiple choice dimensions is too high ({0} / {1})",
                        NotEnoughMultiselections = "The selection does not have enough multiple choice dimensions ({0} / {1})",
                        TooManyMultiselections = "The selection has too many multiple choice dimensions ({0} / {1})",
                        IrregularTimeNotAllowed = "Your selection does not include a regular time variable",
                        IrregularTimeOverMax = "There are too many selections ({0} / {1}) for the selected irregular time variable ({2})",
                        IrregularTimeBelowMin = "There are too few selections ({0} / {1}) for the selected irregular time variable ({2})",
                        TimeOrProgressiveRequired = "The selection does not include time dimension or ordinal scale dimension",
                        ProgressiveNotAllowed = "The selection cannot contain ordinal scale dimension ({0})",
                        ProgressiveRequired = "The selection does not include ordinal scale dimension",
                        CombinationValuesNotAllowed = "The selection dimension ({0}) cannot contain the value: {1}",
                        NegativeDataNotAllowed = "The selection cannot contain negative data",
                        DataRequired = "The selection cannot be empty"
                    },
                    SortingOptions = new SortingOptionTranslation()
                    {
                        Ascending = "Ascending",
                        Descending = "Descending",
                        NoSorting = "No sorting",
                        Sum = "Sum"
                    },
                    MissingData = new MissingDataTranslation()
                    {
                        Missing = "Missing",
                        CannotRepresent = "Not applicable",
                        Confidential = "Data is subject to secrecy",
                        NotAcquired = "Not available",
                        NotAsked = "Not asked",
                        Empty = "......",
                        Nill = "Magnitude nil"
                    },
                    HeaderTimeFormats = new HeaderTimeFormatsTranslation()
                    {
                        SingleTimeValue = "{0}",
                        TimeRange = "in {0} to {1}"
                    },
                    NoteDescription = "Why this chart type is not possible with this selection",
                    TitleVariable = "by",
                    TitleVariablePlural = "by",
                    Source = "Source",
                    Unit = "Unit"
                },
                ["sv"] = new Translation()
                {
                    ChartTypes = new ChartTypeTranslation()
                    {
                        VerticalBar = "Vertikal column",
                        GroupVerticalBar = "Vertikal column grouperad",
                        StackedVerticalBar = "Vertikal Staplad",
                        PercentVerticalBar = "Vertikal Staplad 100%",
                        HorizontalBar = "Horisontal Kolumndiagram",
                        GroupHorizontalBar = "Horisontal Stapel Gruperad",
                        StackedHorizontalBar = "Horizontal Kolumn Stapel",
                        PercentHorizontalBar = "Horizontal Kolumn 100%",
                        Pyramid = "Pyramidi",
                        Pie = "Pajdiagram",
                        Line = "Linjediagram",
                        ScatterPlot = "Spridningsdiagram",
                        Table = "Tabell"
                    },
                    RejectionReasons = new RejectionReasonTranslation()
                    {
                        TimeRequired = "Ditt uttag har ingen tidsdimension",
                        TimeNotAllowed = "Ditt uttag kan inte innehålla en tidsdimension ({0})",
                        TimeOverMax = "Det finns för många val ({0} / {1}) i tidsdimensionen ({2})",
                        TimeBelowMin = "Det finns för få val ({0} / {1}) i tidsdimensionen ({2})",
                        ContentRequired = "Ditt uttag har ingen innehållsdimension",
                        ContentNotAllowed = "Ditt uttag kan inte innehålla en innehållsdimension ({0})",
                        ContentOverMax = "Det finns för många val ({0} / {1}) i innehållsdimensionen ({2})",
                        ContentBelowMin = "Det finns för få val ({0} / {1}) i innehållsdimensionen ({2})",
                        UnambiguousContentUnitRequired = "De värden ({0} st.) som valts i innehållsdimensionen ({1}) ska ha en gemensam enhet",
                        DimensionBelowMin = "Det finns för få val ({0} / {1}) i dimensionen ({2})",
                        DimensionOverMax = "Det finns för många val ({0} / {1}) i dimensionen ({2})",
                        MultiselectProductBelowMin = "Inkomsten i flervalsdimensionerna är för liten ({0} / {1})",
                        MultiselectProductOverMax = "Inkomsten i flervalsdimensionerna är för stor ({0} / {1})",
                        NotEnoughMultiselections = "Det finns inte tillräckligt med flervalsdimensioner ({0} / {1}) i uttaget",
                        TooManyMultiselections = "Det finns för många flervalsdimensioner ({0} / {1}) i uttaget",
                        IrregularTimeNotAllowed = "Ditt uttag har ingen regelbunden tidsvariabel",
                        IrregularTimeOverMax = "Ditt uttag ur den oregelbundna tidsvariabeln ({2}) har för många val ({0} / {1})",
                        IrregularTimeBelowMin = "Ditt uttag ur den oregelbundna tidsvariabeln ({2}) har för få val ({0} / {1})",
                        TimeOrProgressiveRequired = "Uttaget har ingen tidsdimension eller ingen dimension på en ordinalskala",
                        ProgressiveNotAllowed = "Uttaget kan inte innehålla en dimension på en ordinalskala ({0})",
                        ProgressiveRequired = "Uttaget har ingen dimension på en ordinalskala",
                        CombinationValuesNotAllowed = "Uttagsdimensionen ({0}) kan inte innehålla värdet: {1}",
                        NegativeDataNotAllowed = "Uttaget kan inte innehålla negativa data",
                        DataRequired = "Uttaget kan inte vara tomt"
                    },
                    SortingOptions = new SortingOptionTranslation()
                    {
                        Ascending = "Stigande",
                        Descending = "Fallande",
                        NoSorting = "Ingen sortering",
                        Sum = "Summa"
                    },
                    MissingData = new MissingDataTranslation()
                    {
                        Missing = "Saknas",
                        CannotRepresent = "Ej tillämpligt",
                        Confidential = "Data omfattas av sekretess",
                        NotAcquired = "Ej tillgängligt",
                        NotAsked = "Ej frågat",
                        Empty = "......",
                        Nill = "Magnitude nil"
                    },
                    HeaderTimeFormats = new HeaderTimeFormatsTranslation()
                    {
                        SingleTimeValue = "{0}",
                        TimeRange = "i {0} till {1}"
                    },
                    NoteDescription = "Varför denna diagramtyp inte är möjlig med denna val",
                    TitleVariable = "av",
                    TitleVariablePlural = "av",
                    Source = "Källa",
                    Unit = "Enhet"
                }
            };
    }
  }


