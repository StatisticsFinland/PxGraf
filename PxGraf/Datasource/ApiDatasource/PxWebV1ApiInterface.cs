using Microsoft.Extensions.Logging;
using Px.Utils.Language;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Data;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Datasource.ApiDatasource.SerializationModels;
using PxGraf.Datasource.PxWebInterface.SerializationModels;
using PxGraf.Datasource.PxWebInterface;
using PxGraf.Exceptions;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace PxGraf.Datasource.ApiDatasource
{
    public class PxWebV1ApiInterface(IPxWebConnection pxwebConnection, ILogger<PxWebV1ApiInterface> logger) : IApiDatasource
    {
        private readonly IPxWebConnection _pxwebConnection = pxwebConnection;
        private readonly ILogger<PxWebV1ApiInterface> _logger = logger;
        public const string LIST_ITEM_TYPE = "l";
        public const string TABLE_ITEM_TYPE = "t";

        public async Task<DatabaseGroupContents> GetDatabaseItemGroup(IReadOnlyList<string> groupHierarcy)
        {
            List<DatabaseGroupHeader> headers = [];
            List<DatabaseTable> tables = [];

            if (groupHierarcy.Count == 0)
            {
                Dictionary<string, List<DataBaseListResponseItem>> langToDbList = [];
                foreach (string lang in Configuration.Current.LanguageOptions.Available)
                {
                    langToDbList[lang] = await GetDataBaseListingInLangAsync(lang);
                }
                List<DataBaseListResponseItem> someLangItems = langToDbList.First().Value;
                for (int dbIndx = 0; dbIndx < someLangItems.Count; dbIndx++)
                {
                    MultilanguageString name = new(langToDbList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value[dbIndx].Text));
                    headers.Add(new(someLangItems[dbIndx].Dbid, [.. langToDbList.Keys], name));
                }
            }
            else
            {
                Dictionary<string, List<TableListResponseItem>> langToDbList = [];
                foreach (string lang in Configuration.Current.LanguageOptions.Available)
                {
                    langToDbList[lang] = await GetTableItemListingInLangAsync(lang, groupHierarcy);
                }
                List<TableListResponseItem> someLangItems = langToDbList.First().Value;
                for (int tableListIndx = 0; tableListIndx < someLangItems.Count; tableListIndx++)
                {
                    if (someLangItems[tableListIndx].Type == LIST_ITEM_TYPE)
                    {
                        MultilanguageString name = new(langToDbList
                            .Where(kvp => kvp.Value.ElementAtOrDefault(tableListIndx) != null)
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value[tableListIndx].Text
                        ));
                        headers.Add(new(someLangItems[tableListIndx].Id, [.. langToDbList.Keys], name));
                    }
                    else if (someLangItems[tableListIndx].Type == TABLE_ITEM_TYPE)
                    {
                        MultilanguageString name = new(langToDbList
                            .Where(kvp => kvp.Value.ElementAtOrDefault(tableListIndx) != null)
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value[tableListIndx].Text
                        ));
                        DateTime lastUpdated = PxSyntaxConstants.ParseDateTime(someLangItems[tableListIndx].Updated);
                        tables.Add(new(someLangItems[tableListIndx].Id, name, lastUpdated, [.. langToDbList.Keys]));
                    }
                }
            }
            return new DatabaseGroupContents(headers, tables);
        }

        public static DateTime GetLastWriteTime(PxTableReference tableReference)
        {
            throw new NotSupportedException($"Only async methods are supported. Use {nameof(GetLastWriteTimeAsync)} instead.");
        }

        public async Task<DateTime> GetLastWriteTimeAsync(PxTableReference tableReference)
        {
            string defaultLanguage = Configuration.Current.LanguageOptions.Default;
            List<TableListResponseItem> tableListResponseItems = await GetTableItemListingInLangAsync(defaultLanguage, tableReference.Hierarchy);
            string lastUpdatedString = tableListResponseItems.First(t => t.Id == tableReference.Name).Updated;
            return PxSyntaxConstants.ParseDateTime(lastUpdatedString);
        }

        public static IReadOnlyMatrixMetadata GetMatrixMetadata(PxTableReference tableReference)
        {
            throw new NotSupportedException($"Only async methods are supported. Use {nameof(GetMatrixMetadataAsync)} instead.");
        }

        public async Task<IReadOnlyMatrixMetadata> GetMatrixMetadataAsync(PxTableReference tableReference)
        {
            string defaultLang = Configuration.Current.LanguageOptions.Default;
            List<string> langOptions = Configuration.Current.LanguageOptions.Available;

            Dictionary<string, PxMetaResponse> langToResponse = [];
            foreach (string lang in langOptions)
            {
                PxMetaResponse response = await GetPxWebMetaRespInLanguageAsync(lang, tableReference);
                if (response is not null) langToResponse[lang] = response;
                _logger.LogWarning("Could not find metadata for table {TableName} with language {Language}", tableReference.Name, lang);
            }

            if (langToResponse.Count == 0) throw new TableMetadataException("Could not load table with any language.");

            PxMetaResponse map = langToResponse[defaultLang];
            List<Dimension> dimensions = [];
            Dictionary<string, DimensionType> dimensionTypes = await GetDimensionTypesAsync(map, [.. langToResponse.Keys], tableReference);
            for (int dimIndx = 0; dimIndx < map.DimensionMaps.Count; dimIndx++)
            {
                IDimensionMap dimensionMap = map.DimensionMaps[dimIndx];
                List<DimensionValue> values = [];
                for (int valIndx = 0; valIndx < dimensionMap.ValueCodes.Count; valIndx++)
                {
                    MultilanguageString valueName = new(langToResponse.ToDictionary(
                         kvp => kvp.Key,
                         kvp => kvp.Value.Variables[dimIndx].ValueTexts[valIndx]));
                    values.Add(new(dimensionMap.ValueCodes[valIndx], valueName));
                }

                MultilanguageString dimensionName = new(langToResponse.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Variables[dimIndx].Text));

                if (dimensionTypes[dimensionMap.Code] == DimensionType.Time)
                {
                    TimeDimensionInterval interval = Data.TimeVarIntervalParser.DetermineIntervalFromCodes(dimensionMap.ValueCodes);
                    dimensions.Add(new TimeDimension(dimensionMap.Code, dimensionName, [], values, interval));
                    continue;
                }

                Dimension dimension = new(dimensionMap.Code, dimensionName, [], values, dimensionTypes[dimensionMap.Code]);

                if (dimensionTypes[dimensionMap.Code] == DimensionType.Content)
                {
                    dimension = await ConvertToContentDimension(dimension, map, langOptions, tableReference);
                }

                dimensions.Add(dimension);
            }

            return new MatrixMetadata(defaultLang, langOptions, dimensions, []);
        }

        public static Matrix<DecimalDataValue> GetMatrix(PxTableReference tableReference, IReadOnlyMatrixMetadata meta)
        {
            throw new NotSupportedException($"Only async methods are supported. Use {nameof(GetMatrixAsync)} instead.");
        }

        public async Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata meta, CancellationToken? cancellationToken = null)
        {
            string language = meta.DefaultLanguage;
            IEnumerable<PxWebDataQueryPostParams.VariableQuery> queries = meta.Dimensions.Select(v => BuildVariableQuery(v.Code, [.. v.ValueCodes]));

            PxWebDataQueryPostParams postParams = BuildPostParams(queries);
            JsonStat2 dataResult = await GetPxWebDataResponseAsync<JsonStat2>(language, tableReference, postParams);

            decimal?[] values = dataResult.Value;
            DecimalDataValue[] dataValues = new DecimalDataValue[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] is null)
                {
                    string statusCode = dataResult.Status?.GetValueOrDefault(i.ToString());
                    dataValues[i] = new DecimalDataValue(0, (DataValueType)Array.IndexOf(PxSyntaxConstants.MissingValueDotCodes, statusCode));
                }
                else
                {
                    dataValues[i] = new DecimalDataValue(values[i].Value, DataValueType.Exists);
                }
            }

            List<IDimensionMap> dimMaps = [];
            foreach (var dimensionId in dataResult.Id)
            {
                JsonStat2.DimensionObj dimension = dataResult.Dimensions[dimensionId];
                List<string> variableValueCodes = new(new string[dimension.Category.Index.Count]);

                foreach (KeyValuePair<string, int> p in dimension.Category.Index)
                {
                    string dimensionValueCode = p.Key;
                    int dimensionValueIndex = p.Value;

                    variableValueCodes[dimensionValueIndex] = dimensionValueCode;
                }

                dimMaps.Add(new DimensionMap(dimensionId, variableValueCodes));
            }

