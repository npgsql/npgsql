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
using System.IO;
using Npgsql.BackendMessages;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;

namespace Npgsql
{
    // ReSharper disable once UnusedTypeParameter
#pragma warning disable CA1040
    interface ITypeHandler<T> { }
#pragma warning restore CA1040

    public abstract class TypeHandler
    {
        internal PostgresType PostgresType { get; set; }

        internal abstract Type GetFieldType(FieldDescription fieldDescription = null);
        internal abstract Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null);

        protected internal abstract int ValidateAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter = null);

        internal abstract Task WriteWithLength([CanBeNull] object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache,
            NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken);

        internal virtual bool PreferTextWrite => false;

        /// <summary>
        /// Reads a value from the buffer, assuming our read position is at the value's preceding length.
        /// If the length is -1 (null), this method will return the default value.
        /// </summary>
        [ItemCanBeNull]
        internal async ValueTask<T> ReadWithLength<T>(NpgsqlReadBuffer buf, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var len = buf.ReadInt32();
            if (len == -1)
                return default(T);
            return await Read<T>(buf, len, async, fieldDescription);
        }

        /// <summary>
        /// Reads a column, assuming that it is already entirely in memory (i.e. no I/O is necessary).
        /// Called by <see cref="NpgsqlDefaultDataReader"/>, which buffers entire rows in memory.
        /// </summary>
        protected internal abstract T Read<T>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null);

        /// <summary>
        /// Reads a column. If it is not already entirely in memory, sync or async I/O will be performed
        /// as specified by <paramref name="async"/>.
        /// </summary>
        protected internal abstract ValueTask<T> Read<T>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);

        /// <summary>
        /// Reads a column as the type handler's default read type, assuming that it is already entirely
        /// in memory (i.e. no I/O is necessary). Called by <see cref="NpgsqlDefaultDataReader"/>, which
        /// buffers entire rows in memory.
        /// </summary>
        internal abstract object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null);

        /// <summary>
        /// Reads a column as the type handler's default read type. If it is not already entirely in
        /// memory, sync or async I/O will be performed as specified by <paramref name="async"/>.
        /// </summary>
        internal abstract ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);

        /// <summary>
        /// Reads a column as the type handler's provider-specific type, assuming that it is already entirely
        /// in memory (i.e. no I/O is necessary). Called by <see cref="NpgsqlDefaultDataReader"/>, which
        /// buffers entire rows in memory.
        /// </summary>
        internal virtual object ReadPsvAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => ReadAsObject(buf, len, fieldDescription);

        /// <summary>
        /// Reads a column as the type handler's provider-specific type. If it is not already entirely in
        /// memory, sync or async I/O will be performed as specified by <paramref name="async"/>.
        /// </summary>
        internal virtual ValueTask<object> ReadPsvAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => ReadAsObject(buf, len, async, fieldDescription);

        /// <summary>
        /// Creates a type handler for arrays of this handler's type.
        /// </summary>
        internal abstract ArrayHandler CreateArrayHandler(PostgresType arrayBackendType);

        /// <summary>
        /// Creates a type handler for ranges of this handler's type.
        /// </summary>
        internal abstract TypeHandler CreateRangeHandler(PostgresType rangeBackendType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        protected Exception CreateConversionException(Type clrType)
            => new InvalidCastException($"Can't convert .NET type {clrType} to PostgreSQL {PgDisplayName}");

        /// <summary>
        ///
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        protected Exception CreateConversionButNoParamException(Type clrType)
            => new InvalidCastException($"Can't convert .NET type '{clrType}' to PostgreSQL '{PgDisplayName}' within an array");

        internal string PgDisplayName => PostgresType.DisplayName;
    }

    public abstract class TypeHandler<T> : TypeHandler
    {
        internal override Type GetFieldType(FieldDescription fieldDescription = null) => typeof(T);
        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null) => typeof(T);

        internal override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => await Read<T>(buf, len, async, fieldDescription);

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read<T>(buf, len, fieldDescription);

        internal override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => new ArrayHandler<T>(this) { PostgresType = arrayBackendType };

        internal override TypeHandler CreateRangeHandler(PostgresType rangeBackendType)
            => new RangeHandler<T>(this) { PostgresType = rangeBackendType };
    }

    public abstract class SimpleTypeHandler<T> : TypeHandler<T>, ISimpleTypeHandler<T>
    {
        public abstract T Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null);

        internal sealed override async Task WriteWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            if (value == null || value is DBNull)
            {
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);
                buf.WriteInt32(-1);
                return;
            }

            var elementLen = ValidateAndGetLength(value, parameter);
            if (buf.WriteSpaceLeft < 4 + elementLen)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(elementLen);
            Write(value, buf, parameter);
        }

        protected internal sealed override int ValidateAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter = null)
            => ValidateAndGetLength(value, parameter);

        protected abstract int ValidateAndGetLength(object value, NpgsqlParameter parameter = null);
        protected abstract void Write(object value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null);

        protected internal override T2 Read<T2>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Debug.Assert(len <= buf.ReadBytesLeft);

            var asTypedHandler = this as ISimpleTypeHandler<T2>;
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

        protected internal override async ValueTask<T2> Read<T2>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(len, async);
            return Read<T2>(buf, len, fieldDescription);
        }
    }

    /// <summary>
    /// Type handlers that wish to support reading other types in additional to the main one can
    /// implement this interface for all those types.
    /// </summary>
    interface ISimpleTypeHandler<T> : ITypeHandler<T>
    {
        T Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null);
    }

    /// <summary>
    /// A type handler that supports a provider-specific value which is different from the regular value (e.g.
    /// NpgsqlDate and DateTime)
    /// </summary>
    /// <typeparam name="T">the regular value type returned by this type handler</typeparam>
    /// <typeparam name="TPsv">the type of the provider-specific value returned by this type handler</typeparam>
    public abstract class SimpleTypeHandlerWithPsv<T, TPsv> : SimpleTypeHandler<T>, ISimpleTypeHandler<TPsv>
    {
        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null)
            => typeof(TPsv);

        /// <summary>
        /// Reads a column as the type handler's provider-specific type, assuming that it is already entirely
        /// in memory (i.e. no I/O is necessary). Called by <see cref="NpgsqlDefaultDataReader"/>, which
        /// buffers entire rows in memory.
        /// </summary>
        internal override object ReadPsvAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read<TPsv>(buf, len, fieldDescription);

        /// <summary>
        /// Reads a column as the type handler's provider-specific type. If it is not already entirely in
        /// memory, sync or async I/O will be performed as specified by <paramref name="async"/>.
        /// </summary>
        internal override async ValueTask<object> ReadPsvAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => await Read<TPsv>(buf, len, async, fieldDescription);

        protected abstract TPsv ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null);

        TPsv ISimpleTypeHandler<TPsv>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => ReadPsv(buf, len, fieldDescription);

        internal override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => new ArrayHandlerWithPsv<T, TPsv>(this) { PostgresType = arrayBackendType };
    }

    public abstract class ChunkingTypeHandler<T> : TypeHandler<T>, IChunkingTypeHandler<T>
    {
        public abstract ValueTask<T> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);

        /// <remarks>
        /// A type handler may implement IChunkingTypeHandler for types other than its primary one.
        /// This is why this method has type parameter T2 and not T.
        /// </remarks>
        protected internal override ValueTask<T2> Read<T2>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            var asTypedHandler = this as IChunkingTypeHandler<T2>;
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

        /// <summary>
        /// Reads a column, assuming that it is already entirely in memory (i.e. no I/O is necessary).
        /// Called by <see cref="NpgsqlDefaultDataReader"/>, which buffers entire rows in memory.
        /// </summary>
        protected internal override T2 Read<T2>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read<T2>(buf, len, true, fieldDescription).Result;

        internal sealed override async Task WriteWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);

            if (value == null || value is DBNull)
            {
                buf.WriteInt32(-1);
                return;
            }

            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
            await Write(value, buf, lengthCache, parameter, async, cancellationToken);
        }

        protected abstract Task Write(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Type handlers that wish to support reading other types in additional to the main one can
    /// implement this interface for all those types.
    /// </summary>
    interface IChunkingTypeHandler<T> : ITypeHandler<T>
    {
        ValueTask<T> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);
    }

    /// <summary>
    /// Implemented by handlers which support <see cref="DbDataReader.GetTextReader"/>, returns a standard
    /// TextReader given a binary Stream.
    /// </summary>
    interface ITextReaderHandler
    {
        TextReader GetTextReader(Stream stream);
    }

#pragma warning disable CA1032

    /// <summary>
    /// Can be thrown by readers to indicate that interpreting the value failed, but the value was read wholly
    /// and it is safe to continue reading. Any other exception is assumed to leave the row in an unknown state
    /// and the connector is therefore set to Broken.
    /// Note that an inner exception is mandatory, and will get thrown to the user instead of the NpgsqlSafeReadException.
    /// </summary>
    public class NpgsqlSafeReadException : Exception
    {
        public NpgsqlSafeReadException(Exception innerException) : base("", innerException)
        {
            Debug.Assert(innerException != null);
        }

        public NpgsqlSafeReadException(string message) : this(new NpgsqlException(message)) {}
    }
}
