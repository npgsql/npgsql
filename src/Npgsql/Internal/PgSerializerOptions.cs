using System;
using System.Runtime.CompilerServices;
using System.Text;
using Npgsql.Internal.Postgres;
using Npgsql.NameTranslation;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

public class PgSerializerOptions
{
    /// <summary>
    /// Used by GetSchema to be able to attempt to resolve all type catalog types without exceptions.
    /// </summary>
    [field: ThreadStatic]
    internal static bool IntrospectionMode { get; set; }

    readonly Func<string>? _timeZoneProvider;
    object? _typeInfoCache;

    internal PgSerializerOptions(NpgsqlDatabaseInfo typeCatalog, Func<string>? timeZoneProvider = null)
    {
        _timeZoneProvider = timeZoneProvider;
        TypeCatalog = typeCatalog;
        UnknownPgType = typeCatalog.GetPostgresTypeByName("unknown");
    }

    // Represents postgres unknown type, which can be used for reading and writing arbitrary text values.
    public PostgresType UnknownPgType { get; set; }

    // Used purely for type mapping, where we don't have a full set of types but resolvers might know enough.
    internal bool ValidatePgTypeIds { get; init; } = true;

    /// Whether options should return a portable identifier (data type name) to prevent any generated id (oid) confusion across backends, this comes with a perf penalty.
    internal bool PortableTypeIds { get; init; }
    internal NpgsqlDatabaseInfo TypeCatalog { get; }

    public string TimeZone => _timeZoneProvider?.Invoke() ?? throw new NotSupportedException("TimeZone was not configured.");
    public Encoding TextEncoding { get; init; } = Encoding.UTF8;
    public required IPgTypeInfoResolver TypeInfoResolver { get; init; }
    public bool EnableDateTimeInfinityConversions { get; init; } = true;

    public ArrayNullabilityMode ArrayNullabilityMode { get; init; } = ArrayNullabilityMode.Never;
    public INpgsqlNameTranslator DefaultNameTranslator { get; init; } = NpgsqlSnakeCaseNameTranslator.Instance;

    // We don't verify the kind of pgTypeId we get, it'll throw if it's incorrect.
    // It's up to the caller to call GetCanonicalTypeId if they want to use an oid instead of a DataTypeName.
    // This also makes it easier to realize it should be a cached value if infos for different CLR types are requested for the same
    // pgTypeId. Effectively it should be 'impossible' to get the wrong kind via any PgConverterOptions api which is what this is mainly
    // for.
    PgTypeInfo? GetTypeInfoCore(Type? type, PgTypeId? pgTypeId, bool defaultTypeFallback)
        => PortableTypeIds
            ? Unsafe.As<TypeInfoCache<DataTypeName>>(_typeInfoCache ??= new TypeInfoCache<DataTypeName>(this))
                .GetOrAddInfo(type, pgTypeId is { } id1 ? id1.DataTypeName : null, defaultTypeFallback)
            : Unsafe.As<TypeInfoCache<Oid>>(_typeInfoCache ??= new TypeInfoCache<Oid>(this))
                .GetOrAddInfo(type, pgTypeId is { } id2 ? id2.Oid : null, defaultTypeFallback);

    public PgTypeInfo? GetDefaultTypeInfo(PostgresType pgType)
        => GetTypeInfoCore(null, PortableTypeIds ? pgType.DataTypeName : (Oid)pgType.OID, false);

    public PgTypeInfo? GetDefaultTypeInfo(PgTypeId pgTypeId)
        => GetTypeInfoCore(null, pgTypeId, false);

    public PgTypeInfo? GetTypeInfo(Type type, PostgresType pgType)
        => GetTypeInfoCore(type, PortableTypeIds ? pgType?.DataTypeName : (Oid?)pgType?.OID, false);

    public PgTypeInfo? GetTypeInfo(Type type, PgTypeId? pgTypeId = null)
        => GetTypeInfoCore(type, pgTypeId, false);

    public PgTypeInfo? GetObjectOrDefaultTypeInfo(PostgresType pgType)
        => GetTypeInfoCore(typeof(object), PortableTypeIds ? pgType.DataTypeName : (Oid)pgType.OID, true);

    public PgTypeInfo? GetObjectOrDefaultTypeInfo(PgTypeId pgTypeId)
        => GetTypeInfoCore(typeof(object), pgTypeId, true);

    // If a given type id is in the opposite form than what was expected it will be mapped according to the requirement.
    internal PgTypeId GetCanonicalTypeId(PgTypeId pgTypeId)
        => PortableTypeIds ? TypeCatalog.GetDataTypeName(pgTypeId) : TypeCatalog.GetOid(pgTypeId);

    // If a given type id is in the opposite form than what was expected it will be mapped according to the requirement.
    internal PgTypeId ToCanonicalTypeId(PostgresType pgType)
        => PortableTypeIds ? pgType.DataTypeName : (Oid)pgType.OID;

    public PgTypeId GetArrayTypeId(PgTypeId elementTypeId)
    {
        // Static affordance to help the global type mapper.
        if (PortableTypeIds && elementTypeId.IsDataTypeName)
            return elementTypeId.DataTypeName.ToArrayName();

        return ToCanonicalTypeId(TypeCatalog.GetPgType(elementTypeId).Array
                                 ?? throw new NotSupportedException("Cannot resolve array type id"));
    }

    public PgTypeId GetArrayElementTypeId(PgTypeId arrayTypeId)
    {
        // Static affordance to help the global type mapper.
        if (PortableTypeIds && arrayTypeId.IsDataTypeName && arrayTypeId.DataTypeName.UnqualifiedNameSpan.StartsWith("_".AsSpan(), StringComparison.Ordinal))
            return new DataTypeName(arrayTypeId.DataTypeName.Schema + arrayTypeId.DataTypeName.UnqualifiedNameSpan.Slice(1).ToString());

        return ToCanonicalTypeId((TypeCatalog.GetPgType(arrayTypeId) as PostgresArrayType)?.Element
                                 ?? throw new NotSupportedException("Cannot resolve array element type id"));
    }

    public PgTypeId GetRangeTypeId(PgTypeId subtypeTypeId) =>
        ToCanonicalTypeId(TypeCatalog.GetPgType(subtypeTypeId).Range
                          ?? throw new NotSupportedException("Cannot resolve range type id"));

    public PgTypeId GetRangeSubtypeTypeId(PgTypeId rangeTypeId) =>
        ToCanonicalTypeId((TypeCatalog.GetPgType(rangeTypeId) as PostgresRangeType)?.Subtype
                          ?? throw new NotSupportedException("Cannot resolve range subtype type id"));

    public PgTypeId GetMultirangeTypeId(PgTypeId rangeTypeId) =>
        ToCanonicalTypeId((TypeCatalog.GetPgType(rangeTypeId) as PostgresRangeType)?.Multirange
                          ?? throw new NotSupportedException("Cannot resolve multirange type id"));

    public PgTypeId GetMultirangeElementTypeId(PgTypeId multirangeTypeId) =>
        ToCanonicalTypeId((TypeCatalog.GetPgType(multirangeTypeId) as PostgresMultirangeType)?.Subrange
                          ?? throw new NotSupportedException("Cannot resolve multirange element type id"));

    public bool TryGetDataTypeName(PgTypeId pgTypeId, out DataTypeName dataTypeName)
    {
        if (TypeCatalog.FindPgType(pgTypeId) is { } pgType)
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
