using System;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandlers;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandling
{
    /// <summary>
    /// Base class for all type handlers, which read and write CLR types into their PostgreSQL
    /// binary representation. Unless your type is arbitrary-length, consider inheriting from
    /// <see cref="NpgsqlSimpleTypeHandler{T}"/> instead.
    /// </summary>
    /// <typeparam name="TDefault">
    /// The default CLR type that this handler will read and write. For example, calling <see cref="DbDataReader.GetValue"/>
    /// on a column with this handler will return a value with type <typeparamref name="TDefault"/>.
    /// Type handlers can support additional types by implementing <see cref="INpgsqlTypeHandler{T}"/>.
    /// </typeparam>
    public abstract class NpgsqlTypeHandler<TDefault> : NpgsqlTypeHandler, INpgsqlTypeHandler<TDefault>
    {
        /// <summary>
        /// Constructs an <see cref="NpgsqlTypeHandler{TDefault}"/>.
        /// </summary>
        protected NpgsqlTypeHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// using either sync or async I/O.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public abstract ValueTask<TDefault> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null);

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// using either sync or async I/O. Type handlers typically don't need to override this -
        /// override <see cref="Read(NpgsqlReadBuffer, int, bool, FieldDescription)"/> - but may do
        /// so in exceptional cases where reading of arbitrary types is required.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        protected internal override ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            var asTypedHandler = this as INpgsqlTypeHandler<TAny>;
            if (asTypedHandler == null)
                throw new InvalidCastException(fieldDescription == null
                    ? $"Can't cast database type to {typeof(TAny).Name}"
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(TAny).Name}"
                );

            return asTypedHandler.Read(buf, len, async, fieldDescription);
        }

        /// <inheritdoc />
        public override TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => Read<TAny>(buf, len, false, fieldDescription).Result;

        // Since TAny isn't constrained to class? or struct (C# doesn't have a non-nullable constraint that doesn't limit us to either struct or class),
        // we must use the bang operator here to tell the compiler that a null value will never returned.
        internal override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => (await Read<TDefault>(buf, len, async, fieldDescription))!;

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => Read<TDefault>(buf, len, fieldDescription)!;

        #endregion Read

        #region Write

        /// <summary>
        /// Called to validate and get the length of a value of a generic <see cref="NpgsqlParameter{T}"/>.
        /// </summary>
        public abstract int ValidateAndGetLength(TDefault value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter);

        /// <summary>
        /// Called to write the value of a generic <see cref="NpgsqlParameter{T}"/>.
        /// </summary>
        public abstract Task Write(TDefault value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default);

        /// <summary>
        /// Called to validate and get the length of a value of an arbitrary type.
        /// Checks that the current handler supports that type and throws an exception otherwise.
        /// </summary>
        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            var typedHandler = this as INpgsqlTypeHandler<TAny>;
            if (typedHandler is null)
                ThrowHelper.ThrowInvalidCastException_NotSupportedType(this, parameter, typeof(TAny));

            return typedHandler.ValidateAndGetLength(value, ref lengthCache, parameter);
        }

        /// <summary>
        /// In the vast majority of cases writing a parameter to the buffer won't need to perform I/O.
        /// </summary>
        public override Task WriteWithLengthInternal<TAny>([AllowNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong();

            if (value == null || typeof(TAny) == typeof(DBNull))
            {
                buf.WriteInt32(-1);
                return Task.CompletedTask;
            }

            return WriteWithLength(value, buf, lengthCache, parameter, async, cancellationToken);

            async Task WriteWithLengthLong()
            {
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);

                if (value == null || typeof(TAny) == typeof(DBNull))
                {
                    buf.WriteInt32(-1);
                    return;
                }

                await WriteWithLength(value, buf, lengthCache, parameter, async, cancellationToken);
            }
        }

        /// <summary>
        /// Typically does not need to be overridden by type handlers, but may be needed in some
        /// cases (e.g. <see cref="ArrayHandler"/>.
        /// Note that this method assumes it can write 4 bytes of length (already verified by
        /// <see cref="WriteWithLengthInternal{TAny}"/>).
        /// </summary>
        protected virtual Task WriteWithLength<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            Debug.Assert(this is INpgsqlTypeHandler<TAny>);

            var typedHandler = (INpgsqlTypeHandler<TAny>)this;
            buf.WriteInt32(typedHandler.ValidateAndGetLength(value, ref lengthCache, parameter));
            return typedHandler.Write(value, buf, lengthCache, parameter, async, cancellationToken);
        }

        #endregion Write

        #region Misc

        internal override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(TDefault);
        internal override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(TDefault);

        /// <inheritdoc />
        public override ArrayHandler CreateArrayHandler(PostgresArrayType arrayBackendType, ArrayNullabilityMode arrayNullabilityMode)
            => new ArrayHandler<TDefault>(arrayBackendType, this, arrayNullabilityMode);

        /// <inheritdoc />
        public override IRangeHandler CreateRangeHandler(PostgresType rangeBackendType)
            => new RangeHandler<TDefault>(rangeBackendType, this);

        #endregion Misc
    }
}
