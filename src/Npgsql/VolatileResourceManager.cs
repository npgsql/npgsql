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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Transactions;
using JetBrains.Annotations;
using Npgsql.Logging;

namespace Npgsql
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Note that a connection may be closed before its TransactionScope completes. In this case we close the NpgsqlConnection
    /// as usual but the connector in a special list in the pool; it will be closed only when the scope completes.
    /// </remarks>
    class VolatileResourceManager : ISinglePhaseNotification
    {
        [CanBeNull] NpgsqlConnector _connector;
        [CanBeNull] Transaction _transaction;
        [CanBeNull] readonly string _txId;
        [CanBeNull] NpgsqlTransaction _localTx;
        [CanBeNull] string _preparedTxName;
        bool IsPrepared => _preparedTxName != null;
        bool _isDisposed;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        const int MaximumRollbackAttempts = 20;

        internal VolatileResourceManager(NpgsqlConnection connection, [NotNull] Transaction transaction)
        {
            _connector = connection.Connector;
            _transaction = transaction;
            // _tx gets disposed by System.Transactions at some point, but we want to be able to log its local ID
            _txId = transaction.TransactionInformation.LocalIdentifier;
            _localTx = connection.BeginTransaction(ConvertIsolationLevel(_transaction.IsolationLevel));
        }

        public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            CheckDisposed();

            Debug.Assert(_transaction != null, "No transaction");
            Debug.Assert(_localTx != null, "No local transaction");
            Debug.Assert(_connector != null, "No connector");

            Log.Debug($"Single Phase Commit (localid={_txId})", _connector.Id);

            try
            {
                _localTx.Commit();
                singlePhaseEnlistment.Committed();
            }
            catch (PostgresException e)
            {
                singlePhaseEnlistment.Aborted(e);
            }
            catch (Exception e)
            {
                singlePhaseEnlistment.InDoubt(e);
            }
            finally
            {
                Dispose();
            }
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            CheckDisposed();
            Debug.Assert(_transaction != null, "No transaction");
            Debug.Assert(_localTx != null, "No local transaction");
            Debug.Assert(_connector != null, "No connector");

            Log.Debug($"Two-phase transaction prepare (localid={_txId})", _connector.Id);

            // The PostgreSQL prepared transaction name is the distributed GUID + our connection's process ID, for uniqueness
            _preparedTxName = $"{_transaction.TransactionInformation.DistributedIdentifier}/{_connector.BackendProcessId}";

            try
            {
                using (_connector.StartUserAction())
                    _connector.ExecuteInternalCommand($"PREPARE TRANSACTION '{_preparedTxName}'");

                // The MSDTC, which manages escalated distributed transactions, performs the 2nd phase
                // asynchronously - this means that TransactionScope.Dispose() will return before all
                // resource managers have actually commit.
                // If the same connection tries to enlist to a new TransactionScope immediately after
                // disposing an old TransactionScope, its EnlistedTransaction must have been cleared
                // (or we'll throw a double enlistment exception). This must be done here at the 1st phase
                // (which is sync).
                if (_connector.Connection != null)
                    _connector.Connection.EnlistedTransaction = null;

                preparingEnlistment.Prepared();
            }
            catch (Exception e)
            {
                Dispose();
                preparingEnlistment.ForceRollback(e);
            }
        }

        public void Commit(Enlistment enlistment)
        {
            CheckDisposed();
            Debug.Assert(_transaction != null, "No transaction");
            Debug.Assert(_connector != null, "No connector");

            Log.Debug($"Two-phase transaction commit (localid={_txId})", _connector.Id);

            try
            {
                if (_connector.Connection == null)
                {
                    // The connection has been closed before the TransactionScope was disposed.
                    // The connector is unbound from its connection and is sitting in the pool's
                    // pending enlisted connector list. Since there's no risk of the connector being
                    // used by anyone we can executed the 2nd phase on it directly (see below).
                    using (_connector.StartUserAction())
                        _connector.ExecuteInternalCommand($"COMMIT PREPARED '{_preparedTxName}'");
                }
                else
                {
                    // The connection is still open and potentially will be reused by by the user.
                    // The MSDTC, which manages escalated distributed transactions, performs the 2nd phase
                    // asynchronously - this means that TransactionScope.Dispose() will return before all
                    // resource managers have actually commit. This can cause a concurrent connection use scenario
                    // if the user continues to use their connection after disposing the scope, and the MSDTC
                    // requests a commit at that exact time.
                    // To avoid this, we open a new connection for performing the 2nd phase.
                    using (var conn2 = (NpgsqlConnection)((ICloneable)_connector.Connection).Clone())
                    {
                        conn2.Open();
                        var connector = conn2.Connector;
                        Debug.Assert(connector != null);
                        using (connector.StartUserAction())
                            connector.ExecuteInternalCommand($"COMMIT PREPARED '{_preparedTxName}'");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception during two-phase transaction commit (localid={TransactionId})", e, _connector.Id);
            }
            finally
            {
                Dispose();
                enlistment.Done();
            }
        }

        public void Rollback(Enlistment enlistment)
        {
            CheckDisposed();
            Debug.Assert(_transaction != null, "No transaction");
            Debug.Assert(_connector != null, "No connector");

            try
            {
                if (IsPrepared)
                    RollbackTwoPhase();
                else
                    RollbackLocal();
            }
            catch (Exception e)
            {
                Log.Error($"Exception during transaction rollback (localid={_txId})", e, _connector.Id);
            }
            finally
            {
                Dispose();
                enlistment.Done();
            }
        }

        public void InDoubt(Enlistment enlistment)
        {
            Debug.Assert(_transaction != null, "No transaction");
            Debug.Assert(_connector != null, "No connector");

            Log.Warn($"Two-phase transaction in doubt (localid={_txId})", _connector.Id);

            // TODO: Is this the correct behavior?
            try
            {
                RollbackTwoPhase();
            }
            catch (Exception e)
            {
                Log.Error($"Exception during transaction rollback (localid={_txId})", e, _connector.Id);
            }
            finally
            {
                Dispose();
                enlistment.Done();
            }
        }

        void RollbackLocal()
        {
            Debug.Assert(_connector != null, "No connector");
            Debug.Assert(_localTx != null, "No local transaction");

            Log.Debug($"Single-phase transaction rollback (localid={_txId})", _connector.Id);

            var attempt = 0;
            while (true)
            {
                try
                {
                    _localTx.Rollback();
                    return;
                }
                catch (NpgsqlOperationInProgressException)
                {
                    // Repeatedly attempts to rollback, to support timeout-triggered rollbacks that occur
                    // while the connection is busy.

                    // This really shouldn't be necessary, but just in case
                    if (attempt++ == MaximumRollbackAttempts)
                        throw new Exception($"Could not roll back after {MaximumRollbackAttempts} attempts, aborting. Transaction is in an unknown state.");

                    Log.Warn($"Connection in use while trying to rollback, will cancel and retry (localid={_txId}", _connector.Id);
                    _connector.CancelRequest();
                    // Cancellations are asynchronous, give it some time
                    Thread.Sleep(500);
                }
            }
        }

        void RollbackTwoPhase()
        {
            // This only occurs if we've started a two-phase commit but one of the commits has failed.
            Log.Debug($"Two-phase transaction rollback (localid={_txId})", _connector.Id);

            if (_connector.Connection == null)
            {
                // The connection has been closed before the TransactionScope was disposed.
                // The connector is unbound from its connection and is sitting in the pool's
                // pending enlisted connector list. Since there's no risk of the connector being
                // used by anyone we can executed the 2nd phase on it directly (see below).
                using (_connector.StartUserAction())
                    _connector.ExecuteInternalCommand($"ROLLBACK PREPARED '{_preparedTxName}'");
            }
            else
            {
                // The connection is still open and potentially will be reused by by the user.
                // The MSDTC, which manages escalated distributed transactions, performs the 2nd phase
                // asynchronously - this means that TransactionScope.Dispose() will return before all
                // resource managers have actually commit. This can cause a concurrent connection use scenario
                // if the user continues to use their connection after disposing the scope, and the MSDTC
                // requests a commit at that exact time.
                // To avoid this, we open a new connection for performing the 2nd phase.
                using (var conn2 = (NpgsqlConnection)((ICloneable)_connector.Connection).Clone())
                {
                    conn2.Open();
                    var connector = conn2.Connector;
                    Debug.Assert(connector != null);
                    using (connector.StartUserAction())
                        connector.ExecuteInternalCommand($"ROLLBACK PREPARED '{_preparedTxName}'");
                }
            }
        }

        #region Dispose/Cleanup

        void Dispose()
        {
            if (_isDisposed)
                return;
            Debug.Assert(_transaction != null, "No transaction");
            Debug.Assert(_connector != null, "No connector");

            Log.Trace($"Cleaning up resource manager (localid={_txId}", _connector.Id);
            if (_localTx != null)
            {
                _localTx.Dispose();
                _localTx = null;
            }

            if (_connector.Connection != null)
                _connector.Connection.EnlistedTransaction = null;
            else
            {
                // We're here for connections which were closed before their TransactionScope completes.
                // These need to be closed now.
                if (_connector.Settings.Pooling)
                {
                    var found = PoolManager.TryGetValue(_connector.ConnectionString, out var pool);
                    Debug.Assert(found);
                    pool.TryRemovePendingEnlistedConnector(_connector, _transaction);
                    pool.Release(_connector);
                }
                else
                    _connector.Close();
            }

            _connector = null;
            _transaction = null;
            _isDisposed = true;
        }

        void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(VolatileResourceManager));
        }

        #endregion

        static System.Data.IsolationLevel ConvertIsolationLevel(IsolationLevel isolationLevel)
        {
            switch (isolationLevel)
            {
            case IsolationLevel.Chaos:
                return System.Data.IsolationLevel.Chaos;
            case IsolationLevel.ReadCommitted:
                return System.Data.IsolationLevel.ReadCommitted;
            case IsolationLevel.ReadUncommitted:
                return System.Data.IsolationLevel.ReadUncommitted;
            case IsolationLevel.RepeatableRead:
                return System.Data.IsolationLevel.RepeatableRead;
            case IsolationLevel.Serializable:
                return System.Data.IsolationLevel.Serializable;
            case IsolationLevel.Snapshot:
                return System.Data.IsolationLevel.Snapshot;
            case IsolationLevel.Unspecified:
            default:
                return System.Data.IsolationLevel.Unspecified;
            }
        }

    }
}
