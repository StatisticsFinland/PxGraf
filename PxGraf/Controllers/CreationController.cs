using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using PxGraf.ChartTypeSelection;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.PxWebInterface;
using PxGraf.PxWebInterface.SerializationModels;
using PxGraf.Settings;
using PxGraf.Utility;
using PxGraf.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Handles API requests for chart creation by providing endpoints for fetching and validating data, determining valid visualization types, and creating visualizations. Interacts with the PxWeb API through a cached connection.
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    /// <param name="cachedPxWebConnection">Instance of a <see cref="ICachedPxWebConnection"/> object. Used to interact with PxWeb API and cache data.</param>
    /// <param name="logger"><see cref="ILogger"/> instance used for logging.</param>
    [FeatureGate("CreationAPI")]
    [ApiController]
    [Route("api/creation")]
    public class CreationController(ICachedPxWebConnection cachedPxWebConnection, ILogger<CreationController> logger) : ControllerBase
    {
        private readonly ICachedPxWebConnection _cachedPxWebConnection = cachedPxWebConnection;
        private readonly ILogger<CreationController> _logger = logger;

        /// <summary>
        /// Returns a list of database items from the given level of the data base based on the dbPath.
        /// </summary>
        /// <param name="dbPath">Path to the database.</param>
        /// <param name="queryParameters">Parameters for the query request.</param>
        /// <returns>List of <see cref="DataBaseListingItem"/> objects.</returns>
        [HttpGet("data-bases/{*dbPath}")]
        public async Task<List<DataBaseListingItem>> GetDataBaseListingAsync([FromRoute] string dbPath, [FromQuery] Dictionary<string, string> queryParameters)
        {
            _logger.LogDebug("Getting database listing with parameters {QueryParameters} GET: api/creation/data-bases/{DbPath}", queryParameters, dbPath);

            string preferredLanguage = DataBaseListingUtilities.GetPreferredLanguage(queryParameters);

            // Get a list of available languages with the preferred language first
            List<string> languages = DataBaseListingUtilities.GetPrioritizedLanguages(preferredLanguage);

            Dictionary<string, DataBaseListingItem> response = [];

            // Find databases and tables with all available languages and add them to the response
            foreach (string language in languages)
            {
                if (string.IsNullOrEmpty(dbPath))
                {
                    IEnumerable<DataBaseListResponseItem> dataBases = await _cachedPxWebConnection.GetDataBaseListingAsync(language);
                    DataBaseListingUtilities.AddOrAppendDataBaseItems(response, dataBases, language);
                }
                else
                {
                    IEnumerable<TableListResponseItem> tables = await _cachedPxWebConnection.GetDataTableItemListingAsync(language, [.. dbPath.Split("/")]);
                    DataBaseListingUtilities.AddOrAppendTableItems(response, tables, language);
                }
            }

            _logger.LogDebug("data-bases/{DbPath} result: {Response}", dbPath, response);
            return [.. response.Values];
        }

        /// <summary>
        /// Returns a list of available languages for all databases.
        /// </summary>
        /// <returns>A List of available languages defined by the backend settings that are included in the databases.</returns>
        [HttpGet("languages")]
        public async Task<List<string>> GetLanguagesAsync()
        {
            _logger.LogDebug("Getting available languages. GET: api/creation/languages");
            List<string> validLanguages = [];
            // Checks if any database is available for the given languages
            foreach (string language in Configuration.Current.LanguageOptions.Available)
            {
                List<DataBaseListResponseItem> databases = await _cachedPxWebConnection.GetDataBaseListingAsync(language);
                if (databases != null)
                {
                    validLanguages.Add(language);
                }
            }
            _logger.LogDebug("languages result: {ValidLanguages}", validLanguages);
            return validLanguages;
        }

        /// <summary>
        /// Returns all metadata from the table
        /// </summary>
        /// <param name="tablePath">Path to the table</param>
        /// <returns><see cref="CubeMeta"/> object that represents the metadata for the table</returns>
        [HttpGet("cube-meta/{*tablePath}")]
        public async Task<ActionResult<CubeMeta>> GetCubeMetaAsync([FromRoute] string tablePath)
        {
            _logger.LogDebug("Requested cube meta for {TablePath} GET: api/creation/cube-meta", tablePath);
            IReadOnlyCubeMeta readOnlyMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(new(tablePath));
            if (readOnlyMeta == null)
            {
                _logger.LogWarning("cube-meta result or {TablePath}: null. May result from a variable without values.", tablePath);
                return null;
            }
            CubeMeta meta = readOnlyMeta.Clone();
            _logger.LogDebug("cube-meta result {Meta}", meta);
            return meta;
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
            IReadOnlyCubeMeta tableMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(new(tablePath));
            if (tableMeta is null)
            {
                return new(false, false, false);
            }
            else
            {
                return new(
                    tableMeta.Variables.Any(v => v.Type == VariableType.Content),
                    tableMeta.Variables.Any(v => v.Type == VariableType.Time),
                    tableMeta.Variables.All(v => v.IncludedValues.Count > 0)
                );
            }
        }

        /// <summary>
        /// Returns a list of values for a given variable using the provided filter.
        /// </summary>
        /// <param name="filterRequest">Filter request object containing the table reference and the filter.</param>
        /// <returns>Dictionary with variable codes as keys and their value codes as values</returns>
        [HttpPost("filter-variable")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> GetVariableFilterResultAsync([FromBody] FilterRequest filterRequest)
        {
            _logger.LogDebug("Requesting filter result for {FilterRequest} POST: api/creation/filter-variable", filterRequest);
            IReadOnlyCubeMeta tableMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(filterRequest.TableReference);

            Dictionary<string, List<string>> result = filterRequest.Filters.ToDictionary(
                filter => filter.Key,
                filter =>
                {
                    if (tableMeta.Variables.FirstOrDefault(variable => variable.Code == filter.Key) is Variable variable)
                    {
                        IEnumerable<IReadOnlyVariableValue> filteredValues = filter.Value.Filter(variable.IncludedValues);
                        List<string> filteredValueCodes = filteredValues.Select(value => value.Code).ToList();
                        _logger.LogDebug("filter-variable result: {FilteredValueCodes}", filteredValueCodes);
                        return filteredValueCodes;
                    }
                    else
                    {
                        _logger.LogDebug("filter-variable result: []");
                        return [];
                    }
                }
            );

            return result;
        }

        /// <summary>
        /// Returns a default header for a given query.
        /// </summary>
        /// <param name="input"><see cref="CubeQuery"/> object containing the table reference and the query.</param>
        /// <returns><see cref="MultiLanguageString"/> object that contains default headers for the available languages.</returns>
        [HttpPost("default-header")]
        public async Task<ActionResult<MultiLanguageString>> GetDefaultHeaderAsync([FromBody] CubeQuery input)
        {
            _logger.LogDebug("Requesting default header for {Input} POST: api/creation/default-header", input);
            IReadOnlyCubeMeta tableMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(input.TableReference);
            CubeMeta tableMetaCopy = tableMeta.Clone();
            tableMetaCopy.ApplyEditionFromQuery(input);
            tableMetaCopy.Header.Truncate(Configuration.Current.QueryOptions.MaxHeaderLength);

            _logger.LogDebug("default-header result: {Header}", tableMetaCopy.Header);
            return tableMetaCopy.Header;
        }

        /// <summary>
        /// Returns a list of valid visualization type names.
        /// </summary>
        /// <param name="cubeQuery"><see cref="CubeQuery"/> object containing the table reference and the query.</param>
        /// <returns>List of valid visualization type names.</returns>
        [HttpPost("valid-visualizations")]
        public async Task<ActionResult<List<string>>> GetValidVisualizationTypesAsync([FromBody] CubeQuery cubeQuery)
        {
            _logger.LogDebug("Requesting valid visualizations for {CubeQuery} POST: api/creation/valid-visualization", cubeQuery);
            IReadOnlyCubeMeta tableMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(cubeQuery.TableReference);
            CubeMeta tableMetaCopy = tableMeta.Clone();
            tableMetaCopy.ApplyEditionFromQuery(cubeQuery);

            if (tableMetaCopy.Variables.Exists(v => v.IncludedValues.Count == 0))
            {
                _logger.LogDebug("valid-visualizations result: []");
                return new List<string>();
            }

            DataCube dataCube = await _cachedPxWebConnection.GetDataCubeCachedAsync(cubeQuery.TableReference, tableMetaCopy);

            IReadOnlyList<VisualizationType> validTypes = ChartTypeSelector.Selector.GetValidChartTypes(cubeQuery, dataCube);
            List<string> validTypesList = validTypes.Select(ct => ChartTypeEnumConverter.ToJsonString(ct)).ToList();
            _logger.LogDebug("valid-visualizations result: {ValidTypesList}", validTypesList);
            return validTypesList;
        }

        /// <summary>
        /// Returns information about valid and rejected visualization types and query size and size limits.
        /// </summary>
        /// <param name="cubeQuery"><see cref="CubeQuery"/> object containing the table reference and the query.</param>
        /// <returns><see cref="QueryInfoResponse"/> object that contains information about valid visualization type, reasons for rejected visualization types, query size and limits for header length and query size.</returns>
        [HttpPost("query-info")]
        public async Task<ActionResult<QueryInfoResponse>> GetQueryInfoAsync([FromBody] CubeQuery cubeQuery)
        {
            _logger.LogDebug("Requesting query info for {CubeQuery} POST: api/creation/query-info", cubeQuery);
            int maxQuerySize = Configuration.Current.QueryOptions.MaxQuerySize;
            int maxHeaderLength = Configuration.Current.QueryOptions.MaxHeaderLength;

            IReadOnlyCubeMeta tableMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(cubeQuery.TableReference);
            CubeMeta tableMetaCopy = tableMeta.Clone();
            tableMetaCopy.ApplyEditionFromQuery(cubeQuery);

            int includedValuesCount = tableMetaCopy.Variables.Select(x => x.IncludedValues.Count).Aggregate((a, x) => a * x);
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

            DataCube dataCube = await _cachedPxWebConnection.GetDataCubeCachedAsync(cubeQuery.TableReference, tableMetaCopy);

            QueryInfoResponse response = new()
            {
                Size = dataCube.Data.Length,
                SizeWarningLimit = Convert.ToInt32(maxQuerySize * 0.75),
                MaximumSupportedSize = maxQuerySize,
                MaximumHeaderLength = maxHeaderLength
            };

            foreach (KeyValuePair<VisualizationType, IReadOnlyList<ChartRejectionInfo>> reasonKvp in ChartTypeSelector.Selector.GetRejectionReasons(cubeQuery, dataCube))
            {
                if (reasonKvp.Value.Any())
                {
                    response.VisualizationRejectionReasons[reasonKvp.Key] = new MultiLanguageString();
                    foreach (var language in Localization.GetAllAvailableLanguages())
                    {
                        string textInLang = Localization.FromLanguage(language).Translation.RejectionReasons.GetTranslation(reasonKvp.Value[0]);
                        response.VisualizationRejectionReasons[reasonKvp.Key].AddTranslation(language, textInLang);
                    }
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
            IReadOnlyCubeMeta tableMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(rulesQuery.Query.TableReference);
            CubeMeta tableMetaCopy = tableMeta.Clone();
            tableMetaCopy.ApplyEditionFromQuery(rulesQuery.Query);

            // All variables must have atleast one value selected
            if (!tableMetaCopy.Variables.TrueForAll(v => v.IncludedValues.Count != 0))
            {
                _logger.LogDebug("visualization-rules result: All variables must have atleast one value selected.");
                return new VisualizationRules(false, false);
            }

            DataCube dataCube = await _cachedPxWebConnection.GetDataCubeCachedAsync(rulesQuery.Query.TableReference, tableMetaCopy);

            IChartTypeSelector selector = ChartTypeSelector.Selector;
            IReadOnlyList<VisualizationType> validTypes = selector.GetValidChartTypes(rulesQuery.Query, dataCube);

            if (!validTypes.Contains(rulesQuery.SelectedVisualization))
            {
                // Not possible to determine valid settings if the selected visualization is not valid for this query.
                _logger.LogDebug("visualization-rules result: Selected visualization is not valid for this query.");
                return new VisualizationRules(false, false);
            }

            bool manualPivotability = ManualPivotRules.GetManualPivotability(rulesQuery.SelectedVisualization, tableMetaCopy, rulesQuery.Query);
            bool multiSelVar = IsMultivalueSelectableAllowed(rulesQuery.SelectedVisualization, rulesQuery.Query.VariableQueries.Values);
            IReadOnlyList<SortingOption> sortingOptions = CubeSorting.Get(tableMetaCopy, rulesQuery);
            VisualizationRules.TypeSpecificVisualizationRules typeSpecificRules = new (rulesQuery.SelectedVisualization);
            VisualizationRules visualizationRules = new (manualPivotability, multiSelVar, typeSpecificRules, sortingOptions);
            _logger.LogDebug("visualization-rules result: {VisualizationRules}", visualizationRules);
            return visualizationRules;

            static bool IsMultivalueSelectableAllowed(VisualizationType type, IEnumerable<VariableQuery> varQueries)
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
        /// If the request is missing values for any variable or the selected visualization type is not valid, "Bad Request" response is returned.
        /// </returns>
        [HttpPost("visualization")]
        public async Task<ActionResult<VisualizationResponse>> GetVisualizationAsync([FromBody] ChartRequest request)
        {
            _logger.LogDebug("Requesting visualization for {Request} POST: api/creation/visualization", request);
            IReadOnlyCubeMeta readOnlyTableMeta = await _cachedPxWebConnection.GetCubeMetaCachedAsync(request.Query.TableReference);
            CubeMeta tableMetaCopy = readOnlyTableMeta.Clone();
            tableMetaCopy.ApplyEditionFromQuery(request.Query);
            tableMetaCopy.Header.Truncate(Configuration.Current.QueryOptions.MaxHeaderLength);

            // The resulting cube would have volume 0
            if (tableMetaCopy.Variables.Exists(v => v.IncludedValues.Count == 0))
            {
                _logger.LogDebug("visualization result: One or more variables have no included values. {TableMetaCopy}", tableMetaCopy);
                return BadRequest();
            }

            DataCube dataCube = await _cachedPxWebConnection.GetDataCubeCachedAsync(request.Query.TableReference, tableMetaCopy);

            IChartTypeSelector selector = ChartTypeSelector.Selector;

            if (selector.GetValidChartTypes(request.Query, dataCube).Contains(request.VisualizationSettings.SelectedVisualization))
            {
                VisualizationSettings vSettings =
                    request.VisualizationSettings.ToVisualizationSettings(dataCube.Meta, request.Query);

                VisualizationResponse visualizationResponse = PxVisualizerCubeAdapter.BuildVisualizationResponse(dataCube, request.Query, vSettings);
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
