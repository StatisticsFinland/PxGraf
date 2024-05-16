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
                    { "CacheOptions:Meta:SlidingExpiration", "10"},
                    { "CacheOptions:Meta:AbsoluteExpiration", "10"},
                    { "CacheOptions:Data:SlidingExpiration", "10"},
                    { "CacheOptions:Data:AbsoluteExpiration", "10"},
                    { "CacheOptions:Database:SlidingExpiration", "10"},
                    { "CacheOptions:Database:AbsoluteExpiration", "10"},
                    { "CacheOptions:Table:SlidingExpiration", "10"},
                    { "CacheOptions:Table:AbsoluteExpiration", "10"},
                    { "CacheOptions:Visualization:SlidingExpiration", "5"},
                    { "CacheOptions:Visualization:AbsoluteExpiration", "30"},
                    { "CacheOptions:Visualization:SizeLimit", "100"},
                    { "CacheOptions:CacheFreshnessCheckInterval", "45"},
                    { "QueryOptions:MaxHeaderLength", "120"},
                    { "QueryOptions:MaxQuerySize", "100000"},
                    { "Language:Default", "fi"},
                    { "Language:Available:0", "fi"},
                    { "Language:Available:1", "sv"},
                    { "Language:Available:2", "en"}
            };
        }
    }
}
