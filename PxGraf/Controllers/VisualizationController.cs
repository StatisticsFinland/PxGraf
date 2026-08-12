#nullable enable annotations
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Datasource.Cache;
using PxGraf.Datasource.ApiDatasource.SerializationModels;
using PxGraf.Datasource;
using PxGraf.Exceptions;
using PxGraf.Models.Metadata;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using PxGraf.Visualization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Controller for returning data required for visualizing a saved query
    /// </summary>
    /// <param name="sqFileInterface">The interface for reading saved queries</param>
    /// <param name="taskCache">The cache for storing tasks</param>
    /// <param name="cachedDatasource">The cached datasource</param>
    /// <param name="logger">The logger interface</param>
    /// <param name="auditLogService">Service for logging audit events.</param>
    /// <param name="virtualValueComputationService">Service for computing virtual dimension values.</param>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    [ApiController]
    [Route("api/sq")]
    public class VisualizationController(ISqFileInterface sqFileInterface, IMultiStateMemoryTaskCache taskCache, ICachedDatasource cachedDatasource, ILogger<VisualizationController> logger, IAuditLogService auditLogService, IVirtualValueComputationService virtualValueComputationService) : ControllerBase
    {
        private readonly ICachedDatasource _cachedDatasource = cachedDatasource;
        private readonly IMultiStateMemoryTaskCache _taskCache = taskCache;
        private readonly ISqFileInterface _sqFileInterface = sqFileInterface;
        private readonly ILogger<VisualizationController> _logger = logger;
        private readonly IAuditLogService _auditLogService = auditLogService;
        private readonly IVirtualValueComputationService _virtualValueComputationService = virtualValueComputationService;

        private static CacheValues CacheValues => Configuration.Current.CacheOptions.Visualization;
        private static readonly TimeSpan AbsoluteExpiration = TimeSpan.FromMinutes(CacheValues.AbsoluteExpirationMinutes);
        private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(CacheValues.SlidingExpirationMinutes);

        private const string VISUALIZATION_ENDPOINT_PATH = "api/sq/visualization";
        private const string VISUALIZATION_METADATA_ENDPOINT_PATH = "api/sq/visualization/metadata";
        private const string JSONSTAT_VISUALIZATION_ENDPOINT_PATH = "api/sq/jsonstat";
        private const string JSONSTAT_METADATA_ENDPOINT_PATH = "api/sq/jsonstat/metadata";
        private const string JSONSTAT_CACHE_KEY_PREFIX = "jsonstat:";
        private const string ARCHIVED_METADATA_CACHE_KEY_PREFIX = "archived-metadata:";

        #region ACTIONS

        /// <summary>
        /// Get visualization for a saved query
        /// </summary>
        /// <param name="sqId">The id of the saved query</param>
        /// <returns><see cref="VisualizationResponse"/> object containing the properties of the visualization</returns>
        [HttpGet("visualization/{sqId}")]
        [ProducesResponseType<VisualizationResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VisualizationResponse>> GetVisualization([FromRoute] string sqId)
        {
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(VisualizationController),
                [LoggerConstants.ACTION] = VISUALIZATION_ENDPOINT_PATH
            };
            using (_logger.BeginScope(logScope))
            {
                if (!InputValidation.ValidateSqIdString(sqId))
                {
                    _auditLogService.LogAuditEvent(
                        action: VISUALIZATION_ENDPOINT_PATH,
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                        );

                    return BadRequest();
                }

                _logger.LogDebug("Requested visualization.");
                MultiStateMemoryTaskCache.CacheEntryState itemCacheState = _taskCache.TryGet(sqId, out Task<VisualizationResponse> cachedRespTask);
                string maxAge = $"max-age={Configuration.Current.CacheOptions.CacheFreshnessCheckIntervalSeconds}";

                if(itemCacheState != MultiStateMemoryTaskCache.CacheEntryState.Null)
                {
                    _auditLogService.LogAuditEvent(
                        action: VISUALIZATION_ENDPOINT_PATH,
                        resource: sqId
                        );
                }

                if (itemCacheState == MultiStateMemoryTaskCache.CacheEntryState.Fresh)
                {
                    _logger.LogDebug("Fresh cache hit for {SqId}", sqId);
                    VisualizationResponse response = await cachedRespTask;
                    Response.Headers.CacheControl = $"{maxAge}";
                    _logger.LogDebug("Returning visualization.");
                    return response;
                }

                if (itemCacheState == MultiStateMemoryTaskCache.CacheEntryState.Stale)
                {
                    _logger.LogDebug("Stale cache hit for {SqId}", sqId);
                    VisualizationResponse response = await cachedRespTask;
                    _ = HandleStaleCacheResponseAsync(sqId, response); // OBS: No await
                    Response.Headers.CacheControl = $"max-age=0"; // Already stale, so no max-age
                    _logger.LogDebug("Returning visualization.");
                    return response;
                }

                if (itemCacheState == MultiStateMemoryTaskCache.CacheEntryState.Error)
                {
                    _logger.LogWarning("Cache error for {SqId}", sqId);
                    return BadRequest();
                }

                if (await _sqFileInterface.SavedQueryExists(sqId, Configuration.Current.SavedQueryDirectory))
                {
                    _auditLogService.LogAuditEvent(
                        action: VISUALIZATION_ENDPOINT_PATH,
                        resource: sqId
                        );

                    _logger.LogDebug("Cache miss for {SqId}", sqId);
                    SavedQuery sq = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
                    Task<VisualizationResponse> newResponseTask = BuildNewResponseAsync(sqId, sq);
                    _taskCache.Set(sqId, newResponseTask, SlidingExpiration, AbsoluteExpiration);
                    _logger.LogDebug("Returning visualization.");
                    try
                    {
                        VisualizationResponse result = await newResponseTask;
                        Response.Headers.CacheControl = $"{maxAge}";
                        return result;
                    }
                    catch (EmptyDimensionException ex)
                    {
                        _logger.LogDebug(ex, "Saved query {SqId} produced an empty dimension; returning 400.", sqId);
                        return BadRequest();
                    }
                }
                else
                {
                    _auditLogService.LogAuditEvent(
                        action: VISUALIZATION_ENDPOINT_PATH,
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                        );

                    _logger.LogWarning("Could not find a saved query file with the provided id.");
                    return NotFound();
                }
            }
        }

        /// <summary>
        /// Gets the metadata required to configure a visualization without fetching its data points.
        /// </summary>
        /// <param name="sqId">The id of the saved query.</param>
        /// <returns>The non-data portion of the visualization response.</returns>
        [HttpGet("visualization/{sqId}/metadata")]
        [ProducesResponseType<VisualizationMetadataResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VisualizationMetadataResponse>> GetVisualizationMetadataAsync([FromRoute] string sqId)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LoggerConstants.CONTROLLER] = nameof(VisualizationController),
                [LoggerConstants.ACTION] = VISUALIZATION_METADATA_ENDPOINT_PATH
            }))
            {
                if (!InputValidation.ValidateSqIdString(sqId))
                {
                    _auditLogService.LogAuditEvent(VISUALIZATION_METADATA_ENDPOINT_PATH, LoggerConstants.INVALID_OR_MISSING_SQID);
                    return BadRequest();
                }

                if (!await _sqFileInterface.SavedQueryExists(sqId, Configuration.Current.SavedQueryDirectory))
                {
                    _auditLogService.LogAuditEvent(VISUALIZATION_METADATA_ENDPOINT_PATH, LoggerConstants.INVALID_OR_MISSING_SQID);
                    return NotFound();
                }

                _auditLogService.LogAuditEvent(VISUALIZATION_METADATA_ENDPOINT_PATH, sqId);
                SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
                try
                {
                    IReadOnlyMatrixMetadata metadata = await BuildVisualizationMetadataAsync(sqId, savedQuery);
                    return PxVisualizerCubeAdapter.BuildVisualizationMetadataResponse(metadata, savedQuery);
                }
                catch (EmptyDimensionException ex)
                {
                    _logger.LogDebug(ex, "Saved query {SqId} produced empty visualization metadata; returning 400.", sqId);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Gets a JSON-stat 2.0 visualization dataset for a saved query in the requested language.
        /// </summary>
        /// <param name="sqId">The id of the saved query.</param>
        /// <param name="lang">Optional language for localized JSON-stat fields. When omitted, defaults to the table's default language. An explicit unsupported language returns 400.</param>
        /// <returns>A single-language JSON-stat 2.0 dataset.</returns>
        [HttpGet("jsonstat/{sqId}")]
        [ProducesResponseType<JsonStat2>(StatusCodes.Status200OK, "application/vnd.jsonstat2+json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JsonStat2>> GetJsonStat2VisualizationAsync([FromRoute] string sqId, [FromQuery] string? lang = null)
        {
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(VisualizationController),
                [LoggerConstants.ACTION] = JSONSTAT_VISUALIZATION_ENDPOINT_PATH
            };
            using (_logger.BeginScope(logScope))
            {
                if (!InputValidation.ValidateSqIdString(sqId))
                {
                    _auditLogService.LogAuditEvent(
                        action: JSONSTAT_VISUALIZATION_ENDPOINT_PATH,
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                        );

                    return BadRequest();
                }

                string cacheKey = GetJsonStatCacheKey(sqId, lang);
                string maxAge = $"max-age={Configuration.Current.CacheOptions.CacheFreshnessCheckIntervalSeconds}";
                MultiStateMemoryTaskCache.CacheEntryState itemCacheState = _taskCache.TryGet(cacheKey, out Task<JsonStat2> cachedDatasetTask);

                if (itemCacheState == MultiStateMemoryTaskCache.CacheEntryState.Fresh)
                {
                    _logger.LogDebug("Fresh JSON-stat cache hit for {SqId}", sqId);
                    JsonStat2 cachedDataset = await cachedDatasetTask;
                    Response.Headers.CacheControl = maxAge;
                    return CreateJsonStatResult(cachedDataset);
                }

                if (itemCacheState == MultiStateMemoryTaskCache.CacheEntryState.Stale)
                {
                    _logger.LogDebug("Stale JSON-stat cache hit for {SqId}", sqId);
                    JsonStat2 cachedDataset = await cachedDatasetTask;
                    _ = RefreshJsonStatCacheAsync(cacheKey, sqId, lang, cachedDataset);
                    Response.Headers.CacheControl = "max-age=0";
                    return CreateJsonStatResult(cachedDataset);
                }

                if (itemCacheState == MultiStateMemoryTaskCache.CacheEntryState.Error)
                {
                    _logger.LogWarning("JSON-stat cache error for {SqId}", sqId);
                    return BadRequest();
                }

                if (!await _sqFileInterface.SavedQueryExists(sqId, Configuration.Current.SavedQueryDirectory))
                {
                    _auditLogService.LogAuditEvent(
                        action: JSONSTAT_VISUALIZATION_ENDPOINT_PATH,
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                        );

                    _logger.LogWarning("Could not find a saved query file with the provided id.");
                    return NotFound();
                }

                _auditLogService.LogAuditEvent(
                    action: JSONSTAT_VISUALIZATION_ENDPOINT_PATH,
                    resource: sqId
                    );

                SavedQuery sq = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
                try
                {
                    Task<JsonStat2> newDatasetTask = BuildJsonStatDatasetAsync(sqId, sq, lang);
                    _taskCache.Set(cacheKey, newDatasetTask, SlidingExpiration, AbsoluteExpiration);
                    JsonStat2 dataset = await newDatasetTask;
                    Response.Headers.CacheControl = maxAge;
                    _logger.LogDebug("Returning JSON-stat visualization result.");
                    return CreateJsonStatResult(dataset);
                }
                catch (EmptyDimensionException ex)
                {
                    _logger.LogDebug(ex, "Saved query {SqId} produced an empty dimension; returning 400.", sqId);
                    return BadRequest();
                }
                catch (ArgumentException ex)
                {
                    _logger.LogDebug(ex, "Invalid JSON-stat request for saved query {SqId}; returning 400.", sqId);
                    return BadRequest();
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogDebug(ex, "Unable to build JSON-stat output for saved query {SqId}; returning 400.", sqId);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Gets a JSON-stat 2.0 metadata-only dataset for a saved query in the requested language.
        /// </summary>
        /// <param name="sqId">The id of the saved query.</param>
        /// <param name="lang">Optional language for localized JSON-stat fields.</param>
        /// <returns>A JSON-stat 2.0 dataset with an empty value array and no status data.</returns>
        [HttpGet("jsonstat/{sqId}/metadata")]
        [ProducesResponseType<JsonStat2>(StatusCodes.Status200OK, "application/vnd.jsonstat2+json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JsonStat2>> GetJsonStat2MetadataAsync([FromRoute] string sqId, [FromQuery] string? lang = null)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LoggerConstants.CONTROLLER] = nameof(VisualizationController),
                [LoggerConstants.ACTION] = JSONSTAT_METADATA_ENDPOINT_PATH
            }))
            {
                if (!InputValidation.ValidateSqIdString(sqId))
                {
                    _auditLogService.LogAuditEvent(JSONSTAT_METADATA_ENDPOINT_PATH, LoggerConstants.INVALID_OR_MISSING_SQID);
                    return BadRequest();
                }

                if (!await _sqFileInterface.SavedQueryExists(sqId, Configuration.Current.SavedQueryDirectory))
                {
                    _auditLogService.LogAuditEvent(JSONSTAT_METADATA_ENDPOINT_PATH, LoggerConstants.INVALID_OR_MISSING_SQID);
                    return NotFound();
                }

                _auditLogService.LogAuditEvent(JSONSTAT_METADATA_ENDPOINT_PATH, sqId);
                SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
                try
                {
                    IReadOnlyMatrixMetadata metadata = await BuildVisualizationMetadataAsync(sqId, savedQuery);
                    JsonStat2 dataset = JsonStat2DatasetBuilder.BuildMetadata(
                        metadata,
                        lang,
                        savedQuery.Settings,
                        savedQuery.Query);
                    return CreateJsonStatResult(dataset);
                }
                catch (EmptyDimensionException ex)
                {
                    _logger.LogDebug(ex, "Saved query {SqId} produced empty JSON-stat metadata; returning 400.", sqId);
                    return BadRequest();
                }
                catch (ArgumentException ex)
                {
                    _logger.LogDebug(ex, "Invalid JSON-stat metadata request for saved query {SqId}; returning 400.", sqId);
                    return BadRequest();
                }
            }
        }

        #endregion

        #region UTILITY

        private async Task RefreshJsonStatCacheAsync(string cacheKey, string sqId, string lang, JsonStat2 cachedDataset)
        {
            _taskCache.Set(cacheKey, Task.FromResult(cachedDataset), SlidingExpiration, AbsoluteExpiration);

            SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
            Task<JsonStat2> refreshedDatasetTask = BuildJsonStatDatasetAsync(sqId, savedQuery, lang);
            _ = refreshedDatasetTask.ContinueWith(task => _taskCache.Set(cacheKey, task, SlidingExpiration, AbsoluteExpiration));
        }

        private async Task<JsonStat2> BuildJsonStatDatasetAsync(string sqId, SavedQuery sq, string lang)
        {
            Matrix<DecimalDataValue> matrix = await BuildVisualizationMatrixAsync(sqId, sq);
            return JsonStat2DatasetBuilder.Build(
                matrix,
                lang,
                sq.Settings,
                sq.Query);
        }

        private static JsonResult CreateJsonStatResult(JsonStat2 dataset)
        {
            return new JsonResult(dataset)
            {
                ContentType = "application/vnd.jsonstat2+json"
            };
        }

        private static string GetJsonStatCacheKey(string sqId, string lang)
        {
            return $"{JSONSTAT_CACHE_KEY_PREFIX}{sqId}:{lang}";
        }
        
        private async Task HandleStaleCacheResponseAsync(string sqId, VisualizationResponse cachedResp)
        {
            // This refreshes the cache, so no additional update triggers happen.
            _taskCache.Set(sqId, Task.FromResult(cachedResp), SlidingExpiration, AbsoluteExpiration);

            SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
            if (!savedQuery.Archived)
            {
                IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(savedQuery.Query.TableReference);
                DateTime dbTableDate = meta.GetContentDimension().Values
                    .Map(vv => vv.LastUpdated).Max();
                DateTime cachedTableDate = cachedResp.MetaData
                    .Single(v => v.DimensionType == DimensionType.Content).Values
                    .Select(vv => DateTime.Parse(vv.ContentComponent.LastUpdated, CultureInfo.InvariantCulture))
                    .Max();

                if (dbTableDate > cachedTableDate)
                {
                    _ = BuildNewResponseAsync(sqId, savedQuery)
                        .ContinueWith(t => _taskCache.Set(sqId, t, SlidingExpiration, AbsoluteExpiration));
                }
            }
        }

        private async Task<VisualizationResponse> BuildNewResponseAsync(string sqId, SavedQuery sq)
        {
            Matrix<DecimalDataValue> matrix = await BuildVisualizationMatrixAsync(sqId, sq);
            return PxVisualizerCubeAdapter.BuildVisualizationResponse(matrix, sq);
        }

        private async Task<IReadOnlyMatrixMetadata> BuildVisualizationMetadataAsync(string sqId, SavedQuery savedQuery)
        {
            if (savedQuery.Archived)
            {
                return await GetArchivedMetadataAsync(sqId);
            }

            IReadOnlyMatrixMetadata completeMetadata = await _cachedDatasource.GetMatrixMetadataCachedAsync(
                savedQuery.Query.TableReference);
            (IReadOnlyMatrixMetadata fetchMetadata, MatrixMap outputMap) = completeMetadata.BuildVirtualValueMaps(savedQuery.Query);

            if (fetchMetadata.GetSize() == 0 || outputMap.GetSize() == 0)
            {
                throw new EmptyDimensionException($"Saved query '{sqId}' produced metadata with one or more empty dimensions.");
            }

            IReadOnlyMatrixMetadata metadata = savedQuery.Query.DimensionQueries.Values.Any(
                dimensionQuery => dimensionQuery.VirtualValueDefinitions?.Count > 0)
                ? VirtualValueMetadataBuilder.Build(fetchMetadata, savedQuery.Query)
                : fetchMetadata;

            return metadata.GetTransform(outputMap);
        }

        private async Task<IReadOnlyMatrixMetadata> GetArchivedMetadataAsync(string sqId)
        {
            string cacheKey = $"{ARCHIVED_METADATA_CACHE_KEY_PREFIX}{sqId}";
            MultiStateMemoryTaskCache.CacheEntryState cacheState = _taskCache.TryGet(
                cacheKey,
                out Task<IReadOnlyMatrixMetadata> cachedMetadataTask);

            if (cacheState == MultiStateMemoryTaskCache.CacheEntryState.Fresh)
            {
                return await cachedMetadataTask;
            }

            if (cacheState == MultiStateMemoryTaskCache.CacheEntryState.Error)
            {
                _taskCache.TryRemove(cacheKey);
            }

            Task<IReadOnlyMatrixMetadata> metadataTask = ReadArchivedMetadataAsync(sqId);
            _taskCache.Set(cacheKey, metadataTask, SlidingExpiration, AbsoluteExpiration);
            return await metadataTask;
        }

        private async Task<IReadOnlyMatrixMetadata> ReadArchivedMetadataAsync(string sqId)
        {
            ArchiveCube archiveCube = await _sqFileInterface.ReadArchiveCubeFromFile(
                sqId,
                Configuration.Current.ArchiveFileDirectory);
            return archiveCube.Meta;
        }

        private async Task<Matrix<DecimalDataValue>> BuildVisualizationMatrixAsync(string sqId, SavedQuery sq)
        {
            if (sq.Archived)
            {
                ArchiveCube ac = await _sqFileInterface.ReadArchiveCubeFromFile(sqId, Configuration.Current.ArchiveFileDirectory);
                return ac.ToMatrix();
            }
            else
            {
                IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(sq.Query.TableReference);

                (IReadOnlyMatrixMetadata fetchMeta, MatrixMap outputMap) = meta.BuildVirtualValueMaps(sq.Query);

                // Guard: bail out gracefully if any dimension has 0 values (matches the check in GetVisualizationAsync).
                if (fetchMeta.GetSize() == 0 || outputMap.GetSize() == 0)
                {
                    _logger.LogWarning("One or more dimensions have no included values for saved query {SqId}", sqId);
                    throw new EmptyDimensionException($"Saved query '{sqId}' produced a matrix with one or more empty dimensions.");
                }

                Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixAsync(sq.Query.TableReference, fetchMeta);

                if (sq.Query.DimensionQueries.Values.Any(dq => dq.VirtualValueDefinitions?.Count > 0))
                    matrix = _virtualValueComputationService.ApplyVirtualValues(matrix, sq.Query);
                matrix = matrix.GetTransform(outputMap);

                return matrix;
            }
        }

        #endregion
    }
}
