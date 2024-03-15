using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Npgsql;

/// <summary>
/// Provides lookup for a pool based on a connection string.
/// </summary>
/// <remarks>
/// Note that pools created directly as <see cref="NpgsqlDataSource" /> are referenced directly by users, and aren't managed here.
/// </remarks>
static class PoolManager
{
    static readonly object LockObject = new();
    static ConcurrentDictionary<string, NpgsqlDataSource> _pools = new();

    // Keep the IEnumerable API for testing purposes but remove the rest to avoid careless (unlocked) access to the
    // private dictionary.
    // ReSharper disable once InconsistentlySynchronizedField
    internal static IEnumerable<KeyValuePair<string, NpgsqlDataSource>> Pools => _pools.ToImmutableDictionary();

    internal static void Clear(string connString)
    {
        lock (LockObject)
        {
            var pools = _pools;
            if (!pools.TryRemove(connString, out var pool))
                    return;

            pool.Dispose();

            // The same NpgsqlDataSource instance may be referenced via different connection strings
            // in our dictionary, so we have to make sure that we remove all occurrences
            foreach (var pair in pools)
                if (ReferenceEquals(pair.Value, pool))
                    pools.TryRemove(pair);
        }
    }

    internal static void ClearAll()
    {
        lock (LockObject)
        {
            var pools = _pools;
            _pools = new();
            foreach (var pool in pools.Values)
                pool.Dispose();
        }
    }

    static PoolManager()
    {
        // When the appdomain gets unloaded (e.g. web app redeployment) attempt to nicely
        // close idle connectors to prevent errors in PostgreSQL logs (#491).
        AppDomain.CurrentDomain.DomainUnload += (_, _) => ClearAll();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => ClearAll();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NpgsqlDataSource GetOrAddPool(string connectionString, NpgsqlDataSource dataSource)
    {
        lock (LockObject)
        {
            return _pools.GetOrAdd(connectionString, dataSource);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetPool(string connectionString, [MaybeNullWhen(false)] out NpgsqlDataSource dataSource)
        // ReSharper disable once InconsistentlySynchronizedField
        => _pools.TryGetValue(connectionString, out dataSource);
}
