using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;

namespace Npgsql.Internal.TypeHandling;

abstract class NullableHandler<T>
{
    static NullableHandler<T>? _derivedInstance;
    public static bool Exists => default(T) is null && typeof(T).IsValueType;

    static NullableHandler<T> DerivedInstance
    {
        get
        {
            Debug.Assert(Exists);
            return _derivedInstance ??= (NullableHandler<T>?)Activator.CreateInstance(typeof(NullableHandler<,>).MakeGenericType(typeof(T), typeof(T).GenericTypeArguments[0]))!;
        }
    }

    public static T Read(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLength, FieldDescription? fieldDescription = null) =>
        DerivedInstance.ReadImpl(handler, buffer, columnLength, fieldDescription);
    public static ValueTask<T> ReadAsync(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLength, bool async, FieldDescription? fieldDescription = null) =>
        DerivedInstance.ReadAsyncImpl(handler, buffer, columnLength, async, fieldDescription);
    public static int ValidateAndGetLength(NpgsqlTypeHandler handler, T value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter) =>
        DerivedInstance.ValidateAndGetLengthImpl(handler, value, ref lengthCache, parameter);
    public static Task WriteAsync(NpgsqlTypeHandler handler, T value, NpgsqlWriteBuffer buffer, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default) =>
        DerivedInstance.WriteAsyncImpl(handler, value, buffer, lengthCache, parameter, async, cancellationToken);

    protected abstract T ReadImpl(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLength, FieldDescription? fieldDescription = null);
    protected abstract ValueTask<T> ReadAsyncImpl(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLen, bool async, FieldDescription? fieldDescription = null);
    protected abstract int ValidateAndGetLengthImpl(NpgsqlTypeHandler handler, T value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter);
    protected abstract Task WriteAsyncImpl(NpgsqlTypeHandler handler, T value, NpgsqlWriteBuffer buffer, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default);
}

class NullableHandler<T, TUnderlying> : NullableHandler<T>
    where TUnderlying : struct
{
    protected override T ReadImpl(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLength, FieldDescription? fieldDescription = null)
        => (T)(object)handler.Read<TUnderlying>(buffer, columnLength, fieldDescription);

    protected override async ValueTask<T> ReadAsyncImpl(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLength, bool async, FieldDescription? fieldDescription = null)
        => (T)(object)await handler.Read<TUnderlying>(buffer, columnLength, async, fieldDescription);

    protected override int ValidateAndGetLengthImpl(NpgsqlTypeHandler handler, T value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter) =>
        value != null ? handler.ValidateAndGetLength(((TUnderlying?)(object)value).Value, ref lengthCache, parameter) : 0;

    protected override Task WriteAsyncImpl(NpgsqlTypeHandler handler, T value, NpgsqlWriteBuffer buffer, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => value != null
            ? handler.WriteWithLength(((TUnderlying?)(object)value).Value, buffer, lengthCache, parameter, async, cancellationToken)
            : handler.WriteWithLength(DBNull.Value, buffer, lengthCache, parameter, async, cancellationToken);
}
