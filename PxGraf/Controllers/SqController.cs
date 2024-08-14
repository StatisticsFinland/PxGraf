using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.ChartTypeSelection;
using PxGraf.Datasource.PxWebInterface;
using PxGraf.Datasource;
using PxGraf.Enums;
using PxGraf.Exceptions;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Controller for serving and saving queries (sq = saved query).
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    /// <param name="datasource">Instance of a <see cref="ICachedPxWebConnection"/> object. Used to interact with PxWeb API and cache data.</param>
    /// <param name="sqFileInterface">Instance of a <see cref="ISqFileInterface"/> object. Used for interacting with saved queries.</param>
    /// <param name="logger"><see cref="ILogger"/> instance used for logging.</param>
    [FeatureGate("CreationAPI")]
    [ApiController]
    [Route("api/sq")]
    public class SqController(ICachedDatasource datasource, ISqFileInterface sqFileInterface, ILogger<SqController> logger) : ControllerBase
    {
        private readonly ICachedDatasource _cachedDatasource = datasource;
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
                IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(savedQuery.Query.TableReference);
                IReadOnlyMatrixMetadata filteredMeta = meta.FilterDimensionValues(savedQuery.Query);

                if (meta.Dimensions.Any(v => v.Values.Count == 0))
                {
                    _logger.LogWarning("Saved query {SavedQueryId} contains variables with no values", savedQueryId);
                    BadRequest();
                }

                Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixCachedAsync(savedQuery.Query.TableReference, filteredMeta);
                IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(savedQuery.Query, matrix);

                if (!validTypes.Contains(savedQuery.Settings.VisualizationType))
                {
                    _logger.LogWarning("{SavedQueryId} is not valid for saved visualization type {VisualizationType}", savedQueryId, savedQuery.Settings.VisualizationType);
                    return BadRequest();
                }

                SaveQueryParams saveQueryParams = new ()
                {
                    Query = savedQuery.Query,
                    Settings = VisualizationCreationSettings.FromVisualizationSettings(savedQuery, filteredMeta)
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

            IReadOnlyMatrixMetadata tableMeta = await _cachedDatasource.GetMatrixMetadataCachedAsync(parameters.Query.TableReference);
            IReadOnlyMatrixMetadata filteredMeta = tableMeta.FilterDimensionValues(parameters.Query);

            VisualizationSettings visualizationSettings = parameters.Settings.ToVisualizationSettings(filteredMeta, parameters.Query);

            // All variables must have atleast one value selected
            if (!filteredMeta.Dimensions.Any(v => v.Values.Count != 0) || !ValidateVisualizationSettings(filteredMeta, visualizationSettings))
            {
                _logger.LogWarning("Query {NewGuid} is missing a value for a variable. {Parameters}", newGuid, parameters);
                return BadRequest();
            }

            Matrix<DecimalDataValue> dataCube = await _cachedDatasource.GetMatrixCachedAsync(parameters.Query.TableReference, filteredMeta);

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
            IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(parameters.Query.TableReference);
            IReadOnlyMatrixMetadata filteredMeta = meta.FilterDimensionValues(parameters.Query);

            VisualizationSettings visualizationSettings = parameters.Settings.ToVisualizationSettings(filteredMeta, parameters.Query);

            // All variables must have atleast one value selected
            if (filteredMeta.Dimensions.Any(v => v.Values.Count == 0) || !ValidateVisualizationSettings(filteredMeta, visualizationSettings))
            {
                _logger.LogWarning("Archived query {NewGuid} is missing a value for a variable. {Parameters}", newGuid, parameters);
                return BadRequest();
            }

            Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixCachedAsync(parameters.Query.TableReference, filteredMeta);
            IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(parameters.Query, matrix);
            if (validTypes.Contains(visualizationSettings.VisualizationType))
            {
                SavedQuery savedQuery = new(parameters.Query, archived: true, visualizationSettings, DateTime.Now);
                await _sqFileInterface.SerializeToFile(queryFileName, Configuration.Current.SavedQueryDirectory, savedQuery);

                string archiveName = $"{newGuid}.sqa";
                ArchiveCube archiveMatrix = ArchiveCube.FromMatrixAndQuery(matrix, parameters.Query);
                await _sqFileInterface.SerializeToFile(archiveName, Configuration.Current.ArchiveFileDirectory, archiveMatrix);
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
                    IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(baseQuery.Query.TableReference);
                    IReadOnlyMatrixMetadata filteredMeta = meta.FilterDimensionValues(baseQuery.Query);
                    Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixCachedAsync(baseQuery.Query.TableReference, filteredMeta);

                    IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(baseQuery.Query, matrix);
                    if (validTypes.Contains(baseQuery.Settings.VisualizationType))
                    {
                        SavedQuery savedQuery = new (baseQuery.Query, archived: true, baseQuery.Settings, DateTime.Now);
                        await _sqFileInterface.SerializeToFile(queryFileName, Configuration.Current.SavedQueryDirectory, savedQuery);

                        string archiveName = $"{newGuid}.sqa";
                        ArchiveCube archiveMatrix = ArchiveCube.FromMatrixAndQuery(matrix, baseQuery.Query);
                        await _sqFileInterface.SerializeToFile(archiveName, Configuration.Current.ArchiveFileDirectory, archiveMatrix);
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
        private static bool ValidateVisualizationSettings(IReadOnlyMatrixMetadata meta, VisualizationSettings settings)
        {
            if (settings is LineChartVisualizationSettings lcvs &&
                meta.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Content)?.Code == lcvs.MultiselectableVariableCode)
            {
                return false;
            }

            return true;
        }
    }
}
