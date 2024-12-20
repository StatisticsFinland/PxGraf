using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Utility
{
    [ExcludeFromCodeCoverage] // Unable to find a good way to test a fifo mutex
    public class LockByKey(IEqualityComparer<string> keyComparer)
    {
        private sealed class LockInfo {
            public readonly object mutex;
            public int threadCounter;

            public LockInfo()
            {
                mutex = new object();
                threadCounter = 0;
            }
        }

        private readonly Dictionary<string, LockInfo> locks = new(keyComparer);

        public void RunLocked(string key, Action action)
        {
            object mutex = Lock(key);
            try
            {
                lock (mutex)
                {
                    action();
                }
            }
            finally
            {
                UnLock(key);
            }
        }

        public T RunLocked<T>(string key, Func<T> action)
        {
            object mutex = Lock(key);
            try
            {
                lock (mutex)
                {
                    return action();
                }
            }
            finally
            {
                UnLock(key);
            }
        }

        private object Lock(string key)
        {
            lock (locks)
            {
                if (!locks.TryGetValue(key, out LockInfo lockByKey))
                {
                    lockByKey = new ();
                    locks.Add(key, lockByKey);
                }

                lockByKey.threadCounter++;
                return lockByKey.mutex;
            }
        }

        private void UnLock(string key)
        {
            lock (locks)
            {
                LockInfo lockInfo = locks[key];
                lockInfo.threadCounter--;
                if (lockInfo.threadCounter <= 0)
                {
                    locks.Remove(key);
                }
            }
        }
    }
}
