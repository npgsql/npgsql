#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Diagnostics;
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

        static (string Key, ConnectorPool Pool)[] _pools = new (string, ConnectorPool)[InitialPoolsSize];
        static int _nextSlot;

        internal static bool TryGetValue(string key, out ConnectorPool pool)
        {
            // Note that pools never get removed. _pools is strictly append-only.
            var pools = _pools;
            var sw = new SpinWait();

            // First scan the pools and do reference equality on the connection strings
            for (var i = 0; i < _nextSlot; i++)
            {
                if (ReferenceEquals(pools[i].Key, key))
                {
                    // It's possible that this pool entry is currently being written: the connection string
                    // component has already been writte, but the pool component is just about to be. So we
                    // loop on the pool until it's non-null
                    while (Volatile.Read(ref pools[i].Pool) == null)
                        sw.SpinOnce();
                    pool = pools[i].Pool;
                    return true;
                }
            }

            // Next try value comparison on the strings
            for (var i = 0; i < _nextSlot; i++)
            {
                if (pools[i].Key == key)
                {
                    // See comment above
                    while (Volatile.Read(ref pools[i].Pool) == null)
                        sw.SpinOnce();
                    pool = pools[i].Pool;
                    return true;
                }
            }

            pool = null;
            return false;
        }

        internal static ConnectorPool GetOrAdd(string key, ConnectorPool pool)
        {
            lock (_pools)
            {
                if (TryGetValue(key, out var result))
                    return result;

                // May need to grow the array.
                if (_nextSlot == _pools.Length)
                {
                    var newPools = new (string, ConnectorPool)[_pools.Length * 2];
                    Array.Copy(_pools, newPools, _pools.Length);
                    Interlocked.Exchange(ref _pools, newPools);
                }

                _pools[_nextSlot].Key = key;
                _pools[_nextSlot].Pool = pool;
                Interlocked.Increment(ref _nextSlot);
                return pool;
            }
        }

        internal static void Clear(string connString)
        {
            Debug.Assert(connString != null);

            if (TryGetValue(connString, out var pool))
                pool.Clear();
        }

        internal static void ClearAll()
        {
            for (var i = 0; i < _nextSlot; i++)
            {
                if (_pools[i].Key == null)
                    return;
                _pools[i].Pool?.Clear();
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
            lock (_pools)
            {
                ClearAll();
                _pools = new (string, ConnectorPool)[InitialPoolsSize];
                _nextSlot = 0;
            }
        }
    }
}
