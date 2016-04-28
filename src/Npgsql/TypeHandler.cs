﻿#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using System.Diagnostics.Contracts;
using System.IO;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Threading;
using System.Threading.Tasks;
using AsyncRewriter;
using JetBrains.Annotations;
using Npgsql.TypeHandlers;

namespace Npgsql
{
    interface ITypeHandler<T> {}

    internal abstract partial class TypeHandler
    {
        internal IBackendType BackendType { get; }

        internal TypeHandler(IBackendType backendType)
        {
            BackendType = backendType;
        }

        internal abstract Type GetFieldType(FieldDescription fieldDescription = null);
        internal abstract Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null);

        internal abstract object ReadValueAsObjectFully(DataRowMessage row, FieldDescription fieldDescription);

        internal virtual object ReadPsvAsObjectFully(DataRowMessage row, FieldDescription fieldDescription)
        {
            return ReadValueAsObjectFully(row, fieldDescription);
        }

        internal virtual bool PreferTextWrite => false;

        [RewriteAsync]
        internal T ReadFully<T>(DataRowMessage row, int len, FieldDescription fieldDescription = null)
        {
            Contract.Requires(row.PosInColumn == 0);
            Contract.Ensures(row.PosInColumn == row.ColumnLen);

            T result;
            try
            {
                result = ReadFully<T>(row.Buffer, len, fieldDescription);
            }
            finally
            {
                // Important in case a SafeReadException was thrown, position must still be updated
                row.PosInColumn += row.ColumnLen;
            }
            return result;
        }

        internal abstract T ReadFully<T>(ReadBuffer buf, int len, FieldDescription fieldDescription = null);
        internal abstract Task<T> ReadFullyAsync<T>(ReadBuffer buf, int len, CancellationToken cancellationToken, FieldDescription fieldDescription = null);

        /// <summary>
        /// Creates a type handler for arrays of this handler's type.
        /// </summary>
        internal abstract ArrayHandler CreateArrayHandler(IBackendType arrayBackendType);

        /// <summary>
        /// Creates a type handler for ranges of this handler's type.
        /// </summary>
        internal abstract TypeHandler CreateRangeHandler(IBackendType rangeBackendType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        protected Exception CreateConversionException(Type clrType)
        {
            return new InvalidCastException($"Can't convert .NET type {clrType} to PostgreSQL {PgDisplayName}");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        protected Exception CreateConversionButNoParamException(Type clrType)
        {
            return new InvalidCastException($"Can't convert .NET type '{clrType}' to PostgreSQL '{PgDisplayName}' within an array");
        }

        internal string PgDisplayName => BackendType.DisplayName;

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(this is ISimpleTypeHandler ^ this is IChunkingTypeHandler);
        }
    }

    internal abstract class TypeHandler<T> : TypeHandler
    {
        internal TypeHandler(IBackendType backendType) : base(backendType) {}

        internal override Type GetFieldType(FieldDescription fieldDescription = null)
        {
            return typeof(T);
        }

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null)
        {
            return typeof(T);
        }

        internal override object ReadValueAsObjectFully(DataRowMessage row, FieldDescription fieldDescription)
        {
            return ReadFully<T>(row, row.ColumnLen, fieldDescription);
        }

        internal override ArrayHandler CreateArrayHandler(IBackendType arrayBackendType)
            => new ArrayHandler<T>(arrayBackendType, this);

