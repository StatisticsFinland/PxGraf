using PxGraf.Models.Responses;
using System.Threading.Tasks;

namespace PxGraf.Caching
{
    /// <summary>
    /// Interface for a cache that stores <see cref="VisualizationResponse"/> objects.
    /// </summary>
    public interface IVisualizationResponseCache
    {
        void Refresh(string key);
        void Set(string key, Task<VisualizationResponse> response);
        VisualizationResponseCache.CacheEntryState TryGet(string key, out VisualizationResponse value);
    }
}