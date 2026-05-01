using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
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
    internal DbType? _dbType;

    private protected string _name = string.Empty;
    object? _value;
    private protected bool _useSubStream;
    private protected Stream? _subStream;
    private protected string _sourceColumn;

    internal string TrimmedName { get; private protected set; } = PositionalName;
    internal const string PositionalName = "";

    IDbTypeResolver? _dbTypeResolver;
    private protected PgTypeInfo? TypeInfo { get; private set; }
    private protected PgConcreteTypeInfo? ConcreteTypeInfo { get; private set; }

    internal PgTypeId PgTypeId => ConcreteTypeInfo?.PgTypeId ?? default;

    internal DataFormat Format => _binding?.DataFormat ?? DataFormat.Binary;
    object? _providerWriteState;
    private protected PgValueBinding? _binding;

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
                DisposeBindingState();
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
            if (_dbType is { } dbType)
                return dbType;

            if (_dataTypeName is not null)
            {
                var dataTypeName = Internal.Postgres.DataTypeName.FromDisplayName(_dataTypeName);
                if (TryResolveDbType(dataTypeName, out var resolvedDbType))
                    return resolvedDbType;

                return dataTypeName.ToNpgsqlDbType()?.ToDbType() ?? DbType.Object;
            }

            if (_npgsqlDbType is { } npgsqlDbType)
                return npgsqlDbType.ToDbType();

            // Infer from value but don't cache
            // We pass ValueType here for the generic derived type, where we should respect T and not the runtime type.
            if (GetValueType(StaticValueType) is { } valueType)
                return GlobalTypeMapper.Instance.FindDataTypeName(valueType, ValueAsObject)?.ToNpgsqlDbType()?.ToDbType() ?? DbType.Object;

            return DbType.Object;
        }
        set
        {
            ResetTypeInfo();
            _dbType = value;
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

            var valueType = GetValueType(StaticValueType);
            if (_dbType is { } dbType)
            {
                if (TryResolveDbTypeDataTypeName(dbType, valueType, out var dataTypeName))
                    return NpgsqlDbTypeExtensions.ToNpgsqlDbType(dataTypeName) ?? NpgsqlDbType.Unknown;

                return dbType.ToNpgsqlDbType() ?? NpgsqlDbType.Unknown;
            }

            // Infer from value but don't cache
            // We pass ValueType here for the generic derived type, where we should respect T and not the runtime type.
            if (valueType is not null)
                return GlobalTypeMapper.Instance.FindDataTypeName(valueType, ValueAsObject)?.ToNpgsqlDbType() ?? NpgsqlDbType.Unknown;

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

            var valueType = GetValueType(StaticValueType);
            if (_dbType is { } dbType)
            {
                if (TryResolveDbTypeDataTypeName(dbType, valueType, out var dataTypeName))
                    return dataTypeName;

                var unqualifiedName = dbType.ToNpgsqlDbType()?.ToUnqualifiedDataTypeName();
                return unqualifiedName is null ? null : Internal.Postgres.DataTypeName.ValidatedName(
                    "pg_catalog." + unqualifiedName).UnqualifiedDisplayName;
            }

            // Infer from value but don't cache
            // We pass ValueType here for the generic derived type, where we should respect T and not the runtime type.
            if (valueType is not null)
                return GlobalTypeMapper.Instance.FindDataTypeName(valueType, ValueAsObject)?.DisplayName;

            return null;
        }
        set
        {
            ResetTypeInfo();
            _dataTypeName = value;
        }
    }

    // Value without DBNull to pass onto FindDataTypeName.
    object? ValueAsObject
    {
        get
        {
            var value = Value;
            return value is DBNull ? null : value;
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

            DisposeBindingState();
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

    /// The DBNull handling mode parameters apply at their own ADO.NET boundary.
    /// Compositional converters inherit this from BindContext / ProviderValueContext.
    internal const NestedObjectDbNullHandling ParameterDbNullHandling = NestedObjectDbNullHandling.Extended;

    private protected virtual Type StaticValueType => typeof(object);

    Type? GetValueType(Type staticValueType) => staticValueType != typeof(object) ? staticValueType : Value?.GetType();

    bool TryResolveDbType(DataTypeName dataTypeName, out DbType dbType)
    {
        if (_dbTypeResolver?.GetDbType(dataTypeName) is { } result)
        {
            dbType = result;
            return true;
        }

        dbType = default;
        return false;
    }

    bool TryResolveDbTypeDataTypeName(DbType dbType, Type? type, [NotNullWhen(true)]out string? normalizedDataTypeName)
    {
        if (_dbTypeResolver?.GetDataTypeName(dbType, type) is { } result)
        {
            normalizedDataTypeName = Internal.Postgres.DataTypeName.NormalizeName(result);
            return true;
        }

        normalizedDataTypeName = null;
        return false;
    }

    internal void SetOutputValue(NpgsqlDataReader reader, int ordinal)
    {
        // Set Value (not _value) so we also support object typed generic params.
        if (StaticValueType == typeof(object))
            Value = reader.GetValue(ordinal);
        else
            SetOutputTypedValue(reader, ordinal);
    }

    internal bool ShouldResetObjectTypeInfo(object? value)
    {
        var currentType = TypeInfo?.Type;
        if (currentType is null || value is null)
            return false;

        var valueType = value.GetType();
        // We don't want to reset the type info when the value is a DBNull, we're able to write it out with any type info.
        return valueType != typeof(DBNull) && currentType != valueType;
    }

    internal void GetResolutionInfo(out PgTypeInfo? typeInfo, out PgConcreteTypeInfo? concreteTypeInfo)
    {
        typeInfo = TypeInfo;
        concreteTypeInfo = ConcreteTypeInfo;
    }

    internal void SetResolutionInfo(PgTypeInfo typeInfo, PgConcreteTypeInfo concreteTypeInfo)
    {
        if (_binding is not null)
            DisposeBindingState();

        // Dispose any provider-produced _writeState against its current ConcreteTypeInfo before we
        // overwrite it — once reassigned, the restored ConcreteTypeInfo can't dispose state produced
        // by the about-to-be-discarded one.
        if (_providerWriteState is { } ws)
        {
            ConcreteTypeInfo?.DisposeWriteState(ws);
            _providerWriteState = null;
        }

        TypeInfo = typeInfo;
        ConcreteTypeInfo = concreteTypeInfo;
    }

    /// Attempt to resolve a type info based on available (postgres) type information on the parameter.
    /// When <paramref name="willBind"/> is false (e.g. SchemaOnly), any provider-produced write state is
    /// disposed immediately because no Bind call will follow to take ownership of it.
    internal void ResolveTypeInfo(PgSerializerOptions options, IDbTypeResolver? dbTypeResolver, bool willBind = true)
    {
        var typeInfo = TypeInfo;
        var staticValueType = StaticValueType;
        var previouslyResolved = ReferenceEquals(typeInfo?.Options, options);
        Debug.Assert(!previouslyResolved || ConcreteTypeInfo is not null);
        if (!previouslyResolved)
        {
            var valueType = GetValueType(staticValueType);

            string? dataTypeName = null;
            if (_dataTypeName is not null)
            {
                dataTypeName = Internal.Postgres.DataTypeName.NormalizeName(_dataTypeName);
            }
            else if (_npgsqlDbType is { } npgsqlDbType)
            {
                dataTypeName = npgsqlDbType.ToDataTypeName() ?? npgsqlDbType.ToUnqualifiedDataTypeNameOrThrow();
            }
            else if (_dbType is { } dbType)
            {
                if (dbTypeResolver is not null)
                {
                    _dbTypeResolver = dbTypeResolver;
                    if (dbTypeResolver.GetDataTypeName(dbType, valueType) is { } result)
                    {
                        dataTypeName = Internal.Postgres.DataTypeName.NormalizeName(result);
                    }
                }

                // Fall back to builtin mappings if there was no resolver, or it didn't produce a result.
                if (dataTypeName is null)
                {
                    dataTypeName = dbType.ToNpgsqlDbType()?.ToDataTypeName();
                    // If DbType.Object was specified we will only throw (see ThrowNoTypeInfo) if valueType is also null.
                    if (dataTypeName is null && dbType is not DbType.Object)
                        ThrowDbTypeNotSupported();
                }
            }

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

            if (pgTypeId is null && valueType is null)
            {
                ThrowNoTypeInfo();
                return;
            }

            // We treat object typed DBNull values as default info (we don't supply a type).
            // Unless we don't have a pgTypeId either, at which point we'll use an 'unspecified' PgTypeInfo to help us write a NULL.
            if (valueType == typeof(DBNull) && staticValueType == typeof(object))
            {
                TypeInfo = typeInfo = pgTypeId is null
                    ? options.UnspecifiedDBNullTypeInfo
                    : AdoSerializerHelpers.GetTypeInfoForWriting(type: null, pgTypeId, options, _npgsqlDbType);
            }
            else
            {
                TypeInfo = typeInfo = AdoSerializerHelpers.GetTypeInfoForWriting(valueType, pgTypeId, options, _npgsqlDbType);
            }
        }

        // This step isn't part of BindValue because we need to know the PgTypeId beforehand for things like SchemaOnly with null values.
        // We never reuse concrete type infos from providers across executions as a mutable value itself may influence the result.
        // TODO we could expose a property on a Converter/TypeInfo to indicate whether it's immutable, at that point we can reuse.
        if (!previouslyResolved || typeInfo is not PgConcreteTypeInfo)
        {
            Debug.Assert(typeInfo is not null);
            ResetTypeInfo(reresolve: true);

            object? providerWriteState = null;
            try
            {
                PgConcreteTypeInfo concrete;
                if (staticValueType == typeof(object))
                {
                    // Pull from Value (not _value) so we also support object typed generic params.
                    var value = Value;
                    concrete = typeInfo.MakeConcreteForValueAsObject(
                        new() { NestedObjectDbNullHandling = ParameterDbNullHandling },
                        value is DBNull ? null : value,
                        out providerWriteState);
                }
                else
                {
                    concrete = MakeConcreteTypeInfoForTypedValue(typeInfo, out providerWriteState);
                }

                // Skip the write barrier when the resolution produced the same concrete (e.g. cached provider results).
                if (!ReferenceEquals(concrete, ConcreteTypeInfo))
                    ConcreteTypeInfo = concrete;

                // Most cases it's null, don't bother taking the write barrier hit.
                if (providerWriteState is not null)
                    _providerWriteState = providerWriteState;
            }
            catch (Exception ex)
            {
                // MakeConcrete may have assigned _writeState already before throwing, dispose before we throw.
                if (providerWriteState is { } failedWs)
                {
                    typeInfo.DisposeWriteState(failedWs);
                    _providerWriteState = null;
                }
                ThrowWritingNotSupported(options, GetValueType(staticValueType), inner: ex);
            }

            // If no Bind follows (SchemaOnly), release the provider-produced state immediately.
            if ((!willBind || !ConcreteTypeInfo.SupportsWriting) && _providerWriteState is { } ws)
            {
                typeInfo.DisposeWriteState(ws);
                _providerWriteState = null;
            }

            if (!ConcreteTypeInfo.SupportsWriting)
                ThrowWritingNotSupported(options, GetValueType(staticValueType), resolved: true);
        }
        [DoesNotReturn]
        void ThrowWritingNotSupported(PgSerializerOptions options, Type? type, Exception? inner = null, bool resolved = false)
        {
            PgTypeId? pgTypeId = null;
            if (_npgsqlDbType is null && _dataTypeName is { } dataTypeName && options.DatabaseInfo.TryGetPostgresTypeByName(dataTypeName, out var pgType))
                pgTypeId = pgType.DataTypeName;
            AdoSerializerHelpers.ThrowWritingNotSupported(type, options, pgTypeId, _npgsqlDbType, ParameterName, inner, resolved);
        }

        void ThrowNoTypeInfo()
            => ThrowHelper.ThrowInvalidOperationException(
                $"Parameter '{(!string.IsNullOrEmpty(ParameterName) ? ParameterName : $"${Collection?.IndexOf(this) + 1}")}' must have either its DbType, NpgsqlDbType, DataTypeName or its Value set.");

        void ThrowDbTypeNotSupported()
            => ThrowHelper.ThrowNotSupportedException(
                $"The DbType '{_dbType}' isn't supported by Npgsql. There might be an Npgsql plugin with support for this DbType.");

        void ThrowNotSupported(string dataTypeName)
            => ThrowHelper.ThrowNotSupportedException(
                $"The data type name '{dataTypeName}'{(_npgsqlDbType is not null ? $", provided as NpgsqlDbType '{_npgsqlDbType}'," : null)} could not be found in the types that were loaded by Npgsql. " +
                $"Your database details or Npgsql type loading configuration may be incorrect. Alternatively your PostgreSQL installation might need to be upgraded, or an extension adding the missing data type might not have been installed.");
    }

    /// Bind the current value to the type info, truncate (if applicable), take its size, and do any final validation before writing.
    internal void Bind(out DataFormat format, out Size size, DataFormat? requiredFormat = null)
    {
        if (TypeInfo is null || ConcreteTypeInfo is null)
            ThrowHelper.ThrowInvalidOperationException($"Missing type info, {nameof(ResolveTypeInfo)} needs to be called before {nameof(Bind)}.");

        // We might call this twice, once during validation and once during WriteBind, only compute things once.
        // Bind is atomic *and* self-cleaning: the local binding is only committed to _binding
        // (and _writeState nulled) after every check passes, and any exception before commit disposes
        // the resolution-time _writeState ourselves so callers don't need to know about it.
        if (_binding is null)
        {
            if (_size > 0)
                HandleSizeTruncation(ConcreteTypeInfo);

            try
            {
                PgValueBinding binding;
                if (_useSubStream)
                {
                    binding = BindSubStream();
                }
                else if (StaticValueType == typeof(object))
                {
                    // Pull from Value so we also support object typed generic params.
                    var value = Value;
                    if (value is null)
                        ThrowHelper.ThrowInvalidOperationException($"Parameter '{ParameterName}' cannot be null, DBNull.Value should be used instead.");

                    binding = ConcreteTypeInfo.Converter.IsDbNullAsNestedObject(value, _providerWriteState, ParameterDbNullHandling)
                        ? new PgValueBinding(DataFormat.Binary, 0, null, _providerWriteState)
                        : ConcreteTypeInfo.BindParameterValueAsObject(value, _providerWriteState, ParameterDbNullHandling, requiredFormat);
                }
                else
                {
                    binding = BindTypedValue(ConcreteTypeInfo, _providerWriteState, formatPreference: requiredFormat);
                }

                if (requiredFormat is not null && binding.DataFormat != requiredFormat)
                    ThrowHelper.ThrowNotSupportedException($"Parameter '{ParameterName}' must be written in {requiredFormat} format, but does not support this format.");

                // Binding and ownership transfer of state happen together.
                _binding = binding;
                _providerWriteState = null;
            }
            catch (Exception ex)
            {
                if (_providerWriteState is { } ws)
                {
                    ConcreteTypeInfo.DisposeWriteState(ws);
                    _providerWriteState = null;
                }
                if (_subStream is not null)
                {
                    _subStream.Dispose();
                    _subStream = null;
                }
                _useSubStream = false;
                // Wrap with resolution context, which is easier than letting nested converters reconstruct the right context themselves.
                // Use the parameter's value-type logic so the message reflects the type that the caller passed, not what info was resolved.
                ThrowHelper.ThrowInvalidCastException(
                    $"Could not bind parameter '{ParameterName}' of CLR type '{GetValueType(StaticValueType)?.FullName ?? "null"}' to PostgreSQL type '{ConcreteTypeInfo.Options.GetDataTypeName(ConcreteTypeInfo.PgTypeId).DisplayName}'. See the inner exception for details.",
                    innerException: ex);
            }
        }
        else if (requiredFormat is not null && _binding.GetValueOrDefault().DataFormat != requiredFormat)
        {
            ThrowHelper.ThrowNotSupportedException($"Parameter '{ParameterName}' must be written in {requiredFormat} format, but does not support this format.");
        }

        format = Format;
        size = _binding.GetValueOrDefault().Size ?? -1;

        [MethodImpl(MethodImplOptions.NoInlining)]
        PgValueBinding BindSubStream()
        {
            // Pull from Value so we also support object typed generic params.
            var stream = (Stream?)Value;
            Debug.Assert(stream is not null, "_useSubStream should only be true if we had a value during HandleSizeTruncation");
            int subSize;
            if (stream.CanSeek)
            {
                var remaining = Math.Max(0, stream.Length - stream.Position);
                subSize = remaining < _size ? (int)remaining : _size;
                _subStream = new SubReadStream(stream, _size);
            }
            else
            {
                // TODO maybe we can move this IO.
                var buffer = new byte[_size];
                var read = stream.ReadAtLeast(buffer, _size, throwOnEndOfStream: false);
                subSize = Math.Min(_size, read);
                _subStream = new MemoryStream(buffer, 0, subSize);
            }
            return new(DataFormat.Binary, 0, subSize, null);
        }

        // Handle Size truncate behavior for a predetermined set of types and pg types.
        // Doesn't matter if we 'box' Value, all supported types are reference types.
        [MethodImpl(MethodImplOptions.NoInlining)]
        void HandleSizeTruncation(PgConcreteTypeInfo typeInfo)
        {
            var type = typeInfo.Type;
            if ((type != typeof(string) && type != typeof(char[]) && type != typeof(byte[]) && !type.IsAssignableTo(typeof(Stream))) || Value is not { } value)
                return;

            var dataTypeName = typeInfo.Options.GetDataTypeName(PgTypeId);
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
                    // Substream path abandons the resolver-produced state, we must dispose it here to prevent the no swap exception.
                    if (_providerWriteState is { } ws)
                    {
                        typeInfo.DisposeWriteState(ws);
                        _providerWriteState = null;
                    }
                    _useSubStream = true;
                }
            }
        }
    }

    internal async ValueTask Write(bool async, PgWriter writer, CancellationToken cancellationToken)
    {
        if (_binding is not { } binding)
        {
            ThrowHelper.ThrowInvalidOperationException("Missing type info or binding info.");
            return;
        }
        Debug.Assert(ConcreteTypeInfo is not null);

        try
        {
            if (writer.ShouldFlush(sizeof(int)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            var size = binding.Size?.Value ?? -1;
            writer.WriteInt32(size);
            writer.CommitAndResetTotal(sizeof(int));

            if (!binding.IsDbNullBinding)
            {
                if (_useSubStream)
                {
                    Debug.Assert(_subStream is not null);
                    if (async)
                        await _subStream.CopyToAsync(writer.GetStream(), cancellationToken).ConfigureAwait(false);
                    else
                        _subStream.CopyTo(writer.GetStream());
                    writer.CommitAndResetTotal(size);
                }
                else
                {
                    await writer.StartWrite(async, binding, cancellationToken).ConfigureAwait(false);
                    var typeInfo = ConcreteTypeInfo;
                    if (StaticValueType == typeof(object))
                    {
                        // Pull from Value so we also support object typed generic params.
                        var value = Value;
                        Debug.Assert(value is not null);
                        if (async)
                        {
                            await typeInfo.Converter.WriteAsObjectAsync(writer, value, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            typeInfo.Converter.WriteAsObject(writer, value);
                        }
                    }
                    else
                    {
                        await WriteTypedValue(async, typeInfo, writer, cancellationToken).ConfigureAwait(false);
                    }
                    writer.EndWrite(size);
                }
            }
        }
        finally
        {
            DisposeBindingState();
        }
    }

    private protected virtual PgConcreteTypeInfo MakeConcreteTypeInfoForTypedValue(PgTypeInfo typeInfo, out object? providerWriteState)
        => throw new NotSupportedException();

    private protected virtual PgValueBinding BindTypedValue(PgConcreteTypeInfo typeInfo, object? providerWriteState, DataFormat? formatPreference)
        => throw new NotSupportedException();

    private protected virtual ValueTask WriteTypedValue(bool async, PgConcreteTypeInfo typeInfo, PgWriter writer, CancellationToken cancellationToken)
        => throw new NotSupportedException();

    private protected virtual void SetOutputTypedValue(NpgsqlDataReader reader, int ordinal)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override void ResetDbType()
    {
        _dbType = null;
        _npgsqlDbType = null;
        _dataTypeName = null;
        ResetTypeInfo();
    }

    private protected void ResetTypeInfo(bool reresolve = false)
    {
        DisposeBindingState();

        // Dispose any provider-produced _writeState as well.
        if (_providerWriteState is { } ws)
        {
            ConcreteTypeInfo?.DisposeWriteState(ws);
            _providerWriteState = null;
        }

        if (!reresolve)
        {
            ConcreteTypeInfo = null;
            TypeInfo = null;
        }
    }

    private protected void DisposeBindingState()
    {
        try
        {
            if (_binding is not { } binding)
            {
                Debug.Assert(!_useSubStream && _subStream is null);
                return;
            }

            // Dispose write state first as it may hold a reference to _subStream.
            Debug.Assert(ConcreteTypeInfo is not null);
            Exception? disposalException = null;
            if (binding.WriteState is { } writeState)
            {
                try
                {
                    ConcreteTypeInfo.DisposeWriteState(writeState);
                }
                catch (Exception ex)
                {
                    disposalException = ex;
                }
            }

            if (_useSubStream)
            {
                Debug.Assert(_subStream is not null);
                try
                {
                    _subStream.Dispose();
                }
                catch (Exception ex) when (disposalException is not null)
                {
                    throw new AggregateException(disposalException, ex);
                }
            }

            if (disposalException is not null)
                ExceptionDispatchInfo.Throw(disposalException);
        }
        finally
        {
            _useSubStream = false;
            _subStream = null;
            _binding = null;
        }
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
            _dbType = _dbType,
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
