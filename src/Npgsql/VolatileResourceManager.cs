#if NET45 || NET451
#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using Microsoft.Extensions.Logging;
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

            Log.SinglePhaseCommit(_connector.Id, _txId);

            try
            {
                _localTx.Commit();
                singlePhaseEnlistment.Committed();
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

            Log.TwoPhasePrepare(_connector.Id, _txId);

            // The PostgreSQL prepared transaction name is the distributed GUID + our connection's process ID, for uniqueness
            _preparedTxName = $"{_transaction.TransactionInformation.DistributedIdentifier}/{_connector.BackendProcessId}";

            try
            {
                using (_connector.StartUserAction())
                    _connector.ExecuteInternalCommand($"PREPARE TRANSACTION '{_preparedTxName}'");
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

            Log.TwoPhaseCommit(_connector.Id, _txId);

            try
            {
                using (_connector.StartUserAction())
                    _connector.ExecuteInternalCommand($"COMMIT PREPARED '{_preparedTxName}'");
            }
            catch (Exception e)
            {
                Log.Logger.LogError(NpgsqlEventId.TwoPhaseCommitException, e, "[{ConnectorId}] Exception during two-phase transaction commit (localid={TransactionId}", _connector.Id, _txId);
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
                {
                    // This only occurs if we've started a two-phase commit but one of the commits has failed.
                    Log.TwoPhaseRollback(_connector.Id, _txId);
                    using (_connector.StartUserAction())
                        _connector.ExecuteInternalCommand($"ROLLBACK PREPARED '{_preparedTxName}'");
                }
                else
                {
                    Log.SinglePhaseRollback(_connector.Id, _txId);
                    Debug.Assert(_localTx != null);
                    RollbackLocal();
                }
            }
            catch (Exception e)
            {
                Log.Logger.LogError(NpgsqlEventId.RollbackException, e, "[{ConnectorId}] Exception during transaction rollback (localid={TransactionId}", _connector.Id, _txId);
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

            Log.TransactionInDoubt(_connector.Id, _txId);

            // TODO: Is this the correct behavior?
            try
            {
                _connector.ExecuteInternalCommand($"ROLLBACK PREPARED '{_preparedTxName}'");
            }
            catch (Exception e)
            {
                Log.Logger.LogError(NpgsqlEventId.RollbackException, e, "[{ConnectorId}] Exception during transaction rollback (localid={TransactionId}", _connector.Id, _txId);
            }
            finally
            {
                Dispose();
                enlistment.Done();
            }
        }

        /// <summary>
        /// Repeatedly attempts to rollback, to support timeout-triggered rollbacks that occur while the connection is busy.
        /// </summary>
        void RollbackLocal()
        {
            Debug.Assert(_connector != null, "No connector");
            Debug.Assert(_localTx != null, "No local transaction");

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
                    // This really shouldn't be necessary, but just in case
                    if (attempt++ == MaximumRollbackAttempts)
                        throw new Exception($"Could not roll back after {MaximumRollbackAttempts} attempts, aborting. Transaction is in an unknown state.");

                    Log.Logger.LogWarning(NpgsqlEventId.ConnectionInUseDuringRollback, "[{ConnectorId}] Connection in use while trying to rollback, will cancel and retry (localid={TransactionId}", _connector.Id, _txId);
                    _connector.CancelRequest();
                    // Cancellations are asynchronous, give it some time
                    Thread.Sleep(500);
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

            Log.CleaningUpResourceManager(_connector.Id, _txId);
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
                    var pool = PoolManager.GetOrAdd(_connector.Settings);
                    pool.RemovePendingEnlistedConnector(_connector, _transaction);
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
#endif
