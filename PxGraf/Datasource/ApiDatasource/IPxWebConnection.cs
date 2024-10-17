using System.Net.Http;
using System.Threading.Tasks;

namespace PxGraf.Datasource.PxWebInterface
{
    public interface IPxWebConnection
    {
        Task<HttpResponseMessage> GetAsync(string path, string queryString = "");

        Task<HttpResponseMessage> PostAsync(string path, string json);
    }
}
