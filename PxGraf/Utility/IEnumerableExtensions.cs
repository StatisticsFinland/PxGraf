using System;
using System.Collections.Generic;

namespace PxGraf.Utility
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// List.FindIndex like method but works for every IEnumerable
        /// </summary>
        /// <returns>Index of first found element or -1 if not found.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            int index = 0;
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    return index;
                }
                index++;
            }

            return -1;
        }
    }
}
