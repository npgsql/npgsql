﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Npgsql
{
    /// <summary>
    /// Provides lookup for a pool based on a connection string.
    /// </summary>
    /// <remarks>
    /// <see cref="TryGetValue"/> is lock-free to avoid contention, but the same isn't
    /// true of <see cref="GetOrAdd"/>, which acquires a lock. The calling code should always try
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
            var nextSlot = _nextSlot;
            var pools = _pools;
            // There is a race condition between TryGetValue and ClearAll.
            // When the nextSlot has already been read as a non-zero value, but the pools have already been reset.
            // And in this case, we will iterate beyond the size of the array.
            // As a safeguard, we take a min length between the array and the nextSlot.
            var minNextSlot = Math.Min(nextSlot, pools.Length);

            // First scan the pools and do reference equality on the connection strings
            for (var i = 0; i < minNextSlot; i++)
            {
                var cp = pools[i];
                if (ReferenceEquals(cp.Key, key))
                {
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

        internal static void Delete(ConnectorPool pool)
        {
            lock (Lock)
            {
                var poolIndex = -1;
                for (var i = 0; i < _nextSlot; i++)
                {
                    var cp = _pools[i];
                    if (cp.Pool == pool)
                    {
                        poolIndex = i;
                        break;
                    }
                }

                // There was a connection, which was using an already deleted pool. Nothing else to do here.
                if (poolIndex == -1)
                    return;

                var newPools = new (string Key, ConnectorPool Pool)[_pools.Length];
                Array.Copy(_pools, newPools, _pools.Length);

                for (var i = poolIndex; i < _nextSlot; i++)
                {
                    var nextCp = i + 1 < _nextSlot ? newPools[i + 1] : (null!, null!);
                    newPools[i] = nextCp;
                }

                Interlocked.Decrement(ref _nextSlot);
                _pools = newPools;
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
                    if (cp.Key == null)
                        return;
                    cp.Pool?.Clear();
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
                _pools = new (string, ConnectorPool)[InitialPoolsSize];
                _nextSlot = 0;
            }
        }
    }
}
