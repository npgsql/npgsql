using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Npgsql.Util;

namespace Npgsql
{
    class UnpooledConnectorSource : ConnectorSource
    {
        public UnpooledConnectorSource(NpgsqlConnectionStringBuilder settings, string connString)
            : base(settings, connString)
        {
        }

        volatile int _numConnectors;

        internal override (int Total, int Idle, int Busy) Statistics => (_numConnectors, 0, _numConnectors);

        internal override async ValueTask<NpgsqlConnector> Get(
            NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            var connector = new NpgsqlConnector(conn, this);
            await connector.Open(timeout, async, queryState: false, cancellationToken);
            Interlocked.Increment(ref _numConnectors);
            return connector;
        }

        internal override void Return(NpgsqlConnector connector)
        {
            Interlocked.Decrement(ref _numConnectors);
            connector.Close();
        }

        internal override void Clear() {}

        internal override bool TryRentEnlistedPending(Transaction transaction, [NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            connector = null;
            return false;
        }

        internal override void TryRemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        {
        }
    }
}
