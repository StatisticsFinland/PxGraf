using System.Net.Http;
using System.Net;

namespace UnitTests.DatasourceTests
{
    public static class PxWebV1ApiInterfaceFixtures
    {
        public const string MockEnContent = "{ " +
                "\"title\":\"FooBar.en\", " +
                "\"variables\":[" +
                "{" +
                "\"code\":\"variable-0\", " +
                "\"text\":\"variable-0-text.en\", " +
                "\"values\":[\"value-0\", \"value-1\"], " +
                "\"valueTexts\":[\"value-0-text.en\", \"value-1-text.en\"], " +
                "\"elimination\":false, " +
                "\"time\":false" +
                "}," +
                "{" +
                "\"code\":\"variable-1\", " +
                "\"text\":\"variable-1-text.en\", " +
                "\"values\":[\"2000\", \"2001\"], " +
                "\"valueTexts\":[\"2000\", \"2001\"], " +
                "\"elimination\":false, " +
                "\"time\":true" +
                "}" +
                "]" +
                "}";

        public const string MockFiContent = "{ " +
                "\"title\":\"FooBar\", " +
                "\"variables\":[" +
                "{" +
                "\"code\":\"variable-0\", " +
                "\"text\":\"variable-0-text\", " +
                "\"values\":[\"value-0\", \"value-1\"], " +
                "\"valueTexts\":[\"value-0-text\", \"value-1-text\"], " +
                "\"elimination\":false, " +
                "\"time\":false" +
                "}," +
                "{" +
                "\"code\":\"variable-1\", " +
                "\"text\":\"variable-1-text\", " +
                "\"values\":[\"2000\", \"2001\"], " +
                "\"valueTexts\":[\"2000\", \"2001\"], " +
                "\"elimination\":false, " +
                "\"time\":true" +
                "}" +
                "]" +
                "}";

        public const string MockSvContent = "{ " +
                "\"title\":\"FooBar.sv\", " +
                "\"variables\":[" +
                "{" +
                "\"code\":\"variable-0\", " +
                "\"text\":\"variable-0-text.sv\", " +
                "\"values\":[\"value-0\", \"value-1\"], " +
                "\"valueTexts\":[\"value-0-text.sv\", \"value-1-text.sv\"], " +
                "\"elimination\":false, " +
                "\"time\":false" +
                "}," +
                "{" +
                "\"code\":\"variable-1\", " +
                "\"text\":\"variable-1-text.sv\", " +
                "\"values\":[\"2000\", \"2001\"], " +
                "\"valueTexts\":[\"2000\", \"2001\"], " +
                "\"elimination\":false, " +
                "\"time\":true" +
                "}" +
                "]" +
                "}";

        public const string MockEnJsonStat2 = "{ " +
                "\"class\":\"dataset\", " +
                "\"label\":\"FooBarAa\", " +
                "\"source\":\"SourceAa\", " +
                "\"updated\":\"2001-08-24T13:15:00.000\", " +
                "\"id\":[\"variable-0\", \"variable-1\"], " +
                "\"size\":[2, 2], " +
                "\"dimension\":{" +
                "\"variable-0\":{" +
                "\"label\":\"variable-0-label\", " +
                "\"category\":{" +
                "\"index\":{\"value-0\":0, \"value-1\":1}, " +
                "\"label\":{\"value-0\":\"value-0-text.en\", \"value-1\":\"value-1-text.en\"}," +
                "\"unit\":{\"value-0\":{\"base\":\"unit.en\"}, \"value-1\":{\"base\":\"unit.en\"}}" +
                "}" +
                "}," +
                "\"variable-1\":{" +
                "\"label\":\"variable-1-label\", " +
                "\"category\":{" +
                "\"index\":{\"2000\":0, \"2001\":1}, " +
                "\"label\":{\"2000\":\"2000\", \"2001\":\"2001\"}" +
                "}" +
                "}" +
                "}, " +
                "\"value\":[1.0, 2.0, 3.0, 4.0], " +
                "\"status\":{}, " +
                "\"role\":{" +
                "\"time\":[\"variable-1\"], " +
                "\"metric\":[\"variable-0\"]" +
                "}, " +
                "\"version\":\"2.0\", " +
                "\"extension\":{}" +
                "}";

        public const string MockFiJsonStat2 = "{ " +
                "\"class\":\"dataset\", " +
                "\"label\":\"FooBarBb\", " +
                "\"source\":\"SourceBb\", " +
                "\"updated\":\"2001-08-24T13:15:00.000\", " +
                "\"id\":[\"variable-0\", \"variable-1\"], " +
                "\"size\":[2, 2], " +
                "\"dimension\":{" +
                "\"variable-0\":{" +
                "\"label\":\"variable-0-label\", " +
                "\"category\":{" +
                "\"index\":{\"value-0\":0, \"value-1\":1}, " +
                "\"label\":{\"value-0\":\"value-0-text\", \"value-1\":\"value-1-text\"}," +
                "\"unit\":{\"value-0\":{\"base\":\"unit\"}, \"value-1\":{\"base\":\"unit\"}}" +
                "}" +
                "}," +
                "\"variable-1\":{" +
                "\"label\":\"variable-1-label\", " +
                "\"category\":{" +
                "\"index\":{\"2000\":0, \"2001\":1}, " +
                "\"label\":{\"2000\":\"2000\", \"2001\":\"2001\"}" +
                "}" +
                "}" +
                "}, " +
                "\"value\":[1.0, 2.0, 3.0, 4.0], " +
                "\"status\":{}, " +
                "\"role\":{" +
                "\"time\":[\"variable-1\"], " +
                "\"metric\":[\"variable-0\"]" +
                "}, " +
                "\"version\":\"2.0\", " +
                "\"extension\":{}" +
                "}";

        public const string MockSvJsonStat2 = "{ " +
                "\"class\":\"dataset\", " +
                "\"label\":\"FooBarCc\", " +
                "\"source\":\"SourceCc\", " +
                "\"updated\":\"2001-08-24T13:15:00.000\", " +
                "\"id\":[\"variable-0\", \"variable-1\"], " +
                "\"size\":[2, 2], " +
                "\"dimension\":{" +
                "\"variable-0\":{" +
                "\"label\":\"variable-0-label\", " +
                "\"category\":{" +
                "\"index\":{\"value-0\":0, \"value-1\":1}, " +
                "\"label\":{\"value-0\":\"value-0-text.sv\", \"value-1\":\"value-1-text.sv\"}," +
                "\"unit\":{\"value-0\":{\"base\":\"unit.sv\"}, \"value-1\":{\"base\":\"unit.sv\"}}" +
                "}" +
                "}," +
                "\"variable-1\":{" +
                "\"label\":\"variable-1-label\", " +
                "\"category\":{" +
                "\"index\":{\"2000\":0, \"2001\":1}, " +
                "\"label\":{\"2000\":\"2000\", \"2001\":\"2001\"}" +
                "}" +
                "}" +
                "}, " +
                "\"value\":[1.0, 2.0, 3.0, 4.0], " +
                "\"status\":{}, " +
                "\"role\":{" +
                "\"time\":[\"variable-1\"], " +
                "\"metric\":[\"variable-0\"]" +
                "}, " +
                "\"version\":\"2.0\", " +
                "\"extension\":{}" +
                "}";
    }
}
