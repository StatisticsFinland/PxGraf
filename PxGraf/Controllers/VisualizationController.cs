using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Datasource.Cache;
using PxGraf.Datasource;
using PxGraf.Exceptions;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
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
    [Route("api/sq/visualization")]
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

        private const string CONTROLLER_PATH = "api/sq/visualization";

        #region ACTIONS

        /// <summary>
        /// Get visualization for a saved query
        /// </summary>
        /// <param name="sqId">The id of the saved query</param>
        /// <returns><see cref="VisualizationResponse"/> object containing the properties of the visualization</returns>
        [HttpGet("{sqId}")]
        [ProducesResponseType<VisualizationResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VisualizationResponse>> GetVisualization([FromRoute] string sqId)
        {
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(VisualizationController),
                [LoggerConstants.ACTION] = CONTROLLER_PATH
            };
            using (_logger.BeginScope(logScope))
            {
                _logger.LogDebug("Requested visualization.");
                MultiStateMemoryTaskCache.CacheEntryState itemCacheState = _taskCache.TryGet(sqId, out Task<VisualizationResponse> cachedRespTask);
                string maxAge = $"max-age={Configuration.Current.CacheOptions.CacheFreshnessCheckIntervalSeconds}";

                if(itemCacheState != MultiStateMemoryTaskCache.CacheEntryState.Null)
                {
                    _auditLogService.LogAuditEvent(
                        action: CONTROLLER_PATH,
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
                        action: CONTROLLER_PATH,
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
                        action: CONTROLLER_PATH,
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                        );

                    _logger.LogWarning("Could not find a saved query file with the provided id.");
                    return NotFound();
                }
            }
        }

        /// <summary>
        /// Gets a JSON-stat 2.0 visualization dataset for a saved query in the requested language.
        /// </summary>
        /// <param name="sqId">The id of the saved query.</param>
        /// <param name="lang">Optional language for localized JSON-stat fields. When omitted, defaults to the table's default language. An explicit unsupported language returns 400.</param>
        /// <returns>A single-language JSON-stat 2.0 dataset.</returns>
        [HttpGet("jsonstat2/{sqId}")]
        [ProducesResponseType<JsonStat2Dataset>(StatusCodes.Status200OK, "application/vnd.jsonstat2+json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JsonStat2Dataset>> GetJsonStat2VisualizationAsync([FromRoute] string sqId, [FromQuery] string lang)
        {
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(VisualizationController),
                [LoggerConstants.ACTION] = $"{CONTROLLER_PATH}/jsonstat2"
            };
            using (_logger.BeginScope(logScope))
            {
                if (!await _sqFileInterface.SavedQueryExists(sqId, Configuration.Current.SavedQueryDirectory))
                {
                    _auditLogService.LogAuditEvent(
                        action: $"{CONTROLLER_PATH}/jsonstat2",
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                        );

                    _logger.LogWarning("Could not find a saved query file with the provided id.");
                    return NotFound();
                }

                _auditLogService.LogAuditEvent(
                    action: $"{CONTROLLER_PATH}/jsonstat2",
                    resource: sqId
                    );

                SavedQuery sq = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
                try
                {
                    Matrix<DecimalDataValue> matrix = await BuildVisualizationMatrixAsync(sqId, sq);
                    JsonStat2Dataset dataset = JsonStat2DatasetBuilder.Build(
                        matrix,
                        lang,
                        PxVisualizerCubeAdapter.BuildVisualizationSettings(matrix, sq.Settings));
                    Response.Headers.CacheControl = $"max-age={Configuration.Current.CacheOptions.CacheFreshnessCheckIntervalSeconds}";
                    _logger.LogDebug("Returning JSON-stat visualization result.");
                    return new JsonResult(dataset)
                    {
                        ContentType = "application/vnd.jsonstat2+json"
                    };
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

        #endregion

        #region UTILITY
        
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
