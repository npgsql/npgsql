using Npgsql.Internal;
using Npgsql.Util;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Npgsql;

sealed class MultiHostDataSourceWrapper(NpgsqlMultiHostDataSource wrappedSource, TargetSessionAttributes targetSessionAttributes)
    : NpgsqlDataSource(CloneSettingsForTargetSessionAttributes(wrappedSource.Settings, targetSessionAttributes), wrappedSource.Configuration)
{
    internal override bool OwnsConnectors => false;

    public override void Clear() => wrappedSource.Clear();

    static NpgsqlConnectionStringBuilder CloneSettingsForTargetSessionAttributes(
        NpgsqlConnectionStringBuilder settings,
        TargetSessionAttributes targetSessionAttributes)
    {
        var clonedSettings = settings.Clone();
        clonedSettings.TargetSessionAttributesParsed = targetSessionAttributes;
        return clonedSettings;
    }

    internal override (int Total, int Idle, int Busy) Statistics => wrappedSource.Statistics;

    internal override ValueTask<NpgsqlConnector> Get(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        => wrappedSource.Get(conn, timeout, async, cancellationToken);
    internal override bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
        => throw new NpgsqlException("Npgsql bug: trying to get an idle connector from " + nameof(MultiHostDataSourceWrapper));
    internal override ValueTask<NpgsqlConnector?> OpenNewConnector(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        => throw new NpgsqlException("Npgsql bug: trying to open a new connector from " + nameof(MultiHostDataSourceWrapper));
    internal override void Return(NpgsqlConnector connector)
        => wrappedSource.Return(connector);

    internal override void AddPendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        => wrappedSource.AddPendingEnlistedConnector(connector, transaction);
    internal override bool TryRemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        => wrappedSource.TryRemovePendingEnlistedConnector(connector, transaction);
    internal override bool TryRentEnlistedPending(Transaction transaction, NpgsqlConnection connection,
        [NotNullWhen(true)] out NpgsqlConnector? connector)
        => wrappedSource.TryRentEnlistedPending(transaction, connection, out connector);
}
