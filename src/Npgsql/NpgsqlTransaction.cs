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
        protected override DbConnection DbConnection { get { return Connection; } }

        NpgsqlConnector Connector { get { return Connection.Connector; } }

        /// <summary>
        /// Specifies the <see cref="System.Data.IsolationLevel">IsolationLevel</see> for this transaction.
        /// </summary>
        /// <value>The <see cref="System.Data.IsolationLevel">IsolationLevel</see> for this transaction.
        /// The default is <b>ReadCommitted</b>.</value>
        public override IsolationLevel IsolationLevel
        {
            get
            {
                CheckDisposed();
                return _isolationLevel;
            }
        }
        readonly IsolationLevel _isolationLevel;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        internal NpgsqlTransaction(NpgsqlConnection conn)
            : this(conn, IsolationLevel.ReadCommitted)
        {
            Contract.Requires(conn != null);
        }

        internal NpgsqlTransaction(NpgsqlConnection conn, IsolationLevel isolationLevel)
        {
            Contract.Requires(conn != null);

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

        #endregion

        #region Commit and Rollback

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public override void Commit()
        {
            CheckReady();

            Log.Debug("Commit transaction", Connection.Connector.Id);

            Connection.Connector.ExecuteBlind(PregeneratedMessage.CommitTransaction);
            Dispose();
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public override void Rollback()
        {
            CheckReady();

            Log.Debug("Rollback transaction", Connection.Connector.Id);

            Connection.Connector.ExecuteBlindSuppressTimeout(PregeneratedMessage.RollbackTransaction);
            Dispose();
        }

        #endregion

        #region Savepoints

        /// <summary>
        /// Rolls back a transaction from a pending savepoint state.
        /// </summary>
        public void Rollback(string savePointName)
        {
            CheckReady();

            if (!Connection.Connector.SupportsSavepoint)
            {
                throw new InvalidOperationException("Savepoint is not supported by backend.");
            }

            if (savePointName.Contains(";"))
            {
                throw new InvalidOperationException("Savepoint name cannot have semicolon.");
            }

            Connection.Connector.ExecuteBlindSuppressTimeout(string.Format("ROLLBACK TO SAVEPOINT {0}", savePointName));
        }

        /// <summary>
        /// Creates a transaction save point.
        /// </summary>
        public void Save(String savePointName)
        {
            CheckReady();

            if (!Connection.Connector.SupportsSavepoint)
            {
                throw new InvalidOperationException("Savepoint is not supported by backend.");
            }

            if (savePointName.Contains(";"))
            {
                throw new InvalidOperationException("Savepoint name cannot have semicolon.");

            }

            Connection.Connector.ExecuteBlind(string.Format("SAVEPOINT {0}", savePointName));
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (Connection == null) { return; }

            if (disposing && Connection != null) {
                if (Connection.Connector.Transaction != null) {
                    Rollback();
                }
            }

            Connection = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Checks

        [ContractArgumentValidator]
        void CheckReady()
        {
            CheckDisposed();
            Connection.CheckConnectionReady();
            Contract.EndContractBlock();
        }

        [ContractArgumentValidator]
        void CheckDisposed()
        {
            if (Connection == null)
                throw new ObjectDisposedException(typeof(NpgsqlTransaction).Name);
            Contract.EndContractBlock();
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(Connection == null || Connection.Connector.Transaction == this);
        }

        #endregion
    }
}
