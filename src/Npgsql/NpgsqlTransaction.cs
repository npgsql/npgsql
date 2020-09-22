using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        public new NpgsqlConnection? Connection
        {
            get
            {
                CheckReady();
                return _connector.Connection;
            }
        }

        // Note that with ambient transactions, it's possible for a transaction to be pending after its connection
        // is already closed. So we capture the connector and perform everything directly on it.
        readonly NpgsqlConnector _connector;

        /// <summary>
        /// Specifies the <see cref="NpgsqlConnection"/> object associated with the transaction.
        /// </summary>
        /// <value>The <see cref="NpgsqlConnection"/> object associated with the transaction.</value>
        protected override DbConnection? DbConnection => Connection;

        /// <summary>
        /// If true, the transaction has been committed/rolled back, but not disposed.
        /// </summary>
        internal bool IsCompleted => _connector.TransactionStatus == TransactionStatus.Idle;

        internal bool IsDisposed;

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
        IsolationLevel _isolationLevel;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlTransaction));

        const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;

        #endregion

        #region Initialization

        internal NpgsqlTransaction(NpgsqlConnector connector)
            => _connector = connector;

        internal void Init(IsolationLevel isolationLevel = DefaultIsolationLevel)
        {
            Debug.Assert(isolationLevel != IsolationLevel.Chaos);

            if (!_connector.DatabaseInfo.SupportsTransactions)
                return;

            Log.Debug($"Beginning transaction with isolation level {isolationLevel}", _connector.Id);
            switch (isolationLevel)
            {
            case IsolationLevel.RepeatableRead:
            case IsolationLevel.Snapshot:
                _connector.PrependInternalMessage(PregeneratedMessages.BeginTransRepeatableRead, 2);
                break;
            case IsolationLevel.Serializable:
                _connector.PrependInternalMessage(PregeneratedMessages.BeginTransSerializable, 2);
                break;
            case IsolationLevel.ReadUncommitted:
                // PG doesn't really support ReadUncommitted, it's the same as ReadCommitted. But we still
                // send as if.
                _connector.PrependInternalMessage(PregeneratedMessages.BeginTransReadUncommitted, 2);
                break;
            case IsolationLevel.ReadCommitted:
                _connector.PrependInternalMessage(PregeneratedMessages.BeginTransReadCommitted, 2);
                break;
            case IsolationLevel.Unspecified:
                isolationLevel = DefaultIsolationLevel;
                goto case DefaultIsolationLevel;
            default:
                throw new NotSupportedException("Isolation level not supported: " + isolationLevel);
            }

            _connector.TransactionStatus = TransactionStatus.Pending;
            _isolationLevel = isolationLevel;
            IsDisposed = false;
        }

        #endregion

        #region Commit

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public override void Commit() => Commit(false).GetAwaiter().GetResult();

        async Task Commit(bool async, CancellationToken cancellationToken = default)
        {
            CheckReady();

            if (!_connector.DatabaseInfo.SupportsTransactions)
                return;

            using (_connector.StartUserAction())
            {
                Log.Debug("Committing transaction", _connector.Id);
                await _connector.ExecuteInternalCommand(PregeneratedMessages.CommitTransaction, async, cancellationToken);
            }
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
#if !NET461 && !NETSTANDARD2_0
        public override Task CommitAsync(CancellationToken cancellationToken = default)
#else
        public Task CommitAsync(CancellationToken cancellationToken = default)
#endif
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return Commit(true, cancellationToken);
        }

        #endregion

        #region Rollback

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public override void Rollback() => Rollback(false).GetAwaiter().GetResult();

        Task Rollback(bool async, CancellationToken cancellationToken = default)
        {
            CheckReady();
            return _connector.DatabaseInfo.SupportsTransactions
                ? _connector.Rollback(async, cancellationToken)
                : Task.CompletedTask;
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
#if !NET461 && !NETSTANDARD2_0
        public override Task RollbackAsync(CancellationToken cancellationToken = default)
#else
        public Task RollbackAsync(CancellationToken cancellationToken = default)
#endif
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return Rollback(true, cancellationToken);
        }

        #endregion

        #region Savepoints

        async Task Save(string name, bool async, CancellationToken cancellationToken = default)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name can't be empty", nameof(name));
            if (name.Contains(";"))
                throw new ArgumentException("name can't contain a semicolon");

            CheckReady();
            if (!_connector.DatabaseInfo.SupportsTransactions)
                return;
            using (_connector.StartUserAction())
            {
                Log.Debug($"Creating savepoint {name}", _connector.Id);
                await _connector.ExecuteInternalCommand($"SAVEPOINT {name}", async, cancellationToken);
            }
        }

        /// <summary>
        /// Creates a transaction save point.
        /// </summary>
        /// <param name="name">The name of the savepoint.</param>
