#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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

using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Npgsql
{
    ///<summary>
    /// This class represents a parameter to a command that will be sent to server
    ///</summary>
    public class NpgsqlParameter : DbParameter, IDbDataParameter, ICloneable
    {
        #region Fields and Properties

        byte _precision;
        byte _scale;
        int _size;

        // ReSharper disable InconsistentNaming
        // TODO: Switch to private protected once mono's msbuild/csc supports C# 7.2
        internal NpgsqlDbType? _npgsqlDbType;
        internal DbType? _dbType;
        [CanBeNull]
        internal string _dataTypeName;
        // ReSharper restore InconsistentNaming
        [CanBeNull]
        Type _specificType;
        string _name = string.Empty;
        [CanBeNull]
        object _value;

        internal string TrimmedName { get; private set; } = string.Empty;

        /// <summary>
        /// Can be used to communicate a value from the validation phase to the writing phase.
        /// To be used by type handlers only.
        /// </summary>
        public object ConvertedValue { get; set; }

        [CanBeNull]
        internal NpgsqlLengthCache LengthCache { get; set; }

        [CanBeNull]
        internal NpgsqlTypeHandler Handler { get; set; }

        internal FormatCode FormatCode { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see> class.
        /// </summary>
        public NpgsqlParameter()
        {
            SourceColumn = string.Empty;
            Direction = ParameterDirection.Input;
            SourceVersion = DataRowVersion.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>
        /// class with the parameter name and a value of the new <b>NpgsqlParameter</b>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="value">An <see cref="System.Object">Object</see> that is the value of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.</param>
        /// <remarks>
        /// <p>When you specify an <see cref="System.Object">Object</see>
        /// in the value parameter, the <see cref="System.Data.DbType">DbType</see> is
        /// inferred from the .NET Framework type of the <b>Object</b>.</p>
        /// <p>When using this constructor, you must be aware of a possible misuse of the constructor which takes a DbType parameter.
        /// This happens when calling this constructor passing an int 0 and the compiler thinks you are passing a value of DbType.
        /// Use <code> Convert.ToInt32(value) </code> for example to have compiler calling the correct constructor.</p>
        /// </remarks>
        public NpgsqlParameter(string parameterName, object value) : this()
        {
            ParameterName = parameterName;
            // ReSharper disable once VirtualMemberCallInConstructor
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>
        /// class with the parameter name and the data type.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="System.Data.DbType">DbType</see> values.</param>
        public NpgsqlParameter(string parameterName, NpgsqlDbType parameterType)
            : this(parameterName, parameterType, 0, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="System.Data.DbType">DbType</see> values.</param>
        public NpgsqlParameter(string parameterName, DbType parameterType)
            : this(parameterName, parameterType, 0, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> values.</param>
        /// <param name="size">The length of the parameter.</param>
        public NpgsqlParameter(string parameterName, NpgsqlDbType parameterType, int size)
            : this(parameterName, parameterType, size, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="System.Data.DbType">DbType</see> values.</param>
        /// <param name="size">The length of the parameter.</param>
        public NpgsqlParameter(string parameterName, DbType parameterType, int size)
            : this(parameterName, parameterType, size, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> values.</param>
        /// <param name="size">The length of the parameter.</param>
        /// <param name="sourceColumn">The name of the source column.</param>
        public NpgsqlParameter(string parameterName, NpgsqlDbType parameterType, int size, string sourceColumn)
            : this()
        {
            ParameterName = parameterName;
            NpgsqlDbType = parameterType;
            _size = size;
            SourceColumn = sourceColumn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="System.Data.DbType">DbType</see> values.</param>
        /// <param name="size">The length of the parameter.</param>
        /// <param name="sourceColumn">The name of the source column.</param>
        public NpgsqlParameter(string parameterName, DbType parameterType, int size, string sourceColumn)
            : this()
        {
            ParameterName = parameterName;
            DbType = parameterType;
            _size = size;
            SourceColumn = sourceColumn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> values.</param>
        /// <param name="size">The length of the parameter.</param>
        /// <param name="sourceColumn">The name of the source column.</param>
        /// <param name="direction">One of the <see cref="System.Data.ParameterDirection">ParameterDirection</see> values.</param>
        /// <param name="isNullable"><b>true</b> if the value of the field can be null, otherwise <b>false</b>.</param>
        /// <param name="precision">The total number of digits to the left and right of the decimal point to which
        /// <see cref="NpgsqlParameter.Value">Value</see> is resolved.</param>
        /// <param name="scale">The total number of decimal places to which
        /// <see cref="NpgsqlParameter.Value">Value</see> is resolved.</param>
        /// <param name="sourceVersion">One of the <see cref="System.Data.DataRowVersion">DataRowVersion</see> values.</param>
        /// <param name="value">An <see cref="System.Object">Object</see> that is the value
        /// of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.</param>
        public NpgsqlParameter(string parameterName, NpgsqlDbType parameterType, int size, string sourceColumn,
                               ParameterDirection direction, bool isNullable, byte precision, byte scale,
                               DataRowVersion sourceVersion, object value)
            : this()
        {
            ParameterName = parameterName;
            Size = size;
            SourceColumn = sourceColumn;
            Direction = direction;
            IsNullable = isNullable;
            Precision = precision;
            Scale = scale;
            SourceVersion = sourceVersion;
            // ReSharper disable once VirtualMemberCallInConstructor
            Value = value;

            NpgsqlDbType = parameterType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="System.Data.DbType">DbType</see> values.</param>
        /// <param name="size">The length of the parameter.</param>
        /// <param name="sourceColumn">The name of the source column.</param>
        /// <param name="direction">One of the <see cref="System.Data.ParameterDirection">ParameterDirection</see> values.</param>
        /// <param name="isNullable"><b>true</b> if the value of the field can be null, otherwise <b>false</b>.</param>
        /// <param name="precision">The total number of digits to the left and right of the decimal point to which
        /// <see cref="NpgsqlParameter.Value">Value</see> is resolved.</param>
        /// <param name="scale">The total number of decimal places to which
        /// <see cref="NpgsqlParameter.Value">Value</see> is resolved.</param>
        /// <param name="sourceVersion">One of the <see cref="System.Data.DataRowVersion">DataRowVersion</see> values.</param>
        /// <param name="value">An <see cref="System.Object">Object</see> that is the value
        /// of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.</param>
        public NpgsqlParameter(string parameterName, DbType parameterType, int size, string sourceColumn,
                               ParameterDirection direction, bool isNullable, byte precision, byte scale,
                               DataRowVersion sourceVersion, object value)
            : this()
        {
            ParameterName = parameterName;
            Size = size;
            SourceColumn = sourceColumn;
            Direction = direction;
            IsNullable = isNullable;
            Precision = precision;
            Scale = scale;
            SourceVersion = sourceVersion;
            // ReSharper disable once VirtualMemberCallInConstructor
            Value = value;
            DbType = parameterType;
        }

        #endregion

        #region Name

        /// <summary>
        /// Gets or sets The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <value>The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// The default is an empty string.</value>
        [DefaultValue("")]
        public sealed override string ParameterName
        {
            get => _name;
            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (value == null)
                {
                    _name = TrimmedName = string.Empty;
                }
                else if (value.Length > 0 && (value[0] == ':' || value[0] == '@'))
                {
                    TrimmedName = value.Substring(1);
                    _name = value;
                }
                else
                    _name = TrimmedName = value;

                Collection?.InvalidateHashLookups();
            }
        }

        #endregion Name

        #region Value

        /// <inheritdoc />
        [TypeConverter(typeof(StringConverter)), Category("Data")]
        [CanBeNull]
        public override object Value
        {
            get => _value;
            set
            {
                if (_value == null || value == null || _value.GetType() != value.GetType())
                    Handler = null;
                _value = value;
                ConvertedValue = null;
            }
        }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        /// <value>An <see cref="System.Object">Object</see> that is the value of the parameter.
        /// The default value is null.</value>
        [Category("Data")]
        [TypeConverter(typeof(StringConverter))]
        [CanBeNull]
        public object NpgsqlValue
        {
            get => Value;
            set => Value = value;
        }

        #endregion Value

        #region Type

        /// <summary>
        /// Gets or sets the <see cref="System.Data.DbType">DbType</see> of the parameter.
        /// </summary>
        /// <value>One of the <see cref="System.Data.DbType">DbType</see> values. The default is <b>Object</b>.</value>
        [DefaultValue(DbType.Object)]
        [Category("Data"), RefreshProperties(RefreshProperties.All)]
        public sealed override DbType DbType
        {
            get
            {
                if (_dbType.HasValue) {
                    return _dbType.Value;
                }

                if (_value != null) {   // Infer from value
                    return GlobalTypeMapper.Instance.ToDbType(_value.GetType());
                }

                return DbType.Object;
            }
            set
            {
                Handler = null;
                if (value == DbType.Object)
                {
                    _dbType = null;
                    _npgsqlDbType = null;
                }
                else
                {
                    _dbType = value;
                    _npgsqlDbType = GlobalTypeMapper.Instance.ToNpgsqlDbType(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> of the parameter.
        /// </summary>
        /// <value>One of the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> values. The default is <b>Unknown</b>.</value>
        [DefaultValue(NpgsqlDbType.Unknown)]
        [Category("Data"), RefreshProperties(RefreshProperties.All)]
        [DbProviderSpecificTypeProperty(true)]
        public NpgsqlDbType NpgsqlDbType
        {
            get
            {
                if (_npgsqlDbType.HasValue)
                    return _npgsqlDbType.Value;
                if (_value != null)   // Infer from value
                    return GlobalTypeMapper.Instance.ToNpgsqlDbType(_value.GetType());
                return NpgsqlDbType.Unknown;
            }
            set
            {
                if (value == NpgsqlDbType.Array)
                    throw new ArgumentOutOfRangeException(nameof(value), "Cannot set NpgsqlDbType to just Array, Binary-Or with the element type (e.g. Array of Box is NpgsqlDbType.Array | NpgsqlDbType.Box).");
                if (value == NpgsqlDbType.Range)
                    throw new ArgumentOutOfRangeException(nameof(value), "Cannot set NpgsqlDbType to just Range, Binary-Or with the element type (e.g. Range of integer is NpgsqlDbType.Range | NpgsqlDbType.Integer)");

                Handler = null;
                _npgsqlDbType = value;
                _dbType = GlobalTypeMapper.Instance.ToDbType(value);
            }
        }

        /// <summary>
        /// Used to specify which PostgreSQL type will be sent to the database for this parameter.
        /// </summary>
        [PublicAPI, CanBeNull]
        public string DataTypeName
        {
            get
            {
                if (_dataTypeName != null)
                    return _dataTypeName;
                throw new NotImplementedException("Infer from others");
            }
            set
            {
                _dataTypeName = value;
                Handler = null;
            }
        }

        #endregion Type

        #region Other Properties

        /// <inheritdoc />
        public sealed override bool IsNullable { get; set; }

        /// <inheritdoc />
        [DefaultValue(ParameterDirection.Input)]
        [Category("Data")]
        public sealed override ParameterDirection Direction { get; set; }

#pragma warning disable CS0109
        /// <summary>
        /// Gets or sets the maximum number of digits used to represent the
        /// <see cref="NpgsqlParameter.Value">Value</see> property.
        /// </summary>
        /// <value>The maximum number of digits used to represent the
        /// <see cref="NpgsqlParameter.Value">Value</see> property.
        /// The default value is 0, which indicates that the data provider
        /// sets the precision for <b>Value</b>.</value>
        [DefaultValue((byte)0)]
        [Category("Data")]
        public new byte Precision
        {
            get => _precision;
            set
            {
                _precision = value;
                Handler = null;
            }
        }

        /// <summary>
        /// Gets or sets the number of decimal places to which
        /// <see cref="NpgsqlParameter.Value">Value</see> is resolved.
        /// </summary>
        /// <value>The number of decimal places to which
        /// <see cref="NpgsqlParameter.Value">Value</see> is resolved. The default is 0.</value>
        [DefaultValue((byte)0)]
        [Category("Data")]
        public new byte Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                Handler = null;
            }
        }
#pragma warning restore CS0109

        /// <inheritdoc />
        [DefaultValue(0)]
        [Category("Data")]
        public sealed override int Size
        {
            get => _size;
            set
            {
                if (value < -1)
                    throw new ArgumentException($"Invalid parameter Size value '{value}'. The value must be greater than or equal to 0.");

                _size = value;
                Handler = null;
            }
        }

        /// <inheritdoc />
        [DefaultValue("")]
        [Category("Data")]
        public sealed override string SourceColumn { get; set; }

        /// <inheritdoc />
        [Category("Data"), DefaultValue(DataRowVersion.Current)]
        public sealed override DataRowVersion SourceVersion { get; set; }

        /// <inheritdoc />
        public sealed override bool SourceColumnNullMapping { get; set; }

#pragma warning disable CA2227
        /// <summary>
        /// The collection to which this parameter belongs, if any.
        /// </summary>
        [CanBeNull]
        public NpgsqlParameterCollection Collection { get; set; }
#pragma warning restore CA2227

        /// <summary>
        /// The PostgreSQL data type, such as int4 or text, as discovered from pg_type.
        /// This property is automatically set if parameters have been derived via
        /// <see cref="NpgsqlCommandBuilder.DeriveParameters"/> and can be used to
        /// acquire additional information about the parameters' data type.
        /// </summary>
        public PostgresType PostgresType { get; internal set; }

        #endregion Other Properties

        #region Internals

        /// <summary>
        /// Returns whether this parameter has had its type set explicitly via DbType or NpgsqlDbType
        /// (and not via type inference)
        /// </summary>
        internal bool IsTypeExplicitlySet => _npgsqlDbType.HasValue || _dbType.HasValue;

        internal virtual void ResolveHandler(ConnectorTypeMapper typeMapper)
        {
            if (Handler != null)
                return;

            if (_npgsqlDbType.HasValue)
                Handler = typeMapper.GetByNpgsqlDbType(_npgsqlDbType.Value);
            else if (_dataTypeName != null)
                Handler = typeMapper.GetByDataTypeName(_dataTypeName);
            else if (_dbType.HasValue)
                Handler = typeMapper.GetByDbType(_dbType.Value);
            else if (_value != null)
                Handler = typeMapper.GetByClrType(_value.GetType());
            else
                throw new InvalidOperationException($"Parameter '{ParameterName}' must have its value set");
        }

        internal void Bind(ConnectorTypeMapper typeMapper)
        {
            ResolveHandler(typeMapper);
            Debug.Assert(Handler != null);
            FormatCode = Handler.PreferTextWrite ? FormatCode.Text : FormatCode.Binary;
        }

        internal virtual int ValidateAndGetLength()
        {
            Debug.Assert(Handler != null);

            if (_value == null)
                throw new InvalidCastException($"Parameter {ParameterName} must be set");
            if (_value is DBNull)
                return 0;

            var lengthCache = LengthCache;
            var len = Handler.ValidateObjectAndGetLength(Value, ref lengthCache, this);
            LengthCache = lengthCache;
            return len;
        }

        internal virtual Task WriteWithLength(NpgsqlWriteBuffer buf, bool async)
        {
            Debug.Assert(Handler != null);
            return Handler.WriteObjectWithLength(Value, buf, LengthCache, this, async);
        }

        /// <inheritdoc />
        public override void ResetDbType()
        {
            _dbType = null;
            _npgsqlDbType = null;
            _dataTypeName = null;
            Value = Value;
            Handler = null;
        }

        internal bool IsInputDirection => Direction == ParameterDirection.InputOutput || Direction == ParameterDirection.Input;

        internal bool IsOutputDirection => Direction == ParameterDirection.InputOutput || Direction == ParameterDirection.Output;

        #endregion

        #region Clone

        /// <summary>
        /// Creates a new <see cref="NpgsqlParameter">NpgsqlParameter</see> that
        /// is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="NpgsqlParameter">NpgsqlParameter</see> that is a copy of this instance.</returns>
        public NpgsqlParameter Clone()
        {
            // use fields instead of properties
            // to avoid auto-initializing something like type_info
            var clone = new NpgsqlParameter
            {
                _precision = _precision,
                _scale = _scale,
                _size = _size,
                _dbType = _dbType,
                _npgsqlDbType = _npgsqlDbType,
                _specificType = _specificType,
                Direction = Direction,
                IsNullable = IsNullable,
                _name = _name,
                TrimmedName = TrimmedName,
                SourceColumn = SourceColumn,
                SourceVersion = SourceVersion,
                _value = _value,
                SourceColumnNullMapping = SourceColumnNullMapping,
            };
            return clone;
        }

        object ICloneable.Clone() => Clone();

        #endregion
    }
}
