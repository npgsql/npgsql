using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using Npgsql.Util;
using NpgsqlTypes;

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

    private protected NpgsqlDbType? _npgsqlDbType;
    private protected string? _dataTypeName;

    private protected string _name = string.Empty;
    private protected object? _value;
    private protected string _sourceColumn;

    internal string TrimmedName { get; private protected set; } = PositionalName;
    internal const string PositionalName = "";

    private protected object? _writeState;
    internal PgTypeInfo? TypeInfo { get; private set; }
    internal PgTypeId PgTypeId { get; set; }
    internal PgConverter? Converter { get; private set; }
    internal Size? ConvertedSize { get; set; }
    internal bool AsObject { get; private protected set; }
    internal FormatCode Format { get; private protected set; }

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
            if (value == null || _value?.GetType() != value.GetType())
                ResetBinding();
            _value = value;
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
            if (_npgsqlDbType is { } npgsqlDbType)
                return npgsqlDbType.ToDbType();

            if (_dataTypeName is not null)
                return PostgresTypes.DataTypeName.FromDisplayName(_dataTypeName).ToNpgsqlDbType()?.ToDbType() ?? DbType.Object;

            // Infer from value but don't cache
            if (Value is not null)
                // We pass ValueType here for the generic derived type, where we should always respect T and not the runtime type.
                return GlobalTypeMapper.Instance.TryGetDataTypeName(ValueType!, Value)?.ToNpgsqlDbType()?.ToDbType() ?? DbType.Object;

            return DbType.Object;
        }
        set
        {
            TypeInfo = null;
            _npgsqlDbType = value == DbType.Object
                ? null
                : value.ToNpgsqlDbType()
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
                return PostgresTypes.DataTypeName.FromDisplayName(_dataTypeName).ToNpgsqlDbType() ?? NpgsqlDbType.Unknown;

            // Infer from value but don't cache
            if (Value is not null)
                // We pass ValueType here for the generic derived type (NpgsqlParameter<T>) where we should always respect T and not the runtime type.
                return GlobalTypeMapper.Instance.TryGetDataTypeName(ValueType!, Value)?.ToNpgsqlDbType() ?? NpgsqlDbType.Unknown;

            return NpgsqlDbType.Unknown;
        }
        set
        {
            if (value == NpgsqlDbType.Array)
                throw new ArgumentOutOfRangeException(nameof(value), "Cannot set NpgsqlDbType to just Array, Binary-Or with the element type (e.g. Array of Box is NpgsqlDbType.Array | NpgsqlDbType.Box).");
            if (value == NpgsqlDbType.Range)
                throw new ArgumentOutOfRangeException(nameof(value), "Cannot set NpgsqlDbType to just Range, Binary-Or with the element type (e.g. Range of integer is NpgsqlDbType.Range | NpgsqlDbType.Integer)");

            TypeInfo = null;
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

            if (_npgsqlDbType is { } npgsqlDbType)
                return npgsqlDbType.TryToUnqualifiedDataTypeName();

            // Infer from value but don't cache
            if (Value is not null)
                // We pass ValueType here for the generic derived type, where we should always respect T and not the runtime type.
                return GlobalTypeMapper.Instance.TryGetDataTypeName(ValueType!, Value)?.DisplayName;

            return null;
        }
        set
        {
            _dataTypeName = value;
            TypeInfo = null;
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
            TypeInfo = null;
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
            TypeInfo = null;
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
            TypeInfo = null;
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

    internal virtual Type? ValueType => _value?.GetType();

    internal void Bind(PgSerializerOptions options)
    {
        var previouslyBound = TypeInfo?.Options == options;
        if (!previouslyBound)
        {
            var valueType = ValueType;
            // The only exceptional type that we don't want a 'random' default for is byte[] which should always map to bytea.
            if (valueType == typeof(byte[]) && _dataTypeName is null && _npgsqlDbType is null)
                _dataTypeName = DataTypeNames.Bytea.Value;

            PgTypeId? pgTypeId = null;
            if (_npgsqlDbType is { } npgsqlDbType)
                pgTypeId = npgsqlDbType.TryToDataTypeName() switch
                {
                    { } name => options.GetCanonicalTypeId(name),
                    // Handle plugin types via lookup.
                    null => options.ToCanonicalTypeId(GetRepresentationalOrDefault(npgsqlDbType.ToUnqualifiedDataTypeName()))
                };
            else if (_dataTypeName is not null)
            {
                var fqDataTypeName = PostgresTypes.DataTypeName.FromDisplayName(_dataTypeName);
                pgTypeId = fqDataTypeName.ToNpgsqlDbType()?.TryToDataTypeName() switch
                {
                    { } name => options.GetCanonicalTypeId(name),
                    null => options.ToCanonicalTypeId(GetRepresentationalOrDefault(_dataTypeName))
                };
            }

            // We treat object typed DBNull values as default info.
            // For ValueType == DBNull we would still use the type (though don't ask why you would construct a NpgsqlParamter<DBNull>)

            if (valueType is null || _value is DBNull)
            {
                if (pgTypeId is null && valueType is null)
                {
                    var parameterName = !string.IsNullOrEmpty(ParameterName) ? ParameterName : $"${Collection?.IndexOf(this) + 1}";
                    ThrowHelper.ThrowInvalidOperationException(
                        $"Parameter '{parameterName}' must have either its NpgsqlDbType or its DataTypeName or its Value set");
                    return;
                }

                TypeInfo = options.GetDefaultTypeInfo(pgTypeId ?? options.ToCanonicalTypeId(options.PgUnknownType)) ?? throw new NotSupportedException(
                    $"Couldn't find converter for parameter with {(_npgsqlDbType is not null
                        ? $"NpgsqlDbType '{_npgsqlDbType}'" : $" DataTypeName '{_dataTypeName}'")}.");
            }
            else
            {
                TypeInfo = options.GetTypeInfo(valueType, pgTypeId) switch
                {
                    null when typeof(IEnumerable).IsAssignableFrom(valueType) && !typeof(IList).IsAssignableFrom(valueType) && valueType != typeof(string)
                        => throw new NotSupportedException(
                            "IEnumerable parameters are not supported, pass an array or List instead."),
                    null => throw new NotSupportedException(
                        $"Couldn't find converter for parameter of type {valueType}{(_npgsqlDbType is not null
                            ? $" and NpgsqlDbType '{_npgsqlDbType}'" : pgTypeId is not null ? $" and DataTypeName '{_dataTypeName}'" : "")}."),
                    var typeInfo => typeInfo
                };
            }
        }

        // This step isn't part of BindFormatAndLength because we need to know the PgTypeId beforehand for things like SchemaOnly with null values.
        // We never reuse resolutions for resolvers across executions as a mutable value itself may even influence the result.
        // TODO we could expose a property on a Converter/TypeInfo to indicate whether it's immutable, at that point we can reuse.
        if (!previouslyBound || TypeInfo is PgResolverTypeInfo)
        {
            var resolution = GetResolution(TypeInfo!);
            Converter = resolution.Converter;
            PgTypeId = resolution.PgTypeId;
        }

        PostgresType GetRepresentationalOrDefault(string dataTypeName)
        {
            var type = options.TypeCatalog.GetPostgresTypeByName(dataTypeName);
            return type.GetRepresentationalType() ?? type;
        }
    }

    private protected virtual PgConverterResolution GetResolution(PgTypeInfo typeInfo) => typeInfo.GetObjectResolution(_value);

    internal virtual void BindFormatAndLength()
    {
        var value = _value;
        if (value is null)
            ThrowHelper.ThrowInvalidCastException("Parameter '{0}' must be set", ParameterName);

        var info = TypeInfo!.BindObject(new(Converter!, PgTypeId), value, out _writeState, out var dataFormat);
        if (info?.BufferRequirement.Kind is SizeKind.Unknown)
            throw new NotImplementedException();

        ConvertedSize = info?.BufferRequirement;
        AsObject = info?.AsObject ?? false;
        Format = dataFormat is DataFormat.Binary ? FormatCode.Binary : FormatCode.Text;
    }

    internal async ValueTask Write(bool async, PgWriter writer, CancellationToken cancellationToken)
    {
        if (TypeInfo is null)
            throw new InvalidOperationException("Not bound yet");

        switch (ConvertedSize)
        {
        case { Kind: SizeKind.Exact } size:
        {
            try
            {
                writer.Current = new() { Format = Format is FormatCode.Binary ? DataFormat.Binary : DataFormat.Text, Size = size, WriteState = _writeState };

                // TODO check write buffer requirement instead of flushing right away for size.Value.
                if (writer.ShouldFlush(sizeof(int) + size.Value))
                {
                    if (async)
                        await writer.FlushAsync(cancellationToken);
                    else
                        writer.Flush();
                }

                writer.WriteInt32(size.Value);
                await WriteValue(writer, Converter!, async, cancellationToken);
                writer.Commit(size.Value + sizeof(int));
            }
            finally
            {
                if (_writeState is not null)
                    TypeInfo.DisposeWriteState(_writeState);
            }

            break;
        }
        case { Kind: SizeKind.Unknown }:
            throw new NotImplementedException("Should not end up here, yet");
        default:
            if (writer.ShouldFlush(sizeof(int)))
            {
                if (async)
                    await writer.FlushAsync(cancellationToken);
                else
                    writer.Flush();
            }
            writer.WriteInt32(-1);
            writer.Commit(sizeof(int));
            break;
        }
    }

    private protected virtual ValueTask WriteValue(PgWriter writer, PgConverter converter, bool async, CancellationToken cancellationToken)
    {
        if (async)
            return converter.WriteAsObjectAsync(writer, Value!, cancellationToken);

        converter.WriteAsObject(writer, Value!);
        return new();
    }

    /// <inheritdoc />
    public override void ResetDbType()
    {
        _npgsqlDbType = null;
        _dataTypeName = null;
        ResetBinding();
        ResetResolution();
    }

    void ResetBinding() => TypeInfo = null;

    void ResetResolution()
    {
        Converter = null;
        PgTypeId = default;
        ConvertedSize = null;
        AsObject = false;
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
