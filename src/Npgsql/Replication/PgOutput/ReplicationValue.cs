using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.Replication.PgOutput.Messages;

namespace Npgsql.Replication.PgOutput
{
    /// <summary>
    /// Represents a column value in a logical replication session.
    /// </summary>
    public class ReplicationValue
    {
        readonly NpgsqlReadBuffer _readBuffer;

        /// <summary>
        /// The length of the value in bytes.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The kind of data transmitted for a tuple in a Logical Replication Protocol message.
        /// </summary>
        public TupleDataKind Kind { get; private set; }

        bool _columnConsumed;
        FieldDescription _fieldDescription = null!;

        /// <summary>
        /// A stream that has been opened on a column.
        /// </summary>
        readonly NpgsqlReadBuffer.ColumnStream _columnStream;

        internal ReplicationValue(NpgsqlConnector connector)
        {
            _readBuffer = connector.ReadBuffer;
            _columnStream = new NpgsqlReadBuffer.ColumnStream(connector, startCancellableOperations: false);
        }

        internal void Reset(TupleDataKind kind, int length, FieldDescription fieldDescription)
        {
            Kind = kind;
            Length = length;
            _fieldDescription = fieldDescription;
            _columnConsumed = false;
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <returns><b>true</b> if the specified column is equivalent to <see cref="DBNull"/>; otherwise <b>false</b>.</returns>
        public bool IsDBNull
            => Kind == TupleDataKind.Null;

        /// <summary>
        /// Gets a value that indicates whether the column contains an unchanged TOASTed value (the actual value is not sent).
        /// </summary>
        /// <returns>Whether the specified column is an unchanged TOASTed value.</returns>
        public bool IsUnchangedToastedValue
            => Kind == TupleDataKind.UnchangedToastedValue;

        /// <summary>
        /// Gets a representation of the PostgreSQL data type for the specified field.
        /// The returned representation can be used to access various information about the field.
        /// </summary>
        public PostgresType GetPostgresType() => _fieldDescription.PostgresType;

        /// <summary>
        /// Gets the data type information for the specified field.
        /// This is be the PostgreSQL type name (e.g. double precision), not the .NET type
        /// (see <see cref="GetFieldType"/> for that).
        /// </summary>
        public string GetDataTypeName() => _fieldDescription.TypeDisplayName;

        /// <summary>
        /// Gets the data type of the specified column.
        /// </summary>
        /// <returns>The data type of the specified column.</returns>
        public Type GetFieldType() => _fieldDescription.FieldType;

        /// <summary>
        /// Gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="T">The type of the value to be returned.</typeparam>
        /// <param name="cancellationToken">
        /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns></returns>
        public ValueTask<T> Get<T>(CancellationToken cancellationToken = default)
        {
            CheckAndMarkConsumed();

            switch (Kind)
            {
            case TupleDataKind.Null:
                // When T is a Nullable<T> (and only in that case), we support returning null
                if (NullableHandler<T>.Exists)
                    return default!;

                if (typeof(T) == typeof(object))
                    return new ValueTask<T>((T)(object)DBNull.Value);

                ThrowHelper.ThrowInvalidCastException_NoValue(_fieldDescription);
                break;

            case TupleDataKind.UnchangedToastedValue:
                throw new InvalidCastException(
                    $"Column '{_fieldDescription.Name}' is an unchanged TOASTed value (actual value not sent).");
            }

            using (NoSynchronizationContextScope.Enter())
                return GetCore(cancellationToken);

            async ValueTask<T> GetCore(CancellationToken cancellationToken)
            {
                using var tokenRegistration = _readBuffer.ReadBytesLeft < Length
                    ? _readBuffer.Connector.StartNestedCancellableOperation(cancellationToken)
                    : default;

                var position = _readBuffer.ReadPosition;

                try
                {
                    return NullableHandler<T>.Exists
                        ? await NullableHandler<T>.ReadAsync(_fieldDescription.Handler, _readBuffer, Length, async: true, _fieldDescription)
                        : typeof(T) == typeof(object)
                            ? (T)await _fieldDescription.Handler.ReadAsObject(_readBuffer, Length, async: true, _fieldDescription)
                            : await _fieldDescription.Handler.Read<T>(_readBuffer, Length, async: true, _fieldDescription);
                }
                catch
                {
                    if (_readBuffer.Connector.State != ConnectorState.Broken)
                    {
                        var writtenBytes = _readBuffer.ReadPosition - position;
                        var remainingBytes = Length - writtenBytes;
                        if (remainingBytes > 0)
                            _readBuffer.Skip(remainingBytes, false).GetAwaiter().GetResult();
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns></returns>
        public ValueTask<object> Get(CancellationToken cancellationToken = default)
        {
            CheckAndMarkConsumed();

            switch (Kind)
            {
            case TupleDataKind.Null:
                return new ValueTask<object>(DBNull.Value);

            case TupleDataKind.UnchangedToastedValue:
                throw new InvalidCastException(
                    $"Column '{_fieldDescription.Name}' is an unchanged TOASTed value (actual value not sent).");
            }

            using (NoSynchronizationContextScope.Enter())
                return GetCore(cancellationToken);

            async ValueTask<object> GetCore(CancellationToken cancellationToken)
            {
                using var tokenRegistration = _readBuffer.ReadBytesLeft < Length
                    ? _readBuffer.Connector.StartNestedCancellableOperation(cancellationToken)
                    : default;

                var position = _readBuffer.ReadPosition;

                try
                {
                    return await _fieldDescription.Handler.ReadAsObject(_readBuffer, Length, async: true, _fieldDescription);
                }
                catch
                {
                    if (_readBuffer.Connector.State != ConnectorState.Broken)
                    {
                        var writtenBytes = _readBuffer.ReadPosition - position;
                        var remainingBytes = Length - writtenBytes;
                        if (remainingBytes > 0)
                            _readBuffer.Skip(remainingBytes, false).GetAwaiter().GetResult();
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves data as a <see cref="Stream"/>.
        /// </summary>
        public Stream GetStream()
        {
            CheckAndMarkConsumed();

            switch (Kind)
            {
            case TupleDataKind.Null:
                ThrowHelper.ThrowInvalidCastException_NoValue(_fieldDescription);
                break;

            case TupleDataKind.UnchangedToastedValue:
                throw new InvalidCastException($"Column '{_fieldDescription.Name}' is an unchanged TOASTed value (actual value not sent).");
            }

            _columnStream.Init(Length, canSeek: false);
            return _columnStream;
        }

        /// <summary>
        /// Retrieves data as a <see cref="TextReader"/>.
        /// </summary>
        public TextReader GetTextReader()
            => _fieldDescription.Handler is ITextReaderHandler handler
                ? handler.GetTextReader(GetStream())
                : throw new InvalidCastException(
                    $"The GetTextReader method is not supported for type {_fieldDescription.Handler.PgDisplayName}");

        internal async Task Consume(CancellationToken cancellationToken)
        {
            if (!_columnStream.IsDisposed)
                await _columnStream.DisposeAsync();

            if (!_columnConsumed)
            {
                if (_readBuffer.ReadBytesLeft < 4)
                {
                    using var tokenRegistration = _readBuffer.Connector.StartNestedCancellableOperation(cancellationToken);
                    await _readBuffer.Skip(Length, async: true);
                }
                else
                {
                    await _readBuffer.Skip(Length, async: true);
                }
            }

            _columnConsumed = true;
        }

        void CheckAndMarkConsumed()
        {
            if (_columnConsumed)
                throw new InvalidOperationException("Column has already been consumed");
            _columnConsumed = true;
        }
    }
}
