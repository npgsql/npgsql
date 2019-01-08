using JetBrains.Annotations;
using Npgsql.BackendMessages;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Type handlers that wish to support reading other types in additional to the main one can
    /// implement this interface for all those types.
    /// </summary>
    public interface INpgsqlSimpleTypeHandler<T>
    {
        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> with the given length from the provided buffer,
        /// with the assumption that it is entirely present in the provided memory buffer and no I/O will be
        /// required. 
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        T Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null);

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
        int ValidateAndGetLength(T value, [CanBeNull] NpgsqlParameter parameter);

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
        void Write(T value, NpgsqlWriteBuffer buf, [CanBeNull] NpgsqlParameter parameter);
    }
}
