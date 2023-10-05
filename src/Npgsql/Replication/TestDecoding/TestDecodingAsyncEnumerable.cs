using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication.Internal;
using NpgsqlTypes;

namespace Npgsql.Replication.TestDecoding;

sealed class TestDecodingAsyncEnumerable : IAsyncEnumerable<TestDecodingData>
{
    readonly LogicalReplicationConnection _connection;
    readonly TestDecodingReplicationSlot _slot;
    readonly TestDecodingOptions _options;
    readonly CancellationToken _baseCancellationToken;
    readonly NpgsqlLogSequenceNumber? _walLocation;

    readonly TestDecodingData _cachedMessage = new();

    internal TestDecodingAsyncEnumerable(
        LogicalReplicationConnection connection,
        TestDecodingReplicationSlot slot,
        TestDecodingOptions options,
        CancellationToken cancellationToken,
        NpgsqlLogSequenceNumber? walLocation = null)
    {
        _connection = connection;
        _slot = slot;
        _options = options;
        _baseCancellationToken = cancellationToken;
        _walLocation = walLocation;
    }

    public async IAsyncEnumerator<TestDecodingData> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_baseCancellationToken, cancellationToken).Token;

        var stream = _connection.StartLogicalReplication(
            _slot, cancellationToken, _walLocation, _options.GetOptionPairs());
        var encoding = _connection.Encoding!;

        var buffer = ArrayPool<byte>.Shared.Rent(4096);

        try
        {
            await foreach (var msg in stream.ConfigureAwait(false))
            {
                var len = (int)msg.Data.Length;
                Debug.Assert(msg.Data.Position == 0);
                if (len > buffer.Length)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = ArrayPool<byte>.Shared.Rent(len);
                }

                var offset = 0;
                while (offset < len)
                {
                    var read = await msg.Data.ReadAsync(buffer, offset, len - offset, CancellationToken.None).ConfigureAwait(false);
                    if (read == 0)
                        throw new EndOfStreamException();
                    offset += read;
                }

                Debug.Assert(offset == len);
                var data = encoding.GetString(buffer, 0, len);

                yield return _cachedMessage.Populate(msg.WalStart, msg.WalEnd, msg.ServerClock, data);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
