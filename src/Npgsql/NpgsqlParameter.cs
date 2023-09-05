using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
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
    object? _value;
    private protected bool _useSubStream;
    private protected SubReadStream? _subStream;
    private protected string _sourceColumn;

    internal string TrimmedName { get; private protected set; } = PositionalName;
    internal const string PositionalName = "";

    internal PgTypeInfo? TypeInfo { get; private set; }

    internal PgTypeId PgTypeId { get; set; }
    internal PgConverter? Converter { get; private set; }

    internal DataFormat Format { get; private protected set; }
    private protected Size? WriteSize { get; set; }
    private protected bool AsObject => TypeInfo!.IsBoxing || TypeInfo.Type != ValueType || _useSubStream;
    private protected object? _writeState;
    private protected Size _bufferRequirement;

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
                ResetTypeInfo();
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
                return Internal.Postgres.DataTypeName.FromDisplayName(_dataTypeName).ToNpgsqlDbType()?.ToDbType() ?? DbType.Object;

            // Infer from value but don't cache
            if (Value is not null)
                // We pass ValueType here for the generic derived type, where we should always respect T and not the runtime type.
                return GlobalTypeMapper.Instance.TryGetDataTypeName(ValueType!, Value)?.ToNpgsqlDbType()?.ToDbType() ?? DbType.Object;

            return DbType.Object;
        }
        set
        {
            ResetTypeInfo();
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
                return Internal.Postgres.DataTypeName.FromDisplayName(_dataTypeName).ToNpgsqlDbType() ?? NpgsqlDbType.Unknown;

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

            ResetTypeInfo();
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

            // Map it to a display name.
            if (_npgsqlDbType is { } npgsqlDbType)
            {
                var unqualifiedName = npgsqlDbType.ToUnqualifiedDataTypeName();
                return unqualifiedName is null ? null : Internal.Postgres.DataTypeName.ValidatedName(
                    "pg_catalog." + unqualifiedName).UnqualifiedDisplayName;
            }

            // Infer from value but don't cache
            if (Value is not null)
                // We pass ValueType here for the generic derived type, where we should always respect T and not the runtime type.
                return GlobalTypeMapper.Instance.TryGetDataTypeName(ValueType!, Value)?.DisplayName;

            return null;
        }
        set
        {
            ResetTypeInfo();
            _dataTypeName = value;
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
        set => _precision = value;
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
        set => _scale = value;
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

            ResetBindingInfo();
            _size = value;
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

    bool IsGenericParameter => _value is null && ValueType is not null;

    /// Attempt to resolve a type info based on available (postgres) type information on the parameter.
    internal void ResolveTypeInfo(PgSerializerOptions options)
    {
        var previouslyBound = TypeInfo?.Options == options;
        if (!previouslyBound)
        {
            var staticValueType = typeof(object);
            var valueType = ValueType;
            // We do runtime type lookup for generic parameters if the type is object.
            if (IsGenericParameter)
            {
                staticValueType = valueType; // This will contain the strongly typed T.
                valueType = staticValueType == typeof(object) ? Value?.GetType() : staticValueType;
            }

            string? dataTypeName = null;
            DataTypeName? builtinDataTypeName = null;
            if (_npgsqlDbType is { } npgsqlDbType)
            {
                dataTypeName = npgsqlDbType.ToUnqualifiedDataTypeNameOrThrow();
                builtinDataTypeName = npgsqlDbType.ToDataTypeName();
            }
            else if (_dataTypeName is not null)
            {
                dataTypeName = Internal.Postgres.DataTypeName.NormalizeName(_dataTypeName);
                // If we can find a match in an NpgsqlDbType we known we're dealing with a fully qualified built-in data type name.
                builtinDataTypeName = NpgsqlDbTypeExtensions.ToNpgsqlDbType(dataTypeName)?.ToDataTypeName();
            }

            var pgTypeId = dataTypeName is null
                ? (PgTypeId?)null
                : TryGetRepresentationalTypeId(builtinDataTypeName ?? dataTypeName, out var id)
                    ? id
                    : throw new NotSupportedException(_npgsqlDbType is not null
                        ? $"The NpgsqlDbType '{_npgsqlDbType}' isn't present in your database. You may need to install an extension or upgrade to a newer version."
                        : $"The data type name '{builtinDataTypeName ?? dataTypeName}' isn't present in your database. You may need to install an extension or upgrade to a newer version.");

            // We treat object typed DBNull values as default info.
            if (valueType is null || (staticValueType == typeof(object) && valueType == typeof(DBNull)))
            {
                if (pgTypeId is null && valueType is null)
                {
                    var parameterName = !string.IsNullOrEmpty(ParameterName) ? ParameterName : $"${Collection?.IndexOf(this) + 1}";
                    ThrowHelper.ThrowInvalidOperationException(
                        $"Parameter '{parameterName}' must have either its NpgsqlDbType or its DataTypeName or its Value set");
                    return;
                }

                TypeInfo = options.GetDefaultTypeInfo(pgTypeId ?? options.ToCanonicalTypeId(options.UnknownPgType))
                           ?? throw new NotSupportedException($"No configured writer was found for parameter with {
                               (_npgsqlDbType is not null ? $"NpgsqlDbType '{_npgsqlDbType}'" : $" DataTypeName '{_dataTypeName}'")}.");
            }
            else
            {
                TypeInfo = options.GetTypeInfo(valueType, pgTypeId)
                           ?? throw new NotSupportedException($"No configured writer was found for parameter of type {valueType}{(_npgsqlDbType is not null
                                   ? $" and NpgsqlDbType '{_npgsqlDbType}'" : pgTypeId is not null ? $" and DataTypeName '{_dataTypeName}'" : "")}.");
            }
        }

        // This step isn't part of BindValue because we need to know the PgTypeId beforehand for things like SchemaOnly with null values.
        // We never reuse resolutions for resolvers across executions as a mutable value itself may influence the result.
        // TODO we could expose a property on a Converter/TypeInfo to indicate whether it's immutable, at that point we can reuse.
        if (!previouslyBound || TypeInfo is PgResolverTypeInfo)
        {
            ResetConverterResolution();
            var resolution = ResolveConverter(TypeInfo!);
            Converter = resolution.Converter;
            PgTypeId = resolution.PgTypeId;
        }

        bool TryGetRepresentationalTypeId(string dataTypeName, out PgTypeId pgTypeId)
        {
            if (options.TypeCatalog.TryGetPostgresTypeByName(dataTypeName, out var pgType))
            {
                pgTypeId = options.ToCanonicalTypeId(pgType.GetRepresentationalType());
                return true;
            }

            pgTypeId = default;
            return false;
        }
    }

    private protected virtual PgConverterResolution ResolveConverter(PgTypeInfo typeInfo) => typeInfo.GetObjectResolution(_value);

    /// Bind the current value to the type info, truncate (if applicable), take its size, and do any final validation before writing.
    internal void Bind(out DataFormat format, out Size size)
    {
        if (TypeInfo is null)
            ThrowHelper.ThrowInvalidOperationException("Missing type info.");

        if (!TypeInfo.SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Cannot write values for parameters of type '{TypeInfo.Type}' and postgres type '{TypeInfo.Options.TypeCatalog.GetDataTypeName(PgTypeId).DisplayName}'.");

        // We might call this twice, once during validation and once during WriteBind, only compute things once.
        if (WriteSize is not null)
        {
            format = Format;
            size = WriteSize.Value;
            return;
        }

        // Handle Size truncate behavior for a predetermined set of types and pg types.
        // Doesn't matter if we 'box' value, all supported types are reference types.
        if (_size > 0 && Converter!.TypeToConvert is var type &&
            (type == typeof(string) || type == typeof(char[]) || type == typeof(byte[]) || type == typeof(Stream)) &&
            Value is { } value)
        {
            var dataTypeName = TypeInfo!.Options.GetDataTypeName(PgTypeId);
            if (dataTypeName == DataTypeNames.Text || dataTypeName == DataTypeNames.Varchar || dataTypeName == DataTypeNames.Bpchar)
            {
                if (value is string s && s.Length > _size)
                    Value = s.Substring(0, _size);
                else if (value is char[] chars && chars.Length > _size)
                {
                    var truncated = new char[_size];
                    Array.Copy(chars, truncated, _size);
                    Value = truncated;
                }
            }
            else if (dataTypeName == DataTypeNames.Bytea)
            {
                if (value is byte[] bytes && bytes.Length > _size)
                {
                    var truncated = new byte[_size];
                    Array.Copy(bytes, truncated, _size);
                    Value = truncated;
                }
                else if (value is Stream)
                    _useSubStream = true;
            }
        }

        BindCore();
        format = Format;
        size = WriteSize!.Value;
    }

    private protected virtual void BindCore(bool allowNullReference = false)
    {
        // Pull from Value so we also support object typed generic params.
        var value = Value;
        if (value is null && !allowNullReference)
            ThrowHelper.ThrowInvalidOperationException($"Parameter '{ParameterName}' cannot be null, DBNull.Value should be used instead.");

        if (_useSubStream && value is not null)
            value = _subStream = new SubReadStream((Stream)value, _size);

        if (TypeInfo!.BindObject(Converter!, value, out var size, out _writeState, out var dataFormat) is { } info)
        {
            WriteSize = size;
            _bufferRequirement = info.BufferRequirement;
        }
        else
        {
            WriteSize = -1;
            _bufferRequirement = default;
        }
        Format = dataFormat;
    }

    internal async ValueTask Write(bool async, PgWriter writer, CancellationToken cancellationToken)
    {
        if (WriteSize is not { } writeSize)
        {
            ThrowHelper.ThrowInvalidOperationException("Missing type info or binding info.");
            return;
        }

        try
        {
            if (writer.ShouldFlush(sizeof(int)))
                await writer.Flush(async, cancellationToken);

            writer.WriteInt32(writeSize.Value);
            if (writeSize.Value is -1)
            {
                writer.Commit(sizeof(int));
                return;
            }

            var current = new ValueMetadata
            {
                Format = Format,
                BufferRequirement = _bufferRequirement,
                Size = writeSize,
                WriteState = _writeState
            };
            await writer.BeginWrite(async, current, cancellationToken).ConfigureAwait(false);
            await WriteValue(async, writer, cancellationToken);
            writer.Commit(writeSize.Value + sizeof(int));
        }
        finally
        {
            ResetBindingInfo();
        }
    }

    private protected virtual ValueTask WriteValue(bool async, PgWriter writer, CancellationToken cancellationToken)
    {
        // Pull from Value so we also support AsObject base calls from generic parameters.
        var value = (_useSubStream ? _subStream : Value)!;
        if (async)
            return Converter!.WriteAsObjectAsync(writer, value, cancellationToken);

        Converter!.WriteAsObject(writer, value);
        return new();
    }

    /// <inheritdoc />
    public override void ResetDbType()
    {
        _npgsqlDbType = null;
        _dataTypeName = null;
        ResetTypeInfo();
    }

    void ResetTypeInfo()
    {
        TypeInfo = null;
        ResetConverterResolution();
    }

    void ResetConverterResolution()
    {
        Converter = null;
        PgTypeId = default;
        ResetBindingInfo();
    }

    void ResetBindingInfo()
    {
        if (_writeState is not null)
            TypeInfo?.DisposeWriteState(_writeState);
        if (_useSubStream)
        {
            _useSubStream = false;
            _subStream?.Dispose();
            _subStream = null;
        }
        WriteSize = null;
        Format = default;
        _bufferRequirement = default;
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
