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

namespace Npgsql
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Note that a connection may be closed before its TransactionScope completes. In this case we close the NpgsqlConnection
    /// as usual but the connector in a special list in the pool; it will be closed only when the scope completes.
    /// </remarks>
    internal class VolatileResourceManager : ISinglePhaseNotification
    {
        private NpgsqlConnection _connection;
        private Transaction _transaction;
        private readonly string _txId;
        private NpgsqlTransaction _localTx;
        private string _preparedTxName;
        private bool IsPrepared
        {
            get
            {
                return _preparedTxName != null;
            }
        }
        bool _isDisposed;

        const int MaximumRollbackAttempts = 20;

        internal VolatileResourceManager(NpgsqlConnection connection, Transaction transaction)
        {
            _connection = connection;
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
            Debug.Assert(_connection != null, "No connection");

            NpgsqlEventLog.LogMsg(string.Format("Single Phase Commit (localid={0}, processId={1})",
                _txId, _connection.pid), LogLevel.Debug);

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
            Debug.Assert(_connection != null, "No connector");

            NpgsqlEventLog.LogMsg(string.Format("Two-phase transaction prepare (localid={0}, processId={1})",
                _txId, _connection.pid), LogLevel.Debug);

            // The PostgreSQL prepared transaction name is the distributed GUID + our connection's process ID, for uniqueness
            Guid distId = _transaction.TransactionInformation.DistributedIdentifier;
            if (distId != Guid.Empty)
            {
                _preparedTxName = string.Format("{0}/{1}", distId, _connection.pid);
            }
            else
            {
                _preparedTxName = string.Format("{0}/{1}", _txId, _connection.pid);
            }
            try
            {
                _localTx.Cancel();
                _localTx.Dispose();
                _localTx = null;
                NpgsqlCommand.ExecuteBlind(_connection.Connector, string.Format("PREPARE TRANSACTION '{0}'", _preparedTxName));
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
            Debug.Assert(_connection != null, "No connection");

            NpgsqlEventLog.LogMsg(string.Format("Two-phase transaction commit (localid={0}, connection.Id={1})", _txId, _connection.pid), LogLevel.Debug);

            try
            {
                NpgsqlCommand.ExecuteBlind(_connection.Connector, string.Format("COMMIT PREPARED '{0}'", _preparedTxName));
            }
            catch (Exception e)
            {
                NpgsqlEventLog.LogMsg(string.Format("Exception during two-phase transaction commit (localid={0}, processId={1}): {2}",
                    _txId, _connection.pid, e), LogLevel.Normal);
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
            Debug.Assert(_connection != null, "No connector");

            try
            {
                if (IsPrepared)
                {
                    // This only occurs if we've started a two-phase commit but one of the commits has failed.
                    NpgsqlEventLog.LogMsg(string.Format("Two-phase transaction rollback (localid={0}, processId={1})",
                        _txId, _connection.pid), LogLevel.Debug);
                    NpgsqlCommand.ExecuteBlindSuppressTimeout(_connection.Connector,
                        string.Format("ROLLBACK PREPARED '{0}'", _preparedTxName));
                }
                else
                {
                    NpgsqlEventLog.LogMsg(string.Format("Single-phase transaction rollback (localid={0}, processId={1})",
                        _txId, _connection.pid), LogLevel.Debug);
                    Debug.Assert(_localTx != null);
                    RollbackLocal();
                }
            }
            catch (Exception e)
            {
                NpgsqlEventLog.LogMsg(string.Format("Exception during transaction rollback (localid={0}, processId={1}): {2}",
                    _txId, _connection.pid, e), LogLevel.Normal);
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
            Debug.Assert(_connection != null, "No connector");

            NpgsqlEventLog.LogMsg(string.Format("Two-phase transaction in doubt (localid={0}, processId={1})",
                _txId, _connection.pid), LogLevel.Normal);

            // TODO: Is this the correct behavior?
            try
            {
                NpgsqlCommand.ExecuteBlindSuppressTimeout(_connection.Connector,
                    string.Format("ROLLBACK PREPARED '{0}'", _preparedTxName));
            }
            catch (Exception e)
            {
                NpgsqlEventLog.LogMsg(string.Format("Exception during transaction rollback (localid={0}, processId={1}): {2}",
                    _txId, _connection.pid, e), LogLevel.Normal);
            }
            finally
            {
                Dispose();
                enlistment.Done();
            }
        }

        private void RollbackLocal()
        {
            Debug.Assert(_connection != null, "No connector");
            Debug.Assert(_localTx != null, "No local transaction");

            var connector = _connection.Connector;
            if (connector == null)
                return;
            var socket = connector.Socket;
            if (!Monitor.TryEnter(socket, 100))
            {
                connector.CancelRequest();
                Monitor.Enter(socket);
            }
            try
            {
                _localTx.Rollback();
            }
            finally
            {
                Monitor.Exit(socket);
            }
            return;
        }

        #region Dispose/Cleanup

        void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            Debug.Assert(_transaction != null, "No transaction");
            Debug.Assert(_connection != null, "No connector");
            Debug.Assert(_connection.EnlistedTransaction == _transaction, "enlisted transaction mismatch");

            if (_localTx != null)
            {
                _localTx.Dispose();
                _localTx = null;
            }
            _connection.EnlistedTransactionEnded();
            _connection = null;
            _transaction = null;
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
