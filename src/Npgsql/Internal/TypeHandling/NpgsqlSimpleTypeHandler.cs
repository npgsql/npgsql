using System;
using System.Data.Common;
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
        protected NpgsqlSimpleTypeHandler(PostgresType postgresType) : base(postgresType) {}

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

        public sealed override ValueTask<TDefault> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => throw new NotSupportedException();

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
        /// Simple type handlers override <see cref="Write(TDefault,NpgsqlWriteBuffer,NpgsqlParameter)"/> instead of this.
        /// </summary>
        public sealed override Task Write(TDefault value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        /// <summary>
        /// Simple type handlers override <see cref="ValidateAndGetLength(TDefault,NpgsqlParameter)"/> instead of this.
        /// </summary>
        public sealed override int ValidateAndGetLength(TDefault value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => throw new NotSupportedException();

        #endregion
    }
}
