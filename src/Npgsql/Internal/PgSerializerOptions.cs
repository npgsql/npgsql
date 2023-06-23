using System;
using System.Runtime.CompilerServices;
using System.Text;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

// TODO it's either PgSerializerOptions or PgConverterOptions, I have no strong preference.
public class PgSerializerOptions
{
    object? _typeInfoCache;

    internal PgSerializerOptions(NpgsqlDatabaseInfo typeCatalog)
    {
        TypeCatalog = typeCatalog;
        PgTextType = typeCatalog.GetPostgresTypeByName(DataTypeNames.Text);
    }

    public PostgresType PgTextType { get; set; }

    /// Whether options should return a portable identifier (data type name) to prevent any generated id (oid) confusion across backends, this comes with a perf penalty.
    internal bool PortableTypeIds { get; init; }
    internal NpgsqlDatabaseInfo TypeCatalog { get; }

    public required Encoding TextEncoding { get; init; }
    public required IPgTypeInfoResolver TypeInfoResolver { get; init; }
    public bool EnableDateTimeInfinityConversions { get; init; } = true;

    public ArrayNullabilityMode ArrayNullabilityMode { get; init; } = ArrayNullabilityMode.Never;

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

    internal PostgresType GetPgType(PgTypeId pgTypeId)
        => pgTypeId.IsOid
            ? TypeCatalog.GetPostgresTypeByOid(pgTypeId.Oid.Value)
            : TypeCatalog.GetPostgresTypeByName(pgTypeId.DataTypeName.Value);

    internal PostgresType? TryGetPgType(PgTypeId pgTypeId)
        => pgTypeId.IsOid
            ? TypeCatalog.ByOID.TryGetValue(pgTypeId.Oid.Value, out var pgType) ? pgType : null
            : TypeCatalog.TryGetPostgresTypeByName(pgTypeId.DataTypeName.Value, out pgType) ? pgType : null;

    // If a given type id is in the opposite form than what was expected it will be mapped according to the requirement.
    internal PgTypeId GetCanonicalTypeId(PgTypeId pgTypeId)
        => PortableTypeIds ? TypeCatalog.GetDataTypeName(pgTypeId) : TypeCatalog.GetOid(pgTypeId);

    // If a given type id is in the opposite form than what was expected it will be mapped according to the requirement.
    internal PgTypeId GetCanonicalTypeId(PostgresType pgType)
        => PortableTypeIds ? pgType.DataTypeName : (Oid)pgType.OID;

    // public PgTypeId GetTypeId(string dataTypeName)
    //     => RequirePortableTypeIds ? TypeCatalog.GetDataTypeName(dataTypeName) : TypeCatalog.GetOid(TypeCatalog.GetDataTypeName(dataTypeName));
    //
    // public PgTypeId GetTypeId(DataTypeName dataTypeName)
    //     => RequirePortableTypeIds ? TypeCatalog.GetDataTypeName(dataTypeName) : TypeCatalog.GetOid(dataTypeName);
    //
    // public PgTypeId GetArrayTypeId(string elementDataTypeName)
    //     => RequirePortableTypeIds
    //         ? TypeCatalog.GetArrayDataTypeName(TypeCatalog.GetDataTypeName(elementDataTypeName))
    //         : TypeCatalog.GetArrayOid(TypeCatalog.GetDataTypeName(elementDataTypeName));
    //
    // public PgTypeId GetArrayTypeId(DataTypeName elementDataTypeName)
    //     => RequirePortableTypeIds ? TypeCatalog.GetArrayDataTypeName(elementDataTypeName) : TypeCatalog.GetArrayOid(elementDataTypeName);
    //
    // public PgTypeId GetArrayTypeId(PgTypeId elementTypeId)
    //     => RequirePortableTypeIds ? TypeCatalog.GetArrayDataTypeName(elementTypeId) : TypeCatalog.GetArrayOid(elementTypeId);
    //
    // public PgTypeId GetElementTypeId(string arrayDataTypeName)
    //     => RequirePortableTypeIds
    //         ? TypeCatalog.GetElementDataTypeName(TypeCatalog.GetDataTypeName(arrayDataTypeName))
    //         : TypeCatalog.GetElementOid(TypeCatalog.GetDataTypeName(arrayDataTypeName));
    //
    // public PgTypeId GetElementTypeId(DataTypeName arrayDataTypeName)
    //     => RequirePortableTypeIds ? TypeCatalog.GetElementDataTypeName(arrayDataTypeName) : TypeCatalog.GetElementOid(arrayDataTypeName);
    //
    // public PgTypeId GetElementTypeId(PgTypeId arrayTypeId)
    //     => RequirePortableTypeIds ? TypeCatalog.GetElementDataTypeName(arrayTypeId) : TypeCatalog.GetElementOid(arrayTypeId);
    //
    // public DataTypeName GetDataTypeName(PgTypeId pgTypeId) => TypeCatalog.GetDataTypeName(pgTypeId);
}
