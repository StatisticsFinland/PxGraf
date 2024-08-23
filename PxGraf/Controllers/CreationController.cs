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
using PxGraf.Data.MetaData;
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
using PxGraf.Utility;
using PxGraf.Visualization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace PxGraf.Controllers
{
    [FeatureGate("CreationAPI")]
    [ApiController]
    [Route("api/creation")]
    public class CreationController(ICachedDatasource datasource, ILogger<CreationController> logger) : ControllerBase
    {
        private readonly ICachedDatasource _datasource = datasource;
        private readonly ILogger<CreationController> _logger = logger;

        /// <summary>
        /// Returns a list of database items from the given level of the data base based on the dbPath.
        /// </summary>
        /// <param name="dbPath">Path to the database.</param>
        /// <returns>List of <see cref="DataBaseListingItem"/> objects.</returns>
        [HttpGet("data-bases/{*dbPath}")]
        public async Task<DatabaseGroupContents> GetDataBaseListingAsync([FromRoute] string dbPath)
        {
            const string LOGMSG = "Getting the database listing. GET: api/creation/data-bases/{DbPath}";
            _logger.LogDebug(LOGMSG, dbPath);

            string[] hierarchy = dbPath is not null ? dbPath.Split("/") : [];
            // Find databases and tables with all available languages and add them to the response
            DatabaseGroupContents result = await _datasource.GetGroupContentsCachedAsync(hierarchy);

            _logger.LogDebug("data-bases/{DbPath} result: {Result}", dbPath, result);
            return result;
        }

        /// <summary>
        /// Returns all metadata from the table
        /// </summary>
        /// <param name="tablePath">Path to the table</param>
        /// <returns><see cref="CubeMeta"/> object that represents the metadata for the table</returns>
        [HttpGet("cube-meta/{*tablePath}")]
        public async Task<ActionResult<IReadOnlyMatrixMetadata>> GetCubeMetaAsync([FromRoute] string tablePath)
        {
            _logger.LogDebug("Requested cube meta for {TablePath} GET: api/creation/cube-meta", tablePath);
            IReadOnlyMatrixMetadata readOnlyMeta = await _datasource.GetMatrixMetadataCachedAsync(new(tablePath, '/'));
            if (readOnlyMeta == null)
            {
                _logger.LogWarning("cube-meta result or {TablePath}: null. May result from a dimension without values.", tablePath);
                return BadRequest();
            }
            _logger.LogDebug("cube-meta result {Meta}", readOnlyMeta);
            string json = JsonConvert.SerializeObject(readOnlyMeta, new MatrixMetadataConverter());
            return Content(json, "application/json");
        }

        /// <summary>
        /// Validates whether the given table has required metadata for visualization.
        /// </summary>
        /// <param name="tablePath">Path to the table.</param>
        /// <returns><see cref="TableMetaValidationResult"/> object that contains info about whether the table's metadata is sufficient for visualization.</returns>
        [HttpGet("validate-table-metadata/{*tablePath}")]
        public async Task<TableMetaValidationResult> ValidateTableMetaData([FromRoute] string tablePath)
        {
            _logger.LogDebug("Validating table metadata for {TablePath} GET: api/creation/validate-table-metadata", tablePath);
            IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(new(tablePath, '/'));
            if (tableMeta is null)
            {
                return new(
                    tableHasContentVariable: false,
                    tableHasTimeVariable: false,
                    allVariablesContainValues: false
                );
            }
            else
            {
                return new(
                    tableMeta.Dimensions.Any(v => v.Type == DimensionType.Content),
                    tableMeta.Dimensions.Any(v => v.Type == DimensionType.Time),
                    tableMeta.Dimensions.All(v => v.Values.Count > 0)
                );
            }
        }

        /// <summary>
        /// Returns a list of values for a given dimension using the provided filter.
        /// </summary>
        /// <param name="filterRequest">Filter request object containing the table reference and the filter.</param>
        /// <returns>Dictionary with dimension codes as keys and their value codes as values</returns>
        [HttpPost("filter-dimension")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> GetVariableFilterResultAsync([FromBody] FilterRequest filterRequest)
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
                        List<string> filteredValueCodes = filteredValues.Select(value => value.Code).ToList();
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

        [HttpPost("default-header")]
        public async Task<ActionResult<MultilanguageString>> GetDefaultHeaderAsync([FromBody] MatrixQuery input)
        {
            _logger.LogDebug("Requesting default header for {Input} POST: api/creation/default-header", input);
            IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(input.TableReference);
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(tableMeta.Dimensions, null, tableMeta.AvailableLanguages);

            _logger.LogDebug("default-header result: {Header}", header);
            return header;
        }

        /// <summary>
        /// Returns a list of valid visualization type names.
        /// </summary>
        /// <param name="cubeQuery"><see cref="MatrixQuery"/> object containing the table reference and the query.</param>
        /// <returns>List of valid visualization type names.</returns>
        [HttpPost("valid-visualizations")]
        public async Task<ActionResult<List<string>>> GetValidVisualizationTypesAsync([FromBody] MatrixQuery cubeQuery)
        {
            _logger.LogDebug("Requesting valid visualizations for {CubeQuery} POST: api/creation/valid-visualization", cubeQuery);
            IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(cubeQuery.TableReference);
            IReadOnlyMatrixMetadata filteredMeta = tableMeta.FilterDimensionValues(cubeQuery);

            Matrix<DecimalDataValue> matrix = await _datasource.GetMatrixCachedAsync(cubeQuery.TableReference, filteredMeta);

            IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(cubeQuery, matrix);
            List<string> validTypesList = validTypes.Select(ChartTypeEnumConverter.ToJsonString).ToList();
            _logger.LogDebug("valid-visualizations result: {ValidTypesList}", validTypesList);
            return validTypesList;
        }

        /// <summary>
        /// Returns information about valid and rejected visualization types and query size and size limits.
        /// </summary>
        /// <param name="cubeQuery"><see cref="MatrixQuery"/> object containing the table reference and the query.</param>
        /// <returns><see cref="QueryInfoResponse"/> object that contains information about valid visualization type, reasons for rejected visualization types, query size and limits for header length and query size.</returns>
        [HttpPost("query-info")]
        public async Task<ActionResult<QueryInfoResponse>> GetQueryInfoAsync([FromBody] MatrixQuery cubeQuery)
        {
            _logger.LogDebug("Requesting query info for {CubeQuery} POST: api/creation/query-info", cubeQuery);
            int maxQuerySize = Configuration.Current.QueryOptions.MaxQuerySize;
            int maxHeaderLength = Configuration.Current.QueryOptions.MaxHeaderLength;

            IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(cubeQuery.TableReference);
            IReadOnlyMatrixMetadata filteredMeta = tableMeta.FilterDimensionValues(cubeQuery);

            int includedValuesCount = filteredMeta.Dimensions.Select(x => x.Values.Count).Aggregate((a, x) => a * x);
            if (includedValuesCount == 0 || includedValuesCount > maxQuerySize)
            {
                QueryInfoResponse queryInfoResponse = new ()
                {
                    Size = includedValuesCount,
                    SizeWarningLimit = Convert.ToInt32(maxQuerySize * 0.75),
                    MaximumSupportedSize = maxQuerySize,
                    MaximumHeaderLength = maxHeaderLength
                };

                _logger.LogDebug("query-info result: {QueryInfoResponse}", queryInfoResponse);
                return queryInfoResponse;
            }

            Matrix<DecimalDataValue> matrix = await _datasource.GetMatrixCachedAsync(cubeQuery.TableReference, filteredMeta);

            QueryInfoResponse response = new()
            {
                Size = matrix.Data.Length,
                SizeWarningLimit = Convert.ToInt32(maxQuerySize * 0.75),
                MaximumSupportedSize = maxQuerySize,
                MaximumHeaderLength = maxHeaderLength
            };

            foreach (KeyValuePair<VisualizationType, IReadOnlyList<ChartRejectionInfo>> reasonKvp in
                ChartTypeSelector.Selector.GetRejectionReasons(cubeQuery, matrix))
            {
                if (reasonKvp.Value.Any())
                {
                    Dictionary<string, string> translations = [];
                    foreach (var language in Localization.GetAllAvailableLanguages())
                    {
                        string textInLang = Localization.FromLanguage(language).Translation.RejectionReasons.GetTranslation(reasonKvp.Value[0]);
                        translations[language] = textInLang;
                    }
                    response.VisualizationRejectionReasons[reasonKvp.Key] = new (translations);
                }
                else  // no rejection reasons == valid visualization type
                {
                    response.ValidVisualizations.Add(reasonKvp.Key);
                }
            }
            _logger.LogDebug("query-info result: {Response}", response);
            return response;
        }

        /// <summary>
        /// Returns visualization rules for the given query and selected visualization type.
        /// </summary>
        /// <param name="rulesQuery"><see cref="VisualizationSettingsRequest"/> object that contains the selected visualization type and settings for the query.</param>
        /// <returns><see cref="VisualizationRules"/> object that contains rules for which settings are available for the given visualization query.</returns>
        [HttpPost("visualization-rules")]
        public async Task<ActionResult<VisualizationRules>> GetVisualizationRulesAsync([FromBody] VisualizationSettingsRequest rulesQuery)
        {
            _logger.LogDebug("Requesting visualization rules for {RulesQuery} POST: api/creation/visualization-rules", rulesQuery);
            IReadOnlyMatrixMetadata tableMeta = await _datasource.GetMatrixMetadataCachedAsync(rulesQuery.Query.TableReference);
            IReadOnlyMatrixMetadata filteredMeta = tableMeta.FilterDimensionValues(rulesQuery.Query);
            
            // All dimensions must have atleast one value selected
            if (!filteredMeta.Dimensions.All(v => v.Values.Count != 0))
            {
                _logger.LogDebug("visualization-rules result: All dimensions must have atleast one value selected.");
                return new VisualizationRules(false, false);
            }

            Matrix<DecimalDataValue> dataCube = await _datasource.GetMatrixCachedAsync(rulesQuery.Query.TableReference, filteredMeta);

            IChartTypeSelector selector = ChartTypeSelector.Selector;
            IReadOnlyList<VisualizationType> validTypes = selector.GetValidChartTypes(rulesQuery.Query, dataCube);

            if (!validTypes.Contains(rulesQuery.SelectedVisualization))
            {
                // Not possible to determine valid settings if the selected visualization is not valid for this query.
                _logger.LogDebug("visualization-rules result: Selected visualization is not valid for this query.");
                return new VisualizationRules(false, false);
            }

            bool manualPivotability = ManualPivotRules.GetManualPivotability(rulesQuery.SelectedVisualization, filteredMeta, rulesQuery.Query);
            bool multiSelVar = IsMultivalueSelectableAllowed(rulesQuery.SelectedVisualization, rulesQuery.Query.DimensionQueries.Values);
            IReadOnlyList<SortingOption> sortingOptions = CubeSorting.Get(filteredMeta, rulesQuery);
            VisualizationRules.TypeSpecificVisualizationRules typeSpecificRules = new (rulesQuery.SelectedVisualization);
            VisualizationRules visualizationRules = new (manualPivotability, multiSelVar, typeSpecificRules, sortingOptions);
            _logger.LogDebug("visualization-rules result: {VisualizationRules}", visualizationRules);
            return visualizationRules;

            static bool IsMultivalueSelectableAllowed(VisualizationType type, IEnumerable<DimensionQuery> varQueries)
            {
                // MultiselectVariable only allowed for LineChart for now. Will be extended to Table in the future.
                return (type == VisualizationType.LineChart)
                    && varQueries.Any(vq => vq.Selectable);
            }
        }

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
    }
}
