using System.Collections.Generic;

namespace PxGraf.Settings
{
    public class CorsOptions
    {
        public bool AllowAnyOrigin { get; set; }
        public List<string> AllowedOrigins { get; set; }
    }
}
