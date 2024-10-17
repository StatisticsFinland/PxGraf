using PxGraf.Models.Responses;
using PxGraf.Models.Requests;
using Newtonsoft.Json;
using PxGraf.Utility;


namespace Tools.PxUtilsIntegrationTestTool
{
    internal class ResponseCollector : Command
    {
        private readonly string[] _dataSources = [TokenConstants.DATASOURCE_PXUTILS, TokenConstants.DATASOURCE_PXWEBAPI, TokenConstants.DATASOURCE_OLD];
        private readonly JsonSerializerSettings _jsonConverterSettings = new();

        internal override async Task Start()
        {
            _jsonConverterSettings.Converters.Add(new MultilanguageStringConverter());

            foreach (string dataSource in _dataSources)
            {
                string url = ToolsUtilities.GetDataSourceUrl(dataSource);
                string saveLocation = ResponseSaveLocation.SetSaveLocation(dataSource);
                string[] queries = await File.ReadAllLinesAsync(Program.Config.Paths.QueriesFile);
                await StoreSqVisualizationResponses(saveLocation, queries, url);
                await StoreSavedQueryResponses(saveLocation, queries, url);
                await StoreSqMetaResponses(saveLocation, queries, url);
            }
        }

        private async Task StoreSqMetaResponses(string saveLocation, string[] queries, string url)
        {
            string saveDirectory = Path.Combine(saveLocation, TokenConstants.RESPONSE_SQMETA);
            Directory.CreateDirectory(saveDirectory);
            for (int i = 0; i < queries.Length; i++)
            {
                (QueryMetaResponse? queryMeta, string content) = await GetQueryMetaAsync($"{url}/api/sq/meta/" + queries[i]);
                if (queryMeta != null)
                {
                    await File.WriteAllTextAsync(Path.Combine(saveDirectory, queries[i] + ".json"), content);
                    Console.WriteLine($"Query ID: {queries[i]}, meta response stored to {saveDirectory}");
                }
                else
                    Console.WriteLine($"Unable to parse query meta for query ID: {queries[i]}");
            }
        }

        private async Task StoreSqVisualizationResponses(string saveLocation, string[] queries, string url)
        {
            string saveDirectory = Path.Combine(saveLocation, TokenConstants.RESPONSE_SQVISUALIZATION);
            Directory.CreateDirectory(saveDirectory);
            for (int i = 0; i < queries.Length; i++)
            {
                (VisualizationResponse? queryVisualization, string content) = await GetQueryVisualizationAsync($"{url}/api/sq/visualization/" + queries[i]);
                if (queryVisualization != null)
                {
                    await File.WriteAllTextAsync(Path.Combine(saveDirectory, queries[i] + ".json"), content);
                    Console.WriteLine($"Query ID: {queries[i]}, visualization response stored to {saveDirectory}");
                }
                else
                    Console.WriteLine($"Unable to parse query visualization for query ID: {queries[i]}");
            }
        }

        private async Task StoreSavedQueryResponses(string saveLocation, string[] queries, string url)
        {
            string saveDirectory = Path.Combine(saveLocation, TokenConstants.RESPONSE_SQ);
            Directory.CreateDirectory(saveDirectory);
            for (int i = 0; i < queries.Length; i++)
            {
                (SaveQueryParams? queryMeta, string content) = await GetSavedQueryAsync($"{url}/api/sq/" + queries[i]);
                if (queryMeta != null)
                {
                    await File.WriteAllTextAsync(Path.Combine(saveDirectory, queries[i] + ".json"), content);
                    Console.WriteLine($"Query ID: {queries[i]}, saved query response stored to {saveDirectory}");
                }
                else
                    Console.WriteLine($"Unable to parse saved query for query ID: {queries[i]}");
            }
        }

        private async Task<(QueryMetaResponse?, string)> GetQueryMetaAsync(string url)
        {
            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to get query meta with status code {response.StatusCode}");
                return (null, response.StatusCode.ToString());
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            QueryMetaResponse? meta = JsonConvert.DeserializeObject<QueryMetaResponse>(responseBody, _jsonConverterSettings);
            return meta is not null ? (meta, responseBody) : (null, responseBody);
        }

        private async Task<(VisualizationResponse?, string)> GetQueryVisualizationAsync(string url)
        {
            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to get query visualization with status code {response.StatusCode}");
                return (null, response.StatusCode.ToString());
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            VisualizationResponse? visualization = JsonConvert.DeserializeObject<VisualizationResponse>(responseBody, _jsonConverterSettings);
            return visualization is not null ? (visualization, responseBody) : (null, responseBody);
        }

        private async Task<(SaveQueryParams?, string)> GetSavedQueryAsync(string url)
        {
            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to get saved query with status code {response.StatusCode}");
                return (null, response.StatusCode.ToString());
            }
            string responseBody = await response.Content.ReadAsStringAsync();

            SaveQueryParams? savedQuery = JsonConvert.DeserializeObject<SaveQueryParams>(responseBody, _jsonConverterSettings);
            return savedQuery is not null ? (savedQuery, responseBody) : (null, responseBody);
        }
    }
}
