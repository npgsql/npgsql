#if NETSTANDARD2_0
// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

// Helpers for Dictionary before netstandard 2.1
static class DictonaryExtensions
{
    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            return true;
        }

        return false;
    }
}
#endif
