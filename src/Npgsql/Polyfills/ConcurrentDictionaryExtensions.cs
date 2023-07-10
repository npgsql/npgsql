using System;
using System.Collections.Concurrent;

namespace System.Collections.Concurrent;

#if NETSTANDARD2_0
static class ConcurrentDictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue, TArg>(this ConcurrentDictionary<TKey, TValue> instance, TKey key,
        Func<TKey, TArg, TValue> valueFactory, TArg factoryArgument)
    {
        // The actual closure capture exists in a local function to prevent a display class allocation at the start of the method.
        return instance.TryGetValue(key, out var value) ? value : GetOrAddWithClosure(instance, key, valueFactory, factoryArgument);

        static TValue GetOrAddWithClosure(ConcurrentDictionary<TKey, TValue> instance, TKey key, Func<TKey, TArg, TValue> valuefactory, TArg factoryargument) => instance.GetOrAdd(key, key => valuefactory(key, factoryargument));
    }
}
#endif
