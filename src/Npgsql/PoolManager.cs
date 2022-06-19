using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Npgsql;

/// <summary>
/// Provides lookup for a pool based on a connection string.
/// </summary>
/// <remarks>
/// Note that pools created directly as <see cref="NpgsqlDataSource" /> are referenced directly by users, and aren't managed here.
/// </remarks>
static class PoolManager
{
    internal static ConcurrentDictionary<string, NpgsqlDataSource> Pools { get; } = new();

    internal static void Clear(string connString)
    {
        // TODO: Actually remove the pools from here, #3387 (but be careful of concurrency)
        if (Pools.TryGetValue(connString, out var pool))
            pool.Clear();
    }

    internal static void ClearAll()
    {
        // TODO: Actually remove the pools from here, #3387 (but be careful of concurrency)
        foreach (var pool in Pools.Values)
            pool.Clear();
    }

    static PoolManager()
    {
        // When the appdomain gets unloaded (e.g. web app redeployment) attempt to nicely
        // close idle connectors to prevent errors in PostgreSQL logs (#491).
        AppDomain.CurrentDomain.DomainUnload += (_, _) => ClearAll();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => ClearAll();
    }

    /// <summary>
    /// Resets the pool manager to its initial state, for test purposes only.
    /// Assumes that no other threads are accessing the pool.
    /// </summary>
    internal static void Reset()
    {
        // TODO: Remove once #3387 is implemented
        ClearAll();
        Pools.Clear();
    }
}