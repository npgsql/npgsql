using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
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

    internal NpgsqlDbType? _npgsqlDbType;
    internal string? _dataTypeName;

    private protected string _name = string.Empty;
    object? _value;
    private protected bool _useSubStream;
    private protected SubReadStream? _subStream;
    private protected string _sourceColumn;

    internal string TrimmedName { get; private protected set; } = PositionalName;
    internal const string PositionalName = "";

    private protected PgTypeInfo? TypeInfo { get; private set; }

    internal PgTypeId PgTypeId { get; private set; }
    private protected PgConverter? Converter { get; private set; }

    internal DataFormat Format { get; private protected set; }
    private protected Size? WriteSize { get; set; }
    private protected object? _writeState;
    private protected Size _bufferRequirement;
    private protected bool _asObject;

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
        if (value is null)
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
            if (ShouldResetObjectTypeInfo(value))
                ResetTypeInfo();
            else
                ResetBindingInfo();
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
                // We pass ValueType here for the generic derived type, where we should respect T and not the runtime type.
                return GlobalTypeMapper.Instance.FindDataTypeName(GetValueType(StaticValueType)!, Value)?.ToNpgsqlDbType()?.ToDbType() ?? DbType.Object;

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
        get
        {
            if (_npgsqlDbType.HasValue)
                return _npgsqlDbType.Value;

            if (_dataTypeName is not null)
                return Internal.Postgres.DataTypeName.FromDisplayName(_dataTypeName).ToNpgsqlDbType() ?? NpgsqlDbType.Unknown;

            // Infer from value but don't cache
            if (Value is not null)
                // We pass ValueType here for the generic derived type (NpgsqlParameter<T>) where we should respect T and not the runtime type.
                return GlobalTypeMapper.Instance.FindDataTypeName(GetValueType(StaticValueType)!, Value)?.ToNpgsqlDbType() ?? NpgsqlDbType.Unknown;

            return NpgsqlDbType.Unknown;
        }
        set
        {
            if (value == NpgsqlDbType.Array)
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(value), "Cannot set NpgsqlDbType to just Array, Binary-Or with the element type (e.g. Array of Box is NpgsqlDbType.Array | NpgsqlDbType.Box).");
            if (value == NpgsqlDbType.Range)
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(value), "Cannot set NpgsqlDbType to just Range, Binary-Or with the element type (e.g. Range of integer is NpgsqlDbType.Range | NpgsqlDbType.Integer)");

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
                // We pass ValueType here for the generic derived type, where we should respect T and not the runtime type.
                return GlobalTypeMapper.Instance.FindDataTypeName(GetValueType(StaticValueType)!, Value)?.DisplayName;

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

    /// <inheritdoc />
    [DefaultValue(0)]
    [Category("Data")]
    public sealed override int Size
    {
        get => _size;
        set
        {
            if (value < -1)
                ThrowHelper.ThrowArgumentException($"Invalid parameter Size value '{value}'. The value must be greater than or equal to 0.");

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

    /// <summary>
    /// The collection to which this parameter belongs, if any.
    /// </summary>
    public NpgsqlParameterCollection? Collection { get; set; }

    /// <summary>
    /// The PostgreSQL data type, such as int4 or text, as discovered from pg_type.
    /// This property is automatically set if parameters have been derived via
    /// <see cref="NpgsqlCommandBuilder.DeriveParameters"/> and can be used to
    /// acquire additional information about the parameters' data type.
    /// </summary>
    public PostgresType? PostgresType { get; internal set; }

    #endregion Other Properties

    #region Internals

    private protected virtual Type StaticValueType => typeof(object);

    Type? GetValueType(Type staticValueType) => staticValueType != typeof(object) ? staticValueType : Value?.GetType();

    internal bool ShouldResetObjectTypeInfo(object? value)
    {
        var currentType = TypeInfo?.Type;
        if (currentType is null || value is null)
            return false;

        var valueType = value.GetType();
        // We don't want to reset the type info when the value is a DBNull, we're able to write it out with any type info.
        return valueType != typeof(DBNull) && currentType != valueType;
    }

    internal void GetResolutionInfo(out PgTypeInfo? typeInfo, out PgConverter? converter, out PgTypeId pgTypeId)
    {
        typeInfo = TypeInfo;
        converter = Converter;
        pgTypeId = PgTypeId;
    }

    internal void SetResolutionInfo(PgTypeInfo typeInfo, PgConverter converter, PgTypeId pgTypeId)
    {
        if (WriteSize is not null)
            ResetBindingInfo();

        TypeInfo = typeInfo;
        Converter = converter;
        PgTypeId = pgTypeId;
    }

    /// Attempt to resolve a type info based on available (postgres) type information on the parameter.
    internal void ResolveTypeInfo(PgSerializerOptions options)
    {
        var typeInfo = TypeInfo;
        var previouslyResolved = ReferenceEquals(typeInfo?.Options, options);
        if (!previouslyResolved)
        {
            var dataTypeName =
                _npgsqlDbType is { } npgsqlDbType
                    ? npgsqlDbType.ToDataTypeName() ?? npgsqlDbType.ToUnqualifiedDataTypeNameOrThrow()
                    : _dataTypeName is not null
                        ? Internal.Postgres.DataTypeName.NormalizeName(_dataTypeName)
                        : null;

            PgTypeId? pgTypeId = null;
            if (dataTypeName is not null)
            {
                if (!options.DatabaseInfo.TryGetPostgresTypeByName(dataTypeName, out var pgType))
                {
                    ThrowNotSupported(dataTypeName);
                    return;
                }

                pgTypeId = options.ToCanonicalTypeId(pgType.GetRepresentationalType());
            }

            var unspecifiedDBNull = false;
            var valueType = StaticValueType;
            if (valueType == typeof(object))
            {
                valueType = Value?.GetType();
                if (valueType is null && pgTypeId is null)
                {
                    ThrowNoTypeInfo();
                    return;
                }

                // We treat object typed DBNull values as default info.
                // Unless we don't have a pgTypeId either, at which point we'll use an 'unspecified' PgTypeInfo to help us write a NULL.
                if (valueType == typeof(DBNull))
                {
                    if (pgTypeId is null)
                    {
                        unspecifiedDBNull = true;
                        typeInfo = options.UnspecifiedDBNullTypeInfo;
                    }
                    else
                        valueType = null;
                }
            }

            if (!unspecifiedDBNull)
                typeInfo = AdoSerializerHelpers.GetTypeInfoForWriting(valueType, pgTypeId, options, _npgsqlDbType);

            TypeInfo = typeInfo;
        }

        // This step isn't part of BindValue because we need to know the PgTypeId beforehand for things like SchemaOnly with null values.
        // We never reuse resolutions for resolvers across executions as a mutable value itself may influence the result.
        // TODO we could expose a property on a Converter/TypeInfo to indicate whether it's immutable, at that point we can reuse.
        if (!previouslyResolved || typeInfo!.IsResolverInfo)
        {
            ResetBindingInfo();
            var resolution = ResolveConverter(typeInfo!);
            Converter = resolution.Converter;
            PgTypeId = resolution.PgTypeId;
        }

        void ThrowNoTypeInfo()
            => ThrowHelper.ThrowInvalidOperationException(
                $"Parameter '{(!string.IsNullOrEmpty(ParameterName) ? ParameterName : $"${Collection?.IndexOf(this) + 1}")}' must have either its NpgsqlDbType or its DataTypeName or its Value set.");

        void ThrowNotSupported(string dataTypeName)
        {
            ThrowHelper.ThrowNotSupportedException(_npgsqlDbType is not null
                ? $"The NpgsqlDbType '{_npgsqlDbType}' isn't present in your database. You may need to install an extension or upgrade to a newer version."
                : $"The data type name '{dataTypeName}' isn't present in your database. You may need to install an extension or upgrade to a newer version.");
        }
    }

    // Pull from Value so we also support object typed generic params.
    private protected virtual PgConverterResolution ResolveConverter(PgTypeInfo typeInfo)
    {
        _asObject = true;
        return typeInfo.GetObjectResolution(Value);
    }

    /// Bind the current value to the type info, truncate (if applicable), take its size, and do any final validation before writing.
    internal void Bind(out DataFormat format, out Size size, DataFormat? requiredFormat = null)
    {
        if (TypeInfo is null)
            ThrowHelper.ThrowInvalidOperationException($"Missing type info, {nameof(ResolveTypeInfo)} needs to be called before {nameof(Bind)}.");

        if (!TypeInfo.SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Cannot write values for parameters of type '{TypeInfo.Type}' and postgres type '{TypeInfo.Options.DatabaseInfo.GetDataTypeName(PgTypeId).DisplayName}'.");

        // We might call this twice, once during validation and once during WriteBind, only compute things once.
        if (WriteSize is null)
        {
            if (_size > 0)
                HandleSizeTruncation();

            BindCore(requiredFormat);
        }

        format = Format;
        size = WriteSize!.Value;
        if (requiredFormat is not null && format != requiredFormat)
            ThrowHelper.ThrowNotSupportedException($"Parameter '{ParameterName}' must be written in {requiredFormat} format, but does not support this format.");

        // Handle Size truncate behavior for a predetermined set of types and pg types.
        // Doesn't matter if we 'box' Value, all supported types are reference types.
        [MethodImpl(MethodImplOptions.NoInlining)]
        void HandleSizeTruncation()
        {
            var type = Converter!.TypeToConvert;
            if ((type != typeof(string) && type != typeof(char[]) && type != typeof(byte[]) && type != typeof(Stream)) || Value is not { } value)
                return;

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
                {
                    _asObject = true;
                    _useSubStream = true;
                }
            }
        }
    }

    private protected virtual void BindCore(DataFormat? formatPreference, bool allowNullReference = false)
    {
        // Pull from Value so we also support object typed generic params.
        var value = Value;
        if (value is null && !allowNullReference)
            ThrowHelper.ThrowInvalidOperationException($"Parameter '{ParameterName}' cannot be null, DBNull.Value should be used instead.");

        if (_useSubStream && value is not null)
            value = _subStream = new SubReadStream((Stream)value, _size);

        if (TypeInfo!.BindObject(Converter!, value, out var size, out _writeState, out var dataFormat, formatPreference) is { } info)
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
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

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
            await WriteValue(async, writer, cancellationToken).ConfigureAwait(false);
            writer.Commit(writeSize.Value + sizeof(int));
        }
        finally
        {
            ResetBindingInfo();
        }
    }

    private protected virtual ValueTask WriteValue(bool async, PgWriter writer, CancellationToken cancellationToken)
    {
        // Pull from Value so we also support base calls from generic parameters.
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

    private protected void ResetTypeInfo()
    {
        TypeInfo = null;
        _asObject = false;
        Converter = null;
        PgTypeId = default;
        ResetBindingInfo();
    }

    private protected void ResetBindingInfo()
    {
        if (WriteSize is null)
        {
            Debug.Assert(_writeState == default && _useSubStream == default && Format == default && _bufferRequirement == default);
            return;
        }

        if (_writeState is not null)
        {
            TypeInfo?.DisposeWriteState(_writeState);
            _writeState = null;
        }
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
