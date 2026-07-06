namespace PxGraf.Settings
{
    public class QueryOptions
    {
        public int MaxHeaderLength { get; set; }

        public int MaxQuerySize { get; set; }

        public double QuerySizeWarningRatio { get; set; } = 0.75;
    }
}