#if DEBUG
            //Assert that loaded variables match to requested meta
            Debug.Assert(dimMaps.Count == meta.Dimensions.Count);
            for (var v = 0; v < meta.Dimensions.Count; v++)
            {
                IDimensionMap jsonStatVariable = dimMaps[v];
                IReadOnlyDimension cubeMetaVariable = meta.Dimensions[v];
                Debug.Assert(jsonStatVariable.Code == cubeMetaVariable.Code);
                Debug.Assert(jsonStatVariable.ValueCodes.Count == cubeMetaVariable.Values.Count);

                for (var vv = 0; vv < cubeMetaVariable.Values.Count; vv++)
                {
                    Debug.Assert(jsonStatVariable.ValueCodes[vv] == cubeMetaVariable.Values[vv].Code);
                }
            }
#endif
            MatrixMetadata newTable = meta.GetTransform(new MatrixMap(dimMaps));
            return new Matrix<DecimalDataValue>(newTable, dataValues);
        }

        /// <summary>
        /// Fetch a list of databases available in the given language from the PxWeb server.
        /// </summary>
        private async Task<List<DataBaseListResponseItem>> GetDataBaseListingInLangAsync(string lang)
        {
            _logger.LogDebug("PxWeb GET: api/v1/{Lang}/", lang);
            HttpResponseMessage resp = await _pxwebConnection.GetAsync($"api/v1/{lang}/");
            if (resp.IsSuccessStatusCode)
            {
                string json = await resp.Content.ReadAsStringAsync();
                _logger.LogDebug("PxWeb: api/v1/{Lang}/ result: {Json}", lang, json);
                return JsonSerializer.Deserialize<List<DataBaseListResponseItem>>(json, GlobalJsonConverterOptions.Default);
            }
            else
            {
                _logger.LogWarning("PxWeb: api/v1/{Lang}/ failed with status code {StatusCode}", lang, resp.StatusCode);
            }
            return [];
        }

        /// <summary>
        /// Fetch a list of directories in a database in the given language from the PxWeb server.
        /// </summary>
        private async Task<List<TableListResponseItem>> GetTableItemListingInLangAsync(string lang, IReadOnlyList<string> path)
        {
            string joinedPath = string.Join("/", path);
            _logger.LogDebug("PxWeb GET: api/v1/{Lang}/{JoinedPath}/", lang, joinedPath);
            HttpResponseMessage resp = await _pxwebConnection.GetAsync($"api/v1/{lang}/{joinedPath}/");
            if (resp.IsSuccessStatusCode)
            {
                string json = await resp.Content.ReadAsStringAsync();
                _logger.LogDebug("PxWeb: api/v1/{Lang}/{JoinedPath}/ result: {Json}", lang, joinedPath, json);
                return JsonSerializer.Deserialize<List<TableListResponseItem>>(json, GlobalJsonConverterOptions.Default);
            }
            else
            {
                _logger.LogWarning("PxWeb: api/v1/{Lang}/ failed with status code {StatusCode}", lang, resp.StatusCode);
            }
            return [];
        }

        private async Task<JsonStat2> GetSinglePointJsonStat2RespAsync(PxTableReference pxFile, IMatrixMap map, string language)
        {
            if (map.DimensionMaps.Any(v => v.ValueCodes.Count == 0))
            {
                _logger.LogWarning("PxWeb: GetSinglePointJsonStat2RespAsync failed because one of the variables has no values. pxFile: {PxFile}, language: {Language}.", pxFile, language);
                return null;
            }
            IEnumerable<PxWebDataQueryPostParams.VariableQuery> queries = map.DimensionMaps.Select(v => BuildVariableQuery(v.Code, [v.ValueCodes[0]]));
            PxWebDataQueryPostParams postParams = BuildPostParams(queries);
            JsonStat2 jsonStat2 = await GetPxWebDataResponseAsync<JsonStat2>(language, pxFile, postParams);
            FixPxWebBrokenDateFormat(jsonStat2);
            return jsonStat2;
        }

        private async Task<Dictionary<string, DimensionType>> GetDimensionTypesAsync(IMatrixMap matrixMap, List<string> succeededLanguages, PxTableReference tableReference)
        {
            JsonStat2 jsonStat2 = await GetSinglePointJsonStat2RespAsync(tableReference, matrixMap, succeededLanguages[0]);
            Dictionary<string, DimensionType> types = [];
            foreach (string code in matrixMap.DimensionMaps.Select(dm => dm.Code))
            {
                if (jsonStat2.Role.Metric is string[] contentVarCodes && contentVarCodes.Contains(code))
                {
                    types[code] = DimensionType.Content;
                    continue;
                }

                if (jsonStat2.Role.Time is string[] timeVarCodes && timeVarCodes.Contains(code))
                {
                    types[code] = DimensionType.Time;
                    continue;
                }

                if (jsonStat2.Role.Geo is string[] geoVarCodes && geoVarCodes.Contains(code))
                {
                    types[code] = DimensionType.Geographical;
                    continue;
                }

                if (jsonStat2.VarHasOrdinalScaleType(code))
                {
                    types[code] = DimensionType.Ordinal;
                    continue;
                }

                types[code] = DimensionType.Other;
            }

            return types;
        }

        private async Task<ContentDimension> ConvertToContentDimension(
            Dimension dimensionBase,
            IMatrixMap mapForDataFetching,
            List<string> fetchLanguages,
            PxTableReference tableReference)
        {
            IEnumerable<PxWebDataQueryPostParams.VariableQuery> queries = BuildMinimalContentVariableQueries(mapForDataFetching, dimensionBase.Code);
            PxWebDataQueryPostParams postParams = BuildPostParams(queries);

            Dictionary<string, JsonStat2> dataResultsByLanguage = [];
            foreach (var language in fetchLanguages) // OBS! This is only for getting the unit names in all languages! Needs to be changes once the pxweb v2 api is live.
            {
                JsonStat2 dataResult = await GetPxWebDataResponseAsync<JsonStat2>(language, tableReference, postParams);
                FixPxWebBrokenDateFormat(dataResult);

                dataResultsByLanguage.Add(language, dataResult);
            }

            MultilanguageString source = new(dataResultsByLanguage.ToDictionary(
                d => d.Key,
                d => d.Value.Source ?? ""
            ));

            MetaProperty sourceProperty = new MultilanguageStringProperty(source);

            string lastUpdatedString = dataResultsByLanguage.Select(
                d => d.Value.Updated
            )
            .Distinct()
            .Single();

            DateTime updated = PxSyntaxConstants.ParseDateTime(lastUpdatedString);

            List<ContentDimensionValue> contentdimensionValues = [];

            foreach (DimensionValue contentDimVal in dimensionBase.Values)
            {
                MultilanguageString unit = new(dataResultsByLanguage.ToDictionary(
                    d => d.Key,
                    d => d.Value.Dimensions[dimensionBase.Code].Category.Unit[contentDimVal.Code].Base ?? ""
                ));

                int decimals = dataResultsByLanguage.Select(
                    d => d.Value.Dimensions[dimensionBase.Code].Category.Unit[contentDimVal.Code].Decimals
                )
                .Distinct()
                .Single();
                contentdimensionValues.Add(new ContentDimensionValue(contentDimVal.Code, contentDimVal.Name, unit, updated, decimals));
            }

            Dictionary<string, MetaProperty> additionalProperties = dimensionBase.AdditionalProperties;
            additionalProperties[PxSyntaxConstants.SOURCE_KEY] = sourceProperty;
            return new ContentDimension(dimensionBase.Code, dimensionBase.Name, additionalProperties, contentdimensionValues);
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

        private static IEnumerable<PxWebDataQueryPostParams.VariableQuery> BuildMinimalContentVariableQueries(IMatrixMap map, string contentDimCode)
        {
            foreach (IDimensionMap dimMap in map.DimensionMaps)
            {
                string[] selectedValues;
                if (dimMap.Code == contentDimCode)
                {
                    selectedValues = [.. dimMap.ValueCodes];
                }
                else
                {
                    selectedValues = [dimMap.ValueCodes[0]];
                }

                yield return BuildVariableQuery(dimMap.Code, selectedValues);
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
        private async Task<PxMetaResponse> GetPxWebMetaRespInLanguageAsync(string lang, PxTableReference pxTableRef)
        {
            string tableRefPath = pxTableRef.ToPath();
            _logger.LogDebug("PxWeb GET: api/v1/{Lang}/{TableRefPath}", lang, tableRefPath);
            var resp = await _pxwebConnection.GetAsync($"api/v1/{lang}/{tableRefPath}");
            string contentString = await resp.Content.ReadAsStringAsync();

            if (resp.IsSuccessStatusCode)
            {
                _logger.LogDebug("PxWeb: api/v1/{Lang}/{TableRefPath} result: {ContentString}", lang, tableRefPath, contentString);
                return JsonSerializer.Deserialize<PxMetaResponse>(contentString, GlobalJsonConverterOptions.Default);
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

        private async Task<ResponseType> GetPxWebDataResponseAsync<ResponseType>(string lang, PxTableReference pxTableRef, PxWebDataQueryPostParams query)
        {
            string tableRefPath = pxTableRef.ToPath();
            _logger.LogDebug("PxWeb POST: api/v1/{Lang}/{TableRefPath} with query: {Query}", lang, tableRefPath, query);
            HttpResponseMessage resp = await _pxwebConnection.PostAsync($"api/v1/{lang}/{tableRefPath}", JsonSerializer.Serialize(query));
            if (resp.IsSuccessStatusCode)
            {
                string json = await resp.Content.ReadAsStringAsync();
                _logger.LogDebug("PxWeb: api/v1/{Lang}/{TableRefPath} result: {Json}", lang, tableRefPath, json);
                return JsonSerializer.Deserialize<ResponseType>(json, GlobalJsonConverterOptions.Default);
            }
            else
            {
                _logger.LogWarning("PxWeb: api/v1/{Lang}/{TableRefPath} failed with query {Query}", lang, tableRefPath, query);
                throw new BadPxWebResponseException(resp.StatusCode, $"PxWeb responded with a status code {resp.StatusCode}.");
            }
        }

        /*
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
