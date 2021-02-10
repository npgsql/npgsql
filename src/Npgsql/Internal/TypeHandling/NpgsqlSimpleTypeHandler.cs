using System;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandling
{
    /// <summary>
    /// Base class for all simple type handlers, which read and write short, non-arbitrary lengthed
    /// values to PostgreSQL. Provides a simpler API to implement when compared to <see cref="NpgsqlTypeHandler"/> -
    /// Npgsql takes care of all I/O before calling into this type, so no I/O needs to be performed by it.
    /// </summary>
    /// <typeparam name="TDefault">
    /// The default CLR type that this handler will read and write. For example, calling <see cref="DbDataReader.GetValue"/>
    /// on a column with this handler will return a value with type <typeparamref name="TDefault"/>.
    /// Type handlers can support additional types by implementing <see cref="INpgsqlTypeHandler{T}"/>.
    /// </typeparam>
    public abstract class NpgsqlSimpleTypeHandler<TDefault> : NpgsqlTypeHandler<TDefault>, INpgsqlSimpleTypeHandler<TDefault>
    {
        /// <summary>
        /// Constructs an <see cref="NpgsqlSimpleTypeHandler{TDefault}"/>.
        /// </summary>
        protected NpgsqlSimpleTypeHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// with the assumption that it is entirely present in the provided memory buffer and no I/O will be
        /// required.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public abstract TDefault Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null);

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// using either sync or async I/O. This method is sealed for <see cref="NpgsqlSimpleTypeHandler{T}"/>,
        /// override <see cref="Read(NpgsqlReadBuffer,int,Npgsql.BackendMessages.FieldDescription)"/>.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public sealed override ValueTask<TDefault> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => Read<TDefault>(buf, len, async, fieldDescription);

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// using either sync or async I/O. This method is sealed for <see cref="NpgsqlSimpleTypeHandler{T}"/>.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        protected internal sealed override async ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(len, async);
            return Read<TAny>(buf, len, fieldDescription);
        }

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer.
        /// with the assumption that it is entirely present in the provided memory buffer and no I/O will be
        /// required. Type handlers typically don't need to override this - override
        /// <see cref="Read(NpgsqlReadBuffer,int,Npgsql.BackendMessages.FieldDescription)"/> - but may do
        /// so in exceptional cases where reading of arbitrary types is required.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public override TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            Debug.Assert(len <= buf.ReadBytesLeft);

            var asTypedHandler = this as INpgsqlSimpleTypeHandler<TAny>;
            if (asTypedHandler == null)
                throw new InvalidCastException(fieldDescription == null
                    ? $"Can't cast database type to {typeof(TAny).Name}"
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(TAny).Name}"
                );

            return asTypedHandler.Read(buf, len, fieldDescription);
        }

        #endregion Read

        #region Write

        /// <summary>
        /// Responsible for validating that a value represents a value of the correct and which can be
        /// written for PostgreSQL - if the value cannot be written for any reason, an exception shold be thrown.
        /// Also returns the byte length needed to write the value.
        /// </summary>
        /// <param name="value">The value to be written to PostgreSQL</param>
        /// <param name="parameter">
        /// The <see cref="NpgsqlParameter"/> instance where this value resides. Can be used to access additional
        /// information relevant to the write process (e.g. <see cref="NpgsqlParameter.Size"/>).
        /// </param>
        /// <returns>The number of bytes required to write the value.</returns>
        public abstract int ValidateAndGetLength(TDefault value, NpgsqlParameter? parameter);

        /// <summary>
        /// Writes a value to the provided buffer, with the assumption that there is enough space in the buffer
        /// (no I/O will occur). The Npgsql core will have taken care of that.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="buf">The buffer to which to write.</param>
        /// <param name="parameter">
        /// The <see cref="NpgsqlParameter"/> instance where this value resides. Can be used to access additional
        /// information relevant to the write process (e.g. <see cref="NpgsqlParameter.Size"/>).
        /// </param>
        public abstract void Write(TDefault value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter);

        /// <summary>
        /// This method is sealed, override <see cref="ValidateAndGetLength(TDefault,NpgsqlParameter)"/>.
        /// </summary>
        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => this is INpgsqlSimpleTypeHandler<TAny> typedHandler
                ? typedHandler.ValidateAndGetLength(value, parameter)
                : throw new InvalidCastException($"Can't write CLR type {typeof(TAny)} to database type {PgDisplayName}");

        /// <summary>
        /// In the vast majority of cases writing a parameter to the buffer won't need to perform I/O.
        /// </summary>
        public sealed override Task WriteWithLengthInternal<TAny>([AllowNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (value == null || typeof(TAny) == typeof(DBNull))
            {
                if (buf.WriteSpaceLeft < 4)
                    return WriteWithLengthLong();
                buf.WriteInt32(-1);
                return Task.CompletedTask;
            }

            Debug.Assert(this is INpgsqlSimpleTypeHandler<TAny>);
            var typedHandler = (INpgsqlSimpleTypeHandler<TAny>)this;

            var elementLen = typedHandler.ValidateAndGetLength(value, parameter);
            if (buf.WriteSpaceLeft < 4 + elementLen)
                return WriteWithLengthLong();
            buf.WriteInt32(elementLen);
            typedHandler.Write(value, buf, parameter);
            return Task.CompletedTask;

            async Task WriteWithLengthLong()
            {
                if (value == null || typeof(TAny) == typeof(DBNull))
                {
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async, cancellationToken);
                    buf.WriteInt32(-1);
                    return;
                }

                typedHandler = (INpgsqlSimpleTypeHandler<TAny>)this;
                elementLen = typedHandler.ValidateAndGetLength(value, parameter);
                if (buf.WriteSpaceLeft < 4 + elementLen)
                    await buf.Flush(async, cancellationToken);
                buf.WriteInt32(elementLen);
                typedHandler.Write(value, buf, parameter);
            }
        }

        /// <summary>
        /// Simple type handlers override <see cref="Write(TDefault,NpgsqlWriteBuffer,NpgsqlParameter)"/> instead of this.
        /// </summary>
        public sealed override Task Write(TDefault value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        /// <summary>
        /// Simple type handlers override <see cref="ValidateAndGetLength(TDefault,NpgsqlParameter)"/> instead of this.
        /// </summary>
        public sealed override int ValidateAndGetLength(TDefault value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => throw new NotSupportedException();

        // Object overloads for non-generic NpgsqlParameter

        /// <summary>
        /// Called to validate and get the length of a value of a non-generic <see cref="NpgsqlParameter"/>.
        /// Type handlers generally don't need to override this.
        /// </summary>
        public override int ValidateObjectAndGetLength(
            object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateObjectAndGetLength(value, parameter);

        // Implementation is source-generated
        protected abstract int ValidateObjectAndGetLength(object value, NpgsqlParameter? parameter);

        #endregion
    }
}
