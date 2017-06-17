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

    abstract class TypeHandler
    {
        internal PostgresType PostgresType { get; }

        internal TypeHandler(PostgresType postgresType)
        {
            PostgresType = postgresType;
        }

        internal abstract Type GetFieldType(FieldDescription fieldDescription = null);
        internal abstract Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null);

        internal abstract object ReadAsObject(DataRowMessage row, FieldDescription fieldDescription = null);

        internal virtual object ReadPsvAsObject(DataRowMessage row, FieldDescription fieldDescription)
            => ReadAsObject(row, fieldDescription);

        public abstract int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null);

        internal abstract Task WriteWithLength([CanBeNull] object value, WriteBuffer buf, LengthCache lengthCache,
            NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken);

        internal virtual bool PreferTextWrite => false;

        internal async ValueTask<T> Read<T>(DataRowMessage row, int len, bool async, FieldDescription fieldDescription = null)
        {
            Debug.Assert(row.PosInColumn == 0);

            T result;
            try
            {
                result = await Read<T>(row.Buffer, len, async, fieldDescription);
            }
            finally
            {
                // Important in case a SafeReadException was thrown, position must still be updated
                row.PosInColumn += row.ColumnLen;
            }
            return result;
        }

        /// <summary>
        /// Reads a value from the buffer, assuming our read position is at the value's preceding length.
        /// If the length is -1 (null), this method will return the default value.
        /// </summary>
        [ItemCanBeNull]
        internal async ValueTask<T> ReadWithLength<T>(ReadBuffer buf, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var len = buf.ReadInt32();
            if (len == -1)
                return default(T);
            return await Read<T>(buf, len, async, fieldDescription);
        }

        internal abstract ValueTask<T> Read<T>(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);

        internal abstract ValueTask<object> ReadAsObject(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);

        internal virtual ValueTask<object> ReadPsvAsObject(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
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

    abstract class TypeHandler<T> : TypeHandler
    {
        internal TypeHandler(PostgresType postgresType) : base(postgresType) {}

        internal override Type GetFieldType(FieldDescription fieldDescription = null) => typeof(T);
        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null) => typeof(T);

        internal override object ReadAsObject(DataRowMessage row, FieldDescription fieldDescription = null)
            => Read<T>(row, row.ColumnLen, false, fieldDescription).Result;

        internal override async ValueTask<object> ReadAsObject(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => await Read<T>(buf, len, async, fieldDescription);

        internal override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => new ArrayHandler<T>(arrayBackendType, this);

        internal override TypeHandler CreateRangeHandler(PostgresType rangeBackendType)
            => new RangeHandler<T>(rangeBackendType, this);
    }

    abstract class SimpleTypeHandler<T> : TypeHandler<T>, ISimpleTypeHandler<T>
    {
        internal SimpleTypeHandler(PostgresType postgresType) : base(postgresType) { }
        public abstract T Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null);

        internal sealed override async Task WriteWithLength(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
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

        public sealed override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
            => ValidateAndGetLength(value, parameter);

        public abstract int ValidateAndGetLength(object value, NpgsqlParameter parameter = null);
        protected abstract void Write(object value, WriteBuffer buf, NpgsqlParameter parameter = null);

        /// <remarks>
        /// A type handler may implement ISimpleTypeHandler for types other than its primary one.
        /// This is why this method has type parameter T2 and not T.
        /// </remarks>
        internal override async ValueTask<T2> Read<T2>(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(len, async);

            var asTypedHandler = this as ISimpleTypeHandler<T2>;
            if (asTypedHandler == null)
            {
                buf.Skip(len);  // Perform this in sync for performance
                throw new SafeReadException(new InvalidCastException(fieldDescription == null
                    ? "Can't cast database type to " + typeof(T2).Name
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(T2).Name}"
                ));
            }

            return asTypedHandler.Read(buf, len, fieldDescription);
        }
    }

    /// <summary>
    /// Type handlers that wish to support reading other types in additional to the main one can
    /// implement this interface for all those types.
    /// </summary>
    interface ISimpleTypeHandler<T> : ITypeHandler<T>
    {
        T Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null);
    }

    /// <summary>
    /// A type handler that supports a provider-specific value which is different from the regular value (e.g.
    /// NpgsqlDate and DateTime)
    /// </summary>
    /// <typeparam name="T">the regular value type returned by this type handler</typeparam>
    /// <typeparam name="TPsv">the type of the provider-specific value returned by this type handler</typeparam>
    abstract class SimpleTypeHandlerWithPsv<T, TPsv> : SimpleTypeHandler<T>, ISimpleTypeHandler<TPsv>
    {
        internal SimpleTypeHandlerWithPsv(PostgresType postgresType) : base(postgresType) { }

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null)
            => typeof(TPsv);

        internal override object ReadPsvAsObject(DataRowMessage row, FieldDescription fieldDescription)
            => Read<TPsv>(row, row.ColumnLen, false, fieldDescription).Result;

        internal override async ValueTask<object> ReadPsvAsObject(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => await Read<TPsv>(buf, len, async, fieldDescription);

        internal abstract TPsv ReadPsv(ReadBuffer buf, int len, FieldDescription fieldDescription = null);

        TPsv ISimpleTypeHandler<TPsv>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => ReadPsv(buf, len, fieldDescription);

        internal override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => new ArrayHandlerWithPsv<T, TPsv>(arrayBackendType, this);
    }

    abstract class ChunkingTypeHandler<T> : TypeHandler<T>, IChunkingTypeHandler<T>
    {
        internal ChunkingTypeHandler(PostgresType postgresType) : base(postgresType) { }

        public abstract ValueTask<T> Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);

        internal sealed override async Task WriteWithLength(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
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

        protected abstract Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken);

        /// <remarks>
        /// A type handler may implement IChunkingTypeHandler for types other than its primary one.
        /// This is why this method has type parameter T2 and not T.
        /// </remarks>
        internal override ValueTask<T2> Read<T2>(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            var asTypedHandler = this as IChunkingTypeHandler<T2>;
            if (asTypedHandler == null)
            {
                buf.Skip(len);  // Perform this in sync for performance
                throw new SafeReadException(new InvalidCastException(fieldDescription == null
                    ? "Can't cast database type to " + typeof(T2).Name
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(T2).Name}"
                ));
            }

            return asTypedHandler.Read(buf, len, async, fieldDescription);
        }
    }

    /// <summary>
    /// Type handlers that wish to support reading other types in additional to the main one can
    /// implement this interface for all those types.
    /// </summary>
    interface IChunkingTypeHandler<T> : ITypeHandler<T>
    {
        ValueTask<T> Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);
    }

    /// <summary>
    /// Implemented by handlers which support <see cref="NpgsqlDataReader.GetTextReader"/>, returns a standard
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
    /// Note that an inner exception is mandatory, and will get thrown to the user instead of the SafeReadException.
    /// </summary>
    class SafeReadException : Exception
    {
        public SafeReadException(Exception innerException) : base("", innerException)
        {
            Debug.Assert(innerException != null);
        }

        public SafeReadException(string message) : this(new NpgsqlException(message)) {}
    }
}
