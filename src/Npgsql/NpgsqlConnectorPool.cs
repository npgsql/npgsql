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
            Timer.Start();
            
        }
        
        ~NpgsqlConnectorPool()
        {
            Timer.Stop();

        }

        private void TimerElapsedHandler(object sender, ElapsedEventArgs e)
        {
            NpgsqlConnector Connector;
            
            try
            {
                lock (locker)
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
                            }
                            else
                            {
                                Queue.InactiveTime = 0;
                            }
                        }
                    }
                }
            }
            finally
            {
                Timer.Start();
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
        private readonly Timer Timer;

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

            if (Connection.Pooling)
            {
                Connector = RequestPooledConnector(Connection);
            }
            else
            {
                Connector = GetNonPooledConnector(Connection);
            }

            return Connector;
        }

        /// <summary>
        /// Find a pooled connector.  Handle locking and timeout here.
        /// </summary>
        private NpgsqlConnector RequestPooledConnector(NpgsqlConnection Connection)
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
        /// <param name="Connector">The connector to release.</param>
        public void ReleaseConnector(NpgsqlConnection Connection, NpgsqlConnector Connector)
        {
            //We can only clean up a connector with a reader if the current thread hasn't been aborted
            //If it has then we need to just close it (ReleasePooledConnector will do this for an aborted thread)
            if (Connector.CurrentReader != null && (Thread.CurrentThread.ThreadState & (ThreadState.Aborted | ThreadState.AbortRequested)) == 0)
            {
                CleanUpConnector(Connection, Connector);
            }
            else if (Connector.Pooled)
            {
                ReleasePooledConnector(Connection, Connector);
            }
            else
            {
                UngetNonPooledConnector(Connection, Connector);
            }
        }

        /// <summary>
        /// Release a pooled connector.  Handle locking here.
        /// </summary>
        private void ReleasePooledConnector(NpgsqlConnection Connection, NpgsqlConnector Connector)
        {
            //lock (this)
            {
                ReleasePooledConnectorInternal(Connection, Connector);
            }
        }

        /// <summary>
        /// Release a pooled connector.  Handle shared/non-shared here.
        /// </summary>
        private void ReleasePooledConnectorInternal(NpgsqlConnection Connection, NpgsqlConnector Connector)
        {
            if (!Connector.Shared)
            {
                UngetPooledConnector(Connection, Connector);
            }
            else
            {
                // Connection sharing? What's that?
                throw new NotImplementedException("Internal: Shared pooling not implemented");
            }
        }

        /// <summary>
        /// Create a connector without any pooling functionality.
        /// </summary>
        private static NpgsqlConnector GetNonPooledConnector(NpgsqlConnection Connection)
        {
            NpgsqlConnector Connector;

            Connector = CreateConnector(Connection);

            Connector.ProvideClientCertificatesCallback += Connection.ProvideClientCertificatesCallbackDelegate;
            Connector.CertificateSelectionCallback += Connection.CertificateSelectionCallbackDelegate;
            Connector.CertificateValidationCallback += Connection.CertificateValidationCallbackDelegate;
            Connector.PrivateKeySelectionCallback += Connection.PrivateKeySelectionCallbackDelegate;

            Connector.Open();

            return Connector;
        }


        /// <summary>
        /// Find an available pooled connector in the non-shared pool, or create
        /// a new one if none found.
        /// </summary>
        private NpgsqlConnector GetPooledConnector(NpgsqlConnection Connection)
        {
            ConnectorQueue Queue;
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

            if (Connector != null)
            {
                if (!Connector.IsValid())
                {
                    lock (Queue)
                    {
                        Queue.Busy.Remove(Connector);
                    }


                    Connector.Close();
                    return GetPooledConnector(Connection); //Try again
                }

                return Connector;
            }


            lock (Queue)
            {

                if (Queue.Available.Count + Queue.Busy.Count < Connection.MaxPoolSize)
                {
                    Connector = CreateConnector(Connection);
                    Queue.Busy.Add(Connector, null);

                }
                
            }


            if (Connector != null)
            {

                Connector.ProvideClientCertificatesCallback += Connection.ProvideClientCertificatesCallbackDelegate;
                Connector.CertificateSelectionCallback += Connection.CertificateSelectionCallbackDelegate;
                Connector.CertificateValidationCallback += Connection.CertificateValidationCallbackDelegate;
                Connector.PrivateKeySelectionCallback += Connection.PrivateKeySelectionCallbackDelegate;

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
                            NpgsqlConnector Spare = CreateConnector(Connection);

                            Spare.ProvideClientCertificatesCallback += Connection.ProvideClientCertificatesCallbackDelegate;
                            Spare.CertificateSelectionCallback += Connection.CertificateSelectionCallbackDelegate;
                            Spare.CertificateValidationCallback += Connection.CertificateValidationCallbackDelegate;
                            Spare.PrivateKeySelectionCallback += Connection.PrivateKeySelectionCallbackDelegate;

                            Spare.Open();

                            Spare.ProvideClientCertificatesCallback -= Connection.ProvideClientCertificatesCallbackDelegate;
                            Spare.CertificateSelectionCallback -= Connection.CertificateSelectionCallbackDelegate;
                            Spare.CertificateValidationCallback -= Connection.CertificateValidationCallbackDelegate;
                            Spare.PrivateKeySelectionCallback -= Connection.PrivateKeySelectionCallbackDelegate;

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

        private static NpgsqlConnector CreateConnector(NpgsqlConnection Connection)
        {
            return new NpgsqlConnector(Connection.ConnectionStringValues.Clone(), Connection.Pooling, false);
        }


        /// <summary>
        /// This method is only called when NpgsqlConnection.Dispose(false) is called which means a
        /// finalization. This also means, an NpgsqlConnection was leak. We clear pool count so that
        /// client doesn't end running out of connections from pool. When the connection is finalized, its underlying
        /// socket is closed.
        /// </summary>
        public void FixPoolCountBecauseOfConnectionDisposeFalse(NpgsqlConnection Connection)
        {
            ConnectorQueue Queue;

            // Prevent multithread access to connection pool count.
            lock (locker)
            {
                // Try to find a queue.
                if (PooledConnectors.TryGetValue(Connection.ConnectionString, out Queue) && Queue != null)
                {
                    Queue.Busy.Remove(Connection.Connector);
                }
            }
        }

        /// <summary>
        /// Close the connector.
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Connector">Connector to release</param>
        private static void UngetNonPooledConnector(NpgsqlConnection Connection, NpgsqlConnector Connector)
        {
            Connector.ProvideClientCertificatesCallback -= Connection.ProvideClientCertificatesCallbackDelegate;
            Connector.CertificateSelectionCallback -= Connection.CertificateSelectionCallbackDelegate;
            Connector.CertificateValidationCallback -= Connection.CertificateValidationCallbackDelegate;
            Connector.PrivateKeySelectionCallback -= Connection.PrivateKeySelectionCallbackDelegate;

            if (Connector.Transaction != null)
            {
                Connector.Transaction.Cancel();
            }

            Connector.Close();
        }

        /// <summary>
        /// Put a pooled connector into the pool queue.
        /// </summary>
        /// <param name="Connector">Connector to pool</param>
        private void UngetPooledConnector(NpgsqlConnection Connection, NpgsqlConnector Connector)
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

            bool inQueue = false;

            lock (queue)
            {
                inQueue = queue.Busy.ContainsKey(Connector);
                queue.Busy.Remove(Connector);
            }

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
                    }

                    if (inQueue)
                        lock (queue)
                        {
                            queue.Available.Enqueue(Connector);
                        }
                    else
                        Connector.Close();
                }
                else
                {
                    //Thread is being aborted, this connection is possibly broken. So kill it rather than returning it to the pool
                    Connector.Close();
                }
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
