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
using System.Data;
using System.Data.Common;
using System.Diagnostics.Contracts;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;

namespace Npgsql
{
    /// <summary>
    /// Represents a transaction to be made in a PostgreSQL database. This class cannot be inherited.
    /// </summary>
    public sealed class NpgsqlTransaction : DbTransaction
    {
        #region Fields and Properties

        /// <summary>
        /// Specifies the <see cref="NpgsqlConnection"/> object associated with the transaction.
        /// </summary>
        /// <value>The <see cref="NpgsqlConnection"/> object associated with the transaction.</value>
        public new NpgsqlConnection Connection { get; internal set; }

        /// <summary>
        /// Specifies the <see cref="NpgsqlConnection"/> object associated with the transaction.
        /// </summary>
        /// <value>The <see cref="NpgsqlConnection"/> object associated with the transaction.</value>
        protected override DbConnection DbConnection => Connection;

        NpgsqlConnector Connector => Connection.Connector;

        bool _isDisposed;

        /// <summary>
        /// Specifies the <see cref="System.Data.IsolationLevel">IsolationLevel</see> for this transaction.
        /// </summary>
        /// <value>The <see cref="System.Data.IsolationLevel">IsolationLevel</see> for this transaction.
        /// The default is <b>ReadCommitted</b>.</value>
        public override IsolationLevel IsolationLevel
        {
            get
            {
                CheckReady();
                return _isolationLevel;
            }
        }
        readonly IsolationLevel _isolationLevel;

        const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        internal NpgsqlTransaction(NpgsqlConnection conn)
            : this(conn, DefaultIsolationLevel)
        {
            Contract.Requires(conn != null);
        }

        internal NpgsqlTransaction(NpgsqlConnection conn, IsolationLevel isolationLevel)
        {
            Contract.Requires(conn != null);
            Contract.Requires(isolationLevel != IsolationLevel.Chaos);

            Connection = conn;
            Connector.Transaction = this;
            Connector.TransactionStatus = TransactionStatus.Pending;

            switch (isolationLevel) {
                case IsolationLevel.RepeatableRead:
                    Connector.PrependInternalMessage(PregeneratedMessage.BeginTransRepeatableRead);
                    break;
                case IsolationLevel.Serializable:
                case IsolationLevel.Snapshot:
                    Connector.PrependInternalMessage(PregeneratedMessage.BeginTransSerializable);
                    break;
                case IsolationLevel.ReadUncommitted:
                    // PG doesn't really support ReadUncommitted, it's the same as ReadCommitted. But we still
                    // send as if.
                    Connector.PrependInternalMessage(PregeneratedMessage.BeginTransReadUncommitted);
                    break;
                case IsolationLevel.ReadCommitted:
                    Connector.PrependInternalMessage(PregeneratedMessage.BeginTransReadCommitted);
                    break;
                case IsolationLevel.Unspecified:
                    isolationLevel = DefaultIsolationLevel;
                    goto case DefaultIsolationLevel;
                default:
                    throw PGUtil.ThrowIfReached("Isolation level not supported: " + isolationLevel);
            }

            _isolationLevel = isolationLevel;
        }

        #endregion

        #region Commit and Rollback

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public override void Commit()
        {
            CheckReady();
            Log.Debug("Commit transaction", Connection.Connector.Id);
            Connector.ExecuteInternalCommand(PregeneratedMessage.CommitTransaction);
            Connection = null;
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public override void Rollback()
        {
            CheckReady();

            Log.Debug("Rollback transaction", Connection.Connector.Id);

            var connector = Connector;

            try
            {
                // If we're in a failed transaction we can't set the timeout
                var withTimeout = connector.TransactionStatus != TransactionStatus.InFailedTransactionBlock;
                connector.ExecuteInternalCommand(PregeneratedMessage.RollbackTransaction, withTimeout);
            }
            finally
            {
                // The rollback may change the value of statement_value, set to unknown
                connector.SetBackendTimeoutToUnknown();
            }

            Connection = null;
        }

        #endregion

        #region Savepoints

        /// <summary>
        /// Creates a transaction save point.
        /// </summary>
        public void Save(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name can't be empty", nameof(name));
            if (name.Contains(";"))
                throw new ArgumentException("name can't contain a semicolon");
            Contract.EndContractBlock();

            CheckReady();
            Log.Debug("Create savepoint", Connection.Connector.Id);
            Connector.ExecuteInternalCommand(new QueryMessage($"SAVEPOINT {name}"));
        }

        /// <summary>
        /// Rolls back a transaction from a pending savepoint state.
        /// </summary>
        public void Rollback(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name can't be empty", nameof(name));
            if (name.Contains(";"))
                throw new ArgumentException("name can't contain a semicolon");
            Contract.EndContractBlock();

            CheckReady();

            Log.Debug("Rollback savepoint", Connection.Connector.Id);

            try {
                // If we're in a failed transaction we can't set the timeout
                var withTimeout = Connector.TransactionStatus != TransactionStatus.InFailedTransactionBlock;
                Connector.ExecuteInternalCommand(new QueryMessage($"ROLLBACK TO SAVEPOINT {name}"), withTimeout);
            } finally {
                // The rollback may change the value of statement_value, set to unknown
                Connection.Connector.SetBackendTimeoutToUnknown();
            }
        }

        /// <summary>
        /// Rolls back a transaction from a pending savepoint state.
        /// </summary>
        public void Release(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name can't be empty", nameof(name));
            if (name.Contains(";"))
                throw new ArgumentException("name can't contain a semicolon");
            Contract.EndContractBlock();

            CheckReady();

            Log.Debug("Release savepoint", Connection.Connector.Id);

            Connector.ExecuteInternalCommand(new QueryMessage($"RELEASE SAVEPOINT {name}"));
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed) { return; }

            if (disposing && Connection != null) {
                Rollback();
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }

        #endregion

        #region Checks

        void CheckReady()
        {
            CheckDisposed();
            CheckCompleted();
            Connection.CheckReady();
        }

        [ContractArgumentValidator]
        void CheckCompleted()
        {
            if (Connection == null)
                throw new InvalidOperationException("This NpgsqlTransaction has completed; it is no longer usable.");
            Contract.EndContractBlock();
        }

        [ContractArgumentValidator]
        void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(typeof(NpgsqlTransaction).Name);
            Contract.EndContractBlock();
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
        }

        #endregion
    }
}
