// created on 17/11/2002 at 19:04

// Npgsql.NpgsqlTransaction.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using Common.Logging;
using Npgsql.FrontendMessages;
using Npgsql.Localization;

namespace Npgsql
{
    /// <summary>
    /// Represents a transaction to be made in a PostgreSQL database. This class cannot be inherited.
    /// </summary>
    public sealed class NpgsqlTransaction : DbTransaction
    {
        readonly IsolationLevel _isolationLevel;
        bool _disposed;

        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        internal NpgsqlTransaction(NpgsqlConnection conn)
            : this(conn, IsolationLevel.ReadCommitted) {}

        internal NpgsqlTransaction(NpgsqlConnection conn, IsolationLevel isolationLevel)
        {
            Connection = conn;
            _isolationLevel = isolationLevel;
            Connector.Transaction = this;

            switch (isolationLevel) {
                case IsolationLevel.RepeatableRead:
                    Connector.PrependMessage(PregeneratedMessage.BeginTransRepeatableRead);
                    break;
                case IsolationLevel.Serializable:
                case IsolationLevel.Snapshot:
                    Connector.PrependMessage(PregeneratedMessage.BeginTransSerializable);
                    break;
                case IsolationLevel.ReadUncommitted:
                    // PG doesn't really support ReadUncommitted, it's the same as ReadCommitted. But we still
                    // send as if.
                    Connector.PrependMessage(PregeneratedMessage.BeginTransReadUncommitted);
                    break;
                case IsolationLevel.ReadCommitted:
                    Connector.PrependMessage(PregeneratedMessage.BeginTransReadCommitted);
                    break;
                default:
                    throw new NotSupportedException("Isolation level not supported: " + isolationLevel);
            }
        }

        /// <summary>
        /// Gets the <see cref="NpgsqlConnection">NpgsqlConnection</see>
        /// object associated with the transaction, or a null reference if the
        /// transaction is no longer valid.
        /// </summary>
        /// <value>The <see cref="NpgsqlConnection">NpgsqlConnection</see>
        /// object associated with the transaction.</value>
        public new NpgsqlConnection Connection { get; internal set; }

        NpgsqlConnector Connector { get { return Connection.Connector; } }

        /// <summary>
        /// DB connection.
        /// </summary>
        protected override DbConnection DbConnection
        {
            get { return Connection; }
        }

        /// <summary>
        /// Specifies the <see cref="System.Data.IsolationLevel">IsolationLevel</see> for this transaction.
        /// </summary>
        /// <value>The <see cref="System.Data.IsolationLevel">IsolationLevel</see> for this transaction.
        /// The default is <b>ReadCommitted</b>.</value>
        public override IsolationLevel IsolationLevel
        {
            get
            {
                if (Connection == null)
                {
                    throw new InvalidOperationException(L10N.NoTransaction);
                }

                return _isolationLevel;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed) { return; }

            if (disposing && Connection != null)
            {
                if (Connection.Connector.Transaction != null)
                {
                    Rollback();
                }
            }

            _disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public override void Commit()
        {
            _log.Debug("Commit transaction");
            CheckDisposed();

            if (Connection == null) {
                throw new InvalidOperationException(L10N.NoTransaction);
            }

            Connection.Connector.ExecuteBlind(PregeneratedMessage.CommitTransaction);
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public override void Rollback()
        {
            _log.Debug("Rollback transaction");
            CheckDisposed();

            if (Connection == null) {
                throw new InvalidOperationException(L10N.NoTransaction);
            }

            Connection.CheckConnectionReady();

            Connection.Connector.ExecuteBlindSuppressTimeout(PregeneratedMessage.RollbackTransaction);
        }

        /// <summary>
        /// Rolls back a transaction from a pending savepoint state.
        /// </summary>
        public void Rollback(String savePointName)
        {
            CheckDisposed();

            if (Connection == null)
            {
                throw new InvalidOperationException(L10N.NoTransaction);
            }

            Connection.CheckConnectionReady();

            if (!Connection.Connector.SupportsSavepoint)
            {
                throw new InvalidOperationException(L10N.SavePointNotSupported);
            }

            if (savePointName.Contains(";"))
            {
                throw new InvalidOperationException(L10N.SavePointWithSemicolon);
            }

            Connection.Connector.ExecuteBlindSuppressTimeout(string.Format("ROLLBACK TO SAVEPOINT {0}", savePointName));
        }

        /// <summary>
        /// Creates a transaction save point.
        /// </summary>
        public void Save(String savePointName)
        {
            CheckDisposed();

            if (Connection == null)
            {
                throw new InvalidOperationException(L10N.NoTransaction);
            }

            Connection.CheckConnectionReady();

            if (!Connection.Connector.SupportsSavepoint)
            {
                throw new InvalidOperationException(L10N.SavePointNotSupported);
            }

            if (savePointName.Contains(";"))
            {
                throw new InvalidOperationException(L10N.SavePointWithSemicolon);

            }

            Connection.Connector.ExecuteBlind(string.Format("SAVEPOINT {0}", savePointName));
        }

        internal bool Disposed
        {
            get { return _disposed; }
        }

        internal void CheckDisposed()
        {
            if (_disposed) {
                throw new ObjectDisposedException(typeof(NpgsqlTransaction).Name);
            }
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(Connection == null || Connection.Connector.Transaction == this);
        }
    }
}
