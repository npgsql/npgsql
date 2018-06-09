using Npgsql.BackendMessages;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Npgsql.TypeHandling
{
    internal delegate T ReadDelegate<T>(NpgsqlReadBuffer buffer, int columnLength, FieldDescription fieldDescription);
    internal delegate ValueTask<T> ReadAsyncDelegate<T>(NpgsqlReadBuffer columnLength, int columnLen, bool async, FieldDescription fieldDescription);

    internal delegate int ValidateDelegate<T>(NpgsqlTypeHandler handler, T value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter);
    internal delegate Task WriteAsyncDelegate<T>(NpgsqlTypeHandler handler, T value, NpgsqlWriteBuffer buffer, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async);

    internal static class NullableHandler<T>
    {
        public static readonly Type UnderlyingType;
        public static readonly ReadDelegate<T> Read;
        public static readonly ReadAsyncDelegate<T> ReadAsync;
        public static readonly ValidateDelegate<T> Validate;
        public static readonly WriteAsyncDelegate<T> WriteAsync;

        public static bool Exists => UnderlyingType != null;

        static NullableHandler()
        {
            UnderlyingType = Nullable.GetUnderlyingType(typeof(T));
            if (UnderlyingType == null) return;
            Read = NullableHandler.CreateDelegate<ReadDelegate<T>>(UnderlyingType, NullableHandler.ReadMethod);
            ReadAsync = NullableHandler.CreateDelegate<ReadAsyncDelegate<T>>(UnderlyingType, NullableHandler.ReadAsyncMethod);
            Validate = NullableHandler.CreateDelegate<ValidateDelegate<T>>(UnderlyingType, NullableHandler.ValidateMethod);
            WriteAsync = NullableHandler.CreateDelegate<WriteAsyncDelegate<T>>(UnderlyingType, NullableHandler.WriteAsyncMethod);
        }
    }

    static class NullableHandler
    {
        internal static readonly MethodInfo ReadMethod = new ReadDelegate<int?>(Read<int>).Method.GetGenericMethodDefinition();
        internal static readonly MethodInfo ReadAsyncMethod = new ReadAsyncDelegate<int?>(ReadAsync<int>).Method.GetGenericMethodDefinition();
        internal static readonly MethodInfo ValidateMethod = new ValidateDelegate<int?>(ValidateAndGetLength).Method.GetGenericMethodDefinition();
        internal static readonly MethodInfo WriteAsyncMethod = new WriteAsyncDelegate<int?>(WriteAsync).Method.GetGenericMethodDefinition();

        static T? Read<T>(NpgsqlReadBuffer buffer, int columnLength, FieldDescription fieldDescription) where T : struct
            => fieldDescription.Handler.Read<T>(buffer, columnLength, fieldDescription);

        static async ValueTask<T?> ReadAsync<T>(NpgsqlReadBuffer buffer, int columnLength, bool async, FieldDescription fieldDescription) where T : struct
            => await fieldDescription.Handler.Read<T>(buffer, columnLength, async, fieldDescription);

        static int ValidateAndGetLength<T>(NpgsqlTypeHandler handler, T? value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter) where T : struct
            => handler.ValidateAndGetLength(value.Value, ref lengthCache, parameter);

        static Task WriteAsync<T>(NpgsqlTypeHandler handler, T? value, NpgsqlWriteBuffer buffer, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async) where T : struct
            => handler.WriteWithLength(value.Value, buffer, lengthCache, parameter, async);

        internal static TDelegate CreateDelegate<TDelegate>(Type underlyingType, MethodInfo method) where TDelegate : Delegate
            => (TDelegate)method.MakeGenericMethod(underlyingType).CreateDelegate(typeof(TDelegate));
    }
}
