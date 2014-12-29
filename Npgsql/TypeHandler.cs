using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql
{
    interface ITypeHandler
    {
        /// <summary>
        /// If true, the type handler reads values of totally arbitrary length. These type handlers are expected
        /// to  handle reading from socket on its own if the buffer doesn't contain enough data.
        /// Otherwise the entire column data is expected to be loaded in the buffer prior to Read() being invoked.
        /// </summary>
        bool IsArbitraryLength { get; }
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
            Contract.Requires(buf.BytesLeft >= len || IsArbitraryLength);
            return default(T);
        }

        public bool IsArbitraryLength { get { return default(bool); } }
        public bool SupportsBinaryRead { get { return default(bool); } }
    }

    internal abstract class TypeHandler : ITypeHandler
    {
        internal abstract string[] PgNames { get; }
        internal string PgName { get { return PgNames[0]; } }
        internal uint Oid { get; set; }
        internal abstract Type GetFieldType(FieldDescription fieldDescription=null);
        internal abstract Type GetProviderSpecificFieldType(FieldDescription fieldDescription=null);

        /// <summary>
        /// Whether this type handler supports reading the binary Postgresql representation for its type.
        /// </summary>
        public virtual bool SupportsBinaryRead { get { return false; } }

        /// <summary>
        /// If true, the type handler reads values of totally arbitrary length. These type handlers are expected
        /// to handle reading from socket on its own if the buffer doesn't contain enough data.
        /// Otherwise the entire column data is expected to be loaded in the buffer prior to Read() being invoked.
        /// </summary>
        public virtual bool IsArbitraryLength { get { return false; } }

        protected TypeHandler()
        {
            Oid = 0;
        }

        internal abstract object ReadValueAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len);
        internal abstract object ReadPsvAsObject(NpgsqlBuffer buf, FieldDescription fieldDescription, int len);
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
}
