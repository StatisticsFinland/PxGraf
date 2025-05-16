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
using PxGraf.Datasource;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Models.Responses;
using PxGraf.Settings;
using PxGraf.Visualization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using PxGraf.Datasource.FileDatasource;

namespace PxGraf.Controllers
{
    [FeatureGate("CreationAPI")]
    [ApiController]
    [Route("api/creation")]
    public class CreationController(ICachedDatasource datasource, ILogger<CreationController> logger) : ControllerBase
    {
        private readonly ICachedDatasource _datasource = datasource;
        private readonly ILogger<CreationController> _logger = logger;
        private readonly string[] databaseWhitelist = Configuration.Current.LocalFilesystemDatabaseConfig.DatabaseWhitelist;

        /// <summary>
        /// Returns a list of database items from the given level of the data base based on the dbPath.
        /// </summary>
        /// <param name="dbPath">Path to the database.</param>
        /// <returns>
        /// <see cref="DatabaseGroupContents"/> object that contains either the tables or listing groups of the given database level.
        /// If a database whitelist is configured and the database is not whitelisted, "Not Found" response is returned.
        /// </returns>
        [HttpGet("data-bases/{*dbPath}")]
        public async Task<ActionResult<DatabaseGroupContents>> GetDataBaseListingAsync([FromRoute] string dbPath)
        {
            const string LOGMSG = "Getting the database listing. GET: api/creation/data-bases/{DbPath}";
            _logger.LogDebug(LOGMSG, dbPath);

            string[] hierarchy = dbPath is not null ? dbPath.Split("/") : [];
            if (hierarchy.Length > 0 && !PathUtils.IsDatabaseWhitelisted(hierarchy, databaseWhitelist))
            {
                _logger.LogInformation("Database {DbPath} is not whitelisted.", dbPath);
                return NotFound();
            }
            // Find databases and tables with all available languages and add them to the response
            DatabaseGroupContents result = await _datasource.GetGroupContentsCachedAsync(hierarchy);

            // If listing databases, filter out databases that are not whitelisted
            if (hierarchy.Length == 0 && databaseWhitelist.Length > 0)
            {
                List<DatabaseGroupHeader> filteredHeaders = [.. result.Headers.Where(header => PathUtils.IsDatabaseWhitelisted(header.Code, databaseWhitelist))];
                _logger.LogDebug("data-bases/{DbPath} result: {Result}", dbPath, result);
                return new DatabaseGroupContents(filteredHeaders, result.Files);
            }

            _logger.LogDebug("data-bases/{DbPath} result: {Result}", dbPath, result);
            return result;
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
        public async Task<ActionResult<IReadOnlyMatrixMetadata>> GetCubeMetaAsync([FromRoute] string tablePath)
        {
            _logger.LogDebug("Requested cube meta for {TablePath} GET: api/creation/cube-meta", tablePath);

            PxTableReference tableReference = new(tablePath, '/');
            if (!PathUtils.IsDatabaseWhitelisted(tableReference.Hierarchy, databaseWhitelist))
            {
                _logger.LogInformation("Database {TablePath} is not whitelisted.", tablePath);
                return NotFound();
            }

            IReadOnlyMatrixMetadata readOnlyMeta = await _datasource.GetMatrixMetadataCachedAsync(tableReference);
            if (readOnlyMeta == null)
            {
                _logger.LogWarning("cube-meta result or {TablePath}: null. May result from a dimension without values.", tablePath);
                return BadRequest();
            }
            MatrixMetadata metadataClone = readOnlyMeta.GetTransform(readOnlyMeta);
            _logger.LogDebug("cube-meta result {Meta}", metadataClone);
            return metadataClone;
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
        public async Task<ActionResult<TableMetaValidationResult>> ValidateTableMetaData([FromRoute] string tablePath)
        {
            _logger.LogDebug("Validating table metadata for {TablePath} GET: api/creation/validate-table-metadata", tablePath);

            PxTableReference tableReference = new(tablePath, '/');
            if (!PathUtils.IsDatabaseWhitelisted(tableReference.Hierarchy, databaseWhitelist))
            {
                _logger.LogInformation("Database {TablePath} is not whitelisted.", tablePath);
                return NotFound();
            }

            IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(tableReference);
            if (tableMeta is null)
            {
                return new TableMetaValidationResult(
                    tableHasContentDimension: false,
                    tableHasTimeDimension: false,
                    allDimensionsContainValues: false
                );
            }
            else
            {
#nullable enable
                IReadOnlyDimension? contDim = tableMeta.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Content); 
                IReadOnlyDimension? timeDim = tableMeta.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Time);
                return new TableMetaValidationResult(
                    contDim is not null && contDim.Values.Count > 0,
                    timeDim is not null && timeDim.Values.Count > 0,
                    tableMeta.Dimensions.All(v => v.Values.Count > 0)
                );
#nullable disable
            }
        }

