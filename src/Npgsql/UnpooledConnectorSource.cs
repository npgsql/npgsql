using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Npgsql.Internal;
using Npgsql.Util;

namespace Npgsql
{
    sealed class UnpooledConnectorSource : ConnectorSource
    {
        public UnpooledConnectorSource(NpgsqlConnectionStringBuilder settings, string connString)
            : base(settings, connString)
        {
        }

        volatile int _numConnectors;

        internal override (int Total, int Idle, int Busy) Statistics => (_numConnectors, 0, _numConnectors);

        internal override bool OwnsConnectors => true;

        internal override async ValueTask<NpgsqlConnector> Get(
            NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            var connector = new NpgsqlConnector(this, conn);
            await connector.Open(timeout, async, cancellationToken);
            Interlocked.Increment(ref _numConnectors);
            return connector;
        }

        internal override bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            connector = null;
            return false;
        }

        internal override ValueTask<NpgsqlConnector?> OpenNewConnector(
            NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
            => new((NpgsqlConnector?)null);

        internal override void Return(NpgsqlConnector connector)
        {
            Interlocked.Decrement(ref _numConnectors);
            connector.Close();
        }

        internal override void Clear() {}

        internal override bool TryRentEnlistedPending(Transaction transaction, NpgsqlConnection connection,
            [NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            connector = null;
            return false;
        }

        internal override bool TryRemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction) => false;
    }
}
