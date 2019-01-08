using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Type handlers that wish to support reading other types in additional to the main one can
    /// implement this interface for all those types.
    /// </summary>
    public interface INpgsqlTypeHandler<T>
    {
        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> with the given length from the provided buffer,
        /// using either sync or async I/O.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        ValueTask<T> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription);

        /// <summary>
        /// Responsible for validating that a value represents a value of the correct and which can be
        /// written for PostgreSQL - if the value cannot be written for any reason, an exception shold be thrown.
        /// Also returns the byte length needed to write the value.
        /// </summary>
        /// <param name="value">The value to be written to PostgreSQL</param>
        /// <param name="lengthCache">A cache where the length calculated during the validation phase can be stored for use at the writing phase.</param>
        /// <param name="parameter">
        /// The <see cref="NpgsqlParameter"/> instance where this value resides. Can be used to access additional
        /// information relevant to the write process (e.g. <see cref="NpgsqlParameter.Size"/>).
        /// </param>
        /// <returns>The number of bytes required to write the value.</returns>
        int ValidateAndGetLength(T value, ref NpgsqlLengthCache lengthCache, [CanBeNull] NpgsqlParameter parameter);

        /// <summary>
        /// Writes a value to the provided buffer.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="buf">The buffer to which to write.</param>
        /// <param name="lengthCache">A cache where the length calculated during the validation phase can be stored for use at the writing phase.</param>
        /// <param name="parameter">
        /// The <see cref="NpgsqlParameter"/> instance where this value resides. Can be used to access additional
        /// information relevant to the write process (e.g. <see cref="NpgsqlParameter.Size"/>).
        /// </param>
        /// <param name="async">
        /// If I/O will be necessary (i.e. the buffer is full), determines whether it will be done synchronously or asynchronously.
        /// </param>
        Task Write(T value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, [CanBeNull] NpgsqlParameter parameter, bool async);
    }
}
