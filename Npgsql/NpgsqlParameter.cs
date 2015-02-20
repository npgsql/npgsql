// created on 18/5/2002 at 01:25

// Npgsql.NpgsqlParameter.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Resources;
using Npgsql.Localization;
using NpgsqlTypes;

#if WITHDESIGN
using Npgsql.Design;
#endif

namespace Npgsql
{
    ///<summary>
    /// This class represents a parameter to a command that will be sent to server
    ///</summary>
#if WITHDESIGN
    [TypeConverter(typeof(NpgsqlParameterConverter))]
#endif
    public sealed class NpgsqlParameter : DbParameter, ICloneable
    {
        // Fields to implement IDbDataParameter interface.
        byte _precision;
        byte _scale;
        int _size;

        // Fields to implement IDataParameter
        NpgsqlDbType? _npgsqlDbType;
        DbType? _dbType;
        Type _enumType;
        string _name = String.Empty;
        object _value;
        object _npgsqlValue;
        NpgsqlParameterCollection _collection;
        internal LengthCache LengthCache { get;  private set; }

        internal LengthCache GetOrCreateLengthCache(int capacity=0)
        {
            if (LengthCache == null) {
                LengthCache = capacity == 0 ? new LengthCache() : new LengthCache(capacity);
            }
            return LengthCache;
        }

