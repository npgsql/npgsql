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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NpgsqlTypes;

namespace Npgsql
{
    ///<summary>
    /// This class represents a parameter to a command that will be sent to server
    ///</summary>
#if NETSTANDARD1_3
    public sealed class NpgsqlParameter : DbParameter
#else
    public sealed class NpgsqlParameter : DbParameter, ICloneable
#endif
    {
        #region Fields and Properties

        // Fields to implement IDbDataParameter interface.
        byte _precision;
        byte _scale;
        int _size;

        // Fields to implement IDataParameter
        NpgsqlDbType? _npgsqlDbType;
        DbType? _dbType;
        Type _specificType;
        string _name = string.Empty;
        object _value;
        object _npgsqlValue;

        /// <summary>
        /// Can be used to communicate a value from the validation phase to the writing phase.
        /// </summary>
        internal object ConvertedValue { get; set; }

        [CanBeNull]
        internal LengthCache LengthCache { get; private set; }

        internal TypeHandler Handler { get; private set; }
        internal FormatCode FormatCode { get; private set; }

        internal bool AutoAssignedName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see> class.
        /// </summary>
        public NpgsqlParameter()
        {
            SourceColumn = string.Empty;
            Direction = ParameterDirection.Input;
#if NET45 || NET451
            SourceVersion = DataRowVersion.Current;
#endif
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

#if NET45 || NET451
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
            Value = value;

            DbType = parameterType;
        }
#endif

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        /// <value>An <see cref="System.Object">Object</see> that is the value of the parameter.
        /// The default value is null.</value>
#if NET45 || NET451
        [TypeConverter(typeof(StringConverter)), Category("Data")]
#endif
        public override object Value
        {
            get
            {
                return _value;
            }
            set
            {
                ClearBind();
                _value = value;
                _npgsqlValue = value;
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
        public object NpgsqlValue
        {
            get => _npgsqlValue;
            set {
                ClearBind();
                _value = value;
                _npgsqlValue = value;
                ConvertedValue = null;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the parameter accepts null values.
        /// </summary>
        public override bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is input-only,
        /// output-only, bidirectional, or a stored procedure return value parameter.
        /// </summary>
        /// <value>One of the <see cref="System.Data.ParameterDirection">ParameterDirection</see>
        /// values. The default is <b>Input</b>.</value>
        [DefaultValue(ParameterDirection.Input)]
        [Category("Data")]
        public override ParameterDirection Direction { get; set; }

        // Implementation of IDbDataParameter
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
#if NET45
// In mono .NET 4.5 is actually a later version, meaning that virtual Precision and Scale already exist in DbParameter
#pragma warning disable CS0114
        public byte Precision
#pragma warning restore CS0114
#else
        public override byte Precision
#endif
        {
            get { return _precision; }
            set
            {
                _precision = value;
                ClearBind();
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
#if NET45
// In mono .NET 4.5 is actually a later version, meaning that virtual Precision and Scale already exist in DbParameter
#pragma warning disable CS0114
        public byte Scale
#pragma warning restore CS0114
#else
        public override byte Scale
#endif
        {
            get { return _scale; }
            set
            {
                _scale = value;
                ClearBind();
            }
        }

        /// <summary>
        /// Gets or sets the maximum size, in bytes, of the data within the column.
        /// </summary>
        /// <value>The maximum size, in bytes, of the data within the column.
        /// The default value is inferred from the parameter value.</value>
        [DefaultValue(0)]
        [Category("Data")]
        public override int Size
        {
            get => _size;
            set
            {
                if (value < -1)
                    throw new ArgumentException($"Invalid parameter Size value '{value}'. The value must be greater than or equal to 0.");

                _size = value;
                ClearBind();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Data.DbType">DbType</see> of the parameter.
        /// </summary>
        /// <value>One of the <see cref="System.Data.DbType">DbType</see> values. The default is <b>Object</b>.</value>
        [DefaultValue(DbType.Object)]
        [Category("Data"), RefreshProperties(RefreshProperties.All)]
        public override DbType DbType
        {
            get
            {
                if (_dbType.HasValue) {
                    return _dbType.Value;
                }

                if (_value != null) {   // Infer from value
                    return TypeHandlerRegistry.ToDbType(_value.GetType());
                }

                return DbType.Object;
            }
            set
            {
                ClearBind();
                if (value == DbType.Object)
                {
                    _dbType = null;
                    _npgsqlDbType = null;
                }
                else
                {
                    _dbType = value;
                    _npgsqlDbType = TypeHandlerRegistry.ToNpgsqlDbType(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> of the parameter.
        /// </summary>
        /// <value>One of the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> values. The default is <b>Unknown</b>.</value>
        [DefaultValue(NpgsqlDbType.Unknown)]
        [Category("Data"), RefreshProperties(RefreshProperties.All)]
        public NpgsqlDbType NpgsqlDbType
        {
            get
            {
                if (_npgsqlDbType.HasValue) {
                    return _npgsqlDbType.Value;
                }

                if (_value != null) {   // Infer from value
                    return TypeHandlerRegistry.ToNpgsqlDbType(_value);
                }

                return NpgsqlDbType.Unknown;
            }
            set
            {
                if (value == NpgsqlDbType.Array)
                    throw new ArgumentOutOfRangeException(nameof(value), "Cannot set NpgsqlDbType to just Array, Binary-Or with the element type (e.g. Array of Box is NpgsqlDbType.Array | NpgsqlDbType.Box).");
                if (value == NpgsqlDbType.Range)
                    throw new ArgumentOutOfRangeException(nameof(value), "Cannot set NpgsqlDbType to just Range, Binary-Or with the element type (e.g. Range of integer is NpgsqlDbType.Range | NpgsqlDbType.Integer)");

                ClearBind();
                _npgsqlDbType = value;
                _dbType = TypeHandlerRegistry.ToDbType(value);
            }
        }

        /// <summary>
        /// Gets or sets The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <value>The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// The default is an empty string.</value>
        [DefaultValue("")]
        public override string ParameterName
        {
            get => _name;
            set
            {
                _name = value;
                if (value == null)
                {
                    _name = string.Empty;
                }
                // no longer prefix with : so that The name returned is The name set

                _name = _name.Trim();

                if (Collection != null)
                {
                    Collection.InvalidateHashLookups();
                    ClearBind();
                }
                AutoAssignedName = false;
            }
        }

        /// <summary>
        /// Gets or sets The name of the source column that is mapped to the
        /// DataSet and used for loading or
        /// returning the <see cref="Value">Value</see>.
        /// </summary>
        /// <value>The name of the source column that is mapped to the DataSet.
        /// The default is an empty string.</value>
        [DefaultValue("")]
        [Category("Data")]
        public override string SourceColumn { get; set; }

#if NET45 || NET451
        /// <summary>
        /// Gets or sets the <see cref="System.Data.DataRowVersion">DataRowVersion</see>
        /// to use when loading <see cref="NpgsqlParameter.Value">Value</see>.
        /// </summary>
        /// <value>One of the <see cref="System.Data.DataRowVersion">DataRowVersion</see> values.
        /// The default is <b>Current</b>.</value>
        [Category("Data"), DefaultValue(DataRowVersion.Current)]
        public override DataRowVersion SourceVersion { get; set; }
#endif

        /// <summary>
        /// Source column mapping.
        /// </summary>
        public override bool SourceColumnNullMapping { get; set; }

        /// <summary>
        /// Used in combination with NpgsqlDbType.Enum or NpgsqlDbType.Array | NpgsqlDbType.Enum to indicate the enum type.
        /// For other NpgsqlDbTypes, this field is not used.
        /// </summary>
        [Obsolete("Use the SpecificType property instead")]
        [PublicAPI]
        public Type EnumType
        {
            get => SpecificType;
            set => SpecificType = value;
        }

        /// <summary>
        /// Used in combination with NpgsqlDbType.Enum or NpgsqlDbType.Composite to indicate the specific enum or composite type.
        /// For other NpgsqlDbTypes, this field is not used.
        /// </summary>
        [PublicAPI]
        public Type SpecificType
        {
            get {
                if (_specificType != null)
                    return _specificType;

                // Try to infer type if NpgsqlDbType is Enum or has not been set
                if ((!_npgsqlDbType.HasValue || _npgsqlDbType == NpgsqlDbType.Enum) && _value != null)
                {
                    var type = _value.GetType();
                    if (type.GetTypeInfo().IsEnum)
                        return type;
                    if (type.IsArray && type.GetElementType().GetTypeInfo().IsEnum)
                        return type.GetElementType();
                }
                return null;
            }
            set => _specificType = value;
        }

        /// <summary>
        /// The collection to which this parameter belongs, if any.
        /// </summary>
#pragma warning disable CA2227
        [CanBeNull]
        public NpgsqlParameterCollection Collection { get; set; }
#pragma warning restore CA2227

        #endregion

        #region Internals

        /// <summary>
        /// The name scrubbed of any optional marker
        /// </summary>
        internal string CleanName
        {
            get
            {
                var name = ParameterName;
                if (name.Length > 0 && (name[0] == ':' || name[0] == '@'))
                {
                    return name.Substring(1);
                }
                return name;

            }
        }

        /// <summary>
        /// Returns whether this parameter has had its type set explicitly via DbType or NpgsqlDbType
        /// (and not via type inference)
        /// </summary>
        internal bool IsTypeExplicitlySet => _npgsqlDbType.HasValue || _dbType.HasValue;

        internal void ResolveHandler(TypeHandlerRegistry registry)
        {
            if (Handler != null) {
                return;
            }

            if (_npgsqlDbType.HasValue)
            {
                Handler = registry[_npgsqlDbType.Value, SpecificType];
            }
            else if (_dbType.HasValue)
            {
                Handler = registry[_dbType.Value];
            }
            else if (_value != null)
            {
                Handler = registry[_value];
            }
            else
            {
                throw new InvalidOperationException($"Parameter '{ParameterName}' must have its value set");
            }
        }

        internal void Bind(TypeHandlerRegistry registry)
        {
            ResolveHandler(registry);

            Debug.Assert(Handler != null);
            FormatCode = Handler.PreferTextWrite ? FormatCode.Text : FormatCode.Binary;
        }

        internal int ValidateAndGetLength()
        {
            if (_value == null)
                throw new InvalidCastException($"Parameter {ParameterName} must be set");
            if (_value is DBNull)
                return 0;

            var lengthCache = LengthCache;
            var len = Handler.ValidateAndGetLength(Value, ref lengthCache, this);
            LengthCache = lengthCache;
            return len;
        }

        internal Task WriteWithLength(WriteBuffer buf, bool async, CancellationToken cancellationToken)
            => Handler.WriteWithLength(Value, buf, LengthCache, this, async, cancellationToken);

        void ClearBind()
        {
            Handler = null;
        }

        /// <summary>
        /// Reset DBType.
        /// </summary>
        public override void ResetDbType()
        {
            //type_info = NpgsqlTypesHelper.GetNativeTypeInfo(typeof(String));
            _dbType = null;
            _npgsqlDbType = null;
            Value = Value;
            ClearBind();
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
                SourceColumn = SourceColumn,
#if NET45 || NET451
                SourceVersion = SourceVersion,
#endif
                _value = _value,
                _npgsqlValue = _npgsqlValue,
                SourceColumnNullMapping = SourceColumnNullMapping,
                AutoAssignedName = AutoAssignedName
            };
            return clone;
        }

#if NET45 || NET451
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif
        #endregion
    }
}
