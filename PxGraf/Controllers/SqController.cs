using Microsoft.AspNetCore.Http;
#nullable enable
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Dimensions;
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
using PxGraf.Services;
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
    /// <param name="datasource">Instance of a <see cref="ICachedDatasource"/> object. Used to interact with data source and cached data.</param>
    /// <param name="sqFileInterface">Instance of a <see cref="ISqFileInterface"/> object. Used for interacting with saved queries.</param>
    /// <param name="logger"><see cref="ILogger"/> instance used for logging.</param>
    /// <param name="auditLogService">Service for logging audit events.</param>
    /// <param name="webhookService">Service for sending publication webhooks.</param>
    /// <param name="virtualValueValidationService">Service for validating virtual value definitions.</param>
    /// <param name="virtualValueComputationService">Service for computing virtual dimension values.</param>
    [FeatureGate("CreationAPI")]
    [ApiController]
    [Route("api/sq")]
    public class SqController(ICachedDatasource datasource, ISqFileInterface sqFileInterface, ILogger<SqController> logger, IAuditLogService auditLogService, IPublicationWebhookService webhookService, IVirtualValueValidationService virtualValueValidationService, IVirtualValueComputationService virtualValueComputationService) : ControllerBase
    {
        private readonly ICachedDatasource _cachedDatasource = datasource;
        private readonly ISqFileInterface _sqFileInterface = sqFileInterface;
        private readonly ILogger<SqController> _logger = logger;
        private readonly IAuditLogService _auditLogService = auditLogService;
        private readonly IPublicationWebhookService _webhookService = webhookService;
        private readonly IVirtualValueValidationService _virtualValueValidationService = virtualValueValidationService;
        private readonly IVirtualValueComputationService _virtualValueComputationService = virtualValueComputationService;
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
        [ProducesResponseType<SaveQueryParams>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SaveQueryParams>> GetSavedQueryAsync([FromRoute] string savedQueryId)
        {
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(SqController),
                [LoggerConstants.ACTION] = CONTROLLER_PATH
            };
            using (_logger.BeginScope(logScope))
            {
                if (!InputValidation.ValidateSqIdString(savedQueryId))
                {
                    _auditLogService.LogAuditEvent(
                        action: CONTROLLER_PATH,
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                    );

                    return BadRequest();
                }

                _logger.LogDebug("Saved query requested.");
                if (await _sqFileInterface.SavedQueryExists(savedQueryId, Configuration.Current.SavedQueryDirectory))
                {
                    using (_logger.BeginScope(new Dictionary<string, object> { [LoggerConstants.SQ_ID] = savedQueryId }))
                    {
                        _auditLogService.LogAuditEvent(
                            action: CONTROLLER_PATH,
                            resource: savedQueryId
                        );

                        SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(savedQueryId, Configuration.Current.SavedQueryDirectory);
                        IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(savedQuery.Query.TableReference);

                        MatrixQuery reconciledQuery = ReconcileSavedQuery(
                            meta,
                            savedQuery.Query,
                            out HashSet<string> resetDimensionCodes,
                            out bool recoveredWithChanges);
                        SavedQuery reconciledSavedQuery = new(
                            reconciledQuery,
                            savedQuery.Archived,
                            savedQuery.Settings,
                            savedQuery.CreationTime,
                            savedQuery.Draft);
                        foreach (KeyValuePair<string, object> legacyProperty in savedQuery.LegacyProperties)
                        {
                            reconciledSavedQuery.LegacyProperties[legacyProperty.Key] = legacyProperty.Value;
                        }

                        (IReadOnlyMatrixMetadata fetchMeta, MatrixMap outputMap) = meta.BuildVirtualValueMaps(reconciledQuery);
                        if (outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0))
                        {
                            _logger.LogWarning("Saved query metadata contains dimensions with no selected output values.");
                            return BadRequest();
                        }

                        Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixCachedAsync(savedQuery.Query.TableReference, fetchMeta);
                        if (matrix == null)
                        {
                            _logger.LogWarning("Fetching data based on the saved query failed.");
                            return BadRequest();
                        }

                        if (reconciledQuery.DimensionQueries.Values.Any(dq => dq.VirtualValueDefinitions?.Count > 0))
                            matrix = _virtualValueComputationService.ApplyVirtualValues(matrix, reconciledQuery);
                        matrix = matrix.GetTransform(outputMap);

                        IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(reconciledQuery, matrix);
                        if (validTypes.Count == 0)
                        {
                            _logger.LogWarning("No visualization type is valid for the reconciled saved query.");
                            return BadRequest();
                        }

                        if (!validTypes.Contains(savedQuery.Settings.VisualizationType))
                        {
                            VisualizationCreationSettings fallbackCreationSettings = new()
                            {
                                SelectedVisualization = validTypes[0]
                            };
                            reconciledSavedQuery.Settings = fallbackCreationSettings.ToVisualizationSettings(matrix.Metadata, reconciledQuery);
                            _logger.LogInformation(
                                "Saved visualization type was no longer valid. Using {VisualizationType} instead.",
                                validTypes[0]);
                            recoveredWithChanges = true;
                        }

                        VisualizationCreationSettings creationSettings = VisualizationCreationSettings.FromVisualizationSettings(
                            reconciledSavedQuery,
                            matrix.Metadata);
                        recoveredWithChanges |= NormalizeVisualizationCreationSettings(
                            creationSettings,
                            matrix.Metadata,
                            reconciledQuery,
                            resetDimensionCodes);

                        SaveQueryParams saveQueryParams = new()
                        {
                            Query = reconciledQuery,
                            Settings = creationSettings,
                            Draft = savedQuery.Draft,
                            Id = savedQueryId,
                            RecoveredWithChanges = recoveredWithChanges
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
        [ProducesResponseType<SaveQueryResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                if (!HasValidOptionalSqId(parameters.Id))
                {
                    return BadRequest();
                }

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

                    // Validate virtual value definitions before saving
                    if (!TryValidateVirtualValueDefinitions(tableMeta, parameters.Query, out string dimensionCode, out string? validationError))
                    {
                        _logger.LogWarning("Virtual value validation failed for dimension {DimCode}.", dimensionCode);
                        return BadRequest(new { error = validationError });
                    }

                    (IReadOnlyMatrixMetadata fetchMeta, MatrixMap outputMap) = tableMeta.BuildVirtualValueMaps(parameters.Query);
                    if (outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0))
                    {
                        _logger.LogWarning("One or more dimensions have no selected output values.");
                        return BadRequest();
                    }

                    Matrix<DecimalDataValue> dataCube = await _cachedDatasource.GetMatrixCachedAsync(parameters.Query.TableReference, fetchMeta);

                    if (parameters.Query.DimensionQueries.Values.Any(dq => dq.VirtualValueDefinitions?.Count > 0))
                        dataCube = _virtualValueComputationService.ApplyVirtualValues(dataCube, parameters.Query);
                    dataCube = dataCube.GetTransform(outputMap);

                    VisualizationSettings visualizationSettings = parameters.Settings.ToVisualizationSettings(dataCube.Metadata, parameters.Query);

                    // All dimensions must have at least one value selected
                    if (dataCube.Metadata.Dimensions.Any(v => v.Values.Count == 0) || !ValidateVisualizationSettings(dataCube.Metadata, visualizationSettings))
                    {
                        _logger.LogWarning("Query is missing values for a dimension.");
                        return BadRequest();
                    }

                    IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(parameters.Query, dataCube);
                    if (validTypes.Contains(visualizationSettings.VisualizationType))
                    {
                        SavedQuery savedQuery = new(parameters.Query, archived: false, visualizationSettings, DateTime.Now, parameters.Draft);
                        await _sqFileInterface.SerializeToSqFileAsync(fileName, Configuration.Current.SavedQueryDirectory, savedQuery);
                        WebhookPublicationResult webhookResult = new () { Status = QueryPublicationStatus.Unpublished };

                        // Trigger webhook for non-draft queries only if webhook is enabled
                        if (!parameters.Draft && Configuration.Current.PublicationWebhookConfig.IsEnabled)
                        {
                            IReadOnlyMatrixMetadata filteredMeta = tableMeta.FilterDimensionValues(parameters.Query);
                            webhookResult = await _webhookService.TriggerWebhookAsync(guid, savedQuery, filteredMeta.AdditionalProperties);
                        }

                        SaveQueryResponse saveQueryResponse = new()
                        {
                            Id = guid,
                            PublicationStatus = webhookResult.Status,
                            PublicationMessage = webhookResult.Messages
                        };
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
        [ProducesResponseType<SaveQueryResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                if (!HasValidOptionalSqId(parameters.Id))
                {
                    return BadRequest();
                }

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

                    // Validate virtual value definitions BEFORE writing any files
                    if (!TryValidateVirtualValueDefinitions(meta, parameters.Query, out string dimensionCode, out string? error))
                    {
                        _logger.LogWarning("Virtual value validation failed for dimension {DimCode}.", dimensionCode);
                        return BadRequest(new { error });
                    }

                    var (fetchMeta, outputMap) = meta.BuildVirtualValueMaps(parameters.Query);
                    if (outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0))
                    {
                        _logger.LogWarning("One or more dimensions have no selected output values.");
                        _auditLogService.LogAuditEvent(
                            action: actionPath,
                            resource: LoggerConstants.INVALID_VISUALIZATION
                        );
                        return BadRequest();
                    }

                    Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixCachedAsync(parameters.Query.TableReference, fetchMeta);

                    if (parameters.Query.DimensionQueries.Values.Any(dq => dq.VirtualValueDefinitions?.Count > 0))
                        matrix = _virtualValueComputationService.ApplyVirtualValues(matrix, parameters.Query);
                    matrix = matrix.GetTransform(outputMap);

                    VisualizationSettings visualizationSettings = parameters.Settings.ToVisualizationSettings(matrix.Metadata, parameters.Query);
                    if (matrix.Metadata.Dimensions.Any(v => v.Values.Count == 0) || !ValidateVisualizationSettings(matrix.Metadata, visualizationSettings))
                    {
                        _logger.LogWarning("Query is missing values for a dimension.");
                        _auditLogService.LogAuditEvent(
                            action: actionPath,
                            resource: LoggerConstants.INVALID_VISUALIZATION
                        );
                        return BadRequest();
                    }

                    IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(parameters.Query, matrix);
                    if (validTypes.Contains(visualizationSettings.VisualizationType))
                    {
                        SavedQuery savedQuery = new(parameters.Query, archived: true, visualizationSettings, DateTime.Now, parameters.Draft);
                        await _sqFileInterface.SerializeToSqFileAsync(queryFileName, Configuration.Current.SavedQueryDirectory, savedQuery);

                        string archiveName = $"{guid}.sqa";
                        await _sqFileInterface.SerializeToArchiveFileAsync(archiveName, Configuration.Current.ArchiveFileDirectory, new ArchiveCube(matrix));

                        WebhookPublicationResult webhookResult = new() { Status = QueryPublicationStatus.Unpublished };

                        // Trigger webhook for non-draft queries only if webhook is enabled
                        if (!parameters.Draft && Configuration.Current.PublicationWebhookConfig.IsEnabled)
                        {
                            IReadOnlyMatrixMetadata filteredMeta = meta.FilterDimensionValues(parameters.Query);
                            webhookResult = await _webhookService.TriggerWebhookAsync(guid, savedQuery, filteredMeta.AdditionalProperties);
                        }

                        _logger.LogInformation("Query archived successfully.");
                        _logger.LogDebug("Returning archive query result.");
                        return new SaveQueryResponse()
                        {
                            Id = guid,
                            PublicationStatus = webhookResult.Status,
                            PublicationMessage = webhookResult.Messages
                        };
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
        [ProducesResponseType<ReArchiveResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                if (!InputValidation.ValidateSqIdString(request.SqId))
                {
                    _auditLogService.LogAuditEvent(
                        action: actionPath,
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                    );

                    return BadRequest();
                }

                _logger.LogDebug("Re-archiving query.");
                if (await _sqFileInterface.SavedQueryExists(request.SqId, Configuration.Current.SavedQueryDirectory))
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

                            // Validate virtual value definitions BEFORE writing any files
                            foreach (KeyValuePair<string, DimensionQuery> dimEntry in baseQuery.Query.DimensionQueries)
                            {
                                if (dimEntry.Value.VirtualValueDefinitions?.Count > 0)
                                {
                                    IReadOnlyDimension dimension = meta.Dimensions.First(d => d.Code == dimEntry.Key);
                                    List<string> realValueCodes = [.. dimension.Values.Select(v => v.Code)];
                                    if (!_virtualValueValidationService.Validate(dimEntry.Value.VirtualValueDefinitions, realValueCodes, out string? error))
                                    {
                                        _logger.LogWarning("Virtual value validation failed for dimension {DimCode}.", dimEntry.Key);
                                        return BadRequest(new { error });
                                    }
                                }
                            }

                            var (fetchMeta, outputMap) = meta.BuildVirtualValueMaps(baseQuery.Query);
                            if (outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0))
                            {
                                _logger.LogWarning("One or more dimensions have no selected output values.");
                                return BadRequest();
                            }

                            Matrix<DecimalDataValue> matrix = await _cachedDatasource.GetMatrixCachedAsync(baseQuery.Query.TableReference, fetchMeta);

                            if (baseQuery.Query.DimensionQueries.Values.Any(dq => dq.VirtualValueDefinitions?.Count > 0))
                                matrix = _virtualValueComputationService.ApplyVirtualValues(matrix, baseQuery.Query);
                            matrix = matrix.GetTransform(outputMap);

                            IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(baseQuery.Query, matrix);
                            if (validTypes.Contains(baseQuery.Settings.VisualizationType))
                            {
                                SavedQuery savedQuery = new(baseQuery.Query, archived: true, baseQuery.Settings, DateTime.Now, request.Draft);
                                await _sqFileInterface.SerializeToSqFileAsync(queryFileName, Configuration.Current.SavedQueryDirectory, savedQuery);

                                string archiveName = $"{guid}.sqa";
                                await _sqFileInterface.SerializeToArchiveFileAsync(archiveName, Configuration.Current.ArchiveFileDirectory, new ArchiveCube(matrix));

                                WebhookPublicationResult webhookResult = new() { Status = QueryPublicationStatus.Unpublished };

                                // Trigger webhook for non-draft queries only if webhook is enabled
                                if (!request.Draft && Configuration.Current.PublicationWebhookConfig.IsEnabled)
                                {
                                    IReadOnlyMatrixMetadata filteredMeta = meta.FilterDimensionValues(baseQuery.Query);
                                    webhookResult = await _webhookService.TriggerWebhookAsync(guid, savedQuery, filteredMeta.AdditionalProperties);
                                }

                                _logger.LogInformation("Query re-archived successfully.");
                                _logger.LogDebug("Returning re-archive query result.");
                                return new ReArchiveResponse()
                                {
                                    NewSqId = guid,
                                    PublicationStatus = webhookResult.Status,
                                    PublicationMessage = webhookResult.Messages
                                };
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

        private bool TryValidateVirtualValueDefinitions(IReadOnlyMatrixMetadata meta, MatrixQuery query, out string dimensionCode, out string? validationError)
        {
            foreach (KeyValuePair<string, DimensionQuery> dimEntry in query.DimensionQueries.Where(dimEntry => dimEntry.Value.VirtualValueDefinitions?.Count > 0))
            {
                IReadOnlyDimension? dimension = meta.Dimensions.FirstOrDefault(d => d.Code == dimEntry.Key);
                if (dimension is null)
                {
                    dimensionCode = dimEntry.Key;
                    validationError = $"Dimension '{dimEntry.Key}' does not exist in the current table metadata.";
                    return false;
                }
                List<string> realValueCodes = [.. dimension.Values.Select(v => v.Code)];
                if (!_virtualValueValidationService.Validate(dimEntry.Value.VirtualValueDefinitions, realValueCodes, out validationError))
                {
                    dimensionCode = dimEntry.Key;
                    return false;
                }
            }

            dimensionCode = string.Empty;
            validationError = null;
            return true;
        }

        private MatrixQuery ReconcileSavedQuery(
            IReadOnlyMatrixMetadata meta,
            MatrixQuery savedQuery,
            out HashSet<string> resetDimensionCodes,
            out bool recoveredWithChanges)
        {
            Dictionary<string, DimensionQuery> reconciledDimensionQueries = [];
            resetDimensionCodes = [];
            HashSet<string> currentDimensionCodes = [.. meta.Dimensions.Select(dimension => dimension.Code)];
            recoveredWithChanges = !savedQuery.DimensionQueries.Keys.ToHashSet().SetEquals(currentDimensionCodes);

            foreach (IReadOnlyDimension dimension in meta.Dimensions)
            {
                if (savedQuery.DimensionQueries.TryGetValue(dimension.Code, out DimensionQuery? dimensionQuery) &&
                    TryReconcileDimensionQuery(
                        dimension,
                        dimensionQuery,
                        out DimensionQuery reconciledDimensionQuery,
                        out bool dimensionChanged))
                {
                    reconciledDimensionQueries[dimension.Code] = reconciledDimensionQuery;
                    recoveredWithChanges |= dimensionChanged;
                }
                else
                {
                    reconciledDimensionQueries[dimension.Code] = GetDefaultDimensionQuery(dimension);
                    resetDimensionCodes.Add(dimension.Code);
                    recoveredWithChanges = true;
                }
            }

            return new MatrixQuery
            {
                TableReference = savedQuery.TableReference,
                ChartHeaderEdit = savedQuery.ChartHeaderEdit,
                DimensionQueries = reconciledDimensionQueries
            };
        }

        private bool TryReconcileDimensionQuery(
            IReadOnlyDimension dimension,
            DimensionQuery dimensionQuery,
            out DimensionQuery reconciledDimensionQuery,
            out bool changed)
        {
            changed = false;
            HashSet<string> realValueCodes = [.. dimension.ValueCodes];
            List<VirtualValueDefinition> virtualValueDefinitions = dimensionQuery.VirtualValueDefinitions ?? [];
            if (virtualValueDefinitions.Count > 0 &&
                !_virtualValueValidationService.Validate(virtualValueDefinitions, realValueCodes, out _))
            {
                reconciledDimensionQuery = null!;
                return false;
            }

            List<string> availableValueCodes = [.. dimension.ValueCodes, .. virtualValueDefinitions.Select(definition => definition.Code)];
            HashSet<string> availableValueCodeSet = [.. availableValueCodes];
            IValueFilter valueFilter = dimensionQuery.ValueFilter;

            if ((valueFilter is ItemFilter itemFilter && itemFilter.Codes.Any(code => !availableValueCodeSet.Contains(code))) ||
                (valueFilter is FromFilter fromFilter && !availableValueCodeSet.Contains(fromFilter.Code)))
            {
                reconciledDimensionQuery = null!;
                return false;
            }

            if (valueFilter is InverseItemFilter inverseItemFilter)
            {
                List<string> retainedCodes = [.. inverseItemFilter.Codes.Where(availableValueCodeSet.Contains)];
                changed = retainedCodes.Count != inverseItemFilter.Codes.Count;
                valueFilter = new InverseItemFilter(retainedCodes);
            }

            if (!valueFilter.Filter(availableValueCodes).Any())
            {
                reconciledDimensionQuery = null!;
                return false;
            }

            Dictionary<string, DimensionQuery.DimensionValueEdition> retainedValueEdits = dimensionQuery.ValueEdits
                .Where(entry => availableValueCodeSet.Contains(entry.Key))
                .ToDictionary();
            changed |= retainedValueEdits.Count != dimensionQuery.ValueEdits.Count;

            reconciledDimensionQuery = new DimensionQuery
            {
                NameEdit = dimensionQuery.NameEdit,
                ValueEdits = retainedValueEdits,
                ValueFilter = valueFilter,
                VirtualValueDefinitions = virtualValueDefinitions,
                Selectable = dimensionQuery.Selectable
            };
            return true;
        }

        private static DimensionQuery GetDefaultDimensionQuery(IReadOnlyDimension dimension)
        {
            IValueFilter valueFilter;
            if (dimension.Type == DimensionType.Time)
            {
                valueFilter = new AllFilter();
            }
            else
            {
                string? eliminationCode = dimension.GetEliminationValueCode();
                string? defaultCode = eliminationCode is not null && dimension.ValueCodes.Contains(eliminationCode)
                    ? eliminationCode
                    : dimension.ValueCodes.FirstOrDefault();
                valueFilter = new ItemFilter(defaultCode is null ? [] : [defaultCode]);
            }

            return new DimensionQuery
            {
                ValueFilter = valueFilter,
                Selectable = false
            };
        }

        private static bool NormalizeVisualizationCreationSettings(
            VisualizationCreationSettings settings,
            IReadOnlyMatrixMetadata meta,
            MatrixQuery query,
            HashSet<string> resetDimensionCodes)
        {
            Dictionary<string, IReadOnlyDimension> dimensionsByCode = meta.Dimensions.ToDictionary(dimension => dimension.Code);
            bool changed = false;

            if (settings.DefaultSelectableDimensionCodes is not null)
            {
                Dictionary<string, List<string>> normalizedDefaults = settings.DefaultSelectableDimensionCodes
                    .Where(entry => !resetDimensionCodes.Contains(entry.Key) && dimensionsByCode.ContainsKey(entry.Key))
                    .Select(entry => new KeyValuePair<string, List<string>>(
                        entry.Key,
                        [.. entry.Value.Where(dimensionsByCode[entry.Key].ValueCodes.Contains)]))
                    .Where(entry => entry.Value.Count > 0)
                    .ToDictionary();
                changed = normalizedDefaults.Count != settings.DefaultSelectableDimensionCodes.Count ||
                    normalizedDefaults.Any(entry =>
                        !settings.DefaultSelectableDimensionCodes.TryGetValue(entry.Key, out List<string>? originalCodes) ||
                        !entry.Value.SequenceEqual(originalCodes));
                settings.DefaultSelectableDimensionCodes = normalizedDefaults;
            }

            if (settings.MultiselectableDimensionCode is not null &&
                (resetDimensionCodes.Contains(settings.MultiselectableDimensionCode) ||
                 !dimensionsByCode.ContainsKey(settings.MultiselectableDimensionCode)))
            {
                settings.MultiselectableDimensionCode = null;
                changed = true;
            }

            if (settings.SelectedVisualization == VisualizationType.Table)
            {
                List<string> multivalueDimensionCodes = [.. meta.Dimensions
                    .Where(dimension => dimension.Values.Count > 1 && !query.DimensionQueries[dimension.Code].Selectable)
                    .OrderBy(dimension => dimension.Values.Count)
                    .Select(dimension => dimension.Code)];
                List<string> rowDimensionCodes = [.. (settings.RowDimensionCodes ?? [])
                    .Where(multivalueDimensionCodes.Contains)
                    .Distinct()];
                List<string> columnDimensionCodes = [.. (settings.ColumnDimensionCodes ?? [])
                    .Where(code => multivalueDimensionCodes.Contains(code) && !rowDimensionCodes.Contains(code))
                    .Distinct()];
                List<string> unassignedCodes = [.. multivalueDimensionCodes
                    .Where(code => !rowDimensionCodes.Contains(code) && !columnDimensionCodes.Contains(code))];
                int newColumnCount = unassignedCodes.Count / 2;
                columnDimensionCodes.AddRange(unassignedCodes.Take(newColumnCount));
                rowDimensionCodes.AddRange(unassignedCodes.Skip(newColumnCount));
                rowDimensionCodes.AddRange(meta.Dimensions
                    .Select(dimension => dimension.Code)
                    .Where(code => !multivalueDimensionCodes.Contains(code)));
                changed |= !(settings.RowDimensionCodes ?? []).SequenceEqual(rowDimensionCodes) ||
                    !(settings.ColumnDimensionCodes ?? []).SequenceEqual(columnDimensionCodes);
                settings.RowDimensionCodes = rowDimensionCodes;
                settings.ColumnDimensionCodes = columnDimensionCodes;
            }

            return changed;
        }

        /// <summary>
        /// Checks for draft state of a query based on given id
        /// </summary>
        /// <param name="id">Id to check draft state for</param>
        /// <returns>True if the query exists and is in draft state. Otherwise false</returns>
        private async Task<bool> GetIsDraftAsync(string id)
        {
            if (!InputValidation.ValidateSqIdString(id))
            {
                return false;
            }

            if (await _sqFileInterface.SavedQueryExists(id, Configuration.Current.SavedQueryDirectory))
            {
                SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(id, Configuration.Current.SavedQueryDirectory);
                return savedQuery.Draft;
            }

            return false;
        }

        private static bool HasValidOptionalSqId(string? id)
        {
            return string.IsNullOrEmpty(id) || InputValidation.ValidateSqIdString(id);
        }
    }
}
