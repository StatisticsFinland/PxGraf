namespace PxGraf.Settings
{
    public class CacheValues
    {
        // TODO: Name these with units, i.e. AbsoluteExpirationMinutes
        public int AbsoluteExpiration { get; set; }
        public int SlidingExpiration { get; set; }
        public int SizeLimit { get; set; }
    }
}