        internal bool IsBound { get; private set; }
        internal TypeHandler Handler { get; private set; }
        internal FormatCode FormatCode { get; private set; }
        internal uint TypeOID { get; private set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see> class.
        /// </summary>
        public NpgsqlParameter()
        {
            SourceColumn = String.Empty;
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
        public NpgsqlParameter(String parameterName, object value) : this()
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
            : this(parameterName, parameterType, 0, String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="System.Data.DbType">DbType</see> values.</param>
        public NpgsqlParameter(string parameterName, DbType parameterType)
            : this(parameterName, parameterType, 0, String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> values.</param>
        /// <param name="size">The length of the parameter.</param>
        public NpgsqlParameter(string parameterName, NpgsqlDbType parameterType, int size)
            : this(parameterName, parameterType, size, String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="parameterType">One of the <see cref="System.Data.DbType">DbType</see> values.</param>
        /// <param name="size">The length of the parameter.</param>
        public NpgsqlParameter(string parameterName, DbType parameterType, int size)
            : this(parameterName, parameterType, size, String.Empty)
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

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        /// <value>An <see cref="System.Object">Object</see> that is the value of the parameter.
        /// The default value is null.</value>
        [TypeConverter(typeof(StringConverter)), Category("Data")]
        public override object Value
        {
            get
            {
                return _value;
            } // [TODO] Check and validate data type.
            set
            {
                ClearBind();
                _value = value;
                _npgsqlValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        /// <value>An <see cref="System.Object">Object</see> that is the value of the parameter.
        /// The default value is null.</value>
        [TypeConverter(typeof(StringConverter)), Category("Data")]
        public object NpgsqlValue
        {
            get { return _npgsqlValue; }
            set {
                ClearBind();
                _value = value;
                _npgsqlValue = value;
            }
        }

        public override bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is input-only,
        /// output-only, bidirectional, or a stored procedure return value parameter.
        /// </summary>
        /// <value>One of the <see cref="System.Data.ParameterDirection">ParameterDirection</see>
        /// values. The default is <b>Input</b>.</value>
        [Category("Data"), DefaultValue(ParameterDirection.Input)]
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
        [Category("Data"), DefaultValue((Byte)0)]
        public byte Precision
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
        [Category("Data"), DefaultValue((Byte)0)]
        public byte Scale
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
        [Category("Data"), DefaultValue(0)]
        public override int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                ClearBind();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Data.DbType">DbType</see> of the parameter.
        /// </summary>
        /// <value>One of the <see cref="System.Data.DbType">DbType</see> values. The default is <b>Object</b>.</value>
        [Category("Data"), RefreshProperties(RefreshProperties.All), DefaultValue(DbType.Object)]
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
                _dbType = value;
                _npgsqlDbType = TypeHandlerRegistry.ToNpgsqlDbType(value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> of the parameter.
        /// </summary>
        /// <value>One of the <see cref="NpgsqlTypes.NpgsqlDbType">NpgsqlDbType</see> values. The default is <b>Unknown</b>.</value>
        [Category("Data"), RefreshProperties(RefreshProperties.All), DefaultValue(NpgsqlDbType.Unknown)]
        public NpgsqlDbType NpgsqlDbType
        {
            get
            {
                if (_npgsqlDbType.HasValue) {
                    return _npgsqlDbType.Value;
                }

                if (_value != null) {   // Infer from value
                    return TypeHandlerRegistry.ToNpgsqlDbType(_value.GetType());
                }

                return NpgsqlDbType.Unknown;
            }
            set
            {
                if (value == NpgsqlDbType.Array) {
                    throw new ArgumentOutOfRangeException("value", L10N.ParameterTypeIsOnlyArray);
                }
                if (value == NpgsqlDbType.Range) {
                    throw new ArgumentOutOfRangeException("value", "Cannot set NpgsqlDbType to just Range, Binary-Or with the element type (e.g. Range of integer is NpgsqlDbType.Range | NpgsqlDbType.Integer)");
                }
                Contract.EndContractBlock();

                ClearBind(); 
                _npgsqlDbType = value;
                _dbType = TypeHandlerRegistry.ToDbType(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter accepts null values.
        /// </summary>
        /// <value><b>true</b> if null values are accepted; otherwise, <b>false</b>. The default is <b>false</b>.</value>

#if WITHDESIGN
        [EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false), DefaultValue(false), DesignOnly(true)]
#endif

        /// <summary>
        /// Gets or sets The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// </summary>
        /// <value>The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.
        /// The default is an empty string.</value>
        [DefaultValue("")]
        public override string ParameterName
        {
            get { return _name; }
            set
            {
                _name = value;
                if (value == null)
                {
                    _name = String.Empty;
                }
                // no longer prefix with : so that The name returned is The name set

                _name = _name.Trim();

                if (_collection != null)
                {
                    _collection.InvalidateHashLookups();
                    ClearBind();
                }
            }
        }

                /// <summary>
        /// Gets or sets The name of the source column that is mapped to the
        /// <see cref="System.Data.DataSet">DataSet</see> and used for loading or
        /// returning the <see cref="NpgsqlParameter.Value">Value</see>.
        /// </summary>
        /// <value>The name of the source column that is mapped to the
        /// <see cref="System.Data.DataSet">DataSet</see>. The default is an empty string.</value>
        [Category("Data"), DefaultValue("")]
        public override String SourceColumn { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Data.DataRowVersion">DataRowVersion</see>
        /// to use when loading <see cref="NpgsqlParameter.Value">Value</see>.
        /// </summary>
        /// <value>One of the <see cref="System.Data.DataRowVersion">DataRowVersion</see> values.
        /// The default is <b>Current</b>.</value>
        [Category("Data"), DefaultValue(DataRowVersion.Current)]
        public override DataRowVersion SourceVersion { get; set; }

        /// <summary>
        /// Source column mapping.
        /// </summary>
        public override bool SourceColumnNullMapping { get; set; }

        #endregion

        /// <summary>
        /// The name scrubbed of any optional marker
        /// </summary>
        internal string CleanName
        {
            get
            {
                string name = ParameterName;
                if (name[0] == ':' || name[0] == '@')
                {
                    return name.Length > 1 ? name.Substring(1) : string.Empty;
                }
                return name;

            }
        }

        /// <summary>
        /// The collection to which this parameter belongs, if any.
        /// </summary>
        public NpgsqlParameterCollection Collection
        {
            get { return _collection; }

            internal set
            {
                _collection = value;
                ClearBind();
            }
        }

        internal bool IsNull
        {
            get { return _value == null || _value is DBNull; }
        }

        /// <summary>
        /// Used in combination with NpgsqlDbType.Enum or NpgsqlDbType.Array | NpgsqlDbType.Enum to indicate the enum type.
        /// For other NpgsqlDbTypes, this field is not used.
        /// </summary>
        public Type EnumType
        {
            get
            {
                if (_enumType != null)
                    return _enumType;

                // Try to infer type if NpgsqlDbType is Enum or has not been set
                if ((!_npgsqlDbType.HasValue || _npgsqlDbType == NpgsqlDbType.Enum) && _value != null)
                {
                    var type = _value.GetType();
                    if (type.IsEnum)
                        return type;
                    if (type.IsArray && type.GetElementType().IsEnum)
                        return type.GetElementType();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    if (!value.IsEnum)
                        throw new ArgumentException("The type is not an enum type", "value");
                    _enumType = value;
                }
            }
        }

        internal void ResolveHandler(TypeHandlerRegistry registry)
        {
            if (Handler != null) {
                return;
            }

            if (_npgsqlDbType.HasValue)
            {
                Handler = registry[_npgsqlDbType.Value, EnumType];
            }
            else if (_dbType.HasValue)
            {
                Handler = registry[_dbType.Value];
            }
            else if (!IsNull)
            {
                Handler = registry[_value];
            }
            else
            {
                Handler = registry.UnrecognizedTypeHandler;
            }
        }

        internal void Bind(TypeHandlerRegistry registry)
        {
            ResolveHandler(registry);

            Contract.Assert(Handler != null);
            FormatCode = Handler.PreferTextWrite ? FormatCode.Text : FormatCode.Binary;

            IsBound = true;
        }

        internal int ValidateAndGetLength()
        {
            if (IsNull) {
                return 0;
            }

            // No length caching for simple types
            var asSimpleWriter = Handler as ISimpleTypeWriter;
            if (asSimpleWriter != null) {
                return asSimpleWriter.ValidateAndGetLength(Value);
            }

            var asChunkingWriter = Handler as IChunkingTypeWriter;
            Contract.Assert(asChunkingWriter != null);
            return asChunkingWriter.ValidateAndGetLength(Value, this);
        }

        internal void ClearLengthCache()
        {
            if (LengthCache != null) { LengthCache.Clear(); }
        }

        internal void RewindLengthCache()
        {
            if (LengthCache != null) { LengthCache.Rewind(); }
        }

        void ClearBind()
        {
            IsBound = false;
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

        internal bool IsInputDirection
        {
            get { return Direction == ParameterDirection.InputOutput || Direction == ParameterDirection.Input; }
        }

        internal bool IsOutputDirection
        {
            get { return Direction == ParameterDirection.InputOutput || Direction == ParameterDirection.Output; }
        }

        /// <summary>
        /// Creates a new <see cref="NpgsqlParameter">NpgsqlParameter</see> that
        /// is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="NpgsqlParameter">NpgsqlParameter</see> that is a copy of this instance.</returns>
        public NpgsqlParameter Clone()
        {
            // use fields instead of properties
            // to avoid auto-initializing something like type_info
            var clone = new NpgsqlParameter();
            clone._precision = _precision;
            clone._scale = _scale;
            clone._size = _size;
            clone._dbType = _dbType;
            clone._npgsqlDbType = _npgsqlDbType;
            clone._enumType = _enumType;
            clone.Direction = Direction;
            clone.IsNullable = IsNullable;
            clone._name = _name;
            clone.SourceColumn = SourceColumn;
            clone.SourceVersion = SourceVersion;
            clone._value = _value;
            clone._npgsqlValue = _npgsqlValue;
            clone.SourceColumnNullMapping = SourceColumnNullMapping;

            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
