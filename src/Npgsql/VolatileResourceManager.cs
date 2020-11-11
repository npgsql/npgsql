using System;
using System.Diagnostics;
using System.Threading;
using System.Transactions;
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
        NpgsqlConnector _connector;
        Transaction _transaction;
        readonly string _txId;
        NpgsqlTransaction _localTx;
        string? _preparedTxName;
        bool IsPrepared => _preparedTxName != null;
        bool _isDisposed;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(VolatileResourceManager));

        const int MaximumRollbackAttempts = 20;

        internal VolatileResourceManager(NpgsqlConnection connection, Transaction transaction)
        {
            _connector = connection.Connector!;
            _transaction = transaction;
            // _tx gets disposed by System.Transactions at some point, but we want to be able to log its local ID
            _txId = transaction.TransactionInformation.LocalIdentifier;
            _localTx = connection.BeginTransaction(ConvertIsolationLevel(_transaction.IsolationLevel));
        }

        public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            CheckDisposed();
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
                    using var conn2 = (NpgsqlConnection)((ICloneable)_connector.Connection).Clone();
                    conn2.Open();

                    var connector = conn2.Connector!;
                    using (connector.StartUserAction())
                        connector.ExecuteInternalCommand($"COMMIT PREPARED '{_preparedTxName}'");
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
                    _connector.PerformPostgresCancellation();
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
                using var conn2 = (NpgsqlConnection)((ICloneable)_connector.Connection).Clone();
                conn2.Open();

                var connector = conn2.Connector!;
                using (connector.StartUserAction())
                    connector.ExecuteInternalCommand($"ROLLBACK PREPARED '{_preparedTxName}'");
            }
        }

        #region Dispose/Cleanup

#pragma warning disable CS8625
        void Dispose()
        {
            if (_isDisposed)
                return;

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
                    pool!.TryRemovePendingEnlistedConnector(_connector, _transaction);
                    pool.Return(_connector);
                }
                else
                    _connector.Close();
            }

            _connector = null!;
            _transaction = null!;
            _isDisposed = true;
        }
#pragma warning restore CS8625

        void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(VolatileResourceManager));
        }

        #endregion

        static System.Data.IsolationLevel ConvertIsolationLevel(IsolationLevel isolationLevel)
            => isolationLevel switch
            {
                IsolationLevel.Chaos           => System.Data.IsolationLevel.Chaos,
                IsolationLevel.ReadCommitted   => System.Data.IsolationLevel.ReadCommitted,
                IsolationLevel.ReadUncommitted => System.Data.IsolationLevel.ReadUncommitted,
                IsolationLevel.RepeatableRead  => System.Data.IsolationLevel.RepeatableRead,
                IsolationLevel.Serializable    => System.Data.IsolationLevel.Serializable,
                IsolationLevel.Snapshot        => System.Data.IsolationLevel.Snapshot,
                _                              => System.Data.IsolationLevel.Unspecified
            };
    }
}
