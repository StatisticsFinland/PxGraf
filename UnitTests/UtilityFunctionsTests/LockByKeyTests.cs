using NUnit.Framework;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.UtilityFunctionsTests
{
    internal class LockByKeyTests
    {
        private LockByKey _lockByKey;

        [SetUp]
        public void SetUp()
        {
            _lockByKey = new LockByKey(StringComparer.OrdinalIgnoreCase);
        }

        [Test]
        public async Task RunLockedAsync_ExecutesAction()
        {
            // Arrange
            bool executed = false;

            // Act
            await _lockByKey.RunLockedAsync("key", () =>
            {
                executed = true;
                return Task.CompletedTask;
            });

            // Assert
            Assert.That(executed, Is.True);
        }

        [Test]
        public async Task RunLockedAsyncT_ReturnsValue()
        {
            // Act
            int result = await _lockByKey.RunLockedAsync("key", () => Task.FromResult(42));

            // Assert
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public async Task RunLockedAsync_PropagatesException()
        {
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _lockByKey.RunLockedAsync("key", () =>
                    throw new InvalidOperationException("test"));
            });

            // Verify the lock is released and the key can be reused
            bool executed = false;
            await _lockByKey.RunLockedAsync("key", () =>
            {
                executed = true;
                return Task.CompletedTask;
            });
            Assert.That(executed, Is.True);
        }

        [Test]
        public async Task RunLockedAsyncT_PropagatesException()
        {
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _lockByKey.RunLockedAsync<int>("key", () =>
                    throw new InvalidOperationException("test"));
            });

            // Verify the lock is released and the key can be reused
            int result = await _lockByKey.RunLockedAsync("key", () => Task.FromResult(99));
            Assert.That(result, Is.EqualTo(99));
        }

        [Test]
        public async Task RunLockedAsync_SameKey_SerializesExecution()
        {
            // Arrange
            int concurrentCount = 0;
            int maxConcurrentCount = 0;
            List<int> executionOrder = [];

            async Task Action(int id)
            {
                int current = Interlocked.Increment(ref concurrentCount);
                InterlockedMax(ref maxConcurrentCount, current);
                executionOrder.Add(id);
                await Task.Delay(50);
                Interlocked.Decrement(ref concurrentCount);
            }

            // Act
            Task task1 = _lockByKey.RunLockedAsync("shared", () => Action(1));
            Task task2 = _lockByKey.RunLockedAsync("shared", () => Action(2));
            Task task3 = _lockByKey.RunLockedAsync("shared", () => Action(3));
            await Task.WhenAll(task1, task2, task3);

            // Assert
            Assert.That(maxConcurrentCount, Is.EqualTo(1), "Only one action should run at a time for the same key");
            Assert.That(executionOrder, Has.Count.EqualTo(3));
        }

        [Test]
        public async Task RunLockedAsync_DifferentKeys_AllowsConcurrentExecution()
        {
            // Arrange
            int concurrentCount = 0;
            int maxConcurrentCount = 0;
            TaskCompletionSource allStarted = new();
            int startedCount = 0;
            const int taskCount = 3;

            async Task Action()
            {
                int current = Interlocked.Increment(ref concurrentCount);
                InterlockedMax(ref maxConcurrentCount, current);

                if (Interlocked.Increment(ref startedCount) == taskCount)
                {
                    allStarted.SetResult();
                }

                await allStarted.Task;
                Interlocked.Decrement(ref concurrentCount);
            }

            // Act
            Task task1 = _lockByKey.RunLockedAsync("key-a", Action);
            Task task2 = _lockByKey.RunLockedAsync("key-b", Action);
            Task task3 = _lockByKey.RunLockedAsync("key-c", Action);
            await Task.WhenAll(task1, task2, task3);

            // Assert
            Assert.That(maxConcurrentCount, Is.EqualTo(taskCount), "Different keys should allow concurrent execution");
        }

        [Test]
        public async Task RunLockedAsync_CaseInsensitiveKeys_Serializes()
        {
            // Arrange
            int concurrentCount = 0;
            int maxConcurrentCount = 0;

            async Task Action()
            {
                int current = Interlocked.Increment(ref concurrentCount);
                InterlockedMax(ref maxConcurrentCount, current);
                await Task.Delay(50);
                Interlocked.Decrement(ref concurrentCount);
            }

            // Act
            Task task1 = _lockByKey.RunLockedAsync("MyKey", Action);
            Task task2 = _lockByKey.RunLockedAsync("mykey", Action);
            Task task3 = _lockByKey.RunLockedAsync("MYKEY", Action);
            await Task.WhenAll(task1, task2, task3);

            // Assert
            Assert.That(maxConcurrentCount, Is.EqualTo(1), "Case-insensitive keys should serialize");
        }

        [Test]
        public async Task RunLockedAsync_CaseSensitiveComparer_AllowsConcurrentForDifferentCase()
        {
            // Arrange
            LockByKey caseSensitiveLock = new(StringComparer.Ordinal);
            int concurrentCount = 0;
            int maxConcurrentCount = 0;
            TaskCompletionSource allStarted = new();
            int startedCount = 0;

            async Task Action()
            {
                int current = Interlocked.Increment(ref concurrentCount);
                InterlockedMax(ref maxConcurrentCount, current);

                if (Interlocked.Increment(ref startedCount) == 2)
                {
                    allStarted.SetResult();
                }

                await allStarted.Task;
                Interlocked.Decrement(ref concurrentCount);
            }

            // Act
            Task task1 = caseSensitiveLock.RunLockedAsync("Key", Action);
            Task task2 = caseSensitiveLock.RunLockedAsync("key", Action);
            await Task.WhenAll(task1, task2);

            // Assert
            Assert.That(maxConcurrentCount, Is.EqualTo(2), "Case-sensitive comparer should treat different cases as different keys");
        }

        [Test]
        public async Task RunLockedAsync_KeyIsReusableAfterCompletion()
        {
            // Arrange & Act
            int callCount = 0;
            for (int i = 0; i < 5; i++)
            {
                await _lockByKey.RunLockedAsync("reusable", () =>
                {
                    callCount++;
                    return Task.CompletedTask;
                });
            }

            // Assert
            Assert.That(callCount, Is.EqualTo(5));
        }

        [Test]
        public async Task RunLockedAsync_KeyIsReusableAfterException()
        {
            // Arrange
            try
            {
                await _lockByKey.RunLockedAsync("failkey", () =>
                    throw new InvalidOperationException());
            }
            catch (InvalidOperationException)
            {
                // Expected
            }

            // Act - key should be reusable
            bool executed = false;
            await _lockByKey.RunLockedAsync("failkey", () =>
            {
                executed = true;
                return Task.CompletedTask;
            });

            // Assert
            Assert.That(executed, Is.True);
        }

        [Test]
        public async Task RunLockedAsyncT_SameKey_SerializesAndReturnsCorrectValues()
        {
            // Arrange
            int concurrentCount = 0;
            int maxConcurrentCount = 0;

            async Task<int> Action(int value)
            {
                int current = Interlocked.Increment(ref concurrentCount);
                InterlockedMax(ref maxConcurrentCount, current);
                await Task.Delay(50);
                Interlocked.Decrement(ref concurrentCount);
                return value * 2;
            }

            // Act
            Task<int> task1 = _lockByKey.RunLockedAsync("key", () => Action(1));
            Task<int> task2 = _lockByKey.RunLockedAsync("key", () => Action(2));
            Task<int> task3 = _lockByKey.RunLockedAsync("key", () => Action(3));
            await Task.WhenAll(task1, task2, task3);

            // Assert
            Assert.That(maxConcurrentCount, Is.EqualTo(1));
            Assert.That(task1.Result, Is.EqualTo(2));
            Assert.That(task2.Result, Is.EqualTo(4));
            Assert.That(task3.Result, Is.EqualTo(6));
        }

        private static void InterlockedMax(ref int location, int value)
        {
            int current;
            do
            {
                current = Volatile.Read(ref location);
                if (value <= current) return;
            }
            while (Interlocked.CompareExchange(ref location, value, current) != current);
        }
    }
}
