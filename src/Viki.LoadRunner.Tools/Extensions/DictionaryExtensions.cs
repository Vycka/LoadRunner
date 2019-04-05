using System.Collections.Generic;

namespace Viki.LoadRunner.Tools.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out var result);

            return result;
        }
    }
}