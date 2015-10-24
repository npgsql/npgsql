#if NET45 || NET452 || DNX452
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
using System.Reflection;
using System.Transactions;
using Npgsql.Logging;

namespace Npgsql
{
    internal class NpgsqlPromotableSinglePhaseNotification : IPromotableSinglePhaseNotification
    {
        private readonly NpgsqlConnection _connection;
        private IsolationLevel _isolationLevel;
        private NpgsqlTransaction _npgsqlTx;
        private NpgsqlTransactionCallbacks _callbacks;
        private INpgsqlResourceManager _rm;
        private bool _inTransaction;
        internal bool InLocalTransaction => _npgsqlTx != null;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        public NpgsqlPromotableSinglePhaseNotification(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public void Enlist(Transaction tx)
        {
            Log.Debug("Enlist");
            if (tx != null)
            {
                _isolationLevel = tx.IsolationLevel;
                if (!tx.EnlistPromotableSinglePhase(this))
                {
                    // must already have a durable resource
                    // start transaction
                    _npgsqlTx = _connection.BeginTransaction(ConvertIsolationLevel(_isolationLevel));
                    _inTransaction = true;
                    _rm = CreateResourceManager();
                    _callbacks = new NpgsqlTransactionCallbacks(_connection);
                    _rm.Enlist(_callbacks, TransactionInterop.GetTransmitterPropagationToken(tx));
                    // enlisted in distributed transaction
                    // disconnect and cleanup local transaction
                    _connection.Connector.ClearTransaction();
                    _npgsqlTx.Dispose();
                    _npgsqlTx = null;
                }
            }
        }

        /// <summary>
        /// Used when a connection is closed
        /// </summary>
        public void Prepare()
        {
            Log.Debug("Prepare");
            if (_inTransaction)
            {
                // may not be null if Promote or Enlist is called first
                if (_callbacks == null)
                {
                    _callbacks = new NpgsqlTransactionCallbacks(_connection);
                }
                _callbacks.PrepareTransaction();
                if (_npgsqlTx != null)
                {
                    // cancel the NpgsqlTransaction since this will
                    // be handled by a two phase commit.
                    _connection.Connector.ClearTransaction();
                    _npgsqlTx.Dispose();
                    _npgsqlTx = null;
                    _connection.PromotableLocalTransactionEnded();
                }
            }
        }

#region IPromotableSinglePhaseNotification Members

        public void Initialize()
        {
            Log.Debug("Initialize");
            _npgsqlTx = _connection.BeginTransaction(ConvertIsolationLevel(_isolationLevel));
            _inTransaction = true;
        }

        public void Rollback(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            Log.Debug("Rollback");
            // try to rollback the transaction with either the
            // ADO.NET transaction or the callbacks that managed the
            // two phase commit transaction.
            if (_npgsqlTx != null)
            {
                _npgsqlTx.Rollback();
                _npgsqlTx.Dispose();
                _npgsqlTx = null;
                singlePhaseEnlistment.Aborted();
                _connection.PromotableLocalTransactionEnded();
            }
            else if (_callbacks != null)
            {
                if (_rm != null)
                {
                    _rm.RollbackWork(_callbacks.GetName());
                    singlePhaseEnlistment.Aborted();
                }
                else
                {
                    _callbacks.RollbackTransaction();
                    singlePhaseEnlistment.Aborted();
                }
                _callbacks = null;
            }
            _inTransaction = false;
        }

        public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            Log.Debug("Single Phase Commit");
            if (_npgsqlTx != null)
            {
                _npgsqlTx.Commit();
                _npgsqlTx.Dispose();
                _npgsqlTx = null;
                singlePhaseEnlistment.Committed();
                _connection.PromotableLocalTransactionEnded();
            }
            else if (_callbacks != null)
            {
                if (_rm != null)
                {
                    _rm.CommitWork(_callbacks.GetName());
                    singlePhaseEnlistment.Committed();
                }
                else
                {
                    _callbacks.CommitTransaction();
                    singlePhaseEnlistment.Committed();
                }
                _callbacks = null;
            }
            _inTransaction = false;
        }

#endregion

#region ITransactionPromoter Members

        public byte[] Promote()
        {
            Log.Debug("Promote");
            _rm = CreateResourceManager();
            // may not be null if Prepare or Enlist is called first
            if (_callbacks == null)
            {
                _callbacks = new NpgsqlTransactionCallbacks(_connection);
            }
            byte[] token = _rm.Promote(_callbacks);
            // mostly likely case for this is the transaction has been prepared.
            if (_npgsqlTx != null)
            {
                // cancel the NpgsqlTransaction since this will
                // be handled by a two phase commit.
                _connection.Connector.ClearTransaction();
                _npgsqlTx.Dispose();
                _npgsqlTx = null;
                _connection.PromotableLocalTransactionEnded();
            }
            return token;
        }

#endregion

        private static INpgsqlResourceManager _resourceManager;
        private static System.Runtime.Remoting.Lifetime.ClientSponsor _sponser;

        private static INpgsqlResourceManager CreateResourceManager()
        {
            // TODO: create network proxy for resource manager
            if (_resourceManager == null)
            {
                _sponser = new System.Runtime.Remoting.Lifetime.ClientSponsor();
                AppDomain rmDomain = AppDomain.CreateDomain("NpgsqlResourceManager", AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
                _resourceManager =
                    (INpgsqlResourceManager)
                    rmDomain.CreateInstanceAndUnwrap(typeof (NpgsqlResourceManager).Assembly.FullName,
                                                     typeof (NpgsqlResourceManager).FullName);
                _sponser.Register((MarshalByRefObject)_resourceManager);
            }
            return _resourceManager;
            //return new NpgsqlResourceManager();
        }

        private static System.Data.IsolationLevel ConvertIsolationLevel(IsolationLevel _isolationLevel)
        {
            switch (_isolationLevel)
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
