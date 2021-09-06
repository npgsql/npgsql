using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandlers;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandling
{
    /// <summary>
    /// Base class for all type handlers, which read and write CLR types into their PostgreSQL
    /// binary representation.
    /// Type handler writers shouldn't inherit from this class, inherit <see cref="NpgsqlTypeHandler"/>
    /// or <see cref="NpgsqlSimpleTypeHandler{T}"/> instead.
    /// </summary>
    public abstract class NpgsqlTypeHandler
    {
        protected NpgsqlTypeHandler(PostgresType postgresType)
            => PostgresType = postgresType;

        /// <summary>
        /// The PostgreSQL type handled by this type handler.
        /// </summary>
        public PostgresType PostgresType { get; }

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal async ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            switch (this)
            {
            case INpgsqlSimpleTypeHandler<TAny> simpleTypeHandler:
                await buf.Ensure(len, async);
                return simpleTypeHandler.Read(buf, len, fieldDescription);
            case INpgsqlTypeHandler<TAny> typeHandler:
                return await typeHandler.Read(buf, len, async, fieldDescription);
            default:
                return await ReadCustom<TAny>(buf, len, async, fieldDescription);
            }
        }

        /// <summary>
        /// Version of <see cref="Read{TAny}(NpgsqlReadBuffer,int,bool,FieldDescription?)"/> that's called when we know the entire value
        /// is already buffered in memory (i.e. in non-sequential mode).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            Debug.Assert(buf.ReadBytesLeft >= len);

            return this switch
            {
                INpgsqlSimpleTypeHandler<TAny> simpleTypeHandler => simpleTypeHandler.Read(buf, len, fieldDescription),
                INpgsqlTypeHandler<TAny> typeHandler => typeHandler.Read(buf, len, async: false, fieldDescription).Result,
                _ => ReadCustom<TAny>(buf, len, async: false, fieldDescription).Result
            };
        }

        protected internal virtual ValueTask<TAny> ReadCustom<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => throw new InvalidCastException(fieldDescription == null
                ? $"Can't cast database type to {typeof(TAny).Name}"
                : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(TAny).Name}");

        /// <summary>
        /// Reads a column as the type handler's default read type. If it is not already entirely in
        /// memory, sync or async I/O will be performed as specified by <paramref name="async"/>.
        /// </summary>
        public abstract ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null);

        /// <summary>
        /// Version of <see cref="ReadAsObject(NpgsqlReadBuffer,int,bool,FieldDescription?)"/> that's called when we know the entire value
        /// is already buffered in memory (i.e. in non-sequential mode).
        /// </summary>
        internal object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            Debug.Assert(buf.ReadBytesLeft >= len);

            return ReadAsObject(buf, len, async: false, fieldDescription).Result;
        }

        /// <summary>
        /// Reads a column as the type handler's provider-specific type. If it is not already entirely in
        /// memory, sync or async I/O will be performed as specified by <paramref name="async"/>.
        /// </summary>
        internal virtual ValueTask<object> ReadPsvAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => ReadAsObject(buf, len, async, fieldDescription);

        /// <summary>
        /// Version of <see cref="ReadPsvAsObject(NpgsqlReadBuffer,int,bool,FieldDescription?)"/> that's called when we know the entire value
        /// is already buffered in memory (i.e. in non-sequential mode).
        /// </summary>
        internal virtual object ReadPsvAsObject(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            Debug.Assert(buf.ReadBytesLeft >= len);

            return ReadPsvAsObject(buf, len, async: false, fieldDescription).Result;
        }

        /// <summary>
        /// Reads a value from the buffer, assuming our read position is at the value's preceding length.
        /// If the length is -1 (null), this method will return the default value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <para>Called to validate and get the length of a value of a generic <see cref="NpgsqlParameter{T}"/>.</para>
        /// <para><see langword="null"/> and <see cref="DBNull"/> must be handled before calling into this.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal int ValidateAndGetLength<TAny>(
            [DisallowNull] TAny value, [NotNullIfNotNull("lengthCache")] ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            Debug.Assert(value is not DBNull);

            return this switch
            {
                INpgsqlSimpleTypeHandler<TAny> simpleTypeHandler => simpleTypeHandler.ValidateAndGetLength(value, parameter),
                INpgsqlTypeHandler<TAny> typeHandler => typeHandler.ValidateAndGetLength(value, ref lengthCache, parameter),
                _ => ValidateAndGetLengthCustom<TAny>(value, ref lengthCache, parameter)
            };
        }

        protected internal virtual int ValidateAndGetLengthCustom<TAny>(
            [DisallowNull] TAny value, [NotNullIfNotNull("lengthCache")] ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            var parameterName = parameter is null
                ? null
                : parameter.TrimmedName == string.Empty
                    ? $"${parameter.Collection!.IndexOf(parameter) + 1}"
                    : parameter.TrimmedName;

            throw new InvalidCastException(parameterName is null
                ? $"Cannot write a value of CLR type '{typeof(TAny)}' as database type '{PgDisplayName}'."
                : $"Cannot write a value of CLR type '{typeof(TAny)}' as database type '{PgDisplayName}' for parameter '{parameterName}'.");
        }

        /// <summary>
        /// Called to write the value of a generic <see cref="NpgsqlParameter{T}"/>.
        /// </summary>
        /// <summary>
        /// In the vast majority of cases writing a parameter to the buffer won't need to perform I/O.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task WriteWithLength<TAny>(TAny? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            // TODO: Possibly do a sync path when we don't do I/O (e.g. simple type handler, no flush)
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);

            if (value is null or DBNull)
            {
                buf.WriteInt32(-1);
                return;
            }

            switch (this)
            {
            case INpgsqlSimpleTypeHandler<TAny> simpleTypeHandler:
                var len = simpleTypeHandler.ValidateAndGetLength(value, parameter);
                buf.WriteInt32(len);
                if (buf.WriteSpaceLeft < len)
                    await buf.Flush(async, cancellationToken);
                simpleTypeHandler.Write(value, buf, parameter);
                return;
            case INpgsqlTypeHandler<TAny> typeHandler:
                buf.WriteInt32(typeHandler.ValidateAndGetLength(value, ref lengthCache, parameter));
                await typeHandler.Write(value, buf, lengthCache, parameter, async, cancellationToken);
                return;
            default:
                await WriteWithLengthCustom<TAny>(value, buf, lengthCache, parameter, async, cancellationToken);
                return;
            }
        }

        /// <summary>
        /// Typically does not need to be overridden by type handlers, but may be needed in some
        /// cases (e.g. <see cref="ArrayHandler"/>.
        /// Note that this method assumes it can write 4 bytes of length (already verified by
        /// <see cref="WriteWithLength{TAny}"/>).
        /// </summary>
        protected virtual Task WriteWithLengthCustom<TAny>(
            [DisallowNull] TAny value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async,
            CancellationToken cancellationToken)
            => throw new InvalidCastException($"Can't write '{typeof(TAny).Name}' with type handler '{GetType().Name}'");

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
        // Source-generated
        public abstract int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter);

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
        /// <param name="cancellationToken">
        /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        // Source-generated
        public abstract Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default);

        #endregion Write

        #region Misc

        public abstract Type GetFieldType(FieldDescription? fieldDescription = null);
        public abstract Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null);

        internal virtual bool PreferTextWrite => false;

        /// <summary>
        /// Creates a type handler for arrays of this handler's type.
        /// </summary>
        public abstract NpgsqlTypeHandler CreateArrayHandler(PostgresArrayType pgArrayType, ArrayNullabilityMode arrayNullabilityMode);

        /// <summary>
        /// Creates a type handler for ranges of this handler's type.
        /// </summary>
        public abstract NpgsqlTypeHandler CreateRangeHandler(PostgresType pgRangeType);

        /// <summary>
        /// Creates a type handler for multiranges of this handler's type.
        /// </summary>
        public abstract NpgsqlTypeHandler CreateMultirangeHandler(PostgresMultirangeType pgMultirangeType);

        /// <summary>
        /// Used to create an exception when the provided type can be converted and written, but an
        /// instance of <see cref="NpgsqlParameter"/> is required for caching of the converted value
        /// (in <see cref="NpgsqlParameter.ConvertedValue"/>.
        /// </summary>
        protected Exception CreateConversionButNoParamException(Type clrType)
            => new InvalidCastException($"Can't convert .NET type '{clrType}' to PostgreSQL '{PgDisplayName}' within an array");

        internal string PgDisplayName => PostgresType.DisplayName;

        #endregion Misc
    }
}
