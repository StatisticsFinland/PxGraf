using Microsoft.Extensions.Configuration;
using PxGraf.Datasource;
using PxGraf.Datasource.ApiDatasource;
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

        /// <summary>
        /// The active database (Px file data source) configuration. Exactly one must be set.
        /// Use pattern matching to determine the concrete type:
        /// <see cref="PxWebDatabaseConfig"/>, <see cref="LocalFilesystemDatabaseConfig"/>, or <see cref="BlobContainerDatabaseConfig"/>.
        /// </summary>
        public DatabaseConfig DatabaseConfig { get; private set; }

        /// <summary>
        /// The active query storage configuration.
        /// Use pattern matching to determine the concrete type:
        /// <see cref="LocalQueryStorageConfig"/> or <see cref="BlobQueryStorageConfig"/>.
        /// </summary>
        public IQueryStorageConfig QueryStorageConfig { get; private set; }

        public string PxWebUrl => DatabaseConfig is PxWebDatabaseConfig pxWeb ? pxWeb.PxWebUrl : null;
        public bool CreationAPI { get; private set; }
        public string SavedQueryDirectory => QueryStorageConfig?.SavedQueryPath;
        public string ArchiveFileDirectory => QueryStorageConfig?.ArchiveFilePath;
        public QueryOptions QueryOptions { get; private set; }
        public LanguageOptions LanguageOptions { get; private set; }
        public CacheOptions CacheOptions { get; private set; }
        public CorsOptions CorsOptions { get; private set; }
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
                DatabaseConfig = GetDatabaseConfig(configuration),
                QueryStorageConfig = GetQueryStorageConfig(configuration),
                DatabaseWhitelist = configuration.GetSection(nameof(DatabaseWhitelist)).Get<string[]>() ?? [],
                AuditLoggingEnabled = configuration.GetValue<bool?>("LogOptions:AuditLog:Enabled") ?? false,
                AuditLogHeaders = configuration.GetSection("LogOptions:AuditLog:IncludedHeaders").Get<string[]>() ?? [],
                PublicationWebhookConfig = GetPublicationWebhookConfig(configuration),
                ApplicationInsights = new ApplicationInsightsConfig(configuration.GetSection(nameof(ApplicationInsights)))
            };

            if (newConfig.DatabaseConfig == null)
            {
                throw new InvalidConfigurationException(
                    "No database configuration found. " +
                    "Please set DatabaseConfig.Type to one of: PxWeb, LocalFileSystem, BlobContainer " +
                    "in the appsettings.json file."
                );
            }

            Current = newConfig;
        }

        /// <summary>
        /// Reads the DatabaseConfig section, switches on the Type enum, and builds the
        /// corresponding <see cref="DatabaseConfig"/> subclass. Throws if required fields
        /// are missing for the selected type.
        /// </summary>
        private static DatabaseConfig GetDatabaseConfig(IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection(nameof(DatabaseConfig));
            if (!section.Exists())
            {
                return null;
            }

            string typeValue = section["Type"];
            if (string.IsNullOrEmpty(typeValue) || !Enum.TryParse(typeValue, ignoreCase: true, out DatabaseConfigType type))
            {
                return null;
            }

            return type switch
            {
                DatabaseConfigType.PxWeb => BuildPxWebDatabaseConfig(section),
                DatabaseConfigType.LocalFileSystem => BuildLocalFilesystemDatabaseConfig(section),
                DatabaseConfigType.BlobContainer => BuildBlobContainerDatabaseConfig(section),
                _ => null
            };
        }

        private static PxWebDatabaseConfig BuildPxWebDatabaseConfig(IConfigurationSection section)
        {
            string pxWebUrl = section[nameof(PxWebDatabaseConfig.PxWebUrl)];
            if (string.IsNullOrWhiteSpace(pxWebUrl))
                throw new InvalidConfigurationException($"DatabaseConfig.Type is PxWeb but {nameof(PxWebDatabaseConfig.PxWebUrl)} is not set or is empty.");
            return new PxWebDatabaseConfig(pxWebUrl);
        }

        private static LocalFilesystemDatabaseConfig BuildLocalFilesystemDatabaseConfig(IConfigurationSection section)
        {
            string databaseRootPath = section[nameof(LocalFilesystemDatabaseConfig.DatabaseRootPath)];
            if (string.IsNullOrWhiteSpace(databaseRootPath))
                throw new InvalidConfigurationException($"DatabaseConfig.Type is LocalFileSystem but {nameof(LocalFilesystemDatabaseConfig.DatabaseRootPath)} is not set or is empty.");

            string encodingName = section[nameof(LocalFilesystemDatabaseConfig.Encoding)];
            if (string.IsNullOrWhiteSpace(encodingName))
                throw new InvalidConfigurationException($"DatabaseConfig.Type is LocalFileSystem but {nameof(LocalFilesystemDatabaseConfig.Encoding)} is not set or is empty.");

            Encoding encoding;
            try
            {
                encoding = Encoding.GetEncoding(encodingName);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidConfigurationException($"DatabaseConfig.Type is LocalFileSystem but the specified encoding '{encodingName}' is invalid.", ex);
            }
            return new LocalFilesystemDatabaseConfig(databaseRootPath, encoding);
        }

        private static BlobContainerDatabaseConfig BuildBlobContainerDatabaseConfig(IConfigurationSection section)
        {
            string storageAccountName = section[nameof(BlobContainerDatabaseConfig.StorageAccountName)];
            if (string.IsNullOrWhiteSpace(storageAccountName))
                throw new InvalidConfigurationException($"DatabaseConfig.Type is BlobContainer but {nameof(BlobContainerDatabaseConfig.StorageAccountName)} is not set or is empty.");

            string containerName = section[nameof(BlobContainerDatabaseConfig.ContainerName)];
            if (string.IsNullOrWhiteSpace(containerName))
                throw new InvalidConfigurationException($"DatabaseConfig.Type is BlobContainer but {nameof(BlobContainerDatabaseConfig.ContainerName)} is not set or is empty.");

            string rootPath = section[nameof(BlobContainerDatabaseConfig.RootPath)] ?? "";
            string managedIdentityClientId = section[nameof(BlobContainerDatabaseConfig.ManagedIdentityClientId)];
            return new BlobContainerDatabaseConfig(storageAccountName, containerName, rootPath, managedIdentityClientId);
        }

        /// <summary>
        /// Reads the IQueryStorageConfig section, switches on the Type enum, and builds the
        /// corresponding <see cref="QueryStorageConfig"/> subclass. Falls back to legacy
        /// savedQueryDirectory/archiveFileDirectory when the section is absent.
        /// </summary>
        private static IQueryStorageConfig GetQueryStorageConfig(IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection(nameof(QueryStorageConfig));
            if (section.Exists())
            {
                string typeValue = section["Type"];
                if (!string.IsNullOrEmpty(typeValue) && Enum.TryParse(typeValue, ignoreCase: true, out QueryStorageConfigType type))
                {
                    return type switch
                    {
                        QueryStorageConfigType.LocalFileSystem => BuildLocalQueryStorageConfig(section),
                        QueryStorageConfigType.BlobContainer => BuildBlobQueryStorageConfig(section),
                        _ => null
                    };
                }
            }

            // Fallback to legacy configuration
            string legacySavedQueryDirectory = configuration["savedQueryDirectory"];
            string legacyArchiveFileDirectory = configuration["archiveFileDirectory"];

            if (!string.IsNullOrEmpty(legacySavedQueryDirectory) && !string.IsNullOrEmpty(legacyArchiveFileDirectory))
            {
                return new LocalQueryStorageConfig(legacySavedQueryDirectory, legacyArchiveFileDirectory);
            }

            return null;
        }

        private static LocalQueryStorageConfig BuildLocalQueryStorageConfig(IConfigurationSection section)
        {
            string savedQueryDirectory = section[nameof(LocalQueryStorageConfig.SavedQueryDirectory)]
                ?? throw new InvalidConfigurationException(
                    $"IQueryStorageConfig.Type is LocalFileSystem but {nameof(LocalQueryStorageConfig.SavedQueryDirectory)} is not set.");

            string archiveFileDirectory = section[nameof(LocalQueryStorageConfig.ArchiveFileDirectory)]
                ?? throw new InvalidConfigurationException(
                    $"IQueryStorageConfig.Type is LocalFileSystem but {nameof(LocalQueryStorageConfig.ArchiveFileDirectory)} is not set.");

            return new LocalQueryStorageConfig(savedQueryDirectory, archiveFileDirectory);
        }

        private static BlobQueryStorageConfig BuildBlobQueryStorageConfig(IConfigurationSection section)
        {
            string storageAccountName = section[nameof(BlobQueryStorageConfig.StorageAccountName)]
                ?? throw new InvalidConfigurationException(
                    $"IQueryStorageConfig.Type is BlobContainer but {nameof(BlobQueryStorageConfig.StorageAccountName)} is not set.");

            string containerName = section[nameof(BlobQueryStorageConfig.ContainerName)]
                ?? throw new InvalidConfigurationException(
                    $"IQueryStorageConfig.Type is BlobContainer but {nameof(BlobQueryStorageConfig.ContainerName)} is not set.");

            string savedQueryPath = section[nameof(BlobQueryStorageConfig.SavedQueryPath)];
            string archiveFilePath = section[nameof(BlobQueryStorageConfig.ArchiveFilePath)];
            string managedIdentityClientId = section[nameof(BlobQueryStorageConfig.ManagedIdentityClientId)];
            return new BlobQueryStorageConfig(storageAccountName, containerName, savedQueryPath, archiveFilePath, managedIdentityClientId);
        }

        private static PublicationWebhookConfiguration GetPublicationWebhookConfig(IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection(nameof(PublicationWebhookConfiguration));
            if (!section.Exists())
            {
                return new PublicationWebhookConfiguration();
            }

            return new PublicationWebhookConfiguration
            {
                BaseUrl = section[nameof(PublicationWebhookConfiguration.BaseUrl)],
                WebhookEndpointPath = section[nameof(PublicationWebhookConfiguration.WebhookEndpointPath)],
                HealthCheckEndpointPath = section[nameof(PublicationWebhookConfiguration.HealthCheckEndpointPath)],
                AccessTokenHeaderName = section[nameof(PublicationWebhookConfiguration.AccessTokenHeaderName)],
                AccessTokenHeaderValue = section[nameof(PublicationWebhookConfiguration.AccessTokenHeaderValue)],
                BodyContentPropertyNames = section.GetSection(nameof(PublicationWebhookConfiguration.BodyContentPropertyNames)).Get<PublicationPropertyType[]>() ?? [],
                BodyContentPropertyNameEdits = section.GetSection(nameof(PublicationWebhookConfiguration.BodyContentPropertyNameEdits)).Get<Dictionary<PublicationPropertyType, string>>() ?? [],
                VisualizationTypeTranslations = section.GetSection(nameof(PublicationWebhookConfiguration.VisualizationTypeTranslations)).Get<Dictionary<string, string>>() ?? [],
                MetadataProperties = section.GetSection(nameof(PublicationWebhookConfiguration.MetadataProperties)).Get<Dictionary<string, string>>() ?? []
            };
        }
    }
}