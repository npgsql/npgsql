using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
                CheckDisposed();
                return _connector?.Connection;
            }
        }

        // Note that with ambient transactions, it's possible for a transaction to be pending after its connection
        // is already closed. So we capture the connector and perform everything directly on it.
        NpgsqlConnector _connector;

        /// <summary>
        /// Specifies the <see cref="NpgsqlConnection"/> object associated with the transaction.
        /// </summary>
        /// <value>The <see cref="NpgsqlConnection"/> object associated with the transaction.</value>
        protected override DbConnection? DbConnection => Connection;

        /// <summary>
        /// If true, the transaction has been committed/rolled back, but not disposed.
        /// </summary>
        internal bool IsCompleted => _connector is null || _connector.TransactionStatus == TransactionStatus.Idle;

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

            using (_connector.StartUserAction(cancellationToken))
            {
                Log.Debug("Committing transaction", _connector.Id);
                await _connector.ExecuteInternalCommand(PregeneratedMessages.CommitTransaction, async, cancellationToken);
            }
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
#if NETSTANDARD2_0
        public Task CommitAsync(CancellationToken cancellationToken = default)
#else
        public override Task CommitAsync(CancellationToken cancellationToken = default)
#endif
        {
            using (NoSynchronizationContextScope.Enter())
                return Commit(true, cancellationToken);
        }

        #endregion

        #region Rollback

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public override void Rollback() => Rollback(false).GetAwaiter().GetResult();

        async Task Rollback(bool async, CancellationToken cancellationToken = default)
        {
            CheckReady();

            if (!_connector.DatabaseInfo.SupportsTransactions)
                return;

            using (_connector.StartUserAction(cancellationToken))
                await _connector.Rollback(async, cancellationToken);
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
#if NETSTANDARD2_0
        public Task RollbackAsync(CancellationToken cancellationToken = default)
#else
        public override Task RollbackAsync(CancellationToken cancellationToken = default)
#endif
        {
            using (NoSynchronizationContextScope.Enter())
                return Rollback(true, cancellationToken);
        }

        #endregion

        #region Savepoints

        /// <summary>
        /// Creates a transaction save point.
        /// </summary>
        /// <param name="name">The name of the savepoint.</param>
        /// <remarks>
        /// This method does not cause a database roundtrip to be made. The savepoint creation statement will instead be sent along with
        /// the next command.
        /// </remarks>
#if NET
        public override void Save(string name)
#else
        public void Save(string name)
#endif
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name can't be empty", nameof(name));

            CheckReady();
            if (!_connector.DatabaseInfo.SupportsTransactions)
                return;

            // Note that creating a savepoint doesn't actually send anything to the backend (only prepends), so strictly speaking we don't
            // have to start a user action. However, we do this for consistency as if we did (for the checks and exceptions)
            using var _ = _connector.StartUserAction();

            Log.Debug($"Creating savepoint {name}", _connector.Id);

            if (RequiresQuoting(name))
                name = $"\"{name.Replace("\"", "\"\"")}\"";

            // Note: savepoint names are PostgreSQL identifiers, and so limited by default to 63 characters.
            // Since we are prepending, we assume below that the statement will always fit in the buffer.
            _connector.WriteBuffer.WriteByte(FrontendMessageCode.Query);
            _connector.WriteBuffer.WriteInt32(
                sizeof(int)  +                               // Message length (including self excluding code)
                _connector.TextEncoding.GetByteCount("SAVEPOINT ") +
                _connector.TextEncoding.GetByteCount(name) +
                sizeof(byte));                               // Null terminator

            _connector.WriteBuffer.WriteString("SAVEPOINT ");
            _connector.WriteBuffer.WriteString(name);
            _connector.WriteBuffer.WriteByte(0);

            _connector.PendingPrependedResponses += 2;
        }

        /// <summary>
        /// Creates a transaction save point.
        /// </summary>
        /// <param name="name">The name of the savepoint.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <remarks>
        /// This method does not cause a database roundtrip to be made, and will therefore always complete synchronously.
        /// The savepoint creation statement will instead be sent along with the next command.
        /// </remarks>