        /// <summary>
        /// Returns a list of values for a given dimension using the provided filter.
        /// </summary>
        /// <param name="filterRequest">Filter request object containing the table reference and the filter.</param>
        /// <returns>Dictionary with dimension codes as keys and their value codes as values</returns>
        [HttpPost("filter-dimension")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> GetDimensionFilterResultAsync([FromBody] FilterRequest filterRequest)
        {
            _logger.LogDebug("Requesting filter result for {FilterRequest} POST: api/creation/filter-dimension", filterRequest);
            IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(filterRequest.TableReference);

            Dictionary<string, List<string>> result = filterRequest.Filters.ToDictionary(
                filter => filter.Key,
                filter =>
                {
                    if (tableMeta.Dimensions.FirstOrDefault(dimension => dimension.Code == filter.Key) is IReadOnlyDimension dimension)
                    {
                        IEnumerable<IReadOnlyDimensionValue> filteredValues = filter.Value.Filter(dimension.Values);
                        List<string> filteredValueCodes = [.. filteredValues.Select(value => value.Code)];
                        _logger.LogDebug("filter-dimension result: {FilteredValueCodes}", filteredValueCodes);
                        return filteredValueCodes;
                    }
                    else
                    {
                        _logger.LogDebug("filter-dimension result: []");
                        return [];
                    }
                }
            );

            return result;
        }

        /// <summary>
        /// Provides information required for the editor to set up the visualization.
        /// </summary>
        /// <remarks>If the query doesn't contain any dimension queries or the resulting matrix is empty, an empty response is returned.</remarks>
        /// <param name="query">Query object containing the table reference and dimension queries.</param>
        /// <returns><see cref="EditorContentsResponse"/> object that contains the size of the resulting matrix, maximum supported size, header text, maximum header length, visualization options for accepted visualization types and rejection reasons.</returns>
#nullable enable
        [HttpPost("editor-contents")]
        public async Task<ActionResult<EditorContentsResponse>> GetEditorContents([FromBody] MatrixQuery query)
        {
            _logger.LogDebug("GetEditorContents called with {Query} POST: api/creation/query-info", query);
            int maxQuerySize = Configuration.Current.QueryOptions.MaxQuerySize;

            if (query.DimensionQueries.Count == 0)
            {
                return new EditorContentsResponse()
                {
                    Size = 0,
                    MaximumSupportedSize = maxQuerySize,
                    SizeWarningLimit = Convert.ToInt32(maxQuerySize * 0.75),
                    HeaderText = new MultilanguageString(Configuration.Current.LanguageOptions.Available.Select(lang => new KeyValuePair<string, string>(lang, string.Empty))),
                    MaximumHeaderLength = Configuration.Current.QueryOptions.MaxHeaderLength,
                    VisualizationOptions = [],
                    VisualizationRejectionReasons = []
                };
            }
            
            IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(query.TableReference);
            IReadOnlyMatrixMetadata filteredMeta = tableMeta.FilterDimensionValues(query);
            int includedValuesCount = filteredMeta.Dimensions.Select(x => x.Values.Count).Aggregate((a, x) => a * x);

            if (includedValuesCount == 0)
            {
                return new EditorContentsResponse()
                {
                    Size = 0,
                    MaximumSupportedSize = maxQuerySize,
                    SizeWarningLimit = Convert.ToInt32(maxQuerySize * 0.75),
                    HeaderText = new MultilanguageString(Configuration.Current.LanguageOptions.Available.Select(lang => new KeyValuePair<string, string>(lang, string.Empty))),
                    MaximumHeaderLength = Configuration.Current.QueryOptions.MaxHeaderLength,
                    VisualizationOptions = [],
                    VisualizationRejectionReasons = []
                };
            }

            Matrix<DecimalDataValue> matrix = await _datasource.GetMatrixCachedAsync(query.TableReference, filteredMeta);

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
                    rejectionReasons[reasonKvp.Key] = new (translations);
                }
                else  // no rejection reasons == valid visualization type
                {
                    visualizationOptions.Add(GetVisualizationOption(reasonKvp.Key, filteredMeta, query));
                }
            }

            return new EditorContentsResponse()
            {
                Size = includedValuesCount,
                MaximumSupportedSize = maxQuerySize,
                SizeWarningLimit = Convert.ToInt32(maxQuerySize * 0.75),
                HeaderText = HeaderBuildingUtilities.GetHeader(filteredMeta, query, true),
                MaximumHeaderLength = Configuration.Current.QueryOptions.MaxHeaderLength,
                VisualizationOptions = visualizationOptions,
                VisualizationRejectionReasons = rejectionReasons
            };
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
        public async Task<ActionResult<VisualizationResponse>> GetVisualizationAsync([FromBody] ChartRequest request)
        {
            _logger.LogDebug("Requesting visualization for {Request} POST: api/creation/visualization", request);
            IReadOnlyMatrixMetadata completeMeta = await _datasource.GetMatrixMetadataCachedAsync(request.Query.TableReference);
            IReadOnlyMatrixMetadata filteredMeta = completeMeta.FilterDimensionValues(request.Query);
            
            // The resulting cube would have volume 0
            if (filteredMeta.Dimensions.Any(d => d.Values.Count == 0))
            {
                _logger.LogDebug("visualization result: One or more dimensions have no included values. {FilteredMeta}", filteredMeta);
                return BadRequest();
            }

            Matrix<DecimalDataValue> matrix = await _datasource.GetMatrixCachedAsync(request.Query.TableReference, filteredMeta);
            IChartTypeSelector selector = ChartTypeSelector.Selector;

            if (selector.GetValidChartTypes(request.Query, matrix).Contains(request.VisualizationSettings.SelectedVisualization))
            {
                VisualizationSettings vSettings =
                    request.VisualizationSettings.ToVisualizationSettings(filteredMeta, request.Query);

                VisualizationResponse visualizationResponse = PxVisualizerCubeAdapter.BuildVisualizationResponse(matrix, request.Query, vSettings);
                _logger.LogDebug("visualization result: {VisualizationResponse}", visualizationResponse);

                return visualizationResponse;
            }
            else
            {
                _logger.LogWarning("visualization result: Selected visualization is not valid for this query.");
                return BadRequest();
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
