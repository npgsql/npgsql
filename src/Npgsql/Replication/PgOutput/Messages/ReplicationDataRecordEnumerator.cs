using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;

namespace Npgsql.Replication.PgOutput.Messages
{
    sealed class ReplicationDataRecordEnumerator : IAsyncEnumerator<ReplicationTuple>, IEnumerator<ReplicationTuple>
    {
        readonly NpgsqlReadBuffer _readBuffer;
        ReplicationTuple? _current;
        bool _disposed;
        CancellationToken _cancellationToken;
        int _currentFieldIndex = -1;
        int _fieldCount;
        RowDescriptionMessage _tableInfo = default!;

        internal ReplicationDataRecordEnumerator(NpgsqlReadBuffer readBuffer)
            => _readBuffer = readBuffer;

        // Hack: Detect buffering by looking at the underlying stream
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        internal bool IsBuffered => _readBuffer.Underlying is MemoryStream;

        internal int CurrentFieldIndex => _currentFieldIndex;

        public ReplicationTuple Current => _disposed
            ? throw new ObjectDisposedException(nameof(ReplicationDataRecordEnumerator))
            : _current ?? throw new InvalidOperationException();

        object IEnumerator.Current => Current;

        public ValueTask<bool> MoveNextAsync()
            => MoveNext(async: true);

        public bool MoveNext()
            => MoveNext(async: false).GetAwaiter().GetResult();

        internal ValueTask<bool> MoveNext(bool async)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            using (NoSynchronizationContextScope.Enter())
                return MoveNextInternal(async);

            async ValueTask<bool> MoveNextInternal(bool async)
            {
                if(_current is null)
                    _current = new();
                else
                    await _current.Cleanup(async, _cancellationToken);

                _currentFieldIndex++;

                if (_currentFieldIndex >= _fieldCount)
                    return false;

                using var tokenRegistration = IsBuffered
                    ? default
                    : _readBuffer.Connector.StartNestedCancellableOperation(_cancellationToken);

                await _readBuffer.Ensure(1, async);
                var kind = (TupleDataKind)_readBuffer.ReadByte();
                switch (kind)
                {
                case TupleDataKind.Null:
                case TupleDataKind.UnchangedToastedValue:
                    _current.Init(_readBuffer, 0, kind, _tableInfo[_currentFieldIndex]);
                    break;
                case TupleDataKind.TextValue:
                case TupleDataKind.BinaryValue:
                    await _readBuffer.Ensure(4, async);
                    var len = _readBuffer.ReadInt32();
                    _current.Init(_readBuffer, len, kind, _tableInfo[_currentFieldIndex]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                }
                return true;
            }
        }

        public void Reset()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ReplicationDataRecordEnumerator));
            if (IsBuffered)
            {
                Debug.Assert(_readBuffer.Underlying.CanSeek);
                _currentFieldIndex = -1;
                _readBuffer.ReadPosition = 0;
                _readBuffer.Underlying.Position = 0;
            }
            else if (_currentFieldIndex > -1)
                throw new InvalidOperationException("Resetting streaming enumerators is not supported.");
        }

        public void Dispose()
        {
            if (!_disposed) while (MoveNext()) { /* Do nothing, just iterate the enumerator */ }
            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed) while (await MoveNextAsync()) { /* Do nothing, just iterate the enumerator */ }
            _disposed = true;
        }

        internal void Init(int fieldCount, RowDescriptionMessage tableInfo)
        {
            _disposed = false;
            _current = null;
            _currentFieldIndex = -1;
            _fieldCount = fieldCount;
            _tableInfo = tableInfo;
        }

        internal ReplicationDataRecordEnumerator SetCancellationToken(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            return this;
        }
    }
}
