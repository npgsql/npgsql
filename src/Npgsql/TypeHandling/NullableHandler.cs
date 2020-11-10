using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;

// ReSharper disable StaticMemberInGenericType
namespace Npgsql.TypeHandling
{
    delegate T ReadDelegate<T>(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLength, FieldDescription? fieldDescription = null);
    delegate ValueTask<T> ReadAsyncDelegate<T>(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLen, bool async, FieldDescription? fieldDescription = null);

    delegate int ValidateAndGetLengthDelegate<T>(NpgsqlTypeHandler handler, T value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter);
    delegate Task WriteAsyncDelegate<T>(NpgsqlTypeHandler handler, T value, NpgsqlWriteBuffer buffer, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default);

    static class NullableHandler<T>
    {
        public static readonly Type? UnderlyingType;
        [NotNull] public static readonly ReadDelegate<T>? Read;
        [NotNull] public static readonly ReadAsyncDelegate<T>? ReadAsync;
        [NotNull] public static readonly ValidateAndGetLengthDelegate<T>? ValidateAndGetLength;
        [NotNull] public static readonly WriteAsyncDelegate<T>? WriteAsync;

        public static bool Exists => UnderlyingType != null;

        static NullableHandler()
        {
            UnderlyingType = Nullable.GetUnderlyingType(typeof(T));

            if (UnderlyingType == null)
                return;

            Read = NullableHandler.CreateDelegate<ReadDelegate<T>>(UnderlyingType, NullableHandler.ReadMethod);
            ReadAsync = NullableHandler.CreateDelegate<ReadAsyncDelegate<T>>(UnderlyingType, NullableHandler.ReadAsyncMethod);
            ValidateAndGetLength = NullableHandler.CreateDelegate<ValidateAndGetLengthDelegate<T>>(UnderlyingType, NullableHandler.ValidateMethod);
            WriteAsync = NullableHandler.CreateDelegate<WriteAsyncDelegate<T>>(UnderlyingType, NullableHandler.WriteAsyncMethod);
        }
    }

    static class NullableHandler
    {
        internal static readonly MethodInfo ReadMethod = new ReadDelegate<int?>(Read<int>).Method.GetGenericMethodDefinition();
        internal static readonly MethodInfo ReadAsyncMethod = new ReadAsyncDelegate<int?>(ReadAsync<int>).Method.GetGenericMethodDefinition();
        internal static readonly MethodInfo ValidateMethod = new ValidateAndGetLengthDelegate<int?>(ValidateAndGetLength).Method.GetGenericMethodDefinition();
        internal static readonly MethodInfo WriteAsyncMethod = new WriteAsyncDelegate<int?>(WriteAsync).Method.GetGenericMethodDefinition();

        static T? Read<T>(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLength, FieldDescription? fieldDescription)
            where T : struct
            => handler.Read<T>(buffer, columnLength, fieldDescription);

        static async ValueTask<T?> ReadAsync<T>(NpgsqlTypeHandler handler, NpgsqlReadBuffer buffer, int columnLength, bool async, FieldDescription? fieldDescription)
            where T : struct
            => await handler.Read<T>(buffer, columnLength, async, fieldDescription);

        static int ValidateAndGetLength<T>(NpgsqlTypeHandler handler, T? value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            where T : struct
            => value.HasValue ? handler.ValidateAndGetLength(value.Value, ref lengthCache, parameter) : 0;

        static Task WriteAsync<T>(NpgsqlTypeHandler handler, T? value, NpgsqlWriteBuffer buffer, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            where T : struct
            => value.HasValue
                ? handler.WriteWithLengthInternal(value.Value, buffer, lengthCache, parameter, async, cancellationToken)
                : handler.WriteWithLengthInternal(DBNull.Value, buffer, lengthCache, parameter, async, cancellationToken);

        internal static TDelegate CreateDelegate<TDelegate>(Type underlyingType, MethodInfo method)
            where TDelegate : Delegate
            => (TDelegate)method.MakeGenericMethod(underlyingType).CreateDelegate(typeof(TDelegate));
    }
}
