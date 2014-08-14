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
        internal static NpgsqlConnectorPool ConnectorPoolMgr = new NpgsqlConnectorPool();

        private object locker = new object();

        public NpgsqlConnectorPool()
        {
            PooledConnectors = new Dictionary<string, ConnectorQueue>();

            Timer = new Timer(1000);
            Timer.AutoReset = false;
            Timer.Elapsed += new ElapsedEventHandler(TimerElapsedHandler);
        }

        private void StartTimer()
        {
            lock (locker)
            {
                Timer.Start();
            }
        }

        private void TimerElapsedHandler(object sender, ElapsedEventArgs e)
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
                        Timer.Start();
                    else
                        Timer.Stop();
                }
            }
        }

        /// <value>Map of index to unused pooled connectors, avaliable to the
        /// next RequestConnector() call.</value>
        /// <remarks>This hashmap will be indexed by connection string.
        /// This key will hold a list of queues of pooled connectors available to be used.</remarks>
        private readonly Dictionary<string, ConnectorQueue> PooledConnectors;

        /*/// <value>Map of shared connectors, avaliable to the
        /// next RequestConnector() call.</value>
        /// <remarks>This hashmap will be indexed by connection string.
        /// This key will hold a list of shared connectors available to be used.</remarks>
        // To be implemented
        //private Dictionary<?, ?> SharedConnectors;*/

        /// <value>Timer for tracking unused connections in pools.</value>
        // I used System.Timers.Timer because of bad experience with System.Threading.Timer
        // on Windows - it's going mad sometimes and don't respect interval was set.
        private Timer Timer;

        /// <summary>
        /// Searches the shared and pooled connector lists for a
        /// matching connector object or creates a new one.
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
                Connector = RequestPooledConnectorInternal(Connection);
            }

            while (Connector == null && timeoutMilliseconds > 0)
            {

                Int32 ST = timeoutMilliseconds > 1000 ? 1000 : timeoutMilliseconds;

                Thread.Sleep(ST);
                timeoutMilliseconds -= ST;

                //lock (this)
                {
                    Connector = RequestPooledConnectorInternal(Connection);
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

            StartTimer();

            return Connector;
        }

        /// <summary>
        /// Find a pooled connector.  Handle shared/non-shared here.
        /// </summary>
        private NpgsqlConnector RequestPooledConnectorInternal(NpgsqlConnection Connection)
        {
            NpgsqlConnector Connector = null;
            Boolean Shared = false;

            // If sharing were implemented, I suppose Shared would be set based
            // on some property on the Connection.

            if (Shared)
            {
                // Connection sharing? What's that?
                throw new NotImplementedException("Internal: Shared pooling not implemented");

            }
            Connector = GetPooledConnector(Connection);

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
        /// Shared connectors should just have their use count decremented
        /// since they always stay in the shared pool.
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
                    ReleaseConnectorInternal(Connection, Connector);
                }
            }
        }

        /// <summary>
        /// Release a pooled connector.  Handle shared/non-shared here.
        /// </summary>
        private void ReleaseConnectorInternal(NpgsqlConnection Connection, NpgsqlConnector Connector)
        {
            if (!Connector.Shared)
            {
                UngetConnector(Connection, Connector);
            }
            else
            {
                // Connection sharing? What's that?
                throw new NotImplementedException("Internal: Shared pooling not implemented");
            }
        }

        /// <summary>
        /// Find an available pooled connector in the non-shared pool, or create
        /// a new one if none found.
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

            } while (Connector != null && !Connector.IsValid());

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

                            Queue.Available.Enqueue(Spare);
                        }
                    }
                }
            }

            return Connector;
        }

        /*
                /// <summary>
                /// Find an available shared connector in the shared pool, or create
                /// a new one if none found.
                /// </summary>
                private NpgsqlConnector GetSharedConnector(NpgsqlConnection Connection)
                {
                    // To be implemented

                    return null;
                }
        */

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

            if (!Connector.IsInitialized)
            {
                if (Connector.Transaction != null)
                {
                    Connector.Transaction.Cancel();
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

            if (Connector.State == ConnectionState.Open)
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
                    queue.Busy.Remove(Connector);
                    queue.Available.Enqueue(Connector);
                }
            else
                lock (queue)
                {
                    queue.Busy.Remove(Connector);
                }

        }

        /*
                /// <summary>
                /// Stop sharing a shared connector.
                /// </summary>
                /// <param name="Connector">Connector to unshare</param>
                private void UngetSharedConnector(NpgsqlConnection Connection, NpgsqlConnector Connector)
                {
                    // To be implemented
                }
        */

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
    }
}
