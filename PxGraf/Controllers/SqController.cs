using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.ChartTypeSelection;
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
using PxGraf.Services;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Controller for serving and saving queries (sq = saved query).
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    /// <param name="datasource">Instance of a <see cref="ICachedDatasource"/> object. Used to interact with data source and cached data.</param>
    /// <param name="sqFileInterface">Instance of a <see cref="ISqFileInterface"/> object. Used for interacting with saved queries.</param>
    /// <param name="logger"><see cref="ILogger"/> instance used for logging.</param>
    /// <param name="auditLogService">Service for logging audit events.</param>
    [FeatureGate("CreationAPI")]
    [ApiController]
    [Route("api/sq")]
    public class SqController(ICachedDatasource datasource, ISqFileInterface sqFileInterface, ILogger<SqController> logger, IAuditLogService auditLogService) : ControllerBase
    {
        private readonly ICachedDatasource _cachedDatasource = datasource;
        private readonly ISqFileInterface _sqFileInterface = sqFileInterface;
        private readonly ILogger<SqController> _logger = logger;
        private readonly IAuditLogService _auditLogService = auditLogService;
        private const string CONTROLLER_PATH = "api/sq";

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
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(SqController),
                [LoggerConstants.ACTION] = CONTROLLER_PATH
            };
            using (_logger.BeginScope(logScope))
            {
                _logger.LogDebug("Saved query requested.");
                if (_sqFileInterface.SavedQueryExists(savedQueryId, Configuration.Current.SavedQueryDirectory))
                {
                    using (_logger.BeginScope(new Dictionary<string, object> { [LoggerConstants.SQ_ID] = savedQueryId }))
                    {
                        _auditLogService.LogAuditEvent(
                            action: CONTROLLER_PATH,
                            resource: savedQueryId
                        );

                        SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(savedQueryId, Configuration.Current.SavedQueryDirectory);
                        IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(savedQuery.Query.TableReference);
                        IReadOnlyMatrixMetadata filteredMeta = meta.FilterDimensionValues(savedQuery.Query);

                        if (meta.Dimensions.Any(v => v.Values.Count == 0))
                        {
                            _logger.LogWarning("Saved query return metadata which contains dimensions with no values.");
                            BadRequest();
                        }

                        Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixCachedAsync(savedQuery.Query.TableReference, filteredMeta);
                        if (matrix == null)
                        {
                            _logger.LogWarning("Fetching data based on the saved query failed.");
                            return BadRequest();
                        }

                        IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(savedQuery.Query, matrix);

                        if (!validTypes.Contains(savedQuery.Settings.VisualizationType))
                        {
                            _logger.LogWarning("The saved visualization type is not valid for the saved query.");
                            return BadRequest();
                        }

                        SaveQueryParams saveQueryParams = new()
                        {
                            Query = savedQuery.Query,
                            Settings = VisualizationCreationSettings.FromVisualizationSettings(savedQuery, filteredMeta),
                            Draft = savedQuery.Draft,
                            Id = savedQueryId
                        };

                        _logger.LogDebug("Returning saved query result.");
                        return saveQueryParams;
                    }
                }
                else
                {
                    _auditLogService.LogAuditEvent(
                        action: CONTROLLER_PATH,
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                    );
                    _logger.LogWarning("Saved query not found or the query id is invalid.");
                    return NotFound();
                }
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
            string actionPath = $"{CONTROLLER_PATH}/save";
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(SqController),
                [LoggerConstants.ACTION] = actionPath
            };
            using (_logger.BeginScope(logScope))
            {
                _logger.LogDebug("Save request received.");
                string guid = await GetIsDraftAsync(parameters.Id) ? parameters.Id : Guid.NewGuid().ToString();
                string fileName = $"{guid}.sq";

                using (_logger.BeginScope(new Dictionary<string, object> { [LoggerConstants.SQ_ID] = guid }))
                {
                    _auditLogService.LogAuditEvent(
                        action: actionPath,
                        resource: guid
                    );

                    IReadOnlyMatrixMetadata tableMeta = await _cachedDatasource.GetMatrixMetadataCachedAsync(parameters.Query.TableReference);
                    IReadOnlyMatrixMetadata filteredMeta = tableMeta.FilterDimensionValues(parameters.Query);

                    VisualizationSettings visualizationSettings = parameters.Settings.ToVisualizationSettings(filteredMeta, parameters.Query);

                    // All dimensions must have atleast one value selected
                    if (!filteredMeta.Dimensions.Any(v => v.Values.Count != 0) || !ValidateVisualizationSettings(filteredMeta, visualizationSettings))
                    {
                        _logger.LogWarning("Query is missing values for a dimension.");
                        return BadRequest();
                    }

                    Matrix<DecimalDataValue> dataCube = await _cachedDatasource.GetMatrixCachedAsync(parameters.Query.TableReference, filteredMeta);

                    IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(parameters.Query, dataCube);
                    if (validTypes.Contains(visualizationSettings.VisualizationType))
                    {
                        SavedQuery savedQuery = new(parameters.Query, archived: false, visualizationSettings, DateTime.Now, parameters.Draft);
                        await _sqFileInterface.SerializeToFile(fileName, Configuration.Current.SavedQueryDirectory, savedQuery);

                        SaveQueryResponse saveQueryResponse = new() { Id = guid };
                        _logger.LogInformation("Query saved successfully.");
                        _logger.LogDebug("Returning save query result.");
                        return saveQueryResponse;
                    }

                    // If visualization type is not given or it is not valid return 400.
                    _logger.LogWarning("Query has an invalid visualization type.");
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Creates an archived query and saves it to a file.
        /// </summary>
        /// <param name="parameters">Parameters for saving the query.</param>
        /// <returns><see cref="SaveQueryResponse"/> object that contains the name of the archived query.</returns>
        [HttpPost("archive")]
        public async Task<ActionResult<SaveQueryResponse>> ArchiveQueryAsync([FromBody] SaveQueryParams parameters)
        {
            string actionPath = $"{CONTROLLER_PATH}/archive";
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(SqController),
                [LoggerConstants.ACTION] = actionPath
            };
            using (_logger.BeginScope(logScope))
            {
                _logger.LogDebug("Archiving request received.");
                string guid = await GetIsDraftAsync(parameters.Id) ? parameters.Id : Guid.NewGuid().ToString();
                string queryFileName = $"{guid}.sq";
                using (_logger.BeginScope(new Dictionary<string, object> { [LoggerConstants.SQ_ID] = guid }))
                {
                    _auditLogService.LogAuditEvent(
                        action: actionPath,
                        resource: guid
                    );

                    IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(parameters.Query.TableReference);
                    IReadOnlyMatrixMetadata filteredMeta = meta.FilterDimensionValues(parameters.Query);

                    VisualizationSettings visualizationSettings = parameters.Settings.ToVisualizationSettings(filteredMeta, parameters.Query);

                    // All dimensions must have atleast one value selected
                    if (filteredMeta.Dimensions.Any(v => v.Values.Count == 0) || !ValidateVisualizationSettings(filteredMeta, visualizationSettings))
                    {
                        _logger.LogWarning("Query is missing values for a dimension.");
                        _auditLogService.LogAuditEvent(
                            action: actionPath,
                            resource: LoggerConstants.INVALID_VISUALIZATION
                        );
                        return BadRequest();
                    }

                    Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixCachedAsync(parameters.Query.TableReference, filteredMeta);
                    IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(parameters.Query, matrix);
                    if (validTypes.Contains(visualizationSettings.VisualizationType))
                    {
                        SavedQuery savedQuery = new(parameters.Query, archived: true, visualizationSettings, DateTime.Now, parameters.Draft);
                        await _sqFileInterface.SerializeToFile(queryFileName, Configuration.Current.SavedQueryDirectory, savedQuery);

                        string archiveName = $"{guid}.sqa";
                        await _sqFileInterface.SerializeToFile(archiveName, Configuration.Current.ArchiveFileDirectory, new ArchiveCube(matrix));
                        _logger.LogInformation("Query archived successfully.");
                        _logger.LogDebug("Returning archive query result.");
                        return new SaveQueryResponse() { Id = guid };
                    }

                    // If visualization type is not given or it is not valid return 400.
                    _logger.LogWarning("Query has an invalid visualization type.");
                    _auditLogService.LogAuditEvent(
                        action: actionPath,
                        resource: LoggerConstants.INVALID_VISUALIZATION
                    );
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Reads a saved query, fetches the current dimension values and data matching the query,
        /// creates a new saved query based on that data and returns the id of that created query.
        /// </summary>
        /// <param name="request"><see cref="ReArchiveRequest"/> object that contains the ID of the query to be rearchived.</param>
        /// <returns><see cref="ReArchiveResponse"/> object that contains the ID of the rearchived query.</returns>
        [HttpPost("re-archive")]
        public async Task<ActionResult<ReArchiveResponse>> ReArchiveExistingQueryAsync([FromBody] ReArchiveRequest request)
        {
            string actionPath = $"{CONTROLLER_PATH}/re-archive";
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(SqController),
                [LoggerConstants.ACTION] = actionPath
            };
            using (_logger.BeginScope(logScope))
            {
                _logger.LogDebug("Re-archiving query.");
                if (_sqFileInterface.SavedQueryExists(request.SqId, Configuration.Current.SavedQueryDirectory))
                {
                    using (_logger.BeginScope(new Dictionary<string, object> { [LoggerConstants.SQ_ID] = request.SqId }))
                    {
                        _auditLogService.LogAuditEvent(
                            action: actionPath,
                            resource: request.SqId
                        );

                        SavedQuery baseQuery = await _sqFileInterface.ReadSavedQueryFromFile(request.SqId, Configuration.Current.SavedQueryDirectory);
                        try
                        {
                            string guid = await GetIsDraftAsync(request.SqId) ? request.SqId : Guid.NewGuid().ToString();
                            string queryFileName = $"{guid}.sq";
                            IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(baseQuery.Query.TableReference);
                            IReadOnlyMatrixMetadata filteredMeta = meta.FilterDimensionValues(baseQuery.Query);
                            Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixCachedAsync(baseQuery.Query.TableReference, filteredMeta);

                            IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(baseQuery.Query, matrix);
                            if (validTypes.Contains(baseQuery.Settings.VisualizationType))
                            {
                                SavedQuery savedQuery = new(baseQuery.Query, archived: true, baseQuery.Settings, DateTime.Now, request.Draft);
                                await _sqFileInterface.SerializeToFile(queryFileName, Configuration.Current.SavedQueryDirectory, savedQuery);

                                string archiveName = $"{guid}.sqa";
                                await _sqFileInterface.SerializeToFile(archiveName, Configuration.Current.ArchiveFileDirectory, new ArchiveCube(matrix));
                                _logger.LogInformation("Query re-archived successfully.");
                                _logger.LogDebug("Returning re-archive query result.");
                                return new ReArchiveResponse() { NewSqId = guid };
                            }

                            // If visualization type is not valid for the new data return 400.
                            _logger.LogWarning("Rearchiving a query has failed due to an invalid visualization type.");
                            return BadRequest();
                        }
                        catch (BadPxWebResponseException error)
                        {
                            _logger.LogWarning(error, "Re-archiving failed, failed to fetch data.");
                            return BadRequest();
                        }
                    }
                }
                else
                {
                    _auditLogService.LogAuditEvent(
                        action: actionPath,
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                    );

                    _logger.LogWarning("Source query file not found.");
                    return NotFound();
                }
            }
        }

        /// <summary>
        /// Validates visualization settings for th given cube meta
        /// Add all future validation rules here.
        /// </summary>
        private static bool ValidateVisualizationSettings(IReadOnlyMatrixMetadata meta, VisualizationSettings settings)
        {
            if (settings is LineChartVisualizationSettings lcvs &&
                meta.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Content)?.Code == lcvs.MultiselectableDimensionCode)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for draft state of a query based on given id
        /// </summary>
        /// <param name="id">Id to check draft state for</param>
        /// <returns>True if the query exists and is in draft state. Otherwise false</returns>
        private async Task<bool> GetIsDraftAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            if (_sqFileInterface.SavedQueryExists(id, Configuration.Current.SavedQueryDirectory))
            {
                SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(id, Configuration.Current.SavedQueryDirectory);
                return savedQuery.Draft;
            }

            return false;
        }
    }
}
