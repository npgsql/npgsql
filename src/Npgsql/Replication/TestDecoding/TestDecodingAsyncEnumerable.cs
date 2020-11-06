using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication.Internal;
using NpgsqlTypes;

namespace Npgsql.Replication.TestDecoding
{
    class TestDecodingAsyncEnumerable : IAsyncEnumerable<NpgsqlTestDecodingData>
    {
        readonly NpgsqlLogicalReplicationConnection _connection;
        readonly NpgsqlTestDecodingReplicationSlot _slot;
        readonly NpgsqlTestDecodingOptions _options;
        readonly CancellationToken _baseCancellationToken;
        readonly NpgsqlLogSequenceNumber? _walLocation;

        readonly NpgsqlTestDecodingData _cachedMessage = new NpgsqlTestDecodingData();

        internal TestDecodingAsyncEnumerable(
            NpgsqlLogicalReplicationConnection connection,
            NpgsqlTestDecodingReplicationSlot slot,
            NpgsqlTestDecodingOptions options,
            CancellationToken cancellationToken,
            NpgsqlLogSequenceNumber? walLocation = null)
        {
            _connection = connection;
            _slot = slot;
            _options = options;
            _baseCancellationToken = cancellationToken;
            _walLocation = walLocation;
        }

        public IAsyncEnumerator<NpgsqlTestDecodingData> GetAsyncEnumerator(
            CancellationToken cancellationToken = new CancellationToken())
        {
            using (NoSynchronizationContextScope.Enter())
                return StartReplicationInternal(
                    CancellationTokenSource.CreateLinkedTokenSource(_baseCancellationToken, cancellationToken).Token);
        }

        async IAsyncEnumerator<NpgsqlTestDecodingData> StartReplicationInternal(CancellationToken cancellationToken)
        {
            var stream = _connection.StartLogicalReplication(
                _slot, cancellationToken, _walLocation, _options.GetOptionPairs());
            var encoding = _connection.Encoding!;

            await foreach (var msg in stream.WithCancellation(cancellationToken))
            {
                var memoryStream = new MemoryStream();
                await msg.Data.CopyToAsync(memoryStream, 4096, CancellationToken.None);
                var data = encoding.GetString(memoryStream.ToArray());
                yield return _cachedMessage.Populate(msg.WalStart, msg.WalEnd, msg.ServerClock, data);
            }
        }
    }
}
