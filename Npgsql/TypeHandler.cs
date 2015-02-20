using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql
{
    interface ITypeReader<T> {}

    #region Simple type handler

    interface ISimpleTypeWriter
    {
        int ValidateAndGetLength(object value);
        void Write(object value, NpgsqlBuffer buf);
    }

    /// <summary>
    /// A handler which can read small, usually fixed-length values.
    /// </summary>
    /// <typeparam name="T">the type of the value returned by this type handler</typeparam>
    //[ContractClass(typeof(ITypeHandlerContract<>))]
    // ReSharper disable once TypeParameterCanBeVariant
    interface ISimpleTypeReader<T> : ITypeReader<T>
    {
        /// <summary>
        /// The entire data required to read the value is expected to be in the buffer.
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="fieldDescription"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        T Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len);
    }

    #endregion

    #region Chunking type handler

    [ContractClass(typeof(IChunkingTypeWriterContracts))]
    interface IChunkingTypeWriter
    {
        /// <param name="value">the value to be examined</param>
        /// <param name="parameter">
        /// the <see cref="NpgsqlParameter"/> containing <paramref name="value"/>. Consulted for the
        /// length cache and for settings which impact how to send the parameter, e.g.
        /// <see cref="NpgsqlParameter.Size"/>.
        /// </param>
        int ValidateAndGetLength(object value, NpgsqlParameter parameter);

        /// <param name="value">the value to be written</param>
        /// <param name="buf"></param>
        /// <param name="parameter">
        /// the <see cref="NpgsqlParameter"/> containing <paramref name="value"/>. Consulted for the
        /// length cache and for settings which impact how to send the parameter, e.g.
        /// <see cref="NpgsqlParameter.Size"/>.
        /// </param>
        void PrepareWrite(object value, NpgsqlBuffer buf, NpgsqlParameter parameter);
        bool Write(ref DirectBuffer directBuf);
    }

    [ContractClassFor(typeof(IChunkingTypeWriter))]
    // ReSharper disable once InconsistentNaming
    class IChunkingTypeWriterContracts : IChunkingTypeWriter
    {
        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            Contract.Requires(value != null);
            return default(int);
        }

        public void PrepareWrite(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            Contract.Requires(buf != null);
            Contract.Requires(value != null);
        }

        public bool Write(ref DirectBuffer directBuf)
        {
            Contract.Ensures(Contract.Result<bool>() == false || directBuf.Buffer == null);
            return default(bool);
        }
    }

    /// <summary>
    /// A type handler which handles values of totally arbitrary length, and therefore supports chunking them.
    /// </summary>
    [ContractClass(typeof(IChunkingTypeReaderContracts<>))]
    // ReSharper disable once TypeParameterCanBeVariant
    interface IChunkingTypeReader<T> : ITypeReader<T>
    {
        void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len);
        bool Read(out T result);
    }

    [ContractClassFor(typeof(IChunkingTypeReader<>))]
    // ReSharper disable once InconsistentNaming
    class IChunkingTypeReaderContracts<T> : IChunkingTypeReader<T>
    {
        public void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            Contract.Requires(buf != null);
            Contract.Requires(fieldDescription != null);
        }

        public bool Read(out T result)
        {
            //Contract.Ensures(!completed || Contract.ValueAtReturn(out result) == default(T));
            result = default(T);
            return default(bool);
        }
    }

    #endregion

    internal abstract class TypeHandler
    {
        internal string PgName { get; set; }
        internal uint OID { get; set; }
        internal NpgsqlDbType NpgsqlDbType { get; set; }
        internal abstract Type GetFieldType(FieldDescription fieldDescription=null);
        internal abstract Type GetProviderSpecificFieldType(FieldDescription fieldDescription=null);

        internal abstract object ReadValueAsObject(DataRowMessage row, FieldDescription fieldDescription);

        internal virtual object ReadPsvAsObject(DataRowMessage row, FieldDescription fieldDescription)
        {
            return ReadValueAsObject(row, fieldDescription);
        }

        public virtual bool PreferTextWrite { get { return false; } }

        internal T Read<T>(DataRowMessage row, FieldDescription fieldDescription, int len)
        {
            Contract.Requires(row.PosInColumn == 0);
            Contract.Ensures(row.PosInColumn == row.ColumnLen);

            T result;

            var asSimpleReader = this as ISimpleTypeReader<T>;
            if (asSimpleReader != null)
            {
                row.Buffer.Ensure(len);
                result = asSimpleReader.Read(row.Buffer, fieldDescription, len);
            }
            else
            {
                var asChunkingReader = this as IChunkingTypeReader<T>;
                if (asChunkingReader == null) {
                    throw new InvalidCastException(String.Format("Can't cast database type {0} to {1}", fieldDescription.Handler.PgName, typeof(T).Name));
                }

                asChunkingReader.PrepareRead(row.Buffer, fieldDescription, len);
                while (!asChunkingReader.Read(out result)) {
                    row.Buffer.ReadMore();
                }
            }

            row.PosInColumn += row.ColumnLen;
            return result;
        }

        protected static T GetIConvertibleValue<T>(object value) where T : IConvertible
        {
            return value is T ? (T)value : (T)Convert.ChangeType(value, typeof(T), null);
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(!(this is IChunkingTypeWriter && this is ISimpleTypeWriter));
        }
    }

    internal abstract class TypeHandler<T> : TypeHandler
    {
        internal override Type GetFieldType(FieldDescription fieldDescription)
        {
            return typeof(T);
        }

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription)
        {
            return typeof(T);
        }

        internal override object ReadValueAsObject(DataRowMessage row, FieldDescription fieldDescription)
        {
            return Read<T>(row, fieldDescription, row.ColumnLen);
        }

        internal override object ReadPsvAsObject(DataRowMessage row, FieldDescription fieldDescription)
        {
            return Read<T>(row, fieldDescription, row.ColumnLen);
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(this is ISimpleTypeReader<T> || this is IChunkingTypeReader<T>);
        }
    }

    /// <summary>
    /// A marking interface to allow us to know whether a given type handler has a provider-specific type
    /// distinct from its regular type
    /// </summary>
    internal interface ITypeHandlerWithPsv {}

    /// <summary>
    /// A type handler that supports a provider-specific value which is different from the regular value (e.g.
    /// NpgsqlDate and DateTime)
    /// </summary>
    /// <typeparam name="T">the regular value type returned by this type handler</typeparam>
    /// <typeparam name="TPsv">the type of the provider-specific value returned by this type handler</typeparam>
    internal abstract class TypeHandlerWithPsv<T, TPsv> : TypeHandler<T>, ITypeHandlerWithPsv
    {
        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription)
        {
            return typeof (TPsv);
        }

        internal override object ReadPsvAsObject(DataRowMessage row, FieldDescription fieldDescription)
        {
            return Read<TPsv>(row, fieldDescription, row.ColumnLen);
        }
    }

    struct DirectBuffer
    {
        public byte[] Buffer;
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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [SuppressMessage("ReSharper", "LocalizableElement")]
    class TypeMappingAttribute : Attribute
    {
        internal TypeMappingAttribute(string pgName, NpgsqlDbType? npgsqlDbType, DbType[] dbTypes, Type[] types)
        {
            if (String.IsNullOrWhiteSpace(pgName))
                throw new ArgumentException("pgName can't be empty", "pgName");
            Contract.EndContractBlock();

            PgName = pgName;
            NpgsqlDbType = npgsqlDbType;
            DbTypes = dbTypes ?? new DbType[0];
            Types = types ?? new Type[0];
        }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes, Type[] types)
            : this(pgName, (NpgsqlDbType?)npgsqlDbType, dbTypes, types) {}

        //internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes=null, Type type=null)
        //    : this(pgName, npgsqlDbType, dbTypes, type == null ? null : new[] { type }) {}

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType)
            : this(pgName, npgsqlDbType, new DbType[0], new Type[0]) { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes, Type type)
            : this(pgName, npgsqlDbType, dbTypes, new[] {type}) { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType dbType, Type[] types)
            : this(pgName, npgsqlDbType, new[] { dbType }, types) {}

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType dbType, Type type=null)
            : this(pgName, npgsqlDbType, new[] { dbType }, type == null ? null : new[] { type }) {}

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type[] types)
            : this(pgName, npgsqlDbType, new DbType[0], types) { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type type)
            : this(pgName, npgsqlDbType, new DbType[0], new[] { type }) {}

        /// <summary>
        /// Read-only parameter, only used by "unknown"
        /// </summary>
        internal TypeMappingAttribute(string pgName)
            : this(pgName, null, null, null) {}

        internal string PgName { get; private set; }
        internal NpgsqlDbType? NpgsqlDbType { get; private set; }
        internal DbType[] DbTypes { get; private set; }
        internal Type[] Types { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[{0} NpgsqlDbType={1}", PgName, NpgsqlDbType);
            if (DbTypes.Length > 0) {
                sb.Append(" DbTypes=");
                sb.Append(String.Join(",", DbTypes.Select(t => t.ToString())));
            }
            if (Types.Length > 0) {
                sb.Append(" Types=");
                sb.Append(String.Join(",", Types.Select(t => t.Name)));
            }
            sb.AppendFormat("]");
            return sb.ToString();
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(!String.IsNullOrWhiteSpace(PgName));
            Contract.Invariant(Types != null);
            Contract.Invariant(DbTypes != null);
        }
    }
}
