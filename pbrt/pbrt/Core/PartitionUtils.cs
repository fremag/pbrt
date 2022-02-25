using System;
using System.Collections.Generic;

namespace pbrt.Core
{
    public static class PartitionUtils
    {
        // https://en.cppreference.com/w/cpp/algorithm/find
        public static int FindIfNot<T>(this IList<T> values, int first, int last, Func<T, bool> predicate)
        {
            for (; first != last; ++first)
            {
                if (!predicate(values[first]))
                {
                    return first;
                }
            }

            return last;
        }

        // https://en.cppreference.com/w/cpp/algorithm/partition
        public static int StdPartition<T>(this IList<T> values, int first, int last, Func<T, bool> predicate)
        {
            first = values.FindIfNot(first, last, predicate);
            if (first == last) return first;

            for (int i = first + 1; i != last; ++i)
            {
                if (predicate(values[i]))
                {
                    (values[i], values[first]) = (values[first], values[i]);
                    ++first;
                }
            }

            return first;
        }
    }
}