        internal override TypeHandler CreateRangeHandler(IBackendType rangeBackendType)
            => new RangeHandler<T>(rangeBackendType, this);

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(this is ISimpleTypeHandler ^ this is IChunkingTypeHandler);
        }
    }

    internal interface ISimpleTypeHandler
    {
        int ValidateAndGetLength(object value, [CanBeNull] NpgsqlParameter parameter);
        void Write(object value, WriteBuffer buf, [CanBeNull] NpgsqlParameter parameter);
        object ReadAsObject(ReadBuffer buf, int len, FieldDescription fieldDescription = null);
    }

    internal abstract partial class SimpleTypeHandler<T> : TypeHandler<T>, ISimpleTypeHandler<T>
    {
        internal SimpleTypeHandler(IBackendType backendType) : base(backendType) { }
        public abstract T Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null);
        public abstract int ValidateAndGetLength(object value, NpgsqlParameter parameter);
        public abstract void Write(object value, WriteBuffer buf, NpgsqlParameter parameter);

        /// <remarks>
        /// A type handler may implement ISimpleTypeHandler for types other than its primary one.
        /// This is why this method has type parameter T2 and not T.
        /// </remarks>
        [RewriteAsync(true)]
        internal override T2 ReadFully<T2>(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            buf.Ensure(len);
            var asTypedHandler = this as ISimpleTypeHandler<T2>;
            if (asTypedHandler == null)
                throw new InvalidCastException(fieldDescription == null
                    ? "Can't cast database type to " + typeof(T2).Name
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(T2).Name}"
                );

            return asTypedHandler.Read(buf, len, fieldDescription);
        }

        public object ReadAsObject(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            return Read(buf, len, fieldDescription);
        }
    }

    /// <summary>
    /// Type handlers that wish to support reading other types in additional to the main one can
    /// implement this interface for all those types.
    /// </summary>
    interface ISimpleTypeHandler<T> : ISimpleTypeHandler, ITypeHandler<T>
    {
        T Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null);
    }

    /// <summary>
    /// A type handler that supports a provider-specific value which is different from the regular value (e.g.
    /// NpgsqlDate and DateTime)
    /// </summary>
    /// <typeparam name="T">the regular value type returned by this type handler</typeparam>
    /// <typeparam name="TPsv">the type of the provider-specific value returned by this type handler</typeparam>
    internal abstract class SimpleTypeHandlerWithPsv<T, TPsv> : SimpleTypeHandler<T>, ISimpleTypeHandler<TPsv>, ITypeHandlerWithPsv
    {
        internal SimpleTypeHandlerWithPsv(IBackendType backendType) : base(backendType) { }

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null)
        {
            return typeof(TPsv);
        }

        internal override object ReadPsvAsObjectFully(DataRowMessage row, FieldDescription fieldDescription)
        {
            return ReadFully<TPsv>(row, row.ColumnLen, fieldDescription);
        }

        internal abstract TPsv ReadPsv(ReadBuffer buf, int len, FieldDescription fieldDescription);

        TPsv ISimpleTypeHandler<TPsv>.Read(ReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            return ReadPsv(buf, len, fieldDescription);
        }

        internal override ArrayHandler CreateArrayHandler(IBackendType arrayBackendType)
        {
            return new ArrayHandlerWithPsv<T, TPsv>(arrayBackendType, this);
        }
    }

    /// <summary>
    /// A marking interface to allow us to know whether a given type handler has a provider-specific type
    /// distinct from its regular type
    /// </summary>
    internal interface ITypeHandlerWithPsv {}

    internal interface IChunkingTypeHandler
    {
        void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null);
        int ValidateAndGetLength(object value, ref LengthCache lengthCache, [CanBeNull] NpgsqlParameter parameter);
        void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, [CanBeNull] NpgsqlParameter parameter);
        bool Write(ref DirectBuffer directBuf);
        bool ReadAsObject(out object result);
    }

    [ContractClass(typeof(ChunkingTypeHandlerContracts<>))]
    internal abstract partial class ChunkingTypeHandler<T> : TypeHandler<T>, IChunkingTypeHandler<T>
    {
        internal ChunkingTypeHandler(IBackendType backendType) : base(backendType) { }

        public abstract void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null);
        public abstract bool Read(out T result);

        /// <param name="value">the value to be examined</param>
        /// <param name="lengthCache">a cache in which to store length(s) of values to be written</param>
        /// <param name="parameter">
        /// the <see cref="NpgsqlParameter"/> containing <paramref name="value"/>. Consulted for settings
        /// which impact how to send the parameter, e.g. <see cref="NpgsqlParameter.Size"/>. Can be null.
        /// </param>
        public abstract int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter);

        /// <param name="value">the value to be written</param>
        /// <param name="buf"></param>
        /// <param name="lengthCache">a cache in which to store length(s) of values to be written</param>
        /// <param name="parameter">
        /// the <see cref="NpgsqlParameter"/> containing <paramref name="value"/>. Consulted for settings
        /// which impact how to send the parameter, e.g. <see cref="NpgsqlParameter.Size"/>. Can be null.
        /// <see cref="NpgsqlParameter.Size"/>.
        /// </param>
        public abstract void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter);

        public abstract bool Write(ref DirectBuffer directBuf);

        /// <remarks>
        /// A type handler may implement IChunkingTypeHandler for types other than its primary one.
        /// This is why this method has type parameter T2 and not T.
        /// </remarks>
        [RewriteAsync(true)]
        internal override T2 ReadFully<T2>(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var asTypedHandler = this as IChunkingTypeHandler<T2>;
            if (asTypedHandler == null)
                throw new InvalidCastException(fieldDescription == null
                    ? "Can't cast database type to " + typeof(T2).Name
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(T2).Name}"
                );

            asTypedHandler.PrepareRead(buf, len, fieldDescription);
            T2 result;
            while (!asTypedHandler.Read(out result))
            {
                buf.ReadMore();
            }
            return result;
        }

        public bool ReadAsObject(out object result)
        {
            T result2;
            var completed = Read(out result2);
            result = result2;
            return completed;
        }
    }

    /// <summary>
    /// Type handlers that wish to support reading other types in additional to the main one can
    /// implement this interface for all those types.
    /// </summary>
    interface IChunkingTypeHandler<T> : IChunkingTypeHandler, ITypeHandler<T>
    {
        bool Read(out T result);
    }

    [ContractClassFor(typeof(ChunkingTypeHandler<>))]
    // ReSharper disable once InconsistentNaming
    class ChunkingTypeHandlerContracts<T> : ChunkingTypeHandler<T>
    {
        internal ChunkingTypeHandlerContracts(IBackendType backendType) : base(backendType) { }

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Contract.Requires(buf != null);
        }

        public override bool Read(out T result)
        {
            //Contract.Ensures(!completed || Contract.ValueAtReturn(out result) == default(T));
            result = default(T);
            return default(bool);
        }

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            Contract.Requires(value != null);
            return default(int);
        }

        public override void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            Contract.Requires(buf != null);
            Contract.Requires(value != null);
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            Contract.Ensures(Contract.Result<bool>() == false || directBuf.Buffer == null);
            return default(bool);
        }
    }

    /// <summary>
    /// Implemented by handlers which support <see cref="NpgsqlDataReader.GetTextReader"/>, returns a standard
    /// TextReader given a binary Stream.
    /// </summary>
    interface ITextReaderHandler
    {
        TextReader GetTextReader(Stream stream);
    }

    struct DirectBuffer
    {
        public byte[] Buffer;
        public int Offset;
        public int Size;
    }

    /// <summary>
    /// Can be thrown by readers to indicate that interpreting the value failed, but the value was read wholly
    /// and it is safe to continue reading. Any other exception is assumed to leave the row in an unknown state
    /// and the connector is therefore set to Broken.
    /// Note that an inner exception is mandatory, and will get thrown to the user instead of the SafeReadException.
    /// </summary>
    internal class SafeReadException : Exception
    {
        public SafeReadException(Exception innerException) : base("", innerException)
        {
            Contract.Requires(innerException != null);
        }
    }
}
