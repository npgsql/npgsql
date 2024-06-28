using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.PostgresTypes;

namespace Npgsql.Replication.PgOutput;

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

    FieldDescription _fieldDescription = null!;
    ColumnInfo _lastInfo;
    bool _isConsumed;

    PgReader PgReader => _readBuffer.PgReader;

    internal ReplicationValue(NpgsqlConnector connector) => _readBuffer = connector.ReadBuffer;

    internal void Reset(TupleDataKind kind, int length, FieldDescription fieldDescription)
    {
        Kind = kind;
        Length = length;
        _fieldDescription = fieldDescription;
        _lastInfo = default;
        _isConsumed = false;
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
    /// Gets the name of the specified column.
    /// </summary>
    /// <returns>The name of the specified column.</returns>
    public string GetFieldName() => _fieldDescription.Name;

    /// <summary>
    /// Gets the value of the specified column as a type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns></returns>
    public async ValueTask<T> Get<T>(CancellationToken cancellationToken = default)
    {
        CheckActive();

        _fieldDescription.GetInfo(typeof(T), ref _lastInfo);
        var info = _lastInfo;

        switch (Kind)
        {
        case TupleDataKind.Null:
            // When T is a Nullable<T> (and only in that case), we support returning null
            if (default(T) is null && typeof(T).IsValueType)
                return default!;

            if (typeof(T) == typeof(object))
                return (T)(object)DBNull.Value;

            ThrowHelper.ThrowInvalidCastException_NoValue(_fieldDescription);
            break;

        case TupleDataKind.UnchangedToastedValue:
            throw new InvalidCastException(
                $"Column '{_fieldDescription.Name}' is an unchanged TOASTed value (actual value not sent).");
        }

        using var registration = _readBuffer.Connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);

        var reader = PgReader;
        reader.Init(Length, _fieldDescription.DataFormat);
        await reader.StartReadAsync(info.ConverterInfo.BufferRequirement, cancellationToken).ConfigureAwait(false);
        var result = info.AsObject
            ? (T)await info.ConverterInfo.Converter.ReadAsObjectAsync(reader, cancellationToken).ConfigureAwait(false)
            : await info.ConverterInfo.Converter.UnsafeDowncast<T>().ReadAsync(reader, cancellationToken).ConfigureAwait(false);
        await reader.EndReadAsync().ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="object"/>.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns></returns>
    public ValueTask<object> Get(CancellationToken cancellationToken = default) => Get<object>(cancellationToken);

    /// <summary>
    /// Retrieves data as a <see cref="Stream"/>.
    /// </summary>
    public Stream GetStream()
    {
        CheckActive();

        switch (Kind)
        {
        case TupleDataKind.Null:
            ThrowHelper.ThrowInvalidCastException_NoValue(_fieldDescription);
            break;

        case TupleDataKind.UnchangedToastedValue:
            throw new InvalidCastException($"Column '{_fieldDescription.Name}' is an unchanged TOASTed value (actual value not sent).");
        }

        var reader = PgReader;
        reader.Init(Length, _fieldDescription.DataFormat);
        return reader.GetStream(canSeek: false);
    }

    /// <summary>
    /// Retrieves data as a <see cref="TextReader"/>.
    /// </summary>
    public TextReader GetTextReader()
    {
        CheckActive();

        ref var info = ref _lastInfo;
        _fieldDescription.GetInfo(typeof(TextReader), ref info);

        switch (Kind)
        {
        case TupleDataKind.Null:
            ThrowHelper.ThrowInvalidCastException_NoValue(_fieldDescription);
            break;

        case TupleDataKind.UnchangedToastedValue:
            throw new InvalidCastException($"Column '{_fieldDescription.Name}' is an unchanged TOASTed value (actual value not sent).");
        }

        var reader = PgReader;
        reader.Init(Length, _fieldDescription.DataFormat);
        reader.StartRead(info.ConverterInfo.BufferRequirement);
        var result = (TextReader)info.ConverterInfo.Converter.ReadAsObject(reader);
        reader.EndRead();
        return result;
    }

    internal async Task Consume(CancellationToken cancellationToken)
    {
        if (_isConsumed)
            return;

        var reader = PgReader;
        if (!reader.Initialized)
            reader.Init(Length, _fieldDescription.DataFormat);
        await reader.ConsumeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        await reader.CommitAsync().ConfigureAwait(false);

        _isConsumed = true;
    }

    void CheckActive()
    {
        if (PgReader.Initialized)
            throw new InvalidOperationException("Column has already been consumed");
    }
}
