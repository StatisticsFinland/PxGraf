namespace PxGraf.Settings
{
    public class CacheOptions
    {
        public CacheValues Meta { get; set; }
        public CacheValues Data { get; set; }
        public CacheValues Database { get; set; }
        public CacheValues Table { get; set; }
        public CacheValues Visualization { get; set; }
        public int CacheFreshnessCheckIntervalSeconds { get; set; }
    }
}
