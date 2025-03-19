using PxGraf.Exceptions;
using PxGraf.Settings;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace PxGraf.Datasource.ApiDatasource
{
    [ExcludeFromCodeCoverage] // HTTP client object can not be mocked with reasonable effort
    public class PxWebConnection(IHttpClientFactory clientFactory) : IPxWebConnection
    {
        private readonly IHttpClientFactory _httpClientFactory = clientFactory;
        private const int RetryDelayMs = 2000;
        private const int RetryMaxTryCount = 10;

        public async Task<HttpResponseMessage> GetAsync(string path, string queryString = "")
        {
            string basePath = Configuration.Current.PxWebUrl;
            if (!basePath.EndsWith('/')) basePath += "/";
            string query = path + queryString;
            using HttpClient client = _httpClientFactory.CreateClient(Startup.PXWEBCLIENTNAME);
            client.BaseAddress = new Uri(basePath);

            return await WithRetry(() => Get());

            async Task<HttpResponseMessage> Get()
            {
                HttpResponseMessage response = await client.GetAsync(query, HttpCompletionOption.ResponseContentRead);

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

        public async Task<HttpResponseMessage> PostAsync(string path, string json)
        {
            string basePath = Configuration.Current.PxWebUrl;
            if (!basePath.EndsWith('/')) basePath += "/";
            using HttpClient client = _httpClientFactory.CreateClient(Startup.PXWEBCLIENTNAME);

            client.BaseAddress = new Uri(basePath);

            return await WithRetry(() => Post());

            async Task<HttpResponseMessage> Post()
            {
                StringContent data = new(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(path, data);

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
            for (int i = 0; i < RetryMaxTryCount; i++)
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

    }
}
