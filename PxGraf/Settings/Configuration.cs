using Microsoft.Extensions.Configuration;
using PxGraf.Datasource.FileDatasource;
using PxGraf.Exceptions;
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
        public string[] DatabaseWhitelist { get; private set; }

        public static void Load(IConfiguration configuration)
        {
            //Set config defaults
            Configuration newConfig = new()
            {
                PxWebUrl = configuration["pxwebUrl"] ?? null,
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
                LocalFilesystemDatabaseConfig = GetLocalDatabaseConfig(configuration),
                DatabaseWhitelist = configuration.GetSection("DatabaseWhitelist").Get<string[]>() ?? []
            };

            if (string.IsNullOrEmpty(newConfig.PxWebUrl) && (newConfig.LocalFilesystemDatabaseConfig == null || !newConfig.LocalFilesystemDatabaseConfig.Enabled))
            {
                throw new InvalidConfigurationException(
                    "PxWeb URL is not set and Local Filesystem Database is not enabled. " +
                    "Please configure at least one of these options in the appsettings.json file."
                );
            }

            Current = newConfig;
        }

        private static LocalFilesystemDatabaseConfig GetLocalDatabaseConfig(IConfiguration configuration)
        {
            if (!configuration.GetSection("LocalFileSystemDatabaseConfig:Enabled").Exists())
            {
                return null;
            }
            else
            {
                return new(
                    configuration.GetSection("LocalFileSystemDatabaseConfig:Enabled").Get<bool?>() ?? false,
                    configuration["LocalFilesystemDatabaseConfig:DatabaseRootPath"] ?? null,
                    !string.IsNullOrEmpty(configuration["LocalFilesystemDatabaseConfig:Encoding"])
                        ? Encoding.GetEncoding(configuration["LocalFilesystemDatabaseConfig:Encoding"])
                        : null
                );
            }
        }
    }
}