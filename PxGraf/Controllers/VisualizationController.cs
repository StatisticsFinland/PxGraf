using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Datasource.Cache;
using PxGraf.Datasource;
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
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    [ApiController]
    [Route("api/sq/visualization")]
    public class VisualizationController(ISqFileInterface sqFileInterface, IMultiStateMemoryTaskCache taskCache, ICachedDatasource cachedDatasource, ILogger<VisualizationController> logger, IAuditLogService auditLogService) : ControllerBase
    {
        private readonly ICachedDatasource _cachedDatasource = cachedDatasource;
        private readonly IMultiStateMemoryTaskCache _taskCache = taskCache;
        private readonly ISqFileInterface _sqFileInterface = sqFileInterface;
        private readonly ILogger<VisualizationController> _logger = logger;
        private readonly IAuditLogService _auditLogService = auditLogService;

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

                if (_sqFileInterface.SavedQueryExists(sqId, Configuration.Current.SavedQueryDirectory))
                {
                    _auditLogService.LogAuditEvent(
                        action: CONTROLLER_PATH,
                        resource: sqId
                        );

                    _logger.LogDebug("Cache miss for {SqId}", sqId);
                    SavedQuery sq = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
                    Task<VisualizationResponse> newResponseTask = BuildNewResponseAsync(sqId, sq);
                    _taskCache.Set(sqId, newResponseTask, SlidingExpiration, AbsoluteExpiration);
                    Response.Headers.CacheControl = $"{maxAge}";
                    _logger.LogDebug("Returning visualization.");
                    return await newResponseTask; // Return directly if archived
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
            if (sq.Archived)
            {
                ArchiveCube ac = await _sqFileInterface.ReadArchiveCubeFromFile(sqId, Configuration.Current.ArchiveFileDirectory);
                return PxVisualizerCubeAdapter.BuildVisualizationResponse(ac.ToMatrix(), sq);
            }
            else
            {
                IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(sq.Query.TableReference);
                IReadOnlyMatrixMetadata filteredMetes = meta.FilterDimensionValues(sq.Query);
                Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixAsync(sq.Query.TableReference, filteredMetes);
                return PxVisualizerCubeAdapter.BuildVisualizationResponse(matrix, sq);
            }
        }

        #endregion
    }
}
