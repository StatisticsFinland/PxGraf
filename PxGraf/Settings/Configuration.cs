using Microsoft.Extensions.Configuration;
using PxGraf.Datasource.FileDatasource;
using PxGraf.Exceptions;
using PxGraf.Services;
using System;
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

        public bool AuditLoggingEnabled { get; private set; }
        public string[] AuditLogHeaders { get; private set; }
        public ApplicationInsightsConfig ApplicationInsights { get; private set; }
        public PublicationWebhookConfiguration PublicationWebhookConfig { get; private set; }

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
                DatabaseWhitelist = configuration.GetSection("DatabaseWhitelist").Get<string[]>() ?? [],
                AuditLoggingEnabled = configuration.GetValue<bool?>("LogOptions:AuditLog:Enabled") ?? false,
                AuditLogHeaders = configuration.GetSection("LogOptions:AuditLog:IncludedHeaders").Get<string[]>() ?? [],
                PublicationWebhookConfig = GetPublicationWebhookConfig(configuration),
                ApplicationInsights = new ApplicationInsightsConfig(configuration.GetSection(nameof(ApplicationInsights)))
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
            IConfigurationSection section = configuration.GetSection("LocalFileSystemDatabaseConfig");
            if (!section.Exists())
            {
                return null;
            }

            bool enabled = section.GetValue<bool?>("Enabled") ?? false;
            string databaseRootPath = section["DatabaseRootPath"];
            string encodingName = section["Encoding"];
            Encoding encoding = !string.IsNullOrEmpty(encodingName) ? Encoding.GetEncoding(encodingName) : null;

            if (!enabled || string.IsNullOrEmpty(databaseRootPath) || encoding == null)
            {
                return null;
            }

            return new LocalFilesystemDatabaseConfig(enabled, databaseRootPath, encoding);
        }

        private static PublicationWebhookConfiguration GetPublicationWebhookConfig(IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("PublicationWebhookConfiguration");
            if (!section.Exists())
            {
                return new PublicationWebhookConfiguration();
            }

            return new PublicationWebhookConfiguration
            {
                EndpointUrl = section["EndpointUrl"],
                AccessTokenHeaderName = section["AccessTokenHeaderName"],
                AccessTokenHeaderValue = section["AccessTokenHeaderValue"],
                BodyContentPropertyNames = section.GetSection("BodyContentPropertyNames").Get<PublicationPropertyType[]>() ?? [],
                BodyContentPropertyNameEdits = section.GetSection("BodyContentPropertyNameEdits").Get<Dictionary<PublicationPropertyType, string>>() ?? [],
                VisualizationTypeTranslations = section.GetSection("VisualizationTypeTranslations").Get<Dictionary<string, string>>() ?? [],
                MetadataProperties = section.GetSection("MetadataProperties").Get<Dictionary<string, string>>() ?? []
            };
        }
    }
}