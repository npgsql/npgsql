using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Npgsql.Internal.Postgres;
using Npgsql.NameTranslation;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public sealed class PgSerializerOptions
{
    /// <summary>
    /// Used by GetSchema to be able to attempt to resolve all type catalog types without exceptions.
    /// </summary>
    [field: ThreadStatic]
    internal static bool IntrospectionCaller { get; set; }

    readonly PgTypeInfoResolverChain _resolverChain;
    readonly Func<string>? _timeZoneProvider;
    IPgTypeInfoResolver? _typeInfoResolver;
    object? _typeInfoCache;

    internal PgSerializerOptions(NpgsqlDatabaseInfo databaseInfo, PgTypeInfoResolverChain? resolverChain = null, Func<string>? timeZoneProvider = null)
    {
        _resolverChain = resolverChain ?? new();
        _timeZoneProvider = timeZoneProvider;
        DatabaseInfo = databaseInfo;
        UnspecifiedDBNullTypeInfo = new(this, new Converters.Internal.VoidConverter(), DataTypeName.Unspecified, unboxedType: typeof(DBNull));
    }

    internal PgTypeInfo UnspecifiedDBNullTypeInfo { get; }

    PostgresType? _textPgType;
    internal PgTypeId TextPgTypeId => ToCanonicalTypeId(_textPgType ??= DatabaseInfo.GetPostgresType(DataTypeNames.Text));

    // Used purely for type mapping, where we don't have a full set of types but resolvers might know enough.
    readonly bool _introspectionInstance;
    internal bool IntrospectionMode
    {
        get => _introspectionInstance || IntrospectionCaller;
        init => _introspectionInstance = value;
    }

    /// Whether options should return a portable identifier (data type name) to prevent any generated id (oid) confusion across backends, this comes with a perf penalty.
    internal bool PortableTypeIds { get; init; }
    internal NpgsqlDatabaseInfo DatabaseInfo { get; }

    public string TimeZone => _timeZoneProvider?.Invoke() ?? throw new NotSupportedException("TimeZone was not configured.");
    public Encoding TextEncoding { get; init; } = Encoding.UTF8;
    public IPgTypeInfoResolver TypeInfoResolver
    {
        get => _typeInfoResolver ??= new ChainTypeInfoResolver(_resolverChain);
        internal init => _typeInfoResolver = value;
    }
    public bool EnableDateTimeInfinityConversions { get; init; } = true;

    public ArrayNullabilityMode ArrayNullabilityMode { get; init; } = ArrayNullabilityMode.Never;
    public INpgsqlNameTranslator DefaultNameTranslator { get; init; } = NpgsqlSnakeCaseNameTranslator.Instance;

    public static bool IsWellKnownTextType(Type type)
    {
        type = type.IsValueType ? Nullable.GetUnderlyingType(type) ?? type : type;
        return Array.IndexOf([
            typeof(string), typeof(char),
            typeof(char[]), typeof(ReadOnlyMemory<char>), typeof(ArraySegment<char>),
            typeof(byte[]), typeof(ReadOnlyMemory<byte>)
        ], type) != -1 || typeof(Stream).IsAssignableFrom(type);
    }

    internal bool RangesEnabled => _resolverChain.RangesEnabled;
    internal bool MultirangesEnabled => _resolverChain.MultirangesEnabled;
    internal bool ArraysEnabled => _resolverChain.ArraysEnabled;

    // We don't verify the kind of pgTypeId we get, it'll throw if it's incorrect.
    // It's up to the caller to call GetCanonicalTypeId if they want to use an oid instead of a DataTypeName.
    // This also makes it easier to realize it should be a cached value if infos for different CLR types are requested for the same
    // pgTypeId. Effectively it should be 'impossible' to get the wrong kind via any PgConverterOptions api which is what this is mainly
    // for.
    PgTypeInfo? GetTypeInfoCore(Type? type, PgTypeId? pgTypeId)
        => PortableTypeIds
            ? ((TypeInfoCache<DataTypeName>)(_typeInfoCache ??= new TypeInfoCache<DataTypeName>(this))).GetOrAddInfo(type, pgTypeId?.DataTypeName)
            : ((TypeInfoCache<Oid>)(_typeInfoCache ??= new TypeInfoCache<Oid>(this))).GetOrAddInfo(type, pgTypeId?.Oid);

    internal PgTypeInfo? GetTypeInfoInternal(Type? type, PgTypeId? pgTypeId)
        => GetTypeInfoCore(type, pgTypeId);

    public PgTypeInfo? GetDefaultTypeInfo(Type type)
        => GetTypeInfoCore(type, null);

    public PgTypeInfo? GetDefaultTypeInfo(PgTypeId pgTypeId)
        => GetTypeInfoCore(null, GetCanonicalTypeId(pgTypeId));

    public PgTypeInfo? GetTypeInfo(Type type, PgTypeId pgTypeId)
        => GetTypeInfoCore(type, GetCanonicalTypeId(pgTypeId));

    // If a given type id is in the opposite form than what was expected it will be mapped according to the requirement.
    internal PgTypeId GetCanonicalTypeId(PgTypeId pgTypeId)
        => PortableTypeIds ? DatabaseInfo.GetDataTypeName(pgTypeId) : DatabaseInfo.GetOid(pgTypeId);

    // If a given type id is in the opposite form than what was expected it will be mapped according to the requirement.
    internal PgTypeId ToCanonicalTypeId(PostgresType pgType)
        => PortableTypeIds ? pgType.DataTypeName : (Oid)pgType.OID;

    public PgTypeId GetArrayTypeId(PgTypeId elementTypeId)
    {
        // Static affordance to help the global type mapper.
        if (PortableTypeIds && elementTypeId.IsDataTypeName)
            return elementTypeId.DataTypeName.ToArrayName();

        return ToCanonicalTypeId(DatabaseInfo.GetPostgresType(elementTypeId).Array
                                 ?? throw new NotSupportedException("Cannot resolve array type id"));
    }

    public PgTypeId GetArrayElementTypeId(PgTypeId arrayTypeId)
    {
        // Static affordance to help the global type mapper.
        if (PortableTypeIds && arrayTypeId.IsDataTypeName && arrayTypeId.DataTypeName.UnqualifiedNameSpan.StartsWith("_".AsSpan(), StringComparison.Ordinal))
            return new DataTypeName(arrayTypeId.DataTypeName.Schema + arrayTypeId.DataTypeName.UnqualifiedNameSpan.Slice(1).ToString());

        return ToCanonicalTypeId((DatabaseInfo.GetPostgresType(arrayTypeId) as PostgresArrayType)?.Element
                                 ?? throw new NotSupportedException("Cannot resolve array element type id"));
    }

    public PgTypeId GetRangeTypeId(PgTypeId subtypeTypeId) =>
        ToCanonicalTypeId(DatabaseInfo.GetPostgresType(subtypeTypeId).Range
                          ?? throw new NotSupportedException("Cannot resolve range type id"));

    public PgTypeId GetRangeSubtypeTypeId(PgTypeId rangeTypeId) =>
        ToCanonicalTypeId((DatabaseInfo.GetPostgresType(rangeTypeId) as PostgresRangeType)?.Subtype
                          ?? throw new NotSupportedException("Cannot resolve range subtype type id"));

    public PgTypeId GetMultirangeTypeId(PgTypeId rangeTypeId) =>
        ToCanonicalTypeId((DatabaseInfo.GetPostgresType(rangeTypeId) as PostgresRangeType)?.Multirange
                          ?? throw new NotSupportedException("Cannot resolve multirange type id"));

    public PgTypeId GetMultirangeElementTypeId(PgTypeId multirangeTypeId) =>
        ToCanonicalTypeId((DatabaseInfo.GetPostgresType(multirangeTypeId) as PostgresMultirangeType)?.Subrange
                          ?? throw new NotSupportedException("Cannot resolve multirange element type id"));

    public bool TryGetDataTypeName(PgTypeId pgTypeId, out DataTypeName dataTypeName)
    {
        if (DatabaseInfo.FindPostgresType(pgTypeId) is { } pgType)
        {
            dataTypeName = pgType.DataTypeName;
            return true;
        }

        dataTypeName = default;
        return false;
    }

    public DataTypeName GetDataTypeName(PgTypeId pgTypeId)
        => !TryGetDataTypeName(pgTypeId, out var name)
        ? throw new ArgumentException("Unknown type id", nameof(pgTypeId))
        : name;
}
