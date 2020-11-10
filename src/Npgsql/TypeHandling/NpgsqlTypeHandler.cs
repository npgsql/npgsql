using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Base class for all type handlers, which read and write CLR types into their PostgreSQL
    /// binary representation.
    /// Type handler writers shouldn't inherit from this class, inherit <see cref="NpgsqlTypeHandler"/>
    /// or <see cref="NpgsqlSimpleTypeHandler{T}"/> instead.
    /// </summary>
    public abstract class NpgsqlTypeHandler
    {
        /// <summary>
        /// The PostgreSQL type handled by this type handler.
        /// </summary>
        internal PostgresType PostgresType { get; }

        /// <summary>
        /// Constructs a <see cref="NpgsqlTypeHandler"/>.
        /// </summary>
        protected NpgsqlTypeHandler(PostgresType postgresType) => PostgresType = postgresType;

        #region Read

        /// <summary>
        /// Reads a value of type <typeparamref name="TAny"/> with the given length from the provided buffer,
        /// using either sync or async I/O.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        protected internal abstract ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null);

        /// <summary>
        /// Reads a value of type <typeparamref name="TAny"/> with the given length from the provided buffer,
        /// with the assumption that it is entirely present in the provided memory buffer and no I/O will be
        /// required. This can save the overhead of async functions and improves performance.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public abstract TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null);

        /// <summary>
        /// Reads a column as the type handler's default read type, assuming that it is already entirely
        /// in memory (i.e. no I/O is necessary). Called by <see cref="NpgsqlDataReader"/> in non-sequential mode, which
        /// buffers entire rows in memory.
        /// </summary>
        internal abstract object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null);

        /// <summary>
        /// Reads a column as the type handler's default read type. If it is not already entirely in
        /// memory, sync or async I/O will be performed as specified by <paramref name="async"/>.
        /// </summary>
        internal abstract ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null);

        /// <summary>
        /// Reads a column as the type handler's provider-specific type, assuming that it is already entirely
        /// in memory (i.e. no I/O is necessary). Called by <see cref="NpgsqlDataReader"/> in non-sequential mode, which
        /// buffers entire rows in memory.
        /// </summary>
        internal virtual object ReadPsvAsObject(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => ReadAsObject(buf, len, fieldDescription);

        /// <summary>
        /// Reads a column as the type handler's provider-specific type. If it is not already entirely in
        /// memory, sync or async I/O will be performed as specified by <paramref name="async"/>.
        /// </summary>
        internal virtual ValueTask<object> ReadPsvAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => ReadAsObject(buf, len, async, fieldDescription);

        /// <summary>
        /// Reads a value from the buffer, assuming our read position is at the value's preceding length.
        /// If the length is -1 (null), this method will return the default value.
        /// </summary>
        internal async ValueTask<TAny> ReadWithLength<TAny>(NpgsqlReadBuffer buf, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var len = buf.ReadInt32();
            return len == -1
               ? default!
               : NullableHandler<TAny>.Exists
                   ? await NullableHandler<TAny>.ReadAsync(this, buf, len, async, fieldDescription)
                   : await Read<TAny>(buf, len, async, fieldDescription);
        }

        #endregion

        #region Write

        /// <summary>
        /// Called to validate and get the length of a value of a generic <see cref="NpgsqlParameter{T}"/>.
        /// </summary>
        protected internal abstract int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter);

        /// <summary>
        /// Called to write the value of a generic <see cref="NpgsqlParameter{T}"/>.
        /// </summary>
        internal abstract Task WriteWithLengthInternal<TAny>([AllowNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default);

        /// <summary>
        /// Responsible for validating that a value represents a value of the correct and which can be
        /// written for PostgreSQL - if the value cannot be written for any reason, an exception shold be thrown.
        /// Also returns the byte length needed to write the value.
        /// </summary>
        /// <param name="value">The value to be written to PostgreSQL</param>
        /// <param name="lengthCache">
        /// If the byte length calculation is costly (e.g. for UTF-8 strings), its result can be stored in the
        /// length cache to be reused in the writing process, preventing recalculation.
        /// </param>
        /// <param name="parameter">
        /// The <see cref="NpgsqlParameter"/> instance where this value resides. Can be used to access additional
        /// information relevant to the write process (e.g. <see cref="NpgsqlParameter.Size"/>).
        /// </param>
        /// <returns>The number of bytes required to write the value.</returns>
        protected internal abstract int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter);

        /// <summary>
        /// Writes a value to the provided buffer, using either sync or async I/O.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="buf">The buffer to which to write.</param>
        /// <param name="lengthCache"></param>
        /// <param name="parameter">
        /// The <see cref="NpgsqlParameter"/> instance where this value resides. Can be used to access additional
        /// information relevant to the write process (e.g. <see cref="NpgsqlParameter.Size"/>).
        /// </param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        protected internal abstract Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default);

        #endregion Write

        #region Misc

        internal abstract Type GetFieldType(FieldDescription? fieldDescription = null);
        internal abstract Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null);

        internal virtual bool PreferTextWrite => false;

        /// <summary>
        /// Creates a type handler for arrays of this handler's type.
        /// </summary>
        public abstract ArrayHandler CreateArrayHandler(PostgresArrayType arrayBackendType);

        /// <summary>
        /// Creates a type handler for ranges of this handler's type.
        /// </summary>
        public abstract IRangeHandler CreateRangeHandler(PostgresType rangeBackendType);

        /// <summary>
        /// Used to create an exception when the provided type can be converted and written, but an
        /// instance of <see cref="NpgsqlParameter"/> is required for caching of the converted value
        /// (in <see cref="NpgsqlParameter.ConvertedValue"/>.
        /// </summary>
        protected Exception CreateConversionButNoParamException(Type clrType)
            => new InvalidCastException($"Can't convert .NET type '{clrType}' to PostgreSQL '{PgDisplayName}' within an array");

        internal string PgDisplayName => PostgresType.DisplayName;

        #endregion Misc

        #region Code generation for non-generic writing

        internal delegate Task NonGenericWriteWithLength(NpgsqlTypeHandler handler, object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken);

        internal static NonGenericWriteWithLength GenerateNonGenericWriteMethod(Type handlerType, Type interfaceType)
        {
            var interfaces = handlerType.GetInterfaces().Where(i =>
                i.GetTypeInfo().IsGenericType &&
                i.GetGenericTypeDefinition() == interfaceType
            ).Reverse().ToList();

            Expression? ifElseExpression = null;

            // NpgsqlTypeHandler handler, object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async, CancellationToken cancellationToken
            var handlerParam = Expression.Parameter(typeof(NpgsqlTypeHandler), "handler");
            var valueParam = Expression.Parameter(typeof(object), "value");
            var bufParam = Expression.Parameter(typeof(NpgsqlWriteBuffer), "buf");
            var lengthCacheParam = Expression.Parameter(typeof(NpgsqlLengthCache), "lengthCache");
            var parameterParam = Expression.Parameter(typeof(NpgsqlParameter), "parameter");
            var asyncParam = Expression.Parameter(typeof(bool), "async");
            var cancellationTokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            var resultVariable = Expression.Variable(typeof(Task), "result");

            foreach (var i in interfaces)
            {
                var handledType = i.GenericTypeArguments[0];

                ifElseExpression = Expression.IfThenElse(
                    // Test whether the type of the value given to the delegate corresponds
                    // to our current interface's handled type (i.e. the T in INpgsqlTypeHandler<T>)
                    Expression.TypeEqual(valueParam, handledType),
                    // If it corresponds, call the handler's Write method with the appropriate generic parameter
                    Expression.Assign(
                        resultVariable,
                        Expression.Call(
                            handlerParam,
                            // Call the generic WriteWithLengthInternal<T2> with our handled type
                            nameof(WriteWithLengthInternal),
                            new[] { handledType },
                            // Cast the value from object down to the interface's T
                            Expression.Convert(valueParam, handledType),
                            bufParam,
                            lengthCacheParam,
                            parameterParam,
                            asyncParam,
                            cancellationTokenParam
                        )
                    ),
                    // If this is the first interface we're looking at, the else clause throws.
                    // Note that this should never happen since we passed ValidateAndGetLength.
                    // Otherwise we stick the previous interface's IfThenElse in our else clause
                    ifElseExpression ?? Expression.Throw(Expression.New(typeof(InvalidCastException)))
                );
            }

            if (ifElseExpression is null)
                throw new Exception($"Type handler {handlerType.GetType().Name} does not implement the proper interface");

            return Expression.Lambda<NonGenericWriteWithLength>(
                Expression.Block(
                    new[] { resultVariable },
                    ifElseExpression, resultVariable
                ),
                handlerParam, valueParam, bufParam, lengthCacheParam, parameterParam, asyncParam, cancellationTokenParam
            ).Compile();
        }

        #endregion Code generation for non-generic writing
    }
}
