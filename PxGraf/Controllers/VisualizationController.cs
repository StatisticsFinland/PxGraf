using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PxGraf.Caching;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.PxWebInterface;
using PxGraf.Settings;
using PxGraf.Utility;
using PxGraf.Visualization;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Controller for returning data required for visualizing a saved query
    /// </summary>
    [ApiController]
    [Route("api/sq/visualization")]
    public class VisualizationController : ControllerBase
    {
        private readonly ICachedPxWebConnection _cachedPxWebConnection;
        private readonly IVisualizationResponseCache _visualizationResponseCache;
        private readonly ISqFileInterface _sqFileInterface;
        private readonly ILogger<VisualizationController> _logger;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sqFileInterface">Instance of <see cref="ISqFileInterface"/> that is used to interact with sq and sqa files.</param>
        /// <param name="respCache"><see cref="IVisualizationResponseCache"/> instance for caching <see cref="VisualizationResponse"/> objects.</param>
        /// <param name="cachedPxWebConnection"><see cref="ICachedPxWebConnection"/> instance for handling PxWeb connection and cached data.</param>
        /// <param name="logger"><see cref="ILogger"/> instance used for logging.</param>
        public VisualizationController(ISqFileInterface sqFileInterface, IVisualizationResponseCache respCache, ICachedPxWebConnection cachedPxWebConnection, ILogger<VisualizationController> logger)
        {
            _cachedPxWebConnection = cachedPxWebConnection;
            _visualizationResponseCache = respCache;
            _sqFileInterface = sqFileInterface;
            _logger = logger;
        }

        #region ACTIONS

        /// <summary>
        /// Tries to return a visualization data set from cache.
        /// If the data is not found in the cache or it is outdated starts updating the cache in the background.
        /// </summary>
        /// <param name="sqId">The id of the saved query to visualize</param>
        /// <returns>
        /// <see cref="VisualizationResponse"/> object that contains data required for rendering a visualization.
        /// If no query for the given ID is found, "Not Found" response is returned.
        /// If the data is not found in the cache or it is outdated, "Accepted" response is returned.
        /// If an error occurs, "Bad Request" response is returned.
        /// </returns>
        [HttpGet("{sqId}")]
        public async Task<ActionResult<VisualizationResponse>> GetVisualization([FromRoute] string sqId)
        {
            _logger.LogDebug("Requested visualization GET: api/sq/visualization/{SqId}", sqId);
            VisualizationResponseCache.CacheEntryState itemCacheState = _visualizationResponseCache.TryGet(sqId, out VisualizationResponse cachedResp);
            switch (itemCacheState)
            {
                case VisualizationResponseCache.CacheEntryState.Fresh:
                    _logger.LogDebug("{SqId} result: {CachedResp}", sqId, cachedResp);
                    return cachedResp;

                case VisualizationResponseCache.CacheEntryState.Stale:
                    _ = HandleStaleCacheResponseAsync(sqId, cachedResp);

                    _logger.LogDebug("{SqId} result: {CachedResp}", sqId, cachedResp);
                    return cachedResp;

                case VisualizationResponseCache.CacheEntryState.Pending:
                    _logger.LogDebug("{SqId} result: Pending", sqId);
                    return Accepted();

                case VisualizationResponseCache.CacheEntryState.Error:
                    return BadRequest();

                default: // Data not found in the cache
                    if (_sqFileInterface.SavedQueryExists(sqId, Configuration.Current.SavedQueryDirectory))
                    {
                        SavedQuery sq = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
                        Task<VisualizationResponse> newResponseTask = BuildNewResponseAsync(sqId, sq);
                        _visualizationResponseCache.Set(sqId, newResponseTask);
                        if (sq.Archived)
                        {
                            _logger.LogDebug("{SqId} result: {NewResponseTask}", sqId, newResponseTask);
                            return await newResponseTask; // Return directly if archived
                        }
                        else
                        {
                            _logger.LogDebug("{SqId} result: Pending", sqId);
                            return Accepted(); // Else return accepted and fetch data from pxweb in the background
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Could not find saved query with id {SqId}", sqId);
                        return NotFound();
                    }
            }
        }
        #endregion

        #region UTILITY
        private async Task HandleStaleCacheResponseAsync(string sqId, VisualizationResponse cachedResp)
        {
            SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(sqId, Configuration.Current.SavedQueryDirectory);
            if (!savedQuery.Archived)
            {
                IReadOnlyCubeMeta meta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(savedQuery.Query.TableReference);
                DateTime dbTableDate = meta.GetContentVariable().IncludedValues
                    .Select(vv => DateTime.Parse(vv.ContentComponent.LastUpdated, CultureInfo.InvariantCulture)).Max();
                DateTime cachedTableDate = cachedResp.MetaData
                    .Single(v => v.Type == Enums.VariableType.Content).IncludedValues
                    .Select(vv => DateTime.Parse(vv.ContentComponent.LastUpdated, CultureInfo.InvariantCulture))
                    .Max();

                if (dbTableDate > cachedTableDate)
                {
                    _ = BuildNewResponseAsync(sqId, savedQuery)
                        .ContinueWith(t => _visualizationResponseCache.Set(sqId, t));
                }
            }
            _visualizationResponseCache.Refresh(sqId);
        }

        private async Task<VisualizationResponse> BuildNewResponseAsync(string sqId, SavedQuery sq)
        {
            if (sq.Archived)
            {
                ArchiveCube ac = await _sqFileInterface.ReadArchiveCubeFromFile(sqId, Configuration.Current.ArchiveFileDirectory);
                return PxVisualizerCubeAdapter.BuildVisualizationResponse(ac.ToDataCube(), sq);
            }
            else
            {
                DataCube cube = await _cachedPxWebConnection.BuildDataCubeCachedAsync(sq.Query);
                return PxVisualizerCubeAdapter.BuildVisualizationResponse(cube, sq);
            }
        }
        #endregion
    }
}
