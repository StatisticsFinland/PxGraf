using Microsoft.Extensions.Configuration;
using PxGraf.Datasource.DatabaseConnection;
using System.Collections.Generic;
using System.Text;

namespace PxGraf.Settings
{
    public class Configuration
    {
        public static Configuration Current { get; private set; }

        public string PxWebUrl { get; private set; }
        public bool CreationAPI { get; private set; }
        public string SavedQueryDirectory { get; private set; }
        public string ArchiveFileDirectory { get; private set; }
        public QueryOptions QueryOptions { get; private set; }
        public LanguageOptions LanguageOptions { get; private set; }
        public CacheOptions CacheOptions { get; private set; }
        public CorsOptions CorsOptions { get; private set; }
        public LocalFilesystemDatabaseConfig LocalFilesystemDatabaseConfig { get; private set; }

        public static void Load(IConfiguration configuration)
        {
            //Set config defaults
            Configuration newConfig = new()
            {
                PxWebUrl = configuration["pxwebUrl"],
                CreationAPI = configuration.GetSection("FeatureManagement:CreationAPI").Get<bool>(),
                SavedQueryDirectory = configuration["savedQueryDirectory"],
                ArchiveFileDirectory = configuration["archiveFileDirectory"],
                QueryOptions = new()
                {
                    MaxHeaderLength = configuration.GetSection("QueryOptions:MaxHeaderLength").Get<int>(),
                    MaxQuerySize = configuration.GetSection("QueryOptions:MaxQuerySize").Get<int>()
                },
                LanguageOptions = new()
                {
                    Default = configuration.GetSection("Language:Default").Get<string>(),
                    Available = configuration.GetSection("Language:Available").Get<List<string>>()
                },
                CacheOptions = new()
                {
                    Meta = configuration.GetSection("CacheOptions:Meta").Get<CacheValues>(),
                    Data = configuration.GetSection("CacheOptions:Data").Get<CacheValues>(),
                    Database = configuration.GetSection("CacheOptions:Database").Get<CacheValues>(),
                    Table = configuration.GetSection("CacheOptions:Table").Get<CacheValues>(),
                    Visualization = configuration.GetSection("CacheOptions:Visualization").Get<CacheValues>(),
                    CacheFreshnessCheckIntervalSeconds = configuration.GetValue<int>("CacheOptions:CacheFreshnessCheckIntervalSeconds")
                },
                CorsOptions = new()
                {
                    AllowAnyOrigin = configuration.GetSection("Cors:AllowAnyOrigin").Get<bool>(),
                    AllowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>(),
                },
                LocalFilesystemDatabaseConfig = new LocalFilesystemDatabaseConfig(
                    configuration.GetSection("LocalFileSystemDatabaseConfig:Enabled").Get<bool>(),
                    configuration["LocalFilesystemDatabaseConfig:DatabaseRootPath"],
                    Encoding.GetEncoding(configuration["LocalFilesystemDatabaseConfig:Encoding"])
                )
            };

            Current = newConfig;
        }
    }
}
