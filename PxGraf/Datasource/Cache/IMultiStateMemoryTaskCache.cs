using System;
using System.Threading.Tasks;

namespace PxGraf.Datasource.Cache
{
    public interface IMultiStateMemoryTaskCache
    {
        void Set<ItemType>(string key, Task<ItemType> task, TimeSpan slidingExpiration, TimeSpan absoluteExpiration);
        MultiStateMemoryTaskCache.CacheEntryState TryGet<ItemType>(string key, out Task<ItemType> value);
    }
}