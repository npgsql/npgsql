using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql
{
    interface ITypeHandler
    {
        /// <summary>
        /// If true, the type handler reads values of totally arbitrary length. These type handlers are expected
        /// to  handle reading from socket on its own if the buffer doesn't contain enough data.
        /// Otherwise the entire column data is expected to be loaded in the buffer prior to Read() being invoked.
        /// </summary>
        bool IsChunking { get; }
        /// <summary>
        /// Whether this type handler supports reading the binary Postgresql representation for its type.
        /// </summary>
        bool SupportsBinaryRead { get; }
    }

    /// <summary>
    /// A type handler that can read a value from a column and return it as type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">the type of the value returned by this type handler</typeparam>
    [ContractClass(typeof(ITypeHandlerContract<>))]
    // ReSharper disable once TypeParameterCanBeVariant
    interface ITypeHandler<T> : ITypeHandler
    {
        T Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len);
    }

    // ReSharper disable once InconsistentNaming
    [ContractClassFor(typeof(ITypeHandler<>))]
    class ITypeHandlerContract<T> : ITypeHandler<T>
    {
        public T Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            Contract.Requires(SupportsBinaryRead || fieldDescription.IsTextFormat);
            Contract.Requires(buf.ReadBytesLeft >= len || IsChunking);
            return default(T);
        }

        public bool IsChunking { get { return default(bool); } }
        public bool SupportsBinaryRead { get { return default(bool); } }
    }

    internal abstract class TypeHandler : ITypeHandler
    {
        internal string PgName { get; set; }
        internal uint OID { get; set; }
        internal NpgsqlDbType NpgsqlDbType { get; set; }
        internal abstract Type GetFieldType(FieldDescription fieldDescription=null);
        internal abstract Type GetProviderSpecificFieldType(FieldDescription fieldDescription=null);

        /// <summary>
        /// Whether this type handler supports reading the binary Postgresql representation for its type.
        /// </summary>
        public virtual bool SupportsBinaryRead { get { return true; } }

        /// <summary>
        /// If true, the type handler reads and writes values of totally arbitrary length.
        /// These type handlers are expected to handle reading from sockets and writing to them on their own
        /// if the buffer doesn't contain enough data (i.e. perform Ensure).
        /// Otherwise, the type handler expects the buffer to contain enough bytes prior to Read() and
        /// Write() being invoked by the framework.
        /// </summary>
        public virtual bool IsChunking { get { return false; } }

        internal abstract object ReadValueAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len);
        internal abstract object ReadPsvAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len);

        public virtual bool PreferTextWrite { get { return false; } }
        public virtual bool SupportsBinaryWrite { get { return true; } }

        internal virtual int Length(object value)
        {
            Contract.Requires(value != null);
            throw new NotImplementedException("Length for " + GetType());
        }

        internal virtual void PrepareChunkedWrite(object value)
        {
            Contract.Requires(value != null);
            Contract.Requires(IsChunking);
            throw new NotImplementedException("PreparedChunkedWrite for " + GetType());            
        }

        internal virtual bool WriteBinaryChunk(NpgsqlBuffer buf)
        {
            Contract.Requires(IsChunking);
            throw new NotImplementedException("WriteBinaryChunk for " + GetType());
        }

        internal virtual bool WriteBinaryChunk(NpgsqlBuffer buf, out byte[] directBuf)
        {
            Contract.Requires(IsChunking);
            directBuf = null;
            return WriteBinaryChunk(buf);
        }

        internal virtual void WriteBinary(object value, NpgsqlBuffer buf)
        {
            Contract.Requires(value != null);
            Contract.Requires(!IsChunking);
            throw new NotImplementedException("WriteBinary for " + GetType());
        }

        public virtual void WriteText(object value, NpgsqlTextWriter writer)
        {
            writer.WriteString(value.ToString());
        }

        protected static T GetIConvertibleValue<T>(object value) where T : IConvertible
        {
            return value is T ? (T)value : (T)Convert.ChangeType(value, typeof(T), null);
        }
    }

    internal abstract class TypeHandler<T> : TypeHandler, ITypeHandler<T>
    {
        internal override Type GetFieldType(FieldDescription fieldDescription)
        {
            return typeof(T);
        }

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription)
        {
            return typeof(T);
        }

        internal override object ReadValueAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len);            
        }

        internal override object ReadPsvAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len);
        }

        public abstract T Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len);
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

        internal override object ReadPsvAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return ((ITypeHandler<TPsv>)this).Read(buf, fieldDescription, len);
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(this is ITypeHandler<TPsv>);
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

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes=null, Type type=null)
            : this(pgName, npgsqlDbType, dbTypes, type == null ? null : new[] { type }) {}
        
        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType dbType, Type[] types)
            : this(pgName, npgsqlDbType, new[] { dbType }, types) {}

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType dbType, Type type=null)
            : this(pgName, npgsqlDbType, new[] { dbType }, type == null ? null : new[] { type }) {}

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type type)
            : this(pgName, npgsqlDbType, new DbType[0], new[] { type }) {}

        /// <summary>
        /// Read-only parameter, only used by "unknown"
        /// </summary>
        internal TypeMappingAttribute(string pgName)
            : this(pgName, null, null, null) {}

        internal string PgName { get; private set; }
        internal Type[] Types { get; private set; }
        internal DbType[] DbTypes { get; private set; }
        internal NpgsqlDbType? NpgsqlDbType { get; private set; }

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
