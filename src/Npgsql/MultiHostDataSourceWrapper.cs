using Npgsql.Internal;
using Npgsql.Util;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Npgsql;

sealed class MultiHostDataSourceWrapper : NpgsqlDataSource
{
    internal override bool OwnsConnectors => false;

    readonly NpgsqlMultiHostDataSource _wrappedSource;

    public MultiHostDataSourceWrapper(NpgsqlMultiHostDataSource source, TargetSessionAttributes targetSessionAttributes)
        : base(CloneSettingsForTargetSessionAttributes(source.Settings, targetSessionAttributes), source.Configuration)
        => _wrappedSource = source;

    static NpgsqlConnectionStringBuilder CloneSettingsForTargetSessionAttributes(
        NpgsqlConnectionStringBuilder settings,
        TargetSessionAttributes targetSessionAttributes)
    {
        var clonedSettings = settings.Clone();
        clonedSettings.TargetSessionAttributesParsed = targetSessionAttributes;
        return clonedSettings;
    }

    internal override (int Total, int Idle, int Busy) Statistics => _wrappedSource.Statistics;

    internal override void Clear() => _wrappedSource.Clear();
    internal override ValueTask<NpgsqlConnector> Get(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        => _wrappedSource.Get(conn, timeout, async, cancellationToken);
    internal override bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
        => throw new NpgsqlException("Npgsql bug: trying to get an idle connector from " + nameof(MultiHostDataSourceWrapper));
    internal override ValueTask<NpgsqlConnector?> OpenNewConnector(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        => throw new NpgsqlException("Npgsql bug: trying to open a new connector from " + nameof(MultiHostDataSourceWrapper));
    internal override void Return(NpgsqlConnector connector)
        => _wrappedSource.Return(connector);

    internal override void AddPendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        => _wrappedSource.AddPendingEnlistedConnector(connector, transaction);
    internal override bool TryRemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        => _wrappedSource.TryRemovePendingEnlistedConnector(connector, transaction);
    internal override bool TryRentEnlistedPending(Transaction transaction, NpgsqlConnection connection,
        [NotNullWhen(true)] out NpgsqlConnector? connector)
        => _wrappedSource.TryRentEnlistedPending(transaction, connection, out connector);
}