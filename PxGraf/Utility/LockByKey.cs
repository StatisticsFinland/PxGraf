using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PxGraf.Utility
{
    public class LockByKey(IEqualityComparer<string> keyComparer)
    {
        private sealed class AsyncLockInfo
        {
            public readonly SemaphoreSlim semaphore = new(1, 1);
            public int refCount;
        }

        private readonly Dictionary<string, AsyncLockInfo> asyncLocks = new(keyComparer);

        public async Task RunLockedAsync(string key, Func<Task> action)
        {
            SemaphoreSlim semaphore = AsyncLock(key);
            try
            {
                await semaphore.WaitAsync();
                try
                {
                    await action();
                }
                finally
                {
                    semaphore.Release();
                }
            }
            finally
            {
                AsyncUnLock(key);
            }
        }

        public async Task<T> RunLockedAsync<T>(string key, Func<Task<T>> action)
        {
            SemaphoreSlim semaphore = AsyncLock(key);
            try
            {
                await semaphore.WaitAsync();
                try
                {
                    return await action();
                }
                finally
                {
                    semaphore.Release();
                }
            }
            finally
            {
                AsyncUnLock(key);
            }
        }

        private SemaphoreSlim AsyncLock(string key)
        {
            lock (asyncLocks)
            {
                if (!asyncLocks.TryGetValue(key, out AsyncLockInfo info))
                {
                    info = new AsyncLockInfo();
                    asyncLocks.Add(key, info);
                }

                info.refCount++;
                return info.semaphore;
            }
        }

        private void AsyncUnLock(string key)
        {
            lock (asyncLocks)
            {
                AsyncLockInfo info = asyncLocks[key];
                info.refCount--;
                if (info.refCount <= 0)
                {
                    info.semaphore.Dispose();
                    asyncLocks.Remove(key);
                }
            }
        }
    }
}
