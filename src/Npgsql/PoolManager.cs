using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Npgsql
{
    /// <summary>
    /// Provides lookup for a pool based on a connection string.
    /// </summary>
    /// <remarks>
    /// <see cref="TryGetValue"/> is lock-free, to avoid contention, but the same isn't
    /// true of <see cref="GetOrAdd"/>, which acquires a lock. The calling code always tries
    /// <see cref="TryGetValue"/> before trying to <see cref="GetOrAdd"/>.
    /// </remarks>
    static class PoolManager
    {
        internal const int InitialPoolsSize = 10;

        static readonly object Lock = new();
        static volatile (string Key, ConnectorPool Pool)[] _poolsByKey = new (string, ConnectorPool)[InitialPoolsSize];
        static volatile ConnectorPool?[] _pools = new ConnectorPool[InitialPoolsSize];
        static volatile int _nextSlot;

        internal static ConnectorPool?[] Pools => _pools;

        internal static bool TryGetValue(string key, [NotNullWhen(true)] out ConnectorPool? pool)
        {
            // Note that pools never get removed. _poolsAliases is strictly append-only.
            var nextSlot = _nextSlot;
            var pools = _poolsAliases;
            var sw = new SpinWait();

            // First scan the pools and do reference equality on the connection strings
            for (var i = 0; i < nextSlot; i++)
            {
                var cp = pools[i];
                if (ReferenceEquals(cp.Key, key))
                {
                    // It's possible that this pool entry is currently being written: the connection string
                    // component has already been writte, but the pool component is just about to be. So we
                    // loop on the pool until it's non-null
                    while (Volatile.Read(ref cp.Pool) == null)
                        sw.SpinOnce();
                    pool = cp.Pool;
                    return true;
                }
            }

            // Next try value comparison on the strings
            for (var i = 0; i < nextSlot; i++)
            {
                var cp = pools[i];
                if (cp.Key == key)
                {
                    // See comment above
                    while (Volatile.Read(ref cp.Pool) == null)
                        sw.SpinOnce();
                    pool = cp.Pool;
                    return true;
                }
            }

            pool = null;
            return false;
        }

        internal static ConnectorPool GetOrAdd(string key, ConnectorPool pool)
        {
            lock (Lock)
            {
                if (TryGetValue(key, out var result))
                    return result;

                // May need to grow the array.
                if (_nextSlot == _poolsAliases.Length)
                {
                    var newPools = new (string, ConnectorPool)[_poolsAliases.Length * 2];
                    Array.Copy(_poolsAliases, newPools, _poolsAliases.Length);
                    _poolsAliases = newPools;
                }

                _poolsAliases[_nextSlot].Key = key;
                _poolsAliases[_nextSlot].Pool = pool;
                Interlocked.Increment(ref _nextSlot);

                var idx = 0;
                ConnectorPool? existsPool = null;
                for (; idx < _pools.Length; idx++)
                {
                    existsPool = _pools[idx];
                    if (existsPool == null || existsPool == pool)
                        break;
                }

                if (idx == _pools.Length)
                {
                    var newPools = new ConnectorPool[_pools.Length * 2];
                    Array.Copy(_pools, newPools, _pools.Length);
                    _pools = newPools;
                }
                
                if(existsPool == null)
                    _pools[idx] = pool;

                return pool;
            }
        }

        internal static void Clear(string connString)
        {
            if (TryGetValue(connString, out var pool))
                pool.Clear();
        }

        internal static void ClearAll()
        {
            lock (Lock)
            {
                var pools = _pools;
                for (var i = 0; i < _pools.Length; i++)
                {
                    var cp = pools[i];
                    if (cp == null)
                        return;
                    cp.Clear();
                }
            }
        }

        static PoolManager()
        {
            // When the appdomain gets unloaded (e.g. web app redeployment) attempt to nicely
            // close idle connectors to prevent errors in PostgreSQL logs (#491).
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => ClearAll();
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => ClearAll();
        }

        /// <summary>
        /// Resets the pool manager to its initial state, for test purposes only.
        /// Assumes that no other threads are accessing the pool.
        /// </summary>
        internal static void Reset()
        {
            lock (Lock)
            {
                ClearAll();
                _poolsAliases = new (string, ConnectorPool)[InitialPoolsSize];
                _pools = new ConnectorPool?[InitialPoolsSize];
                _nextSlot = 0;
            }
        }
    }
}
