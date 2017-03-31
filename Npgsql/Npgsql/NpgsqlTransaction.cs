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

namespace Npgsql
{
    /// <summary>
    /// Represents a transaction to be made in a PostgreSQL database. This class cannot be inherited.
    /// </summary>
    public sealed class NpgsqlTransaction : DbTransaction
    {
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly ResourceManager resman = new ResourceManager(MethodBase.GetCurrentMethod().DeclaringType);

        private NpgsqlConnection _conn = null;
        private NpgsqlConnector _connector = null;
        private readonly IsolationLevel _isolation = IsolationLevel.ReadCommitted;
        private bool _disposed = false;

        internal NpgsqlTransaction(NpgsqlConnection conn)
            : this(conn, IsolationLevel.ReadCommitted)
        {
        }

        internal NpgsqlTransaction(NpgsqlConnection conn, IsolationLevel isolation)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);

            _conn = conn;
            _connector = _conn.Connector;
            _isolation = isolation;

            if (isolation == IsolationLevel.RepeatableRead || isolation == IsolationLevel.Snapshot)
            {
                NpgsqlCommand.ExecuteBlind(_connector, NpgsqlQuery.BeginTransRepeatableRead);
            }
            else if (isolation == IsolationLevel.Serializable)
            {
                if (conn.SerializableAs != SerializableAs.RepeatableRead)
                    NpgsqlCommand.ExecuteBlind(_connector, NpgsqlQuery.BeginTransSerializable);
                else
                    NpgsqlCommand.ExecuteBlind(_connector, NpgsqlQuery.BeginTransRepeatableRead);
            }
            else
            {
                // Set isolation level default to read committed.
                _isolation = IsolationLevel.ReadCommitted;
                NpgsqlCommand.ExecuteBlind(_connector, NpgsqlQuery.BeginTransReadCommitted);
            }

            _connector.Transaction = this;
        }

        /// <summary>
        /// Gets the <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see>
        /// object associated with the transaction, or a null reference if the
        /// transaction is no longer valid.
        /// </summary>
        /// <value>The <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see>
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
                    throw new InvalidOperationException(resman.GetString("Exception_NoTransaction"));
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
            if (_disposed)
                return;
            if (disposing && _connector != null)
            {
                this.Rollback();
            }
            _disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public override void Commit()
        {
            CheckDisposed();

            if (_connector == null)
            {
                throw new InvalidOperationException(resman.GetString("Exception_NoTransaction"));
            }

            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Commit");

            try
            {
                NpgsqlCommand.ExecuteBlind(_connector, NpgsqlQuery.CommitTransaction);
            }
            finally
            {
                Clear();
            }
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public override void Rollback()
        {
            CheckDisposed();

            if (_connector == null)
            {
                throw new InvalidOperationException(resman.GetString("Exception_NoTransaction"));
            }

            try
            {
                NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Rollback");
                NpgsqlCommand.ExecuteBlindSuppressTimeout(_connector, NpgsqlQuery.RollbackTransaction);
            }
            finally
            {
                Clear();
            }
        }

        /// <summary>
        /// Rolls back a transaction from a pending savepoint state.
        /// </summary>
        public void Rollback(String savePointName)
        {

            CheckDisposed();

            if (_conn == null)
            {
                throw new InvalidOperationException(resman.GetString("Exception_NoTransaction"));
            }

            if (!_connector.SupportsSavepoint)
            {
                throw new InvalidOperationException(resman.GetString("Exception_SavePointNotSupported"));
            }

            if (savePointName.Contains(";"))
            {
                throw new InvalidOperationException(resman.GetString("Exception_SavePointWithSemicolon"));

            }

            NpgsqlCommand.ExecuteBlindSuppressTimeout(_connector, string.Format("ROLLBACK TO SAVEPOINT {0}", savePointName));
        }

        /// <summary>
        /// Creates a transaction save point.
        /// </summary>
        public void Save(String savePointName)
        {

            CheckDisposed();

            if (_conn == null)
            {
                throw new InvalidOperationException(resman.GetString("Exception_NoTransaction"));
            }

            if (!_connector.SupportsSavepoint)
            {
                throw new InvalidOperationException(resman.GetString("Exception_SavePointNotSupported"));
            }

            if (savePointName.Contains(";"))
            {
                throw new InvalidOperationException(resman.GetString("Exception_SavePointWithSemicolon"));

            }

            NpgsqlCommand.ExecuteBlind(_connector, string.Format("SAVEPOINT {0}", savePointName));

        }

        /// <summary>
        /// Cancel the transaction without telling the backend about it.  This is
        /// used to make the transaction go away when closing a connection.
        /// </summary>
        internal void Cancel()
        {
            CheckDisposed();
            Clear();
        }

        internal void Clear()
        {
            _connector.Transaction = null;
            _connector = null;
            _conn = null;
        }

        internal bool Disposed
        {
            get { return _disposed; }
        }

        internal void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(CLASSNAME);
            }
        }
    }
}
