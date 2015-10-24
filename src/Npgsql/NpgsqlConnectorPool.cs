#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Threading;
using Npgsql.Logging;

namespace Npgsql
{
    /// <summary>
    /// This class manages all connector objects, pooled AND non-pooled.
    /// </summary>
    internal class NpgsqlConnectorPool
    {
        /// <summary>
        /// A queue with an extra Int32 for keeping track of busy connections.
        /// </summary>
        private class ConnectorQueue
        {
            /// <summary>
            /// Connections available to the end user
            /// </summary>
            public Queue<NpgsqlConnector> Available = new Queue<NpgsqlConnector>();

            /// <summary>
            /// Connections currently in use
            /// </summary>
            public Dictionary<NpgsqlConnector, object> Busy = new Dictionary<NpgsqlConnector, object>();

            public Int32 ConnectionLifeTime;
            public Int32 InactiveTime = 0;
            public Int32 MinPoolSize;
        }

        /// <value>Unique static instance of the connector pool
        /// mamager.</value>
        internal static NpgsqlConnectorPool ConnectorPoolMgr;

        /// <summary>
        /// Maximum number of possible connections in the pool.
        /// </summary>
        internal const int PoolSizeLimit = 1024;

        private object locker = new object();

        internal NpgsqlConnectorPool()
        {
            PooledConnectors = new Dictionary<string, ConnectorQueue>();

            _timer = new Timer(TimerElapsedHandler, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void StartTimer()
        {
            lock (locker)
            {
                _timer.Change(TimerInterval, Timeout.Infinite);
            }
        }

        private void TimerElapsedHandler(object sender)
        {
            NpgsqlConnector Connector;
            var activeConnectionsExist = false;

            lock (locker)
            {
                try
                {
                    foreach (ConnectorQueue Queue in PooledConnectors.Values)
                    {
                        lock (Queue)
                        {
                            if (Queue.Available.Count > 0)
                            {
                                if (Queue.Available.Count + Queue.Busy.Count > Queue.MinPoolSize)
                                {
                                    if (Queue.InactiveTime >= Queue.ConnectionLifeTime)
                                    {
                                        Int32 diff = Queue.Available.Count + Queue.Busy.Count - Queue.MinPoolSize;
                                        Int32 toBeClosed = (diff + 1) / 2;
                                        toBeClosed = Math.Min(toBeClosed, Queue.Available.Count);

                                        if (diff < 2)
                                        {
                                            diff = 2;
                                        }

                                        Queue.InactiveTime -= Queue.ConnectionLifeTime / (int)(Math.Log(diff) / Math.Log(2));

                                        for (Int32 i = 0; i < toBeClosed; ++i)
                                        {
                                            Connector = Queue.Available.Dequeue();
                                            Connector.Close();
                                        }
                                    }
                                    else
                                    {
                                        Queue.InactiveTime++;
                                    }
                                }
                                else
                                {
                                    Queue.InactiveTime = 0;
                                }
                                if (Queue.Available.Count > 0 || Queue.Busy.Count > 0)
                                    activeConnectionsExist = true;
                            }
                            else
                            {
                                Queue.InactiveTime = 0;
                            }
                        }
                    }
                }
                finally
                {
                    if (activeConnectionsExist)
                        _timer.Change(TimerInterval, Timeout.Infinite);
                }
            }
        }

        /// <value>Map of index to unused pooled connectors, avaliable to the
        /// next RequestConnector() call.</value>
        /// <remarks>This hashmap will be indexed by connection string.
        /// This key will hold a list of queues of pooled connectors available to be used.</remarks>
        private readonly Dictionary<string, ConnectorQueue> PooledConnectors;

        readonly Timer _timer;

        const int TimerInterval = 1000;

        /// <summary>
        /// Searches the pooled connector lists for a matching connector object or creates a new one.
        /// </summary>
        /// <param name="connection">The NpgsqlConnection that is requesting
        /// the connector. Its ConnectionString will be used to search the
        /// pool for available connectors.</param>
        /// <returns>A connector object.</returns>
        internal NpgsqlConnector RequestConnector(NpgsqlConnection connection)
        {
            if (connection.MaxPoolSize < connection.MinPoolSize)
                throw new ArgumentException(
                    $"Connection can't have MaxPoolSize {connection.MaxPoolSize} under MinPoolSize {connection.MinPoolSize}");
            Contract.Ensures(Contract.Result<NpgsqlConnector>().State == ConnectorState.Ready, "Pool returned a connector with state ");

            NpgsqlConnector connector;
            Int32 timeoutMilliseconds = connection.Timeout * 1000;

            // No need for this lock anymore
            //lock (this)
            {
                connector = GetPooledConnector(connection);
            }

            while (connector == null && timeoutMilliseconds > 0)
            {

                Int32 ST = timeoutMilliseconds > 1000 ? 1000 : timeoutMilliseconds;

                Thread.Sleep(ST);
                timeoutMilliseconds -= ST;

                //lock (this)
                {
                    connector = GetPooledConnector(connection);
                }
            }

            if (connector == null)
            {
                if (connection.Timeout > 0)
                {
                    throw new Exception("Timeout while getting a connection from pool.");
                }
                else
                {
                    throw new Exception("Connection pool exceeds maximum size.");
                }
            }

            connector.Connection = connection;

            StartTimer();

            return connector;
        }

        /// <summary>
        /// Find an available pooled connector in the pool, or create a new one if none found.
        /// </summary>
        private NpgsqlConnector GetPooledConnector(NpgsqlConnection Connection)
        {
            ConnectorQueue Queue = null;
            NpgsqlConnector Connector = null;

            // We only need to lock all pools when trying to get one pool or create one.

            lock (locker)
            {

                // Try to find a queue.
                if (!PooledConnectors.TryGetValue(Connection.ConnectionString, out Queue))
                {

                    Queue = new ConnectorQueue();
                    Queue.ConnectionLifeTime = Connection.ConnectionLifeTime;
                    Queue.MinPoolSize = Connection.MinPoolSize;
                    PooledConnectors[Connection.ConnectionString] = Queue;
                }
            }

            // Now we can simply lock on the pool itself.
            lock (Queue)
            {
                if (Queue.Available.Count > 0)
                {
                    // Found a queue with connectors.  Grab the top one.

                    // Check if the connector is still valid.

                    Connector = Queue.Available.Dequeue();
                    Queue.Busy.Add(Connector, null);
                }
            }

            if (Connector != null) return Connector;

            lock (Queue)
            {
                if (Queue.Available.Count + Queue.Busy.Count < Connection.MaxPoolSize)
                {
                    Connector = new NpgsqlConnector(Connection);
                    Queue.Busy.Add(Connector, null);
                }
            }

            if (Connector != null)
            {
                try
                {
                    Connector.Open();
                }
                catch
                {
                    Contract.Assert(Connector.IsBroken);
                    lock (Queue)
                    {
                        Queue.Busy.Remove(Connector);
                    }

                    throw;
                }

                // Meet the MinPoolSize requirement if needed.
                if (Connection.MinPoolSize > 1)
                {

                    lock (Queue)
                    {

                        while (Queue.Available.Count + Queue.Busy.Count < Connection.MinPoolSize)
                        {
                            NpgsqlConnector spare = new NpgsqlConnector(Connection);
                            spare.Open();
                            spare.Connection = null;
                            Queue.Available.Enqueue(spare);
                        }
                    }
                }
            }

            return Connector;
        }


        /// <summary>
        /// Releases a connector, possibly back to the pool for future use.
        /// </summary>
        /// <remarks>
        /// Pooled connectors will be put back into the pool if there is room.
        /// </remarks>
        /// <param name="connection">Connection to which the connector is leased.</param>
        /// <param name="connector">The connector to release.</param>
        internal void ReleaseConnector(NpgsqlConnection connection, NpgsqlConnector connector)
        {
            Contract.Requires(connector.IsReady || connector.IsClosed || connector.IsBroken);

            ConnectorQueue queue;

            // Find the queue.
            // As we are handling all possible queues, we have to lock everything...
            lock (locker)
            {
                PooledConnectors.TryGetValue(connection.ConnectionString, out queue);
            }

            if (queue == null)
            {
                connector.Close(); // Release connection to postgres
                return; // Queue may be emptied by connection problems. See ClearPool below.
            }

            /*bool inQueue = false;

            lock (queue)
            {
                inQueue = queue.Busy.ContainsKey(Connector);
                queue.Busy.Remove(Connector);
            }
            */

            bool inQueue = queue.Busy.ContainsKey(connector);

            if (connector.IsBroken || connector.IsClosed)
            {
                if (connector.InTransaction)
                {
                    connector.ClearTransaction();
                }

                connector.Close();
                inQueue = false;
            }
            else
            {
                Contract.Assert(connector.IsReady);

                //If thread is good
                if ((Thread.CurrentThread.ThreadState & (ThreadState.Aborted | ThreadState.AbortRequested)) == 0)
                {
                    // Release all resources associated with this connector.
                    try {
                        connector.Reset();
                    } catch {
                        connector.Close();
                        inQueue = false;
                    }
                } else {
                    //Thread is being aborted, this connection is possibly broken. So kill it rather than returning it to the pool
                    inQueue = false;
                    connector.Close();
                }
            }

            // Check if Connector should return to the queue of available connectors. If not, this connector is invalid and should
            // only be removed from the busy queue which effectvely removes it from the pool.
            if (inQueue)
                lock (queue)
                {
                    queue.Busy.Remove(connector);
                    queue.Available.Enqueue(connector);
                }
            else
                lock (queue)
                {
                    queue.Busy.Remove(connector);
                }
        }

        private static void ClearQueue(ConnectorQueue Queue)
        {
            if (Queue == null)
            {
                return;
            }

            lock (Queue)
            {
                while (Queue.Available.Count > 0)
                {
                    NpgsqlConnector connector = Queue.Available.Dequeue();

                    try
                    {
                        connector.Close();
                    }
                    catch
                    {
                        // Maybe we should log something here to say we got an exception while closing connector?
                    }
                }

                //Clear the busy list so that the current connections don't get re-added to the queue
                Queue.Busy.Clear();
            }
        }

        internal void ClearPool(NpgsqlConnection Connection)
        {
            // Prevent multithread access to connection pool count.
            lock (locker)
            {
                ConnectorQueue queue;
                // Try to find a queue.
                if (PooledConnectors.TryGetValue(Connection.ConnectionString, out queue))
                {
                    ClearQueue(queue);

                    PooledConnectors.Remove(Connection.ConnectionString);
                }
            }
        }

        internal void ClearAllPools()
        {
            lock (locker)
            {
                foreach (ConnectorQueue Queue in PooledConnectors.Values)
                {
                    ClearQueue(Queue);
                }
                PooledConnectors.Clear();
            }
        }

        static NpgsqlConnectorPool()
        {
            ConnectorPoolMgr = new NpgsqlConnectorPool();
#if NET45 || NET452 || DNX452
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => { Thread.Sleep(3); ConnectorPoolMgr.ClearAllPools(); };
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => { Thread.Sleep(3); ConnectorPoolMgr.ClearAllPools(); };
#endif
        }
    }
}
