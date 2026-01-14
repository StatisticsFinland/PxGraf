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
        public BlobContainerDatabaseConfig BlobContainerDatabaseConfig { get; private set; }
        public LocalQueryStorageConfig LocalQueryStorageConfig { get; private set; }
        public BlobQueryStorageConfig BlobQueryStorageConfig { get; private set; }
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
                BlobContainerDatabaseConfig = GetBlobContainerDatabaseConfig(configuration),
                LocalQueryStorageConfig = GetLocalQueryStorageConfig(configuration),
                BlobQueryStorageConfig = GetBlobQueryStorageConfig(configuration),
                DatabaseWhitelist = configuration.GetSection("DatabaseWhitelist").Get<string[]>() ?? [],
                AuditLoggingEnabled = configuration.GetValue<bool?>("LogOptions:AuditLog:Enabled") ?? false,
                AuditLogHeaders = configuration.GetSection("LogOptions:AuditLog:IncludedHeaders").Get<string[]>() ?? [],
                PublicationWebhookConfig = GetPublicationWebhookConfig(configuration),
                ApplicationInsights = new ApplicationInsightsConfig(configuration.GetSection(nameof(ApplicationInsights)))
            };

            SetQueryDirectoryFields(newConfig, configuration);

            if (string.IsNullOrEmpty(newConfig.PxWebUrl) &&
                (newConfig.LocalFilesystemDatabaseConfig == null || !newConfig.LocalFilesystemDatabaseConfig.Enabled) &&
                (newConfig.BlobContainerDatabaseConfig == null || !newConfig.BlobContainerDatabaseConfig.Enabled))
            {
                throw new InvalidConfigurationException(
                    "PxWeb URL is not set and neither Local Filesystem Database nor Blob Container Database is enabled. " +
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

        private static BlobContainerDatabaseConfig GetBlobContainerDatabaseConfig(IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("BlobContainerDatabaseConfig");
            if (!section.Exists())
            {
                return null;
            }

            bool enabled = section.GetValue<bool?>("Enabled") ?? false;
            string storageAccountName = section["StorageAccountName"];
            string containerName = section["ContainerName"];
            string rootPath = section["RootPath"] ?? "";

            if (!enabled || string.IsNullOrEmpty(storageAccountName) || string.IsNullOrEmpty(containerName))
            {
                return null;
            }

            return new BlobContainerDatabaseConfig(enabled, storageAccountName, containerName, rootPath);
        }

        private static LocalQueryStorageConfig GetLocalQueryStorageConfig(IConfiguration configuration)
        {
            IConfigurationSection newSection = configuration.GetSection("LocalQueryStorageConfig");
            if (newSection.Exists())
            {
                bool enabled = newSection.GetValue<bool?>("Enabled") ?? false;
                string savedQueryDirectory = newSection["SavedQueryDirectory"];
                string archiveFileDirectory = newSection["ArchiveFileDirectory"];

                if (enabled && !string.IsNullOrEmpty(savedQueryDirectory) && !string.IsNullOrEmpty(archiveFileDirectory))
                {
                    return new LocalQueryStorageConfig(enabled, savedQueryDirectory, archiveFileDirectory);
                }
            }

            // Fallback to legacy configuration if new config doesn't exist
            string legacySavedQueryDirectory = configuration["savedQueryDirectory"];
            string legacyArchiveFileDirectory = configuration["archiveFileDirectory"];

            if (!string.IsNullOrEmpty(legacySavedQueryDirectory) && !string.IsNullOrEmpty(legacyArchiveFileDirectory))
            {
                return new LocalQueryStorageConfig(true, legacySavedQueryDirectory, legacyArchiveFileDirectory);
            }

            return null;
        }

        private static BlobQueryStorageConfig GetBlobQueryStorageConfig(IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("BlobQueryStorageConfig");
            if (!section.Exists())
            {
                return null;
            }

            bool enabled = section.GetValue<bool?>("Enabled") ?? false;
            string storageAccountName = section["StorageAccountName"];
            string containerName = section["ContainerName"];
            string savedQueryPath = section["SavedQueryPath"];
            string archiveFilePath = section["ArchiveFilePath"];

            if (!enabled || string.IsNullOrEmpty(storageAccountName) || string.IsNullOrEmpty(containerName))
            {
                return null;
            }

            return new BlobQueryStorageConfig(enabled, storageAccountName, containerName, savedQueryPath, archiveFilePath);
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

        /// <summary>
        /// Sets the legacy SavedQueryDirectory and ArchiveFileDirectory fields based on the active storage configuration.
        /// This ensures backward compatibility with existing code that depends on these fields.
        /// </summary>
        private static void SetQueryDirectoryFields(Configuration config, IConfiguration configuration)
        {
            if (config.BlobQueryStorageConfig?.Enabled ?? false)
            {
                config.SavedQueryDirectory = config.BlobQueryStorageConfig.SavedQueryPath ?? "";
                config.ArchiveFileDirectory = config.BlobQueryStorageConfig.ArchiveFilePath ?? "";
            }
            else if (config.LocalQueryStorageConfig?.Enabled ?? false)
            {
                config.SavedQueryDirectory = config.LocalQueryStorageConfig.SavedQueryDirectory;
                config.ArchiveFileDirectory = config.LocalQueryStorageConfig.ArchiveFileDirectory;
            }
            else
            {
                config.SavedQueryDirectory = configuration["savedQueryDirectory"];
                config.ArchiveFileDirectory = configuration["archiveFileDirectory"];
            }
        }
    }
}