#if NET
        public override Task SaveAsync(string name, CancellationToken cancellationToken = default)
#else
        public Task SaveAsync(string name, CancellationToken cancellationToken = default)
#endif
        {
            Save(name);
            return Task.CompletedTask;
        }

        async Task Rollback(string name, bool async, CancellationToken cancellationToken = default)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name can't be empty", nameof(name));

            CheckReady();
            if (!_connector.DatabaseInfo.SupportsTransactions)
                return;
            using (_connector.StartUserAction(cancellationToken))
            {
                Log.Debug($"Rolling back savepoint {name}", _connector.Id);

                if (RequiresQuoting(name))
                    name = $"\"{name.Replace("\"", "\"\"")}\"";

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
            using (NoSynchronizationContextScope.Enter())
                return Rollback(name, true, cancellationToken);
        }

        async Task Release(string name, bool async, CancellationToken cancellationToken = default)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name can't be empty", nameof(name));

            CheckReady();
            if (!_connector.DatabaseInfo.SupportsTransactions)
                return;
            using (_connector.StartUserAction(cancellationToken))
            {
                Log.Debug($"Releasing savepoint {name}", _connector.Id);

                if (RequiresQuoting(name))
                    name = $"\"{name.Replace("\"", "\"\"")}\"";

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

            if (disposing)
            {
                if (!IsCompleted)
                {
                    try
                    {
                        _connector.CloseOngoingOperations(async: false).GetAwaiter().GetResult();
                        Rollback();
                    }
                    catch (Exception ex)
                    {
                        Debug.Assert(_connector.IsBroken);
                        Log.Error("Exception while disposing a transaction", ex, _connector.Id);
                    }
                }

                IsDisposed = true;
                _connector?.Connection?.EndBindingScope(ConnectorBindingScope.Transaction);
            }
        }

        /// <summary>
        /// Disposes the transaction, rolling it back if it is still pending.
        /// </summary>
#if NETSTANDARD2_0
        public ValueTask DisposeAsync()
#else
        public override ValueTask DisposeAsync()
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
                _connector?.Connection?.EndBindingScope(ConnectorBindingScope.Transaction);
            }
            return default;

            async ValueTask DisposeAsyncInternal()
            {
                // We're disposing, so no cancellation token
                try
                {
                    await _connector.CloseOngoingOperations(async: true);
                    await Rollback(async: true);
                }
                catch (Exception ex)
                {
                    Debug.Assert(_connector.IsBroken);
                    Log.Error("Exception while disposing a transaction", ex, _connector.Id);
                }
                
                IsDisposed = true;
                _connector?.Connection?.EndBindingScope(ConnectorBindingScope.Transaction);
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
            CheckDisposed();
            if (IsCompleted)
                throw new InvalidOperationException("This NpgsqlTransaction has completed; it is no longer usable.");
        }

        void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(typeof(NpgsqlTransaction).Name);
        }

        static bool RequiresQuoting(string identifier)
        {
            Debug.Assert(identifier.Length > 0);

            var first = identifier[0];
            if (first != '_' && !char.IsLower(first))
                return true;

            foreach (var c in identifier.AsSpan(1))
                if (c != '_' && c != '$' && !char.IsLower(c) && !char.IsDigit(c))
                    return true;

            return false;
        }

        #endregion

        #region Misc

        /// <summary>
        /// Unbinds transaction from the connector.
        /// Should be called before the connector is returned to the pool.
        /// </summary>
        internal void UnbindIfNecessary()
        {
            // We're closing the connection, but transaction is not yet disposed
            // We have to unbind the transaction from the connector, otherwise there could be a concurrency issues
            // See #3306
            if (!IsDisposed)
            {
                _connector.Transaction = null;
                _connector = null!;
            }
        }

        #endregion
    }
}
