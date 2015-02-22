//  Copyright (C) 2002 The Npgsql Development Team
//  npgsql-general@gborg.postgresql.org
//  http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
//
//  ConnectorPool.cs
// ------------------------------------------------------------------
//  Status
//      0.00.0000 - 06/17/2002 - ulrich sprick - creation
//                - 05/??/2004 - Glen Parker<glenebob@nwlink.com> rewritten using
//                               System.Queue.

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Npgsql
{
    /// <summary>
    /// This class manages all connector objects, pooled AND non-pooled.
    /// </summary>
    internal class NpgsqlConnectorPool
    {
        /// <summary>
        /// Maintains information for the pool about a connector
        /// </summary>
        private class PooledConnector
        {
            public NpgsqlConnector Connector { get; private set; }
            public DateTime ExpirationDateTime { get; set; }

            public PooledConnector(NpgsqlConnector connector, DateTime expirationDateTime)
            {
                Connector = connector;
                ExpirationDateTime = expirationDateTime;
            }
        }

        /// <summary>
        /// A queue with an extra Int32 for keeping track of busy connections.
        /// </summary>
        private class ConnectorQueue
        {
            /// <summary>
            /// Connections available to the end user
            /// </summary>
            public LinkedList<PooledConnector> Available = new LinkedList<PooledConnector>();

            /// <summary>
            /// Connections currently in use
            /// </summary>
            public Dictionary<NpgsqlConnector, PooledConnector> Busy = new Dictionary<NpgsqlConnector, PooledConnector>();

            public Int32 ConnectionLifeTime { get; private set; }
            public Int32 InactiveTime = 0;
            public Int32 MinPoolSize;

            public delegate void CleanupInactiveConnectorsHandler(ConnectorQueue queue);
            public event CleanupInactiveConnectorsHandler CleanupInactiveConnectorsTick;

            /// <value>Timer for tracking unused connections in pools.</value>
            // I used System.Timers.Timer because of bad experience with System.Threading.Timer
            // on Windows - it's going mad sometimes and don't respect interval was set.
            private Timer Timer;
            private object timerLock = new object();

            public ConnectorQueue(Int32 connectionLifeTime)
            {
                ConnectionLifeTime = connectionLifeTime;

                // If the connection life time of this connection is not set
                // there is no need to periodically close connections.
                if (connectionLifeTime > 0)
                {
                    Timer = new Timer(connectionLifeTime * 1000);
                    Timer.AutoReset = false;
                    Timer.Elapsed += new ElapsedEventHandler(TimerElapsedHandler);
                    Timer.Start();
                }
            }

            public void TimerElapsedHandler(object sender, ElapsedEventArgs e)
            {
                if (CleanupInactiveConnectorsTick != null)
                {
                    CleanupInactiveConnectorsTick(this);
                }
            }

            public void StartTimer()
            {
                if (Timer != null)
                {
                    lock (timerLock)
                    {
                        Timer.Start();
                    }
                }
            }

            public void StopTimer()
            {
                if (Timer != null)
                {
                    lock (timerLock)
                    {
                        Timer.Stop();
                    }
                }
            }
        }

        /// <value>Unique static instance of the connector pool
        /// mamager.</value>
        internal static NpgsqlConnectorPool ConnectorPoolMgr = new NpgsqlConnectorPool();

        private object locker = new object();

        public NpgsqlConnectorPool()
        {
            PooledConnectors = new Dictionary<string, ConnectorQueue>();
        }

        private void TimerElapsedHandler(ConnectorQueue queue)
        {
            if (queue == null) return;

            try
            {
                // Determine if we need to process this queue at all.  If not a lock can be skipped.
                if (queue.Available.Count > 0 && queue.Available.Count + queue.Busy.Count > queue.MinPoolSize)
                {
                    lock (queue)
                    {
                        // Check again after getting the lock.
                        if (queue.Available.Count > 0 && queue.Available.Count + queue.Busy.Count > queue.MinPoolSize)
                        {
                            // Determine the maximum number of connectors that could be closed.
                            Int32 diff = queue.Available.Count + queue.Busy.Count - queue.MinPoolSize;

                            // Only close at most half of the closable connectors at a time.
                            Int32 toBeClosed = (diff + 1) / 2;
                            toBeClosed = Math.Min(toBeClosed, queue.Available.Count);

                            // Start from the end of the list because that's where the old connectors should be
                            // if there are any.
                            LinkedListNode<PooledConnector> currentNode = queue.Available.Last;
                            LinkedListNode<PooledConnector> previousNode = null;
                            Int32 closedConnectors = 0;

                            while (toBeClosed > closedConnectors)
                            {
                                // Get a reference to the previous node now because it will be gone
                                // After the current node is removed from the list.
                                previousNode = currentNode.Previous;

                                if (currentNode.Value.ExpirationDateTime <= DateTime.Now)
                                {
                                    // Remove the connector from the available pool.
                                    queue.Available.Remove(currentNode);

                                    currentNode.Value.Connector.Close();

                                    // Keep track of how many have been closed so that
                                    // too many aren't closed in one tick.
                                    closedConnectors++;
                                }

                                // Keep processing if there are more nodes.
                                if (previousNode != null)
                                {
                                    currentNode = previousNode;
                                }
                                else
                                {
                                    break;
                                }
                            }                            
                        }
                    }
                }
            }
            finally
            {
                if (queue.Available.Count > 0 || queue.Busy.Count > 0)
                    queue.StartTimer();
                else
                    queue.StopTimer();
            }
        }

        /// <value>Map of index to unused pooled connectors, avaliable to the
        /// next RequestConnector() call.</value>
        /// <remarks>This hashmap will be indexed by connection string.
        /// This key will hold a list of queues of pooled connectors available to be used.</remarks>
        private readonly Dictionary<string, ConnectorQueue> PooledConnectors;


        /// <summary>
        /// Searches the pooled connector lists for a matching connector object or creates a new one.
        /// </summary>
        /// <param name="Connection">The NpgsqlConnection that is requesting
        /// the connector. Its ConnectionString will be used to search the
        /// pool for available connectors.</param>
        /// <returns>A connector object.</returns>
        public NpgsqlConnector RequestConnector(NpgsqlConnection Connection)
        {
            NpgsqlConnector Connector;
            Int32 timeoutMilliseconds = Connection.Timeout * 1000;

            // No need for this lock anymore
            //lock (this)
            {
                Connector = GetPooledConnector(Connection);
            }

            while (Connector == null && timeoutMilliseconds > 0)
            {

                Int32 ST = timeoutMilliseconds > 1000 ? 1000 : timeoutMilliseconds;

                Thread.Sleep(ST);
                timeoutMilliseconds -= ST;

                //lock (this)
                {
                    Connector = GetPooledConnector(Connection);
                }
            }

            if (Connector == null)
            {
                if (Connection.Timeout > 0)
                {
                    throw new Exception("Timeout while getting a connection from pool.");
                }
                else
                {
                    throw new Exception("Connection pool exceeds maximum size.");
                }
            }

            return Connector;
        }

        private delegate void CleanUpConnectorDel(NpgsqlConnection Connection, NpgsqlConnector Connector);

        private void CleanUpConnectorMethod(NpgsqlConnection Connection, NpgsqlConnector Connector)
        {
            try
            {
                Connector.CurrentReader.Close();
                Connector.CurrentReader = null;
                ReleaseConnector(Connection, Connector);
            }
            catch
            {
            }
        }

        private void CleanUpConnector(NpgsqlConnection Connection, NpgsqlConnector Connector)
        {
            new CleanUpConnectorDel(CleanUpConnectorMethod).BeginInvoke(Connection, Connector, null, null);
        }

        /// <summary>
        /// Releases a connector, possibly back to the pool for future use.
        /// </summary>
        /// <remarks>
        /// Pooled connectors will be put back into the pool if there is room.
        /// </remarks>
        /// <param name="Connection">Connection to which the connector is leased.</param>
        /// <param name="Connector">The connector to release.</param>
        public void ReleaseConnector(NpgsqlConnection Connection, NpgsqlConnector Connector)
        {
            //We can only clean up a connector with a reader if the current thread hasn't been aborted
            //If it has then we need to just close it (ReleasePooledConnector will do this for an aborted thread)
            if (Connector.CurrentReader != null && (Thread.CurrentThread.ThreadState & (ThreadState.Aborted | ThreadState.AbortRequested)) == 0)
            {
                CleanUpConnector(Connection, Connector);
            }
            else
            {
                //lock (this)
                {
                    UngetConnector(Connection, Connector);
                }
            }
        }

        /// <summary>
        /// Find an available pooled connector in the pool, or create a new one if none found.
        /// </summary>
        private NpgsqlConnector GetPooledConnector(NpgsqlConnection Connection)
        {
            ConnectorQueue Queue = null;
            NpgsqlConnector Connector = null;

            do
            {
                if (Connector != null)
                {
                    //This means Connector was found to be invalid at the end of the loop

                    lock (Queue)
                    {
                        Queue.Busy.Remove(Connector);
                    }

                    Connector.Close();
                    Connector = null;
                }

                // We only need to lock all pools when trying to get one pool or create one.

                lock (locker)
                {

                    // Try to find a queue.
                    if (!PooledConnectors.TryGetValue(Connection.ConnectionString, out Queue))
                    {
                        Queue = new ConnectorQueue(Connection.ConnectionLifeTime);
                        Queue.MinPoolSize = Connection.MinPoolSize;
                        Queue.CleanupInactiveConnectorsTick += TimerElapsedHandler;
                        PooledConnectors[Connection.ConnectionString] = Queue;
                    }
                }

                // Now we can simply lock on the pool itself.
                if (Queue.Available.Count > 0)
                {
                    lock (Queue)
                    {
                        if (Queue.Available.Count > 0)
                        {
                            // Found a queue with connectors.  Grab the top one.

                            // Check if the connector is still valid.
                            PooledConnector pooledConnector = Queue.Available.First.Value;
                            Connector = pooledConnector.Connector;
                            Queue.Available.RemoveFirst();
                            Queue.Busy.Add(pooledConnector.Connector, pooledConnector);
                        }
                    }
                }

            } while (Connector != null && !Connector.IsValid());

            if (Connector != null) return Connector;


            if (Queue.Available.Count + Queue.Busy.Count < Connection.MaxPoolSize)
            {
                lock (Queue)
                {
                    if (Queue.Available.Count + Queue.Busy.Count < Connection.MaxPoolSize)
                    {
                        Connector = new NpgsqlConnector(Connection);
                        Queue.Busy.Add(Connector, new PooledConnector(Connector, DateTime.Now.AddSeconds(Queue.ConnectionLifeTime)));
                    }
                }
            }

            if (Connector != null)
            {

                Connector.ProvideClientCertificatesCallback += Connection.ProvideClientCertificatesCallbackDelegate;
                Connector.CertificateSelectionCallback += Connection.CertificateSelectionCallbackDelegate;
                Connector.CertificateValidationCallback += Connection.CertificateValidationCallbackDelegate;
                Connector.PrivateKeySelectionCallback += Connection.PrivateKeySelectionCallbackDelegate;
                Connector.ValidateRemoteCertificateCallback += Connection.ValidateRemoteCertificateCallbackDelegate;

                try
                {
                    Connector.Open();
                }
                catch
                {
                    lock (Queue)
                    {
                        Queue.Busy.Remove(Connector);
                    }

                    Connector.Close();

                    throw;
                }

                // Meet the MinPoolSize requirement if needed.
                if (Connection.MinPoolSize > 1)
                {
                    if (Queue.Available.Count + Queue.Busy.Count < Connection.MinPoolSize)
                    {
                        lock (Queue)
                        {
                            while (Queue.Available.Count + Queue.Busy.Count < Connection.MinPoolSize)
                            {
                                NpgsqlConnector Spare = new NpgsqlConnector(Connection);

                                Spare.ProvideClientCertificatesCallback += Connection.ProvideClientCertificatesCallbackDelegate;
                                Spare.CertificateSelectionCallback += Connection.CertificateSelectionCallbackDelegate;
                                Spare.CertificateValidationCallback += Connection.CertificateValidationCallbackDelegate;
                                Spare.PrivateKeySelectionCallback += Connection.PrivateKeySelectionCallbackDelegate;
                                Spare.ValidateRemoteCertificateCallback += Connection.ValidateRemoteCertificateCallbackDelegate;

                                Spare.Open();

                                Spare.ProvideClientCertificatesCallback -= Connection.ProvideClientCertificatesCallbackDelegate;
                                Spare.CertificateSelectionCallback -= Connection.CertificateSelectionCallbackDelegate;
                                Spare.CertificateValidationCallback -= Connection.CertificateValidationCallbackDelegate;
                                Spare.PrivateKeySelectionCallback -= Connection.PrivateKeySelectionCallbackDelegate;
                                Spare.ValidateRemoteCertificateCallback -= Connection.ValidateRemoteCertificateCallbackDelegate;

                                Queue.Available.AddLast(new PooledConnector(Spare, DateTime.Now.AddSeconds(Connection.ConnectionLifeTime)));
                            }

                            Queue.StartTimer();
                        }
                    }
                }
            }

            return Connector;
        }

        /// <summary>
        /// Put a pooled connector into the pool queue.
        /// </summary>
        /// <param name="Connection">Connection <paramref name="Connector"/> is leased to.</param>
        /// <param name="Connector">Connector to pool</param>
        private void UngetConnector(NpgsqlConnection Connection, NpgsqlConnector Connector)
        {
            ConnectorQueue queue;

            // Find the queue.
            // As we are handling all possible queues, we have to lock everything...
            lock (locker)
            {
                PooledConnectors.TryGetValue(Connection.ConnectionString, out queue);
            }

            if (queue == null)
            {
                Connector.Close(); // Release connection to postgres
                return; // Queue may be emptied by connection problems. See ClearPool below.
            }

            Connector.ProvideClientCertificatesCallback -= Connection.ProvideClientCertificatesCallbackDelegate;
            Connector.CertificateSelectionCallback -= Connection.CertificateSelectionCallbackDelegate;
            Connector.CertificateValidationCallback -= Connection.CertificateValidationCallbackDelegate;
            Connector.PrivateKeySelectionCallback -= Connection.PrivateKeySelectionCallbackDelegate;
            Connector.ValidateRemoteCertificateCallback -= Connection.ValidateRemoteCertificateCallbackDelegate;

            /*bool inQueue = false;

            lock (queue)
            {
                inQueue = queue.Busy.ContainsKey(Connector);
                queue.Busy.Remove(Connector);
            }
            */

            if (!Connector.IsConnected)
            {
                if (Connector.Transaction != null)
                {
                    Connector.ClearTransaction();
                }

                Connector.Close();
            }
            else
            {
                if (Connector.Transaction != null)
                {
                    try
                    {
                        Connector.Transaction.Rollback();
                    }
                    catch
                    {
                        Connector.Close();
                    }
                }
            }

            bool inQueue = queue.Busy.ContainsKey(Connector);

            if (Connector.State == ConnectorState.Ready)
            {
                //If thread is good
                if ((Thread.CurrentThread.ThreadState & (ThreadState.Aborted | ThreadState.AbortRequested)) == 0)
                {
                    // Release all resources associated with this connector.
                    try
                    {
                        Connector.ReleaseResources();
                    }
                    catch (Exception)
                    {
                        //If the connector fails to release its resources then it is probably broken, so make sure we don't add it to the queue.
                        // Usually it already won't be in the queue as it would of broken earlier
                        inQueue = false;
                        Connector.Close();
                    }

                }
                else
                {
                    //Thread is being aborted, this connection is possibly broken. So kill it rather than returning it to the pool
                    inQueue = false;
                    Connector.Close();
                }
            }

            // Check if Connector should return to the queue of available connectors. If not, this connector is invalid and should
            // only be removed from the busy queue which effectvely removes it from the pool.
            if (inQueue)
                lock (queue)
                {
                    PooledConnector pooledConnector = null;
                    if (queue.Busy.TryGetValue(Connector, out pooledConnector))
                    {
                        queue.Busy.Remove(Connector);

                        // Set the new expiration time of the connection and add it the front
                        // of the available pool.
                        pooledConnector.ExpirationDateTime = DateTime.Now.AddSeconds(queue.ConnectionLifeTime);
                        queue.Available.AddFirst(pooledConnector);

                        queue.StartTimer();
                    }
                }
            else
                lock (queue)
                {
                    queue.Busy.Remove(Connector);
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
                    NpgsqlConnector connector = Queue.Available.First.Value.Connector;
                    Queue.Available.RemoveFirst();

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
    }
}
