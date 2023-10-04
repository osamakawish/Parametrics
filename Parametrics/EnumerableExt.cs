using System;
using System.Collections.Generic;

namespace Parametrics;

public static class EnumerableExt
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    { foreach (T item in source) action(item); }

    public static int BinarySearch<T>(this IReadOnlyList<T> list, T item, IDoubleComparer<T> comparer)
    {
        int low = 0;
        int high = list.Count - 1;
        while (low <= high)
        {
            int mid = (low + high) / 2;
            double comparison = comparer.Compare(list[mid], item);

            if (Math.Abs(comparison) <= comparer.Epsilon) return mid;
            else if (comparison < 0) low = mid + 1;
            else high = mid - 1;
        }
        return low;
    }
}
