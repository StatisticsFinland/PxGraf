using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using PxGraf.ChartTypeSelection;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Exceptions;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.PxWebInterface;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Controller for serving and saving queries (sq = saved query).
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    /// <param name="cachedPxWebConnection">Instance of a <see cref="ICachedPxWebConnection"/> object. Used to interact with PxWeb API and cache data.</param>
    /// <param name="sqFileInterface">Instance of a <see cref="ISqFileInterface"/> object. Used for interacting with saved queries.</param>
    /// <param name="logger"><see cref="ILogger"/> instance used for logging.</param>
    [FeatureGate("CreationAPI")]
    [ApiController]
    [Route("api/sq")]
    public class SqController(ICachedPxWebConnection cachedPxWebConnection, ISqFileInterface sqFileInterface, ILogger<SqController> logger) : ControllerBase
    {
        private readonly ICachedPxWebConnection _cachedPxWebConnection = cachedPxWebConnection;
        private readonly ISqFileInterface _sqFileInterface = sqFileInterface;
        private readonly ILogger<SqController> _logger = logger;

        /// <summary>
        /// Returns a saved query and its settings based on the given id.
        /// </summary>
        /// <param name="savedQueryId">ID of the saved query.</param>
        /// <returns>
        /// <see cref="SaveQueryParams"/> object that contains the saved query and its visualization creation settings.
        /// If the saved visualization type is not valid for the save query, "Bad Request" response is returned
        /// If saved query with the provided ID is not found, "Not Found" response is returned.
        /// </returns>
        [HttpGet("{savedQueryId}")]
        public async Task<ActionResult<SaveQueryParams>> GetSavedQueryAsync([FromRoute] string savedQueryId)
        {
            _logger.LogDebug("Requested saved query GET: api/sq/{SavedQueryId}", savedQueryId);
            if (_sqFileInterface.SavedQueryExists(savedQueryId, Configuration.Current.SavedQueryDirectory))
            {
                SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(savedQueryId, Configuration.Current.SavedQueryDirectory);
                IReadOnlyCubeMeta readOnlyTableMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(savedQuery.Query.TableReference);
                CubeMeta tableMeta = readOnlyTableMeta.Clone();
                tableMeta.ApplyEditionFromQuery(savedQuery.Query);

                if (tableMeta.Variables.Exists(v => v.IncludedValues.Count == 0))
                {
                    _logger.LogWarning("Saved query {SavedQueryId} contains variables with no values", savedQueryId);
                    BadRequest();
                }

                DataCube dataCube = await _cachedPxWebConnection.GetDataCubeCachedAsync(savedQuery.Query.TableReference, tableMeta);
                IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(savedQuery.Query, dataCube);

                if (!validTypes.Contains(savedQuery.Settings.VisualizationType))
                {
                    _logger.LogWarning("{SavedQueryId} is not valid for saved visualization type {VisualizationType}", savedQueryId, savedQuery.Settings.VisualizationType);
                    return BadRequest();
                }

                SaveQueryParams saveQueryParams = new ()
                {
                    Query = savedQuery.Query,
                    Settings = VisualizationCreationSettings.FromVisualizationSettings(savedQuery, tableMeta)
                };

                _logger.LogInformation("{SavedQueryId} result: Query: {Query}, Settings: {Settings}", savedQueryId, saveQueryParams.Query, saveQueryParams.Settings);

                return saveQueryParams;
            }
            else
            {
                _logger.LogWarning("Saved query {SavedQueryId} not found", savedQueryId);
                return NotFound();
            }
        }

        /// <summary>
        /// Saves a query and its settings to a file.
        /// </summary>
        /// <param name="parameters">Parameters for saving the query.</param>
        /// <returns><see cref="SaveQueryResponse"/> object that contains the id of the saved query.</returns>
        [HttpPost("save")]
        public async Task<ActionResult<SaveQueryResponse>> SaveQueryAsync([FromBody] SaveQueryParams parameters)
        {
            _logger.LogDebug("Save request received {Parameters} POST: api/sq/save", parameters);
            string newGuid = Guid.NewGuid().ToString();
            string fileName = $"{newGuid}.sq";

            var tableMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(parameters.Query.TableReference);
            var tableMetaCopy = tableMeta.Clone();
            tableMetaCopy.ApplyEditionFromQuery(parameters.Query);
            tableMetaCopy.Header.Truncate(Configuration.Current.QueryOptions.MaxHeaderLength);

            VisualizationSettings visualizationSettings = parameters.Settings.ToVisualizationSettings(tableMetaCopy, parameters.Query);

            // All variables must have atleast one value selected
            if (!tableMetaCopy.Variables.TrueForAll(v => v.IncludedValues.Count != 0) || !ValidateVisualizationSettings(tableMetaCopy, visualizationSettings))
            {
                _logger.LogWarning("Query {NewGuid} is missing a value for a variable. {Parameters}", newGuid, parameters);
                return BadRequest();
            }

            var dataCube = await _cachedPxWebConnection.GetDataCubeCachedAsync(parameters.Query.TableReference, tableMetaCopy);

            var validTypes = ChartTypeSelector.Selector.GetValidChartTypes(parameters.Query, dataCube);
            if (validTypes.Contains(visualizationSettings.VisualizationType))
            {
                var savedQuery = new SavedQuery(parameters.Query, archived: false, visualizationSettings, DateTime.Now);
                await _sqFileInterface.SerializeToFile(fileName, Configuration.Current.SavedQueryDirectory, savedQuery);
                
                SaveQueryResponse saveQueryResponse = new () { Id = newGuid };
                _logger.LogDebug("Async query saved successfully {SaveQueryResponse}", saveQueryResponse);

                return saveQueryResponse;
            }

            // If visualization type is not given or it is not valid return 400.
            _logger.LogWarning("Query {NewGuid} has an invalid visualization type", newGuid);
            return BadRequest();
        }

        /// <summary>
        /// Creates an archived query and saves it to a file.
        /// </summary>
        /// <param name="parameters">Parameters for saving the query.</param>
        /// <returns><see cref="SaveQueryResponse"/> object that contains the name of the archived query.</returns>
        [HttpPost("archive")]
        public async Task<ActionResult<SaveQueryResponse>> ArchiveQueryAsync([FromBody] SaveQueryParams parameters)
        {
            _logger.LogDebug("Archiving query {Parameters} POST: api/sq/archive", parameters);
            string newGuid = Guid.NewGuid().ToString();
            string queryFileName = $"{newGuid}.sq";
            var archiveCube = await _cachedPxWebConnection.BuildArchiveCubeCachedAsync(parameters.Query);

            VisualizationSettings visualizationSettings = parameters.Settings.ToVisualizationSettings(archiveCube.Meta, parameters.Query);

            // All variables must have atleast one value selected
            if (!archiveCube.Meta.Variables.TrueForAll(v => v.IncludedValues.Count != 0) || !ValidateVisualizationSettings(archiveCube.Meta, visualizationSettings))
            {
                _logger.LogWarning("Archived query {NewGuid} is missing a value for a variable. {Parameters}", newGuid, parameters);
                return BadRequest();
            }

            var validTypes = ChartTypeSelector.Selector.GetValidChartTypes(parameters.Query, archiveCube);
            if (validTypes.Contains(visualizationSettings.VisualizationType))
            {
                var savedQuery = new SavedQuery(parameters.Query, archived: true, visualizationSettings, DateTime.Now);
                await _sqFileInterface.SerializeToFile(queryFileName, Configuration.Current.SavedQueryDirectory, savedQuery);

                string archiveName = $"{newGuid}.sqa";
                await _sqFileInterface.SerializeToFile(archiveName, Configuration.Current.ArchiveFileDirectory, archiveCube);
                _logger.LogInformation("Archiving query {ArchiveName}", archiveName);
                return new SaveQueryResponse() { Id = newGuid };
            }

            // If visualization type is not given or it is not valid return 400.
            _logger.LogWarning("Archived query {NewGuid} has an invalid visualization type", newGuid);
            return BadRequest();
        }

        /// <summary>
        /// Reads a saved query, fetches the current variable values and data matching the query,
        /// creates a new saved query based on that data and returns the id of that created query.
        /// </summary>
        /// <param name="request"><see cref="ReArchiveRequest"/> object that contains the ID of the query to be rearchived.</param>
        /// <returns><see cref="ReArchiveResponse"/> object that contains the ID of the rearchived query.</returns>
        [HttpPost("re-archive")]
        public async Task<ActionResult<ReArchiveResponse>> ReArchiveExistingQueryAsync([FromBody] ReArchiveRequest request)
        {
            _logger.LogDebug("Re-archiving query {SqId} POST: api/sq/re-archive", request.SqId);
            if (_sqFileInterface.SavedQueryExists(request.SqId, Configuration.Current.SavedQueryDirectory))
            {
                SavedQuery baseQuery = await _sqFileInterface.ReadSavedQueryFromFile(request.SqId, Configuration.Current.SavedQueryDirectory);
                try
                {
                    string newGuid = Guid.NewGuid().ToString();
                    string queryFileName = $"{newGuid}.sq";
                    var archiveCube = await _cachedPxWebConnection.BuildArchiveCubeCachedAsync(baseQuery.Query);

                    var validTypes = ChartTypeSelector.Selector.GetValidChartTypes(baseQuery.Query, archiveCube);
                    if (validTypes.Contains(baseQuery.Settings.VisualizationType))
                    {
                        var savedQuery = new SavedQuery(baseQuery.Query, archived: true, baseQuery.Settings, DateTime.Now);
                        await _sqFileInterface.SerializeToFile(queryFileName, Configuration.Current.SavedQueryDirectory, savedQuery);

                        string archiveName = $"{newGuid}.sqa";
                        await _sqFileInterface.SerializeToFile(archiveName, Configuration.Current.ArchiveFileDirectory, archiveCube);
                        _logger.LogInformation("Rearchived query {ArchiveName}", archiveName);
                        return new ReArchiveResponse() { NewSqId = newGuid };
                    }

                    // If visualization type is not valid for the new data return 400.
                    _logger.LogWarning("Rearchiving a query has failed due to an invalid visualization type {Request}", request);
                    return BadRequest();
                }
                catch (BadPxWebResponseException error)
                {
                    _logger.LogWarning(error, "Rearchived query {SqId} failed to fetch data", request.SqId);
                    return BadRequest();
                }
            }
            else
            {
                _logger.LogWarning("Rearchived query {SqId} not found", request.SqId);
                return NotFound();
            }
        }

        /// <summary>
        /// Validates visualization settings for th given cube meta
        /// Add all future validation rules here.
        /// </summary>
        private static bool ValidateVisualizationSettings(IReadOnlyCubeMeta meta, VisualizationSettings settings)
        {
            if (settings is LineChartVisualizationSettings lcvs &&
                meta.Variables.FirstOrDefault(v => v.Type == VariableType.Content)?.Code == lcvs.MultiselectableVariableCode)
            {
                return false;
            }

            return true;
        }
    }
}
