using Microsoft.Extensions.Logging;
using PxGraf.Datasource;
using PxGraf.Models.Responses;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Services
{
    /// <summary>
    /// Health check orchestrator that probes all configured dependencies and aggregates their status.
    /// </summary>
    /// <param name="datasource">Cached datasource for probing the database connection.</param>
    /// <param name="sqFileInterface">File interface for probing saved query and archive storage.</param>
    /// <param name="webhookService">Webhook service for probing the publication webhook endpoint.</param>
    /// <param name="logger">Logger instance.</param>
    public class HealthCheckService(
        ICachedDatasource datasource,
        ISqFileInterface sqFileInterface,
        IPublicationWebhookService webhookService,
        ILogger<HealthCheckService> logger) : IHealthCheckService
    {
        private const string Healthy = "healthy";
        private const string Unhealthy = "unhealthy";
        private const string Database = "database";

        /// <inheritdoc/>
        public async Task<HealthResponse> CheckHealthAsync()
        {
            Task<DatabaseHealthStatus> dbTask = ProbeDatabaseAsync();
            Task<ServiceHealthStatus> sqTask = ProbeStorageAsync("saved-query-storage", () => sqFileInterface.CanAccessSavedQueriesAsync(Configuration.Current.SavedQueryDirectory));
            Task<ServiceHealthStatus> archiveTask = ProbeStorageAsync("archive-file-storage", () => sqFileInterface.CanAccessArchivesAsync(Configuration.Current.ArchiveFileDirectory));

            List<DatabaseHealthStatus> databases = [await dbTask];
            List<ServiceHealthStatus> services = [await sqTask, await archiveTask];

            if (Configuration.Current.CreationAPI && Configuration.Current.PublicationWebhookConfig is { IsEnabled: true, HasHealthCheckEndpoint: true })
            {
                services.Add(await ProbeWebhookAsync());
            }

            bool allHealthy = databases.All(d => d.Status == Healthy) && services.All(s => s.Status == Healthy);
            string overallStatus = allHealthy ? Healthy : Unhealthy;

            return new HealthResponse(overallStatus, databases, services);
        }

        /// <summary>
        /// Probes the configured database connection by requesting the root group contents.
        /// </summary>
        private async Task<DatabaseHealthStatus> ProbeDatabaseAsync()
        {
            try
            {
                await datasource.GetGroupContentsCachedAsync([]);
                return new DatabaseHealthStatus(Database, Healthy);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Health check failed for database {DatabaseId}", Database);
                return new DatabaseHealthStatus(Database, Unhealthy);
            }
        }

        /// <summary>
        /// Probes a storage service by executing the provided access check function.
        /// </summary>
        /// <param name="serviceId">Identifier for the service being probed.</param>
        /// <param name="accessCheck">Function that returns true if the storage is accessible.</param>
        private async Task<ServiceHealthStatus> ProbeStorageAsync(string serviceId, Func<Task<bool>> accessCheck)
        {
            try
            {
                bool accessible = await accessCheck();
                return new ServiceHealthStatus(serviceId, accessible ? Healthy : Unhealthy);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Health check failed for service {ServiceId}", serviceId);
                return new ServiceHealthStatus(serviceId, Unhealthy);
            }
        }

        /// <summary>
        /// Probes the publication webhook endpoint by checking its reachability.
        /// </summary>
        private async Task<ServiceHealthStatus> ProbeWebhookAsync()
        {
            try
            {
                bool reachable = await webhookService.CheckWebhookReachabilityAsync();
                return new ServiceHealthStatus("publication-webhook", reachable ? Healthy : Unhealthy);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Health check failed for service publication-webhook");
                return new ServiceHealthStatus("publication-webhook", Unhealthy);
            }
        }
    }
}
