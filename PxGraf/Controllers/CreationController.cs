using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using Px.Utils.Language;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.ChartTypeSelection;
using PxGraf.Data;
using PxGraf.Datasource.FileDatasource;
using PxGraf.Datasource;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Models.Responses;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using PxGraf.Visualization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Px.Utils.Models.Metadata.ExtensionMethods;

namespace PxGraf.Controllers
{
    [FeatureGate("CreationAPI")]
    [ApiController]
    [Route("api/creation")]
    public class CreationController(ICachedDatasource datasource, ILogger<CreationController> logger, IAuditLogService auditLogService, IVirtualValueComputationService virtualValueComputationService) : ControllerBase
    {
        private readonly ICachedDatasource _datasource = datasource;
        private readonly ILogger<CreationController> _logger = logger;
        private readonly IAuditLogService _auditLogService = auditLogService;
        private readonly IVirtualValueComputationService _virtualValueComputationService = virtualValueComputationService;
        private readonly string[] databaseWhitelist = Configuration.Current.DatabaseWhitelist;

        /// <summary>
        /// Returns a list of database items from the given level of the data base based on the dbPath.
        /// </summary>
        /// <param name="dbPath">Path to the database.</param>
        /// <returns>
        /// <see cref="DatabaseGroupContents"/> object that contains either the tables or listing groups of the given database level.
        /// If a database whitelist is configured and the database is not whitelisted, "Not Found" response is returned.
        /// </returns>
        [HttpGet("data-bases/{*dbPath}")]
        [ProducesResponseType<DatabaseGroupContents>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DatabaseGroupContents>> GetDataBaseListingAsync([FromRoute] string dbPath)
        {
            Dictionary<string, object> logScope = new() { 
                [LoggerConstants.CONTROLLER] = nameof(CreationController),
                [LoggerConstants.ACTION] = "api/creation/data-bases"
            };
            using(_logger.BeginScope(logScope))
            {
                _logger.LogDebug("Database listing requested. GET api/creation/data-bases/");
                string[] hierarchy = dbPath is not null ? dbPath.Split("/") : [];
                if (!hierarchy.All(InputValidation.ValidateFilePathPart))
                {
                    _logger.LogWarning("Invalid path input.");
                    return BadRequest();
                }

                // Add validated dbPath to the logging scope
                using (_logger.BeginScope(new Dictionary<string, object> { [LoggerConstants.DB_PATH] = dbPath }))
                {
                    if (hierarchy.Length > 0 && !PathUtils.IsDatabaseWhitelisted(hierarchy, databaseWhitelist))
                    {
                        _logger.LogDebug("Requested database is not whitelisted.");
                        return NotFound();
                    }

                    // Find databases and tables with all available languages and add them to the response
                    DatabaseGroupContents result = await _datasource.GetGroupContentsCachedAsync(hierarchy);

                    _auditLogService.LogAuditEvent(
                        action: "api/creation/data-bases",
                        resource: hierarchy.Length > 0 ? string.Join("/", hierarchy) : "/"
                    );

                    // If listing databases, filter out databases that are not whitelisted
                    if (hierarchy.Length == 0 && databaseWhitelist.Length > 0)
                    {
                        List<DatabaseGroupHeader> filteredHeaders = [.. result.Headers.Where(header => PathUtils.IsDatabaseWhitelisted(header.Code, databaseWhitelist))];
                        DatabaseGroupContents resp = new(filteredHeaders, result.Files);
                        _logger.LogDebug("Returning table listing result: {Result}", resp);
                        return resp;
                    }

                    _logger.LogDebug("Returning table listing result: {Result}", result);
                    return result;
                }
            }
        }

        /// <summary>
        /// Returns all metadata from the table
        /// </summary>
        /// <param name="tablePath">Path to the table</param>
        /// <returns>
        /// Serialized json of <see cref="IReadOnlyMatrixMetadata"/> that contains the metadata of the cube
        /// If the database is not whitelisted, "Not Found" response is returned.
        /// </returns>
        [HttpGet("cube-meta/{*tablePath}")]
        [ProducesResponseType<IReadOnlyMatrixMetadata>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyMatrixMetadata>> GetCubeMetaAsync([FromRoute] string tablePath)
        {
            using(_logger.BeginScope(new Dictionary<string, object> {
                [LoggerConstants.CONTROLLER] = nameof(CreationController),
                [LoggerConstants.ACTION] = "api/creation/cube-meta",
            }))
            {
                _logger.LogDebug("Cube meta requested. GET: api/creation/cube-meta");

                PxTableReference tableReference = new(tablePath, '/');
                using (_logger.BeginScope(new Dictionary<string, object> { [LoggerConstants.DB_PATH] = tableReference.ToPath()}))
                {
                    if (!PathUtils.IsDatabaseWhitelisted(tableReference.Hierarchy, databaseWhitelist))
                    {
                        _logger.LogDebug("Requested database is not whitelisted.");
                        return NotFound();
                    }

                    _auditLogService.LogAuditEvent(
                        action: "api/creation/cube-meta",
                        resource: tableReference.ToPath()
                    );

                    IReadOnlyMatrixMetadata readOnlyMeta = await _datasource.GetMatrixMetadataCachedAsync(tableReference);
                    if (readOnlyMeta == null)
                    {
                        _logger.LogWarning("Cube-meta result was null. May result from a dimension without values.");
                        return BadRequest();
                    }
                    MatrixMetadata metadataClone = readOnlyMeta.GetTransform(readOnlyMeta);
                    _logger.LogDebug("Returning metadata result.");
                    return metadataClone;
                }
            }
        }

        /// <summary>
        /// Validates whether the given table has required metadata for visualization.
        /// </summary>
        /// <param name="tablePath">Path to the table.</param>
        /// <returns>
        /// <see cref="TableMetaValidationResult"/> object that contains info about whether the table's metadata is sufficient for visualization.
        /// If the database is not whitelisted, "Not Found" response is returned.
        /// </returns>
        [HttpGet("validate-table-metadata/{*tablePath}")]
        [ProducesResponseType<TableMetaValidationResult>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TableMetaValidationResult>> ValidateTableMetaData([FromRoute] string tablePath)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LoggerConstants.CONTROLLER] = nameof(CreationController),
                [LoggerConstants.ACTION] = "api/creation/validate-table-metadata",
            }))
            {
                _logger.LogDebug("Table metadata validation requested. GET: api/creation/validate-table-metadata");

                PxTableReference tableReference = new(tablePath, '/');
                using (_logger.BeginScope(new Dictionary<string, object> { [LoggerConstants.DB_PATH] = tableReference.ToPath() }))
                {
                    _auditLogService.LogAuditEvent(
                        action: "api/creation/validate-table-metadata",
                        resource: tableReference.ToPath()
                    );

                    if (!PathUtils.IsDatabaseWhitelisted(tableReference.Hierarchy, databaseWhitelist))
                    {
                        _logger.LogDebug("Requested database is not whitelisted.");
                        return NotFound();
                    }

                    IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(tableReference);
#nullable enable
                    IReadOnlyDimension? contDim = tableMeta.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Content);
                    IReadOnlyDimension? timeDim = tableMeta.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Time);
                    TableMetaValidationResult resp = new(
                        contDim is not null && contDim.Values.Count > 0,
                        timeDim is not null && timeDim.Values.Count > 0,
                        tableMeta.Dimensions.All(v => v.Values.Count > 0)
                    );
                    _logger.LogDebug("Returning table metadata validation result: {Result}", resp);
                    return resp;
#nullable disable
                }
            }
        }

        /// <summary>
        /// Returns a list of values for a given dimension using the provided filter.
        /// </summary>
        /// <param name="filterRequest">Filter request object containing the table reference and the filter.</param>
        /// <returns>Dictionary with dimension codes as keys and their value codes as values</returns>
        [HttpPost("filter-dimension")]
        [ProducesResponseType<Dictionary<string, List<string>>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, List<string>>>> GetDimensionFilterResultAsync([FromBody] FilterRequest filterRequest)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LoggerConstants.CONTROLLER] = nameof(CreationController),
                [LoggerConstants.ACTION] = "api/creation/filter-dimension",
                [LoggerConstants.DB_PATH] = filterRequest.TableReference.ToPath()
            }))
            {
                _logger.LogDebug("Dimension filtering requested. POST: api/creation/filter-dimension");

                _auditLogService.LogAuditEvent(
                    action: "api/creation/filter-dimension",
                    resource: filterRequest.TableReference.ToPath()
                );

                IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(filterRequest.TableReference);

                Dictionary<string, List<string>> result = filterRequest.Filters.ToDictionary(
                    filter => filter.Key,
                    filter =>
                    {
                        if (tableMeta.Dimensions.FirstOrDefault(dimension => dimension.Code == filter.Key) is IReadOnlyDimension dimension)
                        {
                            if (filterRequest.VirtualValueDefinitions != null &&
                                filterRequest.VirtualValueDefinitions.TryGetValue(filter.Key, out List<string> virtualCodes))
                            {
                                return filter.Value.Filter([.. dimension.ValueCodes, .. virtualCodes]).ToList();
                            }
                            return filter.Value.Filter(dimension.ValueCodes).ToList();
                        }
                        else
                        {
                            return [];
                        }
                    }
                );

                _logger.LogDebug("Returning dimension filtering result: {Result}", result);
                return result;
            }
        }

        /// <summary>
        /// Provides information required for the editor to set up the visualization.
        /// </summary>
        /// <remarks>If the query doesn't contain any dimension queries or the resulting matrix is empty, an empty response is returned.</remarks>
        /// <param name="query">Query object containing the table reference and dimension queries.</param>
        /// <returns><see cref="EditorContentsResponse"/> object that contains the size of the resulting matrix, maximum supported size, header text, maximum header length, visualization options for accepted visualization types and rejection reasons.</returns>
