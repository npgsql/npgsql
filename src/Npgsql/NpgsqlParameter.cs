using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql;

///<summary>
/// This class represents a parameter to a command that will be sent to server
///</summary>
public class NpgsqlParameter : DbParameter, IDbDataParameter, ICloneable
{
    #region Fields and Properties

    private protected byte _precision;
    private protected byte _scale;
    private protected int _size;

    // ReSharper disable InconsistentNaming
    private protected NpgsqlDbType? _npgsqlDbType;
    private protected string? _dataTypeName;
    // ReSharper restore InconsistentNaming

    private protected  string _name = string.Empty;
    private protected  object? _value;
    private protected  string _sourceColumn;

    internal string TrimmedName { get; private protected set; } = PositionalName;
    internal const string PositionalName = ""; 
        
    /// <summary>
    /// Can be used to communicate a value from the validation phase to the writing phase.
    /// To be used by type handlers only.
    /// </summary>
    public object? ConvertedValue { get; set; }

    internal NpgsqlLengthCache? LengthCache { get; set; }

    internal NpgsqlTypeHandler? Handler { get; set; }

    internal FormatCode FormatCode { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/> class.
    /// </summary>
    public NpgsqlParameter()
    {
        _sourceColumn = string.Empty;
        Direction = ParameterDirection.Input;
        SourceVersion = DataRowVersion.Current;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/> class with the parameter name and a value.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="value">The value of the <see cref="NpgsqlParameter"/>.</param>
    /// <remarks>
    /// <p>
    /// When you specify an <see cref="object"/> in the value parameter, the <see cref="System.Data.DbType"/> is
    /// inferred from the CLR type.
    /// </p>
    /// <p>
    /// When using this constructor, you must be aware of a possible misuse of the constructor which takes a <see cref="DbType"/>
    /// parameter. This happens when calling this constructor passing an int 0 and the compiler thinks you are passing a value of
    /// <see cref="DbType"/>. Use <see cref="Convert.ToInt32(object)"/> for example to have compiler calling the correct constructor.
    /// </p>
    /// </remarks>
    public NpgsqlParameter(string? parameterName, object? value)
        : this()
    {
        ParameterName = parameterName;
        // ReSharper disable once VirtualMemberCallInConstructor
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/> class with the parameter name and the data type.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="parameterType">One of the <see cref="NpgsqlTypes.NpgsqlDbType"/> values.</param>
    public NpgsqlParameter(string? parameterName, NpgsqlDbType parameterType)
        : this(parameterName, parameterType, 0, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/>.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="parameterType">One of the <see cref="System.Data.DbType"/> values.</param>
    public NpgsqlParameter(string? parameterName, DbType parameterType)
        : this(parameterName, parameterType, 0, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/>.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="parameterType">One of the <see cref="NpgsqlTypes.NpgsqlDbType"/> values.</param>
    /// <param name="size">The length of the parameter.</param>
    public NpgsqlParameter(string? parameterName, NpgsqlDbType parameterType, int size)
        : this(parameterName, parameterType, size, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/>.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="parameterType">One of the <see cref="System.Data.DbType"/> values.</param>
    /// <param name="size">The length of the parameter.</param>
    public NpgsqlParameter(string? parameterName, DbType parameterType, int size)
        : this(parameterName, parameterType, size, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/>
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="parameterType">One of the <see cref="NpgsqlTypes.NpgsqlDbType"/> values.</param>
    /// <param name="size">The length of the parameter.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    public NpgsqlParameter(string? parameterName, NpgsqlDbType parameterType, int size, string? sourceColumn)
    {
        ParameterName = parameterName;
        NpgsqlDbType = parameterType;
        _size = size;
        _sourceColumn = sourceColumn ?? string.Empty;
        Direction = ParameterDirection.Input;
        SourceVersion = DataRowVersion.Current;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/>.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="parameterType">One of the <see cref="System.Data.DbType"/> values.</param>
    /// <param name="size">The length of the parameter.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    public NpgsqlParameter(string? parameterName, DbType parameterType, int size, string? sourceColumn)
    {
        ParameterName = parameterName;
        DbType = parameterType;
        _size = size;
        _sourceColumn = sourceColumn ?? string.Empty;
        Direction = ParameterDirection.Input;
        SourceVersion = DataRowVersion.Current;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/>.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="parameterType">One of the <see cref="NpgsqlTypes.NpgsqlDbType"/> values.</param>
    /// <param name="size">The length of the parameter.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    /// <param name="direction">One of the <see cref="System.Data.ParameterDirection"/> values.</param>
    /// <param name="isNullable">
    /// <see langword="true"/> if the value of the field can be <see langword="null"/>, otherwise <see langword="false"/>.
    /// </param>
    /// <param name="precision">
    /// The total number of digits to the left and right of the decimal point to which <see cref="Value"/> is resolved.
    /// </param>
    /// <param name="scale">The total number of decimal places to which <see cref="Value"/> is resolved.</param>
    /// <param name="sourceVersion">One of the <see cref="System.Data.DataRowVersion"/> values.</param>
    /// <param name="value">An <see cref="object"/> that is the value of the <see cref="NpgsqlParameter"/>.</param>
    public NpgsqlParameter(string parameterName, NpgsqlDbType parameterType, int size, string? sourceColumn,
        ParameterDirection direction, bool isNullable, byte precision, byte scale,
        DataRowVersion sourceVersion, object value)
    {
        ParameterName = parameterName;
        Size = size;
        _sourceColumn = sourceColumn ?? string.Empty;
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
    /// Initializes a new instance of the <see cref="NpgsqlParameter"/>.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="parameterType">One of the <see cref="System.Data.DbType"/> values.</param>
    /// <param name="size">The length of the parameter.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    /// <param name="direction">One of the <see cref="System.Data.ParameterDirection"/> values.</param>
    /// <param name="isNullable">
    /// <see langword="true"/> if the value of the field can be <see langword="null"/>, otherwise <see langword="false"/>.
    /// </param>
    /// <param name="precision">
    /// The total number of digits to the left and right of the decimal point to which <see cref="Value"/> is resolved.
    /// </param>
    /// <param name="scale">The total number of decimal places to which <see cref="Value"/> is resolved.</param>
    /// <param name="sourceVersion">One of the <see cref="System.Data.DataRowVersion"/> values.</param>
    /// <param name="value">An <see cref="object"/> that is the value of the <see cref="NpgsqlParameter"/>.</param>
    public NpgsqlParameter(string parameterName, DbType parameterType, int size, string? sourceColumn,
        ParameterDirection direction, bool isNullable, byte precision, byte scale,
        DataRowVersion sourceVersion, object value)
    {
        ParameterName = parameterName;
        Size = size;
        _sourceColumn = sourceColumn ?? string.Empty;
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
    /// Gets or sets The name of the <see cref="NpgsqlParameter"/>.
    /// </summary>
    /// <value>The name of the <see cref="NpgsqlParameter"/>.
    /// The default is an empty string.</value>
    [AllowNull, DefaultValue("")]
    public sealed override string ParameterName
    {
        get => _name;
        set
        {
            if (Collection is not null)
                Collection.ChangeParameterName(this, value);
            else 
                ChangeParameterName(value);
        }
    }

    internal void ChangeParameterName(string? value)
    {
        if (value == null)
            _name = TrimmedName = PositionalName;
        else if (value.Length > 0 && (value[0] == ':' || value[0] == '@'))
            TrimmedName = (_name = value).Substring(1);
        else
            _name = TrimmedName = value;
    }

    internal bool IsPositional => ParameterName.Length == 0;

    #endregion Name

    #region Value

    /// <inheritdoc />
    [TypeConverter(typeof(StringConverter)), Category("Data")]
    public override object? Value
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
    /// <value>
    /// An <see cref="object" /> that is the value of the parameter.
    /// The default value is <see langword="null" />.
    /// </value>
    [Category("Data")]
    [TypeConverter(typeof(StringConverter))]
    public object? NpgsqlValue
    {
        get => Value;
        set => Value = value;
    }

    #endregion Value

    #region Type

    /// <summary>
    /// Gets or sets the <see cref="System.Data.DbType"/> of the parameter.
    /// </summary>
    /// <value>One of the <see cref="System.Data.DbType"/> values. The default is <see cref="object"/>.</value>
    [DefaultValue(DbType.Object)]
    [Category("Data"), RefreshProperties(RefreshProperties.All)]
    public sealed override DbType DbType
    {
        get
        {
            if (_npgsqlDbType.HasValue)
                return GlobalTypeMapper.NpgsqlDbTypeToDbType(_npgsqlDbType.Value);

            if (_dataTypeName is not null)
                return GlobalTypeMapper.NpgsqlDbTypeToDbType(GlobalTypeMapper.DataTypeNameToNpgsqlDbType(_dataTypeName));

            if (Value is not null) // Infer from value but don't cache
            {
                return GlobalTypeMapper.Instance.TryResolveMappingByValue(Value, out var mapping)
                    ? mapping.DbType
                    : DbType.Object;
            }

            return DbType.Object;
        }
        set
        {
            Handler = null;
            _npgsqlDbType = value == DbType.Object
                ? null
                : GlobalTypeMapper.DbTypeToNpgsqlDbType(value)
                  ?? throw new NotSupportedException($"The parameter type DbType.{value} isn't supported by PostgreSQL or Npgsql");
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="NpgsqlTypes.NpgsqlDbType"/> of the parameter.
    /// </summary>
    /// <value>One of the <see cref="NpgsqlTypes.NpgsqlDbType"/> values. The default is <see cref="NpgsqlTypes.NpgsqlDbType"/>.</value>
    [DefaultValue(NpgsqlDbType.Unknown)]
    [Category("Data"), RefreshProperties(RefreshProperties.All)]
    [DbProviderSpecificTypeProperty(true)]
    public NpgsqlDbType NpgsqlDbType
    {
        [RequiresUnreferencedCode("The NpgsqlDbType getter isn't trimming-safe")]
        get
        {
            if (_npgsqlDbType.HasValue)
                return _npgsqlDbType.Value;

            if (_dataTypeName is not null)
                return GlobalTypeMapper.DataTypeNameToNpgsqlDbType(_dataTypeName);

            if (Value is not null) // Infer from value
            {
                return GlobalTypeMapper.Instance.TryResolveMappingByValue(Value, out var mapping)
                    ? mapping.NpgsqlDbType ?? NpgsqlDbType.Unknown
                    : throw new NotSupportedException("Can't infer NpgsqlDbType for type " + Value.GetType());
            }

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
        }
    }

    /// <summary>
    /// Used to specify which PostgreSQL type will be sent to the database for this parameter.
    /// </summary>
    public string? DataTypeName
    {
        get
        {
            if (_dataTypeName != null)
                return _dataTypeName;

            if (_npgsqlDbType.HasValue)
                return GlobalTypeMapper.NpgsqlDbTypeToDataTypeName(_npgsqlDbType.Value);

            if (Value != null) // Infer from value
            {
                return GlobalTypeMapper.Instance.TryResolveMappingByValue(Value, out var mapping)
                    ? mapping.DataTypeName
                    : null;
            }

            return null;
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
    /// Gets or sets the maximum number of digits used to represent the <see cref="Value"/> property.
    /// </summary>
    /// <value>
    /// The maximum number of digits used to represent the <see cref="Value"/> property.
    /// The default value is 0, which indicates that the data provider sets the precision for <see cref="Value"/>.</value>
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
    /// Gets or sets the number of decimal places to which <see cref="Value"/> is resolved.
    /// </summary>
    /// <value>The number of decimal places to which <see cref="Value"/> is resolved. The default is 0.</value>
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
    [AllowNull, DefaultValue("")]
    [Category("Data")]
    public sealed override string SourceColumn
    {
        get => _sourceColumn;
        set => _sourceColumn = value ?? string.Empty;
    }

    /// <inheritdoc />
    [Category("Data"), DefaultValue(DataRowVersion.Current)]
    public sealed override DataRowVersion SourceVersion { get; set; }

    /// <inheritdoc />
    public sealed override bool SourceColumnNullMapping { get; set; }

#pragma warning disable CA2227
    /// <summary>
    /// The collection to which this parameter belongs, if any.
    /// </summary>
    public NpgsqlParameterCollection? Collection { get; set; }
#pragma warning restore CA2227

    /// <summary>
    /// The PostgreSQL data type, such as int4 or text, as discovered from pg_type.
    /// This property is automatically set if parameters have been derived via
    /// <see cref="NpgsqlCommandBuilder.DeriveParameters"/> and can be used to
    /// acquire additional information about the parameters' data type.
    /// </summary>
    public PostgresType? PostgresType { get; internal set; }

    #endregion Other Properties

    #region Internals

    internal virtual void ResolveHandler(TypeMapper typeMapper)
    {
        if (Handler is not null)
            return;

        if (_npgsqlDbType.HasValue)
            Handler = typeMapper.ResolveByNpgsqlDbType(_npgsqlDbType.Value);
        else if (_dataTypeName is not null)
            Handler = typeMapper.ResolveByDataTypeName(_dataTypeName);
        else if (_value is not null)
            Handler = typeMapper.ResolveByValue(_value);
        else
        {
            var parameterName = !string.IsNullOrEmpty(ParameterName) ? ParameterName : $"${Collection?.IndexOf(this) + 1}";
            throw new InvalidOperationException($"Parameter '{parameterName}' must have either its NpgsqlDbType or its DataTypeName or its Value set");
        }
    }

    internal void Bind(TypeMapper typeMapper)
    {
        ResolveHandler(typeMapper);
        FormatCode = Handler!.PreferTextWrite ? FormatCode.Text : FormatCode.Binary;
    }

    internal virtual int ValidateAndGetLength()
    {
        if (_value is DBNull)
            return 0;
        if (_value == null)
            throw new InvalidCastException($"Parameter {ParameterName} must be set");

        var lengthCache = LengthCache;
        var len = Handler!.ValidateObjectAndGetLength(_value, ref lengthCache, this);
        LengthCache = lengthCache;
        return len;
    }

    internal virtual Task WriteWithLength(NpgsqlWriteBuffer buf, bool async, CancellationToken cancellationToken = default)
        => Handler!.WriteObjectWithLength(_value!, buf, LengthCache, this, async, cancellationToken);

    /// <inheritdoc />
    public override void ResetDbType()
    {
        _npgsqlDbType = null;
        _dataTypeName = null;
        Handler = null;
    }

    internal bool IsInputDirection => Direction == ParameterDirection.InputOutput || Direction == ParameterDirection.Input;

    internal bool IsOutputDirection => Direction == ParameterDirection.InputOutput || Direction == ParameterDirection.Output;

    #endregion

    #region Clone

    /// <summary>
    /// Creates a new <see cref="NpgsqlParameter"/> that is a copy of the current instance.
    /// </summary>
    /// <returns>A new <see cref="NpgsqlParameter"/> that is a copy of this instance.</returns>
    public NpgsqlParameter Clone() => CloneCore();

    private protected virtual NpgsqlParameter CloneCore() =>
        // use fields instead of properties
        // to avoid auto-initializing something like type_info
        new()
        {
            _precision = _precision,
            _scale = _scale,
            _size = _size,
            _npgsqlDbType = _npgsqlDbType,
            _dataTypeName = _dataTypeName,
            Direction = Direction,
            IsNullable = IsNullable,
            _name = _name,
            TrimmedName = TrimmedName,
            SourceColumn = SourceColumn,
            SourceVersion = SourceVersion,
            _value = _value,
            SourceColumnNullMapping = SourceColumnNullMapping,
        };

    object ICloneable.Clone() => Clone();

    #endregion
}