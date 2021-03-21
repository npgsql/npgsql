using Npgsql.Util;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Npgsql
{
    sealed class MultiHostConnectorPoolWrapper : ConnectorSource
    {
        readonly ConnectorSource _wrappedSource;

        public MultiHostConnectorPoolWrapper(NpgsqlConnectionStringBuilder settings, string connString, ConnectorSource source) : base(settings, connString)
            => _wrappedSource = source;

        internal override (int Total, int Idle, int Busy) Statistics => _wrappedSource.Statistics;

        internal override void Clear() => _wrappedSource.Clear();
        internal override ValueTask<NpgsqlConnector> Get(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
            => _wrappedSource.Get(conn, timeout, async, cancellationToken);
        internal override void Return(NpgsqlConnector connector)
            => _wrappedSource.Return(connector);

        internal override void AddPendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
            => _wrappedSource.AddPendingEnlistedConnector(connector, transaction);
        internal override void TryRemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
            => _wrappedSource.TryRemovePendingEnlistedConnector(connector, transaction);
        internal override bool TryRentEnlistedPending(Transaction transaction, [NotNullWhen(true)] out NpgsqlConnector connector)
            => _wrappedSource.TryRentEnlistedPending(transaction, out connector);
    }
}
