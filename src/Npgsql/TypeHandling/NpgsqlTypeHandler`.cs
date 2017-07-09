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
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Base class for all type handlers, which read and write CLR types into their PostgreSQL
    /// binary representation. Unless your type is arbitrary-length, consider inheriting from
    /// <see cref="NpgsqlSimpleTypeHandler{T}"/> instead.
    /// </summary>
    /// <typeparam name="T">
    /// The default CLR type that this handler will read and write. For example, calling <see cref="DbDataReader.GetValue"/>
    /// on a column with this handler will return a value with type <typeparamref name="T"/>.
    /// Type handlers can support additional types by implementing <see cref="INpgsqlTypeHandler{T}"/>.
    /// </typeparam>
    public abstract class NpgsqlTypeHandler<T> : NpgsqlTypeHandler, INpgsqlTypeHandler<T>
    {
        #region Read

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> with the given length from the provided buffer,
        /// using either sync or async I/O.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public abstract ValueTask<T> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> with the given length from the provided buffer,
        /// using either sync or async I/O. Type handlers typically don't need to override this -
        /// override <see cref="Read(NpgsqlReadBuffer, int, bool, FieldDescription)"/> - but may do
        /// so in exceptional cases where reading of arbitrary types is required.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        protected internal override ValueTask<T2> Read<T2>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            var asTypedHandler = this as INpgsqlTypeHandler<T2>;
            if (asTypedHandler == null)
            {
                buf.Skip(len);  // Perform this in sync for performance
                throw new NpgsqlSafeReadException(new InvalidCastException(fieldDescription == null
                    ? $"Can't cast database type to {typeof(T2).Name}"
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(T2).Name}"
                ));
            }

            return asTypedHandler.Read(buf, len, async, fieldDescription);
        }

        internal override T2 Read<T2>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read<T2>(buf, len, false, fieldDescription).Result;

        internal override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => await Read<T>(buf, len, async, fieldDescription);

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read<T>(buf, len, fieldDescription);

        #endregion Read

        #region Write

        internal override Task WriteWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong(value, buf, lengthCache, parameter, async);

            if (value == null || value is DBNull)
            {
                buf.WriteInt32(-1);
                return PGUtil.CompletedTask;
            }

            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
            return Write(value, buf, lengthCache, parameter, async);
        }

        async Task WriteWithLengthLong(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async);

            if (value == null || value is DBNull)
            {
                buf.WriteInt32(-1);
                return;
            }

            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
            await Write(value, buf, lengthCache, parameter, async);
        }

        #endregion Write

        #region Misc

        internal override Type GetFieldType(FieldDescription fieldDescription = null) => typeof(T);
        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null) => typeof(T);

        internal override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => new ArrayHandler<T>(this) { PostgresType = arrayBackendType };

        internal override NpgsqlTypeHandler CreateRangeHandler(PostgresType rangeBackendType)
            => new RangeHandler<T>(this) { PostgresType = rangeBackendType };

        #endregion Misc
    }
}
