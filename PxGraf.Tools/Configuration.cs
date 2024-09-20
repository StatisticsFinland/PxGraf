namespace Tools
{
    public class Urls
    {
        public string PxUtils { get; set; }
        public string PxWebApi { get; set; }
        public string PxWebApiOld { get; set; }

        public Urls() { }

        public Urls(string pxUtils, string pxWebApi, string pxWebApiOld)
        {
            PxUtils = pxUtils;
            PxWebApi = pxWebApi;
            PxWebApiOld = pxWebApiOld;
        }
    }

    public class Paths
    {
        public string ResponseDirectory { get; set; }
        public string QueriesFile { get; set; }
        public string ResultsDirectory { get; set; }

        public Paths() { }

        public Paths(string responseTempDirectory, string queriesFile, string resultsDirectory)
        {
            ResponseDirectory = responseTempDirectory;
            QueriesFile = queriesFile;
            ResultsDirectory = resultsDirectory;
        }
    }

    public class Configuration
    {
        public Urls Urls { get; set; }
        public Paths Paths { get; set; }

        public Configuration() { }

        public Configuration(Urls urls, Paths paths)
        {
            Urls = urls;
            Paths = paths;
        }
    }
}
