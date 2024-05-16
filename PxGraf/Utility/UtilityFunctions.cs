using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Utility
{
    /// <summary>
    /// Collection of static utility functions
    /// </summary>
    public static class UtilityFunctions
    {
        private static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> a, IEnumerable<T> b)
        {
            foreach (T b_item in b)
            {
                foreach (IEnumerable<T> a_subSet in a)
                {
                    yield return BuildSet(a_subSet, b_item);
                }
            }

            // Yields cannot be done in anonymous functions
            static IEnumerable<T> BuildSet(IEnumerable<T> a, T b)
            {
                foreach (T a_item in a) yield return a_item;
                yield return b;
            }
        }

        /// <summary>
        /// Returns a cartesian product of provided sets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setOfSets"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> setOfSets)
        {
            //Naturally this implementation calculates product in unconventional order, sort of inverted.
            //This is compensated by pre and post reversal.
            IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
            foreach (IEnumerable<T> set in setOfSets.Reverse()) result = CartesianProduct(result, set);
            return result.Select(r => r.Reverse());
        }

        /// <summary>
        /// If all values in the collection are simillar (default equality comparer) returns that value.
        /// Otherwise returns default.
        /// </summary>
        public static T UnambiguousOrDefault<T>(IEnumerable<T> items)
        {
            var uniqueItems = items.Distinct().ToList();
            if (uniqueItems.Count == 1)
            {
                return uniqueItems[0];
            }
            else
            {
                return default;
            }
        }
    }
}
