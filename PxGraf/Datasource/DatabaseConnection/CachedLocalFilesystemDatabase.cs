using Microsoft.Extensions.Caching.Memory;
using Px.Utils.Models.Metadata;
using PxGraf.Models.Queries;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PxGraf.Datasource.DatabaseConnection
{
    public class CachedLocalFilesystemDatabase(IMemoryCache cache, ILocalFilesystemDatabaseConnection dbConnection)
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ILocalFilesystemDatabaseConnection _dbConnection = dbConnection;

        private TimeSpan _slidingExpiration = TimeSpan.FromHours(1);
        private TimeSpan _absoluteExpiration = TimeSpan.FromHours(12);
        private const string FILE_HASH_SEED = "pNlNfoAkxtzCDkNaFqx9";

        private sealed class MetadataCacheHousing(DateTime writeTime, IReadOnlyMatrixMetadata metadata)
        {
            public DateTime CacheTime { get; } = DateTime.Now;
            public DateTime WriteTime { get; } = writeTime;
            public IReadOnlyMatrixMetadata Metadata { get; } = metadata;
        }

        public IReadOnlyMatrixMetadata GetMatrixMetadataCached(PxTableReference fileReference)
        {
            string hash = GetCacheKeyFromFileReference(fileReference);
            DateTime lastWriteTime = _dbConnection.CheckLastWritetime(fileReference);
            if (_cache.TryGetValue(hash, out MetadataCacheHousing item) && item.WriteTime == lastWriteTime)
            {
                return item.Metadata;
            }
            else
            {
                IReadOnlyMatrixMetadata newMeta = _dbConnection.ReadMatrixMetadata(fileReference);
                MetadataCacheHousing newCacheItem = new(lastWriteTime, newMeta);
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_slidingExpiration)
                    .SetAbsoluteExpiration(_absoluteExpiration);
                _cache.Set(hash, newCacheItem, cacheEntryOptions);
                return newMeta;
            }
        }

        private static string GetCacheKeyFromFileReference(PxTableReference fileReference)
        {
            StringBuilder builder = new();

            fileReference.Hierarchy.ForEach(x => builder.Append(x));
            builder.Append(fileReference.Name);
            builder.Append(FILE_HASH_SEED);

            byte[] inputBytes = Encoding.UTF8.GetBytes(builder.ToString());
            byte[] hashBytes = MD5.HashData(inputBytes);

            return BitConverter.ToString(hashBytes);
        }
    }
}
