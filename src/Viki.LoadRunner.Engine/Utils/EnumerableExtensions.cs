using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Viki.LoadRunner.Engine.Utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEachReturn<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);

                yield return item;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}