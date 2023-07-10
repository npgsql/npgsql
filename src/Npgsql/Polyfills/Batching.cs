#if !NET6_0_OR_GREATER
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 1591,RS0016

// ReSharper disable once CheckNamespace
namespace System.Data.Common
{
    public abstract class DbBatch : IDisposable, IAsyncDisposable
    {
        public DbBatchCommandCollection BatchCommands => DbBatchCommands;

        protected abstract DbBatchCommandCollection DbBatchCommands { get; }

        public abstract int Timeout { get; set; }

        public DbConnection? Connection
        {
            get => DbConnection;
            set => DbConnection = value;
        }

        protected abstract DbConnection? DbConnection { get; set; }

        public DbTransaction? Transaction
        {
            get => DbTransaction;
            set => DbTransaction = value;
        }

        protected abstract DbTransaction? DbTransaction { get; set; }

        public DbDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
            => ExecuteDbDataReader(behavior);

        protected abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);

        public Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
            => ExecuteDbDataReaderAsync(CommandBehavior.Default, cancellationToken);

        public Task<DbDataReader> ExecuteReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken = default)
            => ExecuteDbDataReaderAsync(behavior, cancellationToken);

        protected abstract Task<DbDataReader> ExecuteDbDataReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken);

        public abstract int ExecuteNonQuery();

        public abstract Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default);

        public abstract object? ExecuteScalar();

        public abstract Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken = default);

        public abstract void Prepare();

        public abstract Task PrepareAsync(CancellationToken cancellationToken = default);

        public abstract void Cancel();

        public DbBatchCommand CreateBatchCommand() => CreateDbBatchCommand();

        protected abstract DbBatchCommand CreateDbBatchCommand();

        public virtual void Dispose() {}

        public virtual ValueTask DisposeAsync()
        {
            Dispose();
            return default;
        }
    }

    public abstract class DbBatchCommand
    {
        public abstract string CommandText { get; set; }

        public abstract CommandType CommandType { get; set; }

        public abstract int RecordsAffected { get; }

        public DbParameterCollection Parameters => DbParameterCollection;

        protected abstract DbParameterCollection DbParameterCollection { get; }
    }

    public abstract class DbBatchCommandCollection : IList<DbBatchCommand>
    {
        public abstract IEnumerator<DbBatchCommand> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public abstract void Add(DbBatchCommand item);

        public abstract void Clear();

        public abstract bool Contains(DbBatchCommand item);

        public abstract void CopyTo(DbBatchCommand[] array, int arrayIndex);

        public abstract bool Remove(DbBatchCommand item);

        public abstract int Count { get; }

        public abstract bool IsReadOnly { get; }

        public abstract int IndexOf(DbBatchCommand item);

        public abstract void Insert(int index, DbBatchCommand item);

        public abstract void RemoveAt(int index);

        public DbBatchCommand this[int index]
        {
            get => GetBatchCommand(index);
            set => SetBatchCommand(index, value);
        }

        protected abstract DbBatchCommand GetBatchCommand(int index);

        protected abstract void SetBatchCommand(int index, DbBatchCommand batchCommand);
    }
}
#endif