#nullable enable
        [HttpPost("editor-contents")]
        [ProducesResponseType<EditorContentsResponse>(StatusCodes.Status200OK)]
        public async Task<ActionResult<EditorContentsResponse>> GetEditorContents([FromBody] MatrixQuery query)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LoggerConstants.CONTROLLER] = nameof(CreationController),
                [LoggerConstants.ACTION] = "api/creation/editor-contents",
                [LoggerConstants.DB_PATH] = query.TableReference.ToPath()
            }))
            {
                _logger.LogDebug("Editor contents requested. POST: api/creation/editor-contents");

                _auditLogService.LogAuditEvent(
                    action: "api/creation/editor-contents",
                    resource: query.TableReference.ToPath()
                );

                int maxQuerySize = Configuration.Current.QueryOptions.MaxQuerySize;

                if (query.DimensionQueries.Count == 0)
                {
                    _logger.LogDebug("Query did not contain any dimension queries, returning empty editor contents result.");
                    return EditorContentsResponse.Empty;
                }

                IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(query.TableReference);

                (IReadOnlyMatrixMetadata fetchMeta, MatrixMap outputMap) = tableMeta.BuildVirtualValueMaps(query);
                long outputMapSize = outputMap.GetSize();
                if (outputMapSize ==  0)
                {
                    _logger.LogDebug("One or more dimensions have no selected output values. Returning empty editor contents result.");
                    return EditorContentsResponse.Empty;
                }

                long fetchSize = fetchMeta.GetSize();

                if (outputMapSize == 0 || outputMapSize > maxQuerySize || fetchSize > maxQuerySize)
                {
                    _logger.LogDebug("Output size {OutputMapSize} or fetch size {FetchSize} exceeds maximum query size {MaxQuerySize}. Returning response with no valid visualization types.", outputMapSize, fetchSize, maxQuerySize);
                    return new EditorContentsResponse()
                    {
                        Size = outputMapSize,
                        MaximumSupportedSize = maxQuerySize,
                        SizeWarningLimit = Convert.ToInt32(maxQuerySize * Configuration.Current.QueryOptions.QuerySizeWarningRatio),
                        HeaderText = new MultilanguageString(Configuration.Current.LanguageOptions.Available.Select(lang => new KeyValuePair<string, string>(lang, string.Empty))),
                        MaximumHeaderLength = Configuration.Current.QueryOptions.MaxHeaderLength,
                        VisualizationOptions = [],
                        VisualizationRejectionReasons = [],
                        PublicationWebhookEnabled = Configuration.Current.PublicationWebhookConfig.IsEnabled
                    };
                }

                Matrix<DecimalDataValue> matrix = await _datasource.GetMatrixCachedAsync(query.TableReference, fetchMeta);

                if (query.DimensionQueries.Values.Any(dq => dq.VirtualValueDefinitions?.Count > 0))
                    matrix = _virtualValueComputationService.ApplyVirtualValues(matrix, query);
                matrix = matrix.GetTransform(outputMap);

                Dictionary<VisualizationType, MultilanguageString> rejectionReasons = [];
                List<VisualizationOption> visualizationOptions = [];

                foreach (KeyValuePair<VisualizationType, IReadOnlyList<ChartRejectionInfo>> reasonKvp in
                    ChartTypeSelector.Selector.GetRejectionReasons(query, matrix))
                {
                    if (reasonKvp.Value.Any())
                    {
                        Dictionary<string, string> translations = [];
                        foreach (string language in Localization.GetAllAvailableLanguages())
                        {
                            translations[language] = Localization.FromLanguage(language)
                                .Translation.RejectionReasons.GetTranslation(reasonKvp.Value[0]);
                        }
                        rejectionReasons[reasonKvp.Key] = new(translations);
                    }
                    else  // no rejection reasons == valid visualization type
                    {
                        visualizationOptions.Add(GetVisualizationOption(reasonKvp.Key, matrix.Metadata, query));
                    }
                }

                _logger.LogDebug("Returning editor contents result with size {OutputValuesCount} and {VisualizationOptionsCount} visualization options.", outputMapSize, visualizationOptions.Count);
                return new EditorContentsResponse()
                {
                    Size = outputMapSize,
                    MaximumSupportedSize = maxQuerySize,
                    SizeWarningLimit = Convert.ToInt32(maxQuerySize * Configuration.Current.QueryOptions.QuerySizeWarningRatio),
                    HeaderText = HeaderBuildingUtilities.GetHeader(matrix.Metadata, query, true),
                    MaximumHeaderLength = Configuration.Current.QueryOptions.MaxHeaderLength,
                    VisualizationOptions = visualizationOptions,
                    VisualizationRejectionReasons = rejectionReasons,
                    PublicationWebhookEnabled = Configuration.Current.PublicationWebhookConfig.IsEnabled
                };
            }
        }
