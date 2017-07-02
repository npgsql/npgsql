#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Npgsql.BackendMessages;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Base class for all simple type handlers, which read and write short, non-arbitrary lengthed
    /// values to PostgreSQL. Provides a simpler API to implement when compared to <see cref="NpgsqlTypeHandler"/> -
    /// Npgsql takes care of all I/O before calling into this type, so no I/O needs to be performed by it.
    /// </summary>
    /// <typeparam name="T">
    /// The default CLR type that this handler will read and write. For example, calling <see cref="DbDataReader.GetValue"/>
    /// on a column with this handler will return a value with type <typeparamref name="T"/>.
    /// Type handlers can support additional types by implementing <see cref="INpgsqlTypeHandler{T}"/>.
    /// </typeparam>
    public abstract class NpgsqlSimpleTypeHandler<T> : NpgsqlTypeHandler<T>, INpgsqlSimpleTypeHandler<T>
    {
        #region Read

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> with the given length from the provided buffer,
        /// with the assumption that it is entirely present in the provided memory buffer and no I/O will be
        /// required. 
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public abstract T Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null);

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> with the given length from the provided buffer,
        /// using either sync or async I/O. This method is sealed for <see cref="NpgsqlSimpleTypeHandler{T}"/>,
        /// override <see cref="Read(Npgsql.NpgsqlReadBuffer,int,Npgsql.BackendMessages.FieldDescription)"/>.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public sealed override ValueTask<T> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => Read<T>(buf, len, async, fieldDescription);

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> with the given length from the provided buffer,
        /// using either sync or async I/O. This method is sealed for <see cref="NpgsqlSimpleTypeHandler{T}"/>.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        protected internal sealed override async ValueTask<T2> Read<T2>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(len, async);
            return Read<T2>(buf, len, fieldDescription);
        }

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> with the given length from the provided buffer.
        /// with the assumption that it is entirely present in the provided memory buffer and no I/O will be
        /// required. Type handlers typically don't need to override this - override
        /// <see cref="Read(Npgsql.NpgsqlReadBuffer,int,Npgsql.BackendMessages.FieldDescription)"/> - but may do
        /// so in exceptional cases where reading of arbitrary types is required.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        internal override T2 Read<T2>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Debug.Assert(len <= buf.ReadBytesLeft);

            var asTypedHandler = this as INpgsqlSimpleTypeHandler<T2>;
            if (asTypedHandler == null)
            {
                buf.Skip(len);  // Perform this in sync for performance
                throw new NpgsqlSafeReadException(new InvalidCastException(fieldDescription == null
                    ? $"Can't cast database type to {typeof(T2).Name}"
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(T2).Name}"
                ));
            }

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
        protected abstract int ValidateAndGetLength(object value, NpgsqlParameter parameter = null);

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
        protected abstract void Write(object value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null);

        internal sealed override async Task WriteWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (value == null || value is DBNull)
            {
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);
                buf.WriteInt32(-1);
                return;
            }

            var elementLen = ValidateAndGetLength(value, parameter);
            if (buf.WriteSpaceLeft < 4 + elementLen)
                await buf.Flush(async);
            buf.WriteInt32(elementLen);
            Write(value, buf, parameter);
        }

        /// <summary>
        /// This method is sealed, override <see cref="ValidateAndGetLength(object,Npgsql.NpgsqlParameter)"/>.
        /// </summary>
        protected internal sealed override int ValidateAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter = null)
            => ValidateAndGetLength(value, parameter);

        /// <summary>
        /// This method is sealed, override <see cref="Write(object,Npgsql.NpgsqlWriteBuffer,Npgsql.NpgsqlParameter)"/>.
        /// </summary>
        protected sealed override Task Write(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => throw new NotSupportedException(); // SimpleTypeHandlers implement the overload without async

        #endregion
    }
}
