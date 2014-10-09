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
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using Common.Logging;
using Npgsql.Localization;

namespace Npgsql
{
    /// <summary>
    /// Represents a transaction to be made in a PostgreSQL database. This class cannot be inherited.
    /// </summary>
    public sealed class NpgsqlTransaction : DbTransaction
    {
        private NpgsqlConnection _conn = null;
        private readonly IsolationLevel _isolation = IsolationLevel.ReadCommitted;
        private bool _disposed = false;

        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        internal NpgsqlTransaction(NpgsqlConnection conn)
            : this(conn, IsolationLevel.ReadCommitted)
        {
        }

        internal NpgsqlTransaction(NpgsqlConnection conn, IsolationLevel isolation)
        {
            _conn = conn;
            _isolation = isolation;

            if (isolation == IsolationLevel.RepeatableRead)
            {
                conn.Connector.ExecuteBlind(QueryManager.BeginTransRepeatableRead);
            }
            else if ((isolation == IsolationLevel.Serializable) ||
                (isolation == IsolationLevel.Snapshot))
            {
                conn.Connector.ExecuteBlind(QueryManager.BeginTransSerializable);
            }
            else
            {
                // Set isolation level default to read committed.
                _isolation = IsolationLevel.ReadCommitted;
                conn.Connector.ExecuteBlind(QueryManager.BeginTransReadCommitted);
            }

            _conn.Connector.Transaction = this;
        }

        /// <summary>
        /// Gets the <see cref="NpgsqlConnection">NpgsqlConnection</see>
        /// object associated with the transaction, or a null reference if the
        /// transaction is no longer valid.
        /// </summary>
        /// <value>The <see cref="NpgsqlConnection">NpgsqlConnection</see>
        /// object associated with the transaction.</value>
        public new NpgsqlConnection Connection
        {
            get { return _conn; }
        }

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
                if (_conn == null)
                {
                    throw new InvalidOperationException(L10N.NoTransaction);
                }

                return _isolation;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this._conn != null)
            {
                if (_conn.Connector.Transaction != null)
                {
                    if ((Thread.CurrentThread.ThreadState & (ThreadState.Aborted | ThreadState.AbortRequested)) != 0)
                    {
                        // can't count on Rollback working if the thread has been aborted
                        // need to copy since Cancel will set it to null
                        NpgsqlConnection conn = _conn;
                        Cancel();
                        // must close connection since transaction hasn't been rolled back
                        conn.Close();
                    }
                    else
                    {
                        this.Rollback();
                    }
                }

                this._disposed = true;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public override void Commit()
        {
            _log.Debug("Commit transaction");
            CheckDisposed();

            if (_conn == null) {
                throw new InvalidOperationException(L10N.NoTransaction);
            }

            _conn.Connector.ExecuteBlind(QueryManager.CommitTransaction);

            _conn.Connector.Transaction = null;
            _conn = null;
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public override void Rollback()
        {
            _log.Debug("Rollback transaction");
            CheckDisposed();

            if (_conn == null) {
                throw new InvalidOperationException(L10N.NoTransaction);
            }

            _conn.Connector.ExecuteBlindSuppressTimeout(QueryManager.RollbackTransaction);
            _conn.Connector.Transaction = null;
            _conn = null;
        }

        /// <summary>
        /// Rolls back a transaction from a pending savepoint state.
        /// </summary>
        public void Rollback(String savePointName)
        {

            CheckDisposed();

            if (_conn == null)
            {
                throw new InvalidOperationException(L10N.NoTransaction);
            }

            if (!_conn.Connector.SupportsSavepoint)
            {
                throw new InvalidOperationException(L10N.SavePointNotSupported);
            }

            if (savePointName.Contains(";"))
            {
                throw new InvalidOperationException(L10N.SavePointWithSemicolon);
            }

            _conn.Connector.ExecuteBlindSuppressTimeout(string.Format("ROLLBACK TO SAVEPOINT {0}", savePointName));
        }

        /// <summary>
        /// Creates a transaction save point.
        /// </summary>
        public void Save(String savePointName)
        {

            CheckDisposed();

            if (_conn == null)
            {
                throw new InvalidOperationException(L10N.NoTransaction);
            }

            if (!_conn.Connector.SupportsSavepoint)
            {
                throw new InvalidOperationException(L10N.SavePointNotSupported);
            }

            if (savePointName.Contains(";"))
            {
                throw new InvalidOperationException(L10N.SavePointWithSemicolon);

            }

            _conn.Connector.ExecuteBlind(string.Format("SAVEPOINT {0}", savePointName));

        }

        /// <summary>
        /// Cancel the transaction without telling the backend about it.  This is
        /// used to make the transaction go away when closing a connection.
        /// </summary>
        internal void Cancel()
        {
            CheckDisposed();

            if (_conn != null)
            {
                _conn.Connector.Transaction = null;
                _conn = null;
            }
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
    }
}