#nullable disable

        /// <summary>
        /// Returns visualization object matching the request.
        /// </summary>
        /// <param name="request"><see cref="ChartRequest"/> object containing the query, visualization settings and selected visualization type.</param>
        /// <returns>
        /// <see cref="VisualizationResponse"/> object that contains the data required for rendering the visualization. 
        /// If the request is missing values for any dimension or the selected visualization type is not valid, "Bad Request" response is returned.
        /// </returns>
        [HttpPost("visualization")]
        [ProducesResponseType<VisualizationResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VisualizationResponse>> GetVisualizationAsync([FromBody] ChartRequest request)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LoggerConstants.CONTROLLER] = nameof(CreationController),
                [LoggerConstants.ACTION] = "api/creation/visualization",
                [LoggerConstants.DB_PATH] = request.Query.TableReference.ToPath()
            }))
            {
                _auditLogService.LogAuditEvent(
                    action: "api/creation/visualization",
                    resource: request.Query.TableReference.ToPath()
                );

                _logger.LogDebug("Requesting visualization. POST: api/creation/visualization");
                IReadOnlyMatrixMetadata completeMeta = await _datasource.GetMatrixMetadataCachedAsync(request.Query.TableReference);

                (IReadOnlyMatrixMetadata fetchMeta, MatrixMap outputMap) = completeMeta.BuildVirtualValueMaps(request.Query);
                if (outputMap.DimensionMaps.Any(dm => dm.ValueCodes.Count == 0))
                {
                    _logger.LogDebug("One or more dimensions have no selected output values.");
                    return BadRequest();
                }

                // The resulting cube would have volume 0 (e.g. filter produced empty after expansion)
                if (fetchMeta.Dimensions.Any(d => d.Values.Count == 0))
                {
                    _logger.LogDebug("One or more dimensions have no included values.");
                    return BadRequest();
                }

                Matrix<DecimalDataValue> matrix = await _datasource.GetMatrixCachedAsync(request.Query.TableReference, fetchMeta);

                if (request.Query.DimensionQueries.Values.Any(dq => dq.VirtualValueDefinitions?.Count > 0))
                    matrix = _virtualValueComputationService.ApplyVirtualValues(matrix, request.Query);
                matrix = matrix.GetTransform(outputMap);

                IChartTypeSelector selector = ChartTypeSelector.Selector;
                if (selector.GetValidChartTypes(request.Query, matrix).Contains(request.VisualizationSettings.SelectedVisualization))
                {
                    VisualizationSettings vSettings = request.VisualizationSettings.ToVisualizationSettings(matrix.Metadata, request.Query);
                    VisualizationResponse visualizationResponse = PxVisualizerCubeAdapter.BuildVisualizationResponse(matrix, request.Query, vSettings);
                    _logger.LogDebug("Returning visualization result.");

                    return visualizationResponse;
                }
                else
                {
                    _logger.LogWarning("Selected visualization type is not valid for this query.");
                    return BadRequest();
                }
            }
        }

#nullable enable
        private static VisualizationOption GetVisualizationOption(VisualizationType type, IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            bool manualPivotability = ManualPivotRules.GetManualPivotability(type, meta, query);

            bool AllowCuttingYAxis =
                type == VisualizationType.LineChart ||
                type == VisualizationType.ScatterPlot;

            bool AllowMatchXLabelsToEnd =
                type == VisualizationType.VerticalBarChart ||
                type == VisualizationType.GroupVerticalBarChart ||
                type == VisualizationType.PercentVerticalBarChart ||
                type == VisualizationType.StackedVerticalBarChart;

            return new VisualizationOption()
            {
                Type = type,
                AllowManualPivot = manualPivotability,
                allowMultiselect = (type == VisualizationType.LineChart) && query.DimensionQueries.Any(vq => vq.Value.Selectable),
                AllowShowingDataPoints = type == VisualizationType.VerticalBarChart,
                AllowCuttingYAxis = AllowCuttingYAxis,
                AllowMatchXLabelsToEnd = AllowMatchXLabelsToEnd,
                AllowSetMarkerScale = type == VisualizationType.ScatterPlot,
                SortingOptions = CubeSorting.Get(type, meta, manualPivotability, query)
            };
        }
#nullable disable
    }
}
