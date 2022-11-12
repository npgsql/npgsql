#if !NET7_0_OR_GREATER

using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member (compatibility shim for old TFMs)

// ReSharper disable once CheckNamespace
namespace System.Data.Common;

public abstract class DbDataSource : IDisposable, IAsyncDisposable
{
    public abstract string ConnectionString { get; }

    protected abstract DbConnection CreateDbConnection();

    // No need for an actual implementation in this compat shim - it's only implementation will be NpgsqlDataSource, which overrides this.
    protected virtual DbConnection OpenDbConnection()
        => throw new NotSupportedException();

    // No need for an actual implementation in this compat shim - it's only implementation will be NpgsqlDataSource, which overrides this.
    protected virtual ValueTask<DbConnection> OpenDbConnectionAsync(CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    // No need for an actual implementation in this compat shim - it's only implementation will be NpgsqlDataSource, which overrides this.
    protected virtual DbCommand CreateDbCommand(string? commandText = null)
        => throw new NotSupportedException();

    // No need for an actual implementation in this compat shim - it's only implementation will be NpgsqlDataSource, which overrides this.
    protected virtual DbBatch CreateDbBatch()
        => throw new NotSupportedException();

    public DbConnection CreateConnection()
        => CreateDbConnection();

    public DbConnection OpenConnection()
        => OpenDbConnection();

    public ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
        => OpenDbConnectionAsync(cancellationToken);

    public DbCommand CreateCommand(string? commandText = null)
        => CreateDbCommand(commandText);

    public DbBatch CreateBatch()
        => CreateDbBatch();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    protected virtual ValueTask DisposeAsyncCore()
        => default;
}

#endif