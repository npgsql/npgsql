﻿using System;
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
        static volatile (string Key, ConnectorPool Pool)[] _pools = new (string, ConnectorPool)[InitialPoolsSize];
        static volatile int _nextSlot;

        internal static (string Key, ConnectorPool Pool)[] Pools => _pools;

        internal static bool TryGetValue(string key, [NotNullWhen(true)] out ConnectorPool? pool)
        {
            // Note that pools never get removed. _pools is strictly append-only.
            var nextSlot = _nextSlot;
            var pools = _pools;
            // There is a race condition between TryGetValue and ClearAll.
            // When the nextSlot has already been read as a non-zero value, but the pools have already been reset.
            // And in this case, we will iterate beyond the size of the array.
            // As a safeguard, we take a min length between the array and the nextSlot.
            var minNextSlot = Math.Min(nextSlot, pools.Length);
            var sw = new SpinWait();

            // First scan the pools and do reference equality on the connection strings
            for (var i = 0; i < minNextSlot; i++)
            {
                var cp = pools[i];
                if (ReferenceEquals(cp.Key, key))
                {
                    // It's possible that this pool entry is currently being written: the connection string
                    // component has already been written, but the pool component is just about to be. So we
                    // loop on the pool until it's non-null
                    while (Volatile.Read(ref cp.Pool) == null)
                        sw.SpinOnce();
                    pool = cp.Pool;
                    return true;
                }
            }

            // Next try value comparison on the strings
            for (var i = 0; i < minNextSlot; i++)
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
                if (_nextSlot == _pools.Length)
                {
                    var newPools = new (string, ConnectorPool)[_pools.Length * 2];
                    Array.Copy(_pools, newPools, _pools.Length);
                    _pools = newPools;
                }

                _pools[_nextSlot].Key = key;
                _pools[_nextSlot].Pool = pool;
                Interlocked.Increment(ref _nextSlot);
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
                for (var i = 0; i < _nextSlot; i++)
                {
                    var cp = pools[i];
                    // We shouldn't ever get here, as _nextSlot is only incremented under a lock
                    // and there shouldn't be null values in-between. But just in case...
                    if (cp.Key == null)
                        break;
                    cp.Pool?.Clear();
                }

                // _nextSlot should be changed before the _pools.
                // Otherwise, there will be a race condition with TryGetValue.
                _nextSlot = 0;
                _pools = new (string, ConnectorPool)[InitialPoolsSize];
            }
        }

        static PoolManager()
        {
            // When the appdomain gets unloaded (e.g. web app redeployment) attempt to nicely
            // close idle connectors to prevent errors in PostgreSQL logs (#491).
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => ClearAll();
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => ClearAll();
        }
    }
}
