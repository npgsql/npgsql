using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// A generic version of <see cref="NpgsqlParameter"/> which provides more type safety and
    /// avoids boxing of value types. Use <see cref="TypedValue"/> instead of <see cref="NpgsqlParameter.Value"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value that will be stored in the parameter.</typeparam>
    public sealed class NpgsqlParameter<T> : NpgsqlParameter
    {
        /// <summary>
        /// Gets or sets the strongly-typed value of the parameter.
        /// </summary>
        public T TypedValue { get; set; }

        /// <summary>
        /// Gets or sets the value of the parameter. This delegates to <see cref="TypedValue"/>.
        /// </summary>
        public override object Value
        {
            get => TypedValue;
            set => TypedValue = (T)value;
        }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlParameter{T}" />.
        /// </summary>
        public NpgsqlParameter() {}

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlParameter{T}" /> with a parameter name and value.
        /// </summary>
        public NpgsqlParameter(string parameterName, T value)
        {
            ParameterName = parameterName;
            TypedValue = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlParameter{T}" /> with a parameter name and type.
        /// </summary>
        public NpgsqlParameter(string parameterName, NpgsqlDbType npgsqlDbType)
        {
            ParameterName = parameterName;
            NpgsqlDbType = npgsqlDbType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlParameter{T}" /> with a parameter name and type.
        /// </summary>
        public NpgsqlParameter(string parameterName, DbType dbType)
        {
            ParameterName = parameterName;
            DbType = dbType;
        }

        #endregion Constructors

        internal override void ResolveHandler(ConnectorTypeMapper typeMapper)
        {
            if (Handler != null)
                return;

            // TODO: Better exceptions in case of cast failure etc.
            if (_npgsqlDbType.HasValue)
                Handler = typeMapper.GetByNpgsqlDbType(_npgsqlDbType.Value);
            else if (_dataTypeName != null)
                Handler = typeMapper.GetByDataTypeName(_dataTypeName);
            else if (_dbType.HasValue)
                Handler = typeMapper.GetByDbType(_dbType.Value);
            else
                Handler = typeMapper.GetByClrType(typeof(T));
        }

        internal override int ValidateAndGetLength()
        {
            Debug.Assert(Handler != null);

            if (TypedValue == null)
                return 0;

            // TODO: Why do it like this rather than a handler?
            if (typeof(T) == typeof(DBNull))
                return 0;

            var lengthCache = LengthCache;
            var len = Handler.ValidateAndGetLength(TypedValue, ref lengthCache, this);
            LengthCache = lengthCache;
            return len;
        }

        internal override Task WriteWithLength(NpgsqlWriteBuffer buf, bool async)
        {
            Debug.Assert(Handler != null);
            return Handler.WriteWithLengthInternal(TypedValue, buf, LengthCache, this, async);
        }
    }
}
