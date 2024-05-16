using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Exceptions;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface.SerializationModels;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.PxWebInterface
{
    [ExcludeFromCodeCoverage] // HTTP client object can not be mocked with reasonable effort
    public class PxWebV1ApiInterface : IPxWebApiInterface
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<PxWebV1ApiInterface> _logger;
        private const int RetryDelayMs = 2000;
        private const int RetryMaxTryCount = 10;


        public PxWebV1ApiInterface(IHttpClientFactory clientFactory, ILogger<PxWebV1ApiInterface> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IReadOnlyCubeMeta> GetPxTableMetaAsync(PxFileReference tableReference, IEnumerable<string> languages = null)
        {
            IEnumerable<string> langOptions = languages ?? Configuration.Current.LanguageOptions.Available;
            CubeMeta resultMeta = new();

            List<string> succeededLanguages = new();
            foreach (var lang in langOptions)
            {
                PxMetaResponse response = await GetPxWebMetaRespInLanguageAsync(lang, tableReference);
                if (response != null)
                {
                    succeededLanguages.Add(lang);
                    resultMeta.AppendPxWebMetaData(lang, response);
                }
            }

            if (succeededLanguages.Count == 0)
            {
                throw new TableMetadataException("Could not load table with any language.");
            }

            if (resultMeta.Variables.Exists(v => v.IncludedValues.Count == 0))
            {
                _logger.LogWarning("PxWeb: GetPxTableMetaAsync failed because one of the variables has no values. pxFile: {PxFile}, referenceMeta: {ReferenceMeta}, languages: {Languages}.", tableReference, resultMeta, langOptions);
                return null;
            }

            resultMeta = await FillVariableTypesAsync(resultMeta, succeededLanguages, tableReference);

            return await FillVariableValuesAsync(resultMeta, succeededLanguages, tableReference);
        }

        public async Task<DataCube> GetPxTableDataAsync(PxFileReference tableReference, IReadOnlyCubeMeta table)
        {
            string language = table.Languages[0]; // Language does not matter here but it must to be valid
            var queries = table.Variables.Select(v => BuildVariableQuery(v.Code, v.IncludedValues.Select(vv => vv.Code).ToArray()));

            var postParams = BuildPostParams(queries);
            JsonStat2 dataResult = await GetPxWebDataResponseAsync<JsonStat2>(language, tableReference, postParams);

            var values = dataResult.Value;

            var dataValues = new DataValue[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                if (value == null)
                {
                    var statusCode = dataResult.Status?.GetValueOrDefault(i.ToString());
                    dataValues[i] = DataValue.FromDotCode(statusCode, DataValueType.Missing);
                }
                else
                {
                    dataValues[i] = DataValue.FromRaw(value.Value);
                }
            }

            var variables = new List<VariableMap>();
            foreach (var dimensionId in dataResult.Id)
            {
                var dimension = dataResult.Dimensions[dimensionId];

                var variableValueCodes = new string[dimension.Category.Index.Count];

                foreach (var p in dimension.Category.Index)
                {
                    var dimensionValueCode = p.Key;
                    var dimensionValueIndex = p.Value;

                    variableValueCodes[dimensionValueIndex] = dimensionValueCode;
                }

                variables.Add(new VariableMap(dimensionId, variableValueCodes));
            }

            //Assert that loaded variables match to requested meta
            Debug.Assert(variables.Count == table.Variables.Count);
            for (var v = 0; v < table.Variables.Count; v++)
            {
                var jsonStatVariable = variables[v];
                var cubeMetaVariable = table.Variables[v];
                Debug.Assert(jsonStatVariable.Code == cubeMetaVariable.Code);
                Debug.Assert(jsonStatVariable.ValueCodes.Count == cubeMetaVariable.IncludedValues.Count);

                for (var vv = 0; vv < cubeMetaVariable.IncludedValues.Count; vv++)
                {
                    Debug.Assert(jsonStatVariable.ValueCodes[vv] == cubeMetaVariable.IncludedValues[vv].Code);
                }
            }

            var newTable = table.GetTransform(variables);
            return new DataCube(newTable, dataValues);
        }

        /// <summary>
        /// Fetch a list of databases available in the given language from the PxWeb server.
        /// </summary>
        public async Task<List<DataBaseListResponseItem>> GetDataBaseListingAsync(string lang)
        {
            _logger.LogDebug("PxWeb GET: api/v1/{Lang}/", lang);
            var resp = await GetAsync($"api/v1/{lang}/");
            if (resp.IsSuccessStatusCode)
            {
                string json = await resp.Content.ReadAsStringAsync();
                _logger.LogDebug("PxWeb: api/v1/{Lang}/ result: {Json}", lang, json);
                return JsonConvert.DeserializeObject<List<DataBaseListResponseItem>>(json);
            }
            else
            {
                _logger.LogWarning("PxWeb: api/v1/{Lang}/ failed with status code {StatusCode}", lang, resp.StatusCode);
            }
            return new List<DataBaseListResponseItem>();

        }

        /// <summary>
        /// Fetch a list of directories in a database in the given language from the PxWeb server.
        /// </summary>
        public async Task<List<TableListResponseItem>> GetTableItemListingAsync(string lang, IReadOnlyList<string> path)
        {
            string joinedPath = string.Join("/", path);
            _logger.LogDebug("PxWeb GET: api/v1/{Lang}/{JoinedPath}/", lang, joinedPath);
            var resp = await GetAsync($"api/v1/{lang}/{joinedPath}/");
            if (resp.IsSuccessStatusCode)
            {
                string json = await resp.Content.ReadAsStringAsync();
                _logger.LogDebug("PxWeb: api/v1/{Lang}/{JoinedPath}/ result: {Json}", lang, joinedPath, json);
                return JsonConvert.DeserializeObject<List<TableListResponseItem>>(json);
            }
            else
            {
                _logger.LogWarning("PxWeb: api/v1/{Lang}/ failed with status code {StatusCode}", lang, resp.StatusCode);
            }
            return new List<TableListResponseItem>();
        }

        /// <summary>
        /// Requests minimal amount of data from the PxWeb api in order to determine when the requested table has been updated.
        /// </summary>
        public async Task<string> GetPxFileUpdateTimeAsync(PxFileReference pxFile, IReadOnlyCubeMeta referenceMeta)
        {
            string defaultLanguage = Configuration.Current.LanguageOptions.Default;
            string language = referenceMeta.Languages.Contains(defaultLanguage) ? defaultLanguage : referenceMeta.Languages[0];
            JsonStat2 jsonStat2 = await GetSinglePointJsonStat2RespAsync(pxFile, referenceMeta, language);
            return jsonStat2.Updated;
        }

        private async Task<JsonStat2> GetSinglePointJsonStat2RespAsync(PxFileReference pxFile, IReadOnlyCubeMeta referenceMeta, string language)
        {
            if (referenceMeta.Variables.Any(v => v.IncludedValues.Count == 0))
            {
                _logger.LogWarning("PxWeb: GetSinglePointJsonStat2RespAsync failed because one of the variables has no values. pxFile: {PxFile}, referenceMeta: {ReferenceMeta}, language: {Language}.", pxFile, referenceMeta, language);
                return null;
            }
            var queries = referenceMeta.Variables.Select(v => BuildVariableQuery(v.Code, new string[] { v.IncludedValues[0].Code }));
            var postParams = BuildPostParams(queries);
            var jsonStat2 = await GetPxWebDataResponseAsync<JsonStat2>(language, pxFile, postParams);
            FixPxWebBrokenDateFormat(jsonStat2);
            return jsonStat2;
        }

        private async Task<CubeMeta> FillVariableTypesAsync(CubeMeta resultMeta, List<string> succeededLanguages, PxFileReference tableReference)
        {
            JsonStat2 response = await GetSinglePointJsonStat2RespAsync(tableReference, resultMeta, succeededLanguages[0]);
            resultMeta.SetVariableTypes(response);
            return resultMeta;
        }

        private async Task<CubeMeta> FillVariableValuesAsync(CubeMeta resultMeta, List<string> succeededLanguages, PxFileReference tableReference)
        {
            var queries = BuildMinimalContentVariableQueries(resultMeta.Variables);
            var postParams = BuildPostParams(queries);

            var dataResultsByLanguage = new Dictionary<string, JsonStat2>();
            foreach (var language in succeededLanguages) // OBS! This is only for getting the unit names in all languages! Needs to be changes once the pxweb v2 api is live.
            {
                JsonStat2 dataResult = await GetPxWebDataResponseAsync<JsonStat2>(language, tableReference, postParams);
                FixPxWebBrokenDateFormat(dataResult);

                dataResultsByLanguage.Add(language, dataResult);
            }

            resultMeta.AppendPxWebDataResult(dataResultsByLanguage);

            return resultMeta;
        }

        private static PxWebDataQueryPostParams BuildPostParams(IEnumerable<PxWebDataQueryPostParams.VariableQuery> queries)
        {
            return new PxWebDataQueryPostParams()
            {
                Query = queries.ToArray(),
                Response = new PxWebDataQueryPostParams.ResponseInfo()
                {
                    Format = "json-stat2"
                }
            };
        }

        private static IEnumerable<PxWebDataQueryPostParams.VariableQuery> BuildMinimalContentVariableQueries(IEnumerable<Variable> variables)
        {
            foreach (var variable in variables)
            {
                string[] selectedValues;
                if (variable.Type == Enums.VariableType.Content)
                {
                    selectedValues = variable.IncludedValues.Select(vv => vv.Code).ToArray();
                }
                else
                {
                    selectedValues = new string[] { variable.IncludedValues[0].Code };
                }

                yield return BuildVariableQuery(variable.Code, selectedValues);
            }
        }

        private static PxWebDataQueryPostParams.VariableQuery BuildVariableQuery(string variableCode, string[] variableValues)
        {
            return new PxWebDataQueryPostParams.VariableQuery()
            {
                Code = variableCode,
                Selectrion = new PxWebDataQueryPostParams.VariableQuery.Selection()
                {
                    Filter = "item",
                    Values = variableValues,
                }
            };
        }

        /// <returns>PxMetaResponse or null if language does not match.</returns>
        /// <exception cref="Exception"></exception>
        private async Task<PxMetaResponse> GetPxWebMetaRespInLanguageAsync(string lang, PxFileReference pxTableRef)
        {
            string tableRefPath = pxTableRef.ToPath();
            _logger.LogDebug("PxWeb GET: api/v1/{Lang}/{TableRefPath}", lang, tableRefPath);
            var resp = await GetAsync($"api/v1/{lang}/{tableRefPath}");
            string contentString = await resp.Content.ReadAsStringAsync();

            if (resp.IsSuccessStatusCode)
            {
                _logger.LogDebug("PxWeb: api/v1/{Lang}/{TableRefPath} result: {ContentString}", lang, tableRefPath, contentString);
                return JsonConvert.DeserializeObject<PxMetaResponse>(contentString);
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                if (contentString == "Parameter error") // Can be due table is not found (for given language)
                {
                    _logger.LogWarning("PxWeb: api/v1/{Lang}/{TableRefPath} could not find table for given language", lang, tableRefPath);
                    return null;
                }

                _logger.LogWarning("PxWeb: api/v1/{Lang}/{TableRefPath} failed with status code {StatusCode}: {Error}", lang, tableRefPath, resp.StatusCode, contentString);
                return null;
            }

            _logger.LogWarning("PxWeb: api/v1/{Lang}/{TableRefPath} failed with status code {StatusCode}: {ContentString}", lang, tableRefPath, resp.StatusCode, contentString);
            throw new BadPxWebResponseException(resp.StatusCode, $"PxWeb responded {resp.StatusCode}: {contentString}");
        }

        private async Task<ResponseType> GetPxWebDataResponseAsync<ResponseType>(string lang, PxFileReference pxTableRef, PxWebDataQueryPostParams query)
        {
            string tableRefPath = pxTableRef.ToPath();
            _logger.LogDebug("PxWeb POST: api/v1/{Lang}/{TableRefPath} with query: {Query}", lang, tableRefPath, query);
            var resp = await PostAsync($"api/v1/{lang}/{tableRefPath}", JsonConvert.SerializeObject(query));
            if (resp.IsSuccessStatusCode)
            {
                string json = await resp.Content.ReadAsStringAsync();
                _logger.LogDebug("PxWeb: api/v1/{Lang}/{TableRefPath} result: {Json}", lang, tableRefPath, json);
                return JsonConvert.DeserializeObject<ResponseType>(json);
            }
            else
            {
                _logger.LogWarning("PxWeb: api/v1/{Lang}/{TableRefPath} failed with query {Query}", lang, tableRefPath, query);
                throw new BadPxWebResponseException(resp.StatusCode, $"PxWeb responded with a status code {resp.StatusCode}.");
            }
        }

        private async Task<HttpResponseMessage> GetAsync(string path, string queryString = "")
        {
            string basePath = Configuration.Current.PxWebUrl;
            if (!basePath.EndsWith('/')) basePath += "/";
            string query = path + queryString;
            using var client = _clientFactory.CreateClient(Startup.PXWEBCLIENTNAME);
            client.BaseAddress = new Uri(basePath);

            return await WithRetry(() => Get());

            async Task<HttpResponseMessage> Get()
            {
                var response = await client.GetAsync(query, HttpCompletionOption.ResponseContentRead);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    throw new TooManyRequestsException();
                }
                else
                {
                    return response;
                }
            }
        }

        private async Task<HttpResponseMessage> PostAsync(string path, string json)
        {
            string basePath = Configuration.Current.PxWebUrl;
            if (!basePath.EndsWith('/')) basePath += "/";
            using var client = _clientFactory.CreateClient(Startup.PXWEBCLIENTNAME);

            client.BaseAddress = new Uri(basePath);

            return await WithRetry(() => Post());

            async Task<HttpResponseMessage> Post()
            {
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(path, data);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    throw new TooManyRequestsException();
                }
                else
                {
                    return response;
                }
            }
        }

        private static async Task<R> WithRetry<R>(Func<Task<R>> func)
        {
            for (var i = 0; i < RetryMaxTryCount; i++)
            {
                try
                {
                    return await func();
                }
                catch (TooManyRequestsException)
                {
                    await Task.Delay(RetryDelayMs);
                }
            }

            throw new TooManyRequestsException("Max retry count reached.");
        }

        /*
          TODO: Fix original bug in PxWeb and remove this workaround.
          
          Currently PxWeb may return dates at yyyy-MM-ddTHH.mm.ssZ format while ISO-8601 is clearly intended: yyyy-MM-ddTHH:mm:ssZ
          Bug may originate for something like: dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ") or similiar.
          Point there is that while "-", "T", "Z" are just meaningless separators that will come to final output as is, ":" IS NOT!
          ":" is LOCALE DEPENDENT TIME SEPARATOR
            en-US -> ":"
            sv-SE -> ":"
            fi-FI -> "." !!!
          
          To actually get ":" locale independent way, either fixed culture must be used or it must be quoted like "HH':'mm"
        */
        private static void FixPxWebBrokenDateFormat(JsonStat2 dataResult)
        {
            if (DateTime.TryParseExact(dataResult.Updated, "yyyy'-'MM'-'dd'T'HH'.'mm'.'ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsedBuggyDate))
            {
                dataResult.Updated = parsedBuggyDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture);
            }

            if (dataResult.Updated != null && !DateTime.TryParse(dataResult.Updated, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _))
            {
                throw new ArgumentException($"Invalid date format: {dataResult.Updated}");
            }
        }
    }
}
