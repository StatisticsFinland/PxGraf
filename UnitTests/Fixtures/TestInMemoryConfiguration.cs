using System.Collections.Generic;

namespace UnitTests.Fixtures
{
    public static class TestInMemoryConfiguration
    {
        public static Dictionary<string, string> Get()
        {
            return new Dictionary<string, string>()
            {
                    {"pxwebUrl", "http://pxwebtesturl:12345/"},
                    { "pxgrafUrl", "http://pxgraftesturl:8443/PxGraf"},
                    { "savedQueryDirectory", "goesNowhere"},
                    { "archiveFileDirectory", "goesNowhere"},
                    { "CacheOptions:Meta:SlidingExpirationMinutes", "10"},
                    { "CacheOptions:Meta:AbsoluteExpirationMinutes", "10"},
                    { "CacheOptions:Data:SlidingExpirationMinutes", "10"},
                    { "CacheOptions:Data:AbsoluteExpirationMinutes", "10"},
                    { "CacheOptions:Database:SlidingExpirationMinutes", "10"},
                    { "CacheOptions:Database:AbsoluteExpirationMinutes", "10"},
                    { "CacheOptions:Table:SlidingExpirationMinutes", "10"},
                    { "CacheOptions:Table:AbsoluteExpirationMinutes", "10"},
                    { "CacheOptions:Visualization:SlidingExpirationMinutes", "5"},
                    { "CacheOptions:Visualization:AbsoluteExpirationMinutes", "30"},
                    { "CacheOptions:Visualization:ItemAmountLimit", "100"},
                    { "CacheOptions:CacheFreshnessCheckIntervalSeconds", "45"},
                    { "QueryOptions:MaxHeaderLength", "120"},
                    { "QueryOptions:MaxQuerySize", "100000"},
                    { "Language:Default", "fi"},
                    { "Language:Available:0", "fi"},
                    { "Language:Available:1", "sv"},
                    { "Language:Available:2", "en"},
                    { "LocalFilesystemDatabaseConfig:Encoding", "latin1"}
            };
        }
    }
}
