using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
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

        internal sealed override (int Total, int Idle, int Busy) Statistics => (_numConnectors, 0, _numConnectors);

        internal sealed override async ValueTask<NpgsqlConnector> Get(
            NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            var connector = new NpgsqlConnector(conn, this);
            await connector.Open(timeout, async, queryClusterState: false, cancellationToken);
            Interlocked.Increment(ref _numConnectors);
            return connector;
        }

        internal sealed override void Return(NpgsqlConnector connector)
        {
            Interlocked.Decrement(ref _numConnectors);
            connector.Close();
        }

        internal sealed override void Clear() {}

        internal sealed override bool TryRentEnlistedPending(Transaction transaction, [NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            connector = null;
            return false;
        }

        internal sealed override void TryRemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        {
        }
    }
}