#if NET
        public override void Save(string name) => Save(name, false).GetAwaiter().GetResult();
#else
        public void Save(string name) => Save(name, false).GetAwaiter().GetResult();
#endif

        /// <summary>
        /// Creates a transaction save point.
        /// </summary>
        /// <param name="name">The name of the savepoint.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
#if NET
        public override Task SaveAsync(string name, CancellationToken cancellationToken = default)
#else
        public Task SaveAsync(string name, CancellationToken cancellationToken = default)
#endif
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return Save(name, true, cancellationToken);
        }

        async Task Rollback(string name, bool async, CancellationToken cancellationToken = default)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name can't be empty", nameof(name));
            if (name.Contains(";"))
                throw new ArgumentException("name can't contain a semicolon");

            CheckReady();
            if (!_connector.DatabaseInfo.SupportsTransactions)
                return;
            using (_connector.StartUserAction())
            {
                Log.Debug($"Rolling back savepoint {name}", _connector.Id);
                await _connector.ExecuteInternalCommand($"ROLLBACK TO SAVEPOINT {name}", async, cancellationToken);
            }
        }

        /// <summary>
        /// Rolls back a transaction from a pending savepoint state.
        /// </summary>
        /// <param name="name">The name of the savepoint.</param>
#if NET
        public override void Rollback(string name)
#else
        public void Rollback(string name)
#endif
            => Rollback(name, false).GetAwaiter().GetResult();

        /// <summary>
        /// Rolls back a transaction from a pending savepoint state.
        /// </summary>
        /// <param name="name">The name of the savepoint.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
#if NET
        public override Task RollbackAsync(string name, CancellationToken cancellationToken = default)
#else
        public Task RollbackAsync(string name, CancellationToken cancellationToken = default)
#endif
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return Rollback(name, true, cancellationToken);
        }

        async Task Release(string name, bool async, CancellationToken cancellationToken = default)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name can't be empty", nameof(name));
            if (name.Contains(";"))
                throw new ArgumentException("name can't contain a semicolon");

            CheckReady();
            if (!_connector.DatabaseInfo.SupportsTransactions)
                return;
            using (_connector.StartUserAction())
            {
                Log.Debug($"Releasing savepoint {name}", _connector.Id);
                await _connector.ExecuteInternalCommand($"RELEASE SAVEPOINT {name}", async, cancellationToken);
            }
        }

        /// <summary>
        /// Releases a transaction from a pending savepoint state.
        /// </summary>
        /// <param name="name">The name of the savepoint.</param>
#if NET
        public override void Release(string name) => Release(name, false).GetAwaiter().GetResult();
#else
        public void Release(string name) => Release(name, false).GetAwaiter().GetResult();
#endif

        /// <summary>
        /// Releases a transaction from a pending savepoint state.
        /// </summary>
        /// <param name="name">The name of the savepoint.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
#if NET
        public override Task ReleaseAsync(string name, CancellationToken cancellationToken = default)
#else
        public Task ReleaseAsync(string name, CancellationToken cancellationToken = default)
#endif
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return Release(name, true, cancellationToken);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Disposes the transaction, rolling it back if it is still pending.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing && !IsCompleted)
            {
                // We're disposing, so no cancellation token
                _connector.CloseOngoingOperations(async: false).GetAwaiter().GetResult();
                Rollback();
            }

            IsDisposed = true;
        }

        /// <summary>
        /// Disposes the transaction, rolling it back if it is still pending.
        /// </summary>
#if !NET461 && !NETSTANDARD2_0
        public override ValueTask DisposeAsync()
#else
        public ValueTask DisposeAsync()
#endif
        {
            if (!IsDisposed)
            {
                if (!IsCompleted)
                {
                    using (NoSynchronizationContextScope.Enter())
                        return DisposeAsyncInternal();
                }

                IsDisposed = true;
            }
            return default;

            async ValueTask DisposeAsyncInternal()
            {
                // We're disposing, so no cancellation token
                await _connector.CloseOngoingOperations(async: true);
                await Rollback(async: true);
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Disposes the transaction, without rolling back. Used only in special circumstances, e.g. when
        /// the connection is broken.
        /// </summary>
        internal void DisposeImmediately() => IsDisposed = true;

        #endregion

        #region Checks

        void CheckReady()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(typeof(NpgsqlTransaction).Name);
            if (IsCompleted)
                throw new InvalidOperationException("This NpgsqlTransaction has completed; it is no longer usable.");
        }

        #endregion
    }
}
