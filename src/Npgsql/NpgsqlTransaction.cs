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
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
        [CanBeNull]
        public new NpgsqlConnection Connection { get; internal set; }

        // Note that with ambient transactions, it's possible for a transaction to be pending after its connection
        // is already closed. So we capture the connector and perform everything directly on it.
        NpgsqlConnector _connector;

        /// <summary>
        /// Specifies the completion state of the transaction.
        /// </summary>
        /// <value>The completion state of the transaction.</value>
        public bool IsCompleted => _connector == null;

        /// <summary>
        /// Specifies the <see cref="NpgsqlConnection"/> object associated with the transaction.
        /// </summary>
        /// <value>The <see cref="NpgsqlConnection"/> object associated with the transaction.</value>
        [CanBeNull]
        protected override DbConnection DbConnection => Connection;

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

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;

        #endregion

        #region Constructors

        internal NpgsqlTransaction(NpgsqlConnection conn, IsolationLevel isolationLevel = DefaultIsolationLevel)
        {
            Debug.Assert(conn != null);
            Debug.Assert(isolationLevel != IsolationLevel.Chaos);


            Connection = conn;
            _connector = Connection.CheckReadyAndGetConnector();
            Log.Debug($"Beginning transaction with isolation level {isolationLevel}", _connector.Id);
            _connector.Transaction = this;
            _connector.TransactionStatus = TransactionStatus.Pending;

            switch (isolationLevel) {
                case IsolationLevel.RepeatableRead:
                    _connector.PrependInternalMessage(PregeneratedMessage.BeginTrans);
                    _connector.PrependInternalMessage(PregeneratedMessage.SetTransRepeatableRead);
                    break;
                case IsolationLevel.Serializable:
                case IsolationLevel.Snapshot:
                    _connector.PrependInternalMessage(PregeneratedMessage.BeginTrans);
                    _connector.PrependInternalMessage(PregeneratedMessage.SetTransSerializable);
                    break;
                case IsolationLevel.ReadUncommitted:
                    // PG doesn't really support ReadUncommitted, it's the same as ReadCommitted. But we still
                    // send as if.
                    _connector.PrependInternalMessage(PregeneratedMessage.BeginTrans);
                    _connector.PrependInternalMessage(PregeneratedMessage.SetTransReadUncommitted);
                    break;
                case IsolationLevel.ReadCommitted:
                    _connector.PrependInternalMessage(PregeneratedMessage.BeginTrans);
                    _connector.PrependInternalMessage(PregeneratedMessage.SetTransReadCommitted);
                    break;
                case IsolationLevel.Unspecified:
                    isolationLevel = DefaultIsolationLevel;
                    goto case DefaultIsolationLevel;
                default:
                    throw new NotSupportedException("Isolation level not supported: " + isolationLevel);
            }

            _isolationLevel = isolationLevel;
        }

        #endregion

        #region Commit

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public override void Commit() => Commit(false, CancellationToken.None).GetAwaiter().GetResult();

        async Task Commit(bool async, CancellationToken cancellationToken)
        {
            CheckReady();
            using (_connector.StartUserAction())
            {
                Log.Debug("Committing transaction", _connector.Id);
                await _connector.ExecuteInternalCommand(PregeneratedMessage.CommitTransaction, async, cancellationToken);
                Clear();
            }
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        [PublicAPI]
        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                await Commit(true, cancellationToken);
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        [PublicAPI]
        public Task CommitAsync() => CommitAsync(CancellationToken.None);

        #endregion

        #region Rollback

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public override void Rollback() => Rollback(false, CancellationToken.None).GetAwaiter().GetResult();

        async Task Rollback(bool async, CancellationToken cancellationToken)
        {
            CheckReady();
            await _connector.Rollback(async, cancellationToken);
            Clear();
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        [PublicAPI]
        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                await Rollback(true, cancellationToken);
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        [PublicAPI]
        public Task RollbackAsync() => RollbackAsync(CancellationToken.None);

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

            CheckReady();
            using (_connector.StartUserAction())
            {
                Log.Debug($"Creating savepoint {name}", _connector.Id);
                _connector.ExecuteInternalCommand($"SAVEPOINT {name}");
            }
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

            CheckReady();
            using (_connector.StartUserAction())
            {
                Log.Debug($"Rolling back savepoint {name}", _connector.Id);
                _connector.ExecuteInternalCommand($"ROLLBACK TO SAVEPOINT {name}");
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

            CheckReady();
            using (_connector.StartUserAction())
            {
                Log.Debug($"Releasing savepoint {name}", _connector.Id);
                _connector.ExecuteInternalCommand($"RELEASE SAVEPOINT {name}");
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) => Dispose(disposing, true);

        internal void Dispose(bool disposing, bool doRollbackIfNeeded)
        {
            if (_isDisposed) { return; }

            if (disposing && doRollbackIfNeeded && !IsCompleted)
                Rollback();

            Clear();

            base.Dispose(disposing);
            _isDisposed = true;
        }

        internal void Clear()
        {
            _connector = null;
            Connection = null;
        }

        #endregion

        #region Checks

        void CheckReady()
        {
            CheckDisposed();
            CheckCompleted();
        }

        void CheckCompleted()
        {
            if (IsCompleted)
                throw new InvalidOperationException("This NpgsqlTransaction has completed; it is no longer usable.");
        }

        void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(typeof(NpgsqlTransaction).Name);
        }

        #endregion
    }
}
