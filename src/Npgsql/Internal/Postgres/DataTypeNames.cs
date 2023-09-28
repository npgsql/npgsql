using static Npgsql.Internal.Postgres.DataTypeName;

namespace Npgsql.Internal.Postgres;

/// <summary>
/// Well-known PostgreSQL data type names.
/// </summary>
static class DataTypeNames
{
    // Note: The names are fully qualified in source so the strings are constants and instances will be interned after the first call.
    // Uses an internal constructor bypassing the public DataTypeName constructor validation, as we don't want to store all these names on
    // fields either.
    public static DataTypeName Int2 => ValidatedName("pg_catalog.int2");
    public static DataTypeName Int4 => ValidatedName("pg_catalog.int4");
    public static DataTypeName Int4Range => ValidatedName("pg_catalog.int4range");
    public static DataTypeName Int4Multirange => ValidatedName("pg_catalog.int4multirange");
    public static DataTypeName Int8 => ValidatedName("pg_catalog.int8");
    public static DataTypeName Int8Range => ValidatedName("pg_catalog.int8range");
    public static DataTypeName Int8Multirange => ValidatedName("pg_catalog.int8multirange");
    public static DataTypeName Float4 => ValidatedName("pg_catalog.float4");
    public static DataTypeName Float8 => ValidatedName("pg_catalog.float8");
    public static DataTypeName Numeric => ValidatedName("pg_catalog.numeric");
    public static DataTypeName NumRange => ValidatedName("pg_catalog.numrange");
    public static DataTypeName NumMultirange => ValidatedName("pg_catalog.nummultirange");
    public static DataTypeName Money => ValidatedName("pg_catalog.money");
    public static DataTypeName Bool => ValidatedName("pg_catalog.bool");
    public static DataTypeName Box => ValidatedName("pg_catalog.box");
    public static DataTypeName Circle => ValidatedName("pg_catalog.circle");
    public static DataTypeName Line => ValidatedName("pg_catalog.line");
    public static DataTypeName LSeg => ValidatedName("pg_catalog.lseg");
    public static DataTypeName Path => ValidatedName("pg_catalog.path");
    public static DataTypeName Point => ValidatedName("pg_catalog.point");
    public static DataTypeName Polygon => ValidatedName("pg_catalog.polygon");
    public static DataTypeName Bpchar => ValidatedName("pg_catalog.bpchar");
    public static DataTypeName Text => ValidatedName("pg_catalog.text");
    public static DataTypeName Varchar => ValidatedName("pg_catalog.varchar");
    public static DataTypeName Char => ValidatedName("pg_catalog.char");
    public static DataTypeName Name => ValidatedName("pg_catalog.name");
    public static DataTypeName Bytea => ValidatedName("pg_catalog.bytea");
    public static DataTypeName Date => ValidatedName("pg_catalog.date");
    public static DataTypeName DateRange => ValidatedName("pg_catalog.daterange");
    public static DataTypeName DateMultirange => ValidatedName("pg_catalog.datemultirange");
    public static DataTypeName Time => ValidatedName("pg_catalog.time");
    public static DataTypeName Timestamp => ValidatedName("pg_catalog.timestamp");
    public static DataTypeName TsRange => ValidatedName("pg_catalog.tsrange");
    public static DataTypeName TsMultirange => ValidatedName("pg_catalog.tsmultirange");
    public static DataTypeName TimestampTz => ValidatedName("pg_catalog.timestamptz");
    public static DataTypeName TsTzRange => ValidatedName("pg_catalog.tstzrange");
    public static DataTypeName TsTzMultirange => ValidatedName("pg_catalog.tstzmultirange");
    public static DataTypeName Interval => ValidatedName("pg_catalog.interval");
    public static DataTypeName TimeTz => ValidatedName("pg_catalog.timetz");
    public static DataTypeName Inet => ValidatedName("pg_catalog.inet");
    public static DataTypeName Cidr => ValidatedName("pg_catalog.cidr");
    public static DataTypeName MacAddr => ValidatedName("pg_catalog.macaddr");
    public static DataTypeName MacAddr8 => ValidatedName("pg_catalog.macaddr8");
    public static DataTypeName Bit => ValidatedName("pg_catalog.bit");
    public static DataTypeName Varbit => ValidatedName("pg_catalog.varbit");
    public static DataTypeName TsVector => ValidatedName("pg_catalog.tsvector");
    public static DataTypeName TsQuery => ValidatedName("pg_catalog.tsquery");
    public static DataTypeName RegConfig => ValidatedName("pg_catalog.regconfig");
    public static DataTypeName Uuid => ValidatedName("pg_catalog.uuid");
    public static DataTypeName Xml => ValidatedName("pg_catalog.xml");
    public static DataTypeName Json => ValidatedName("pg_catalog.json");
    public static DataTypeName Jsonb => ValidatedName("pg_catalog.jsonb");
    public static DataTypeName Jsonpath => ValidatedName("pg_catalog.jsonpath");
    public static DataTypeName Record => ValidatedName("pg_catalog.record");
    public static DataTypeName RefCursor => ValidatedName("pg_catalog.refcursor");
    public static DataTypeName OidVector => ValidatedName("pg_catalog.oidvector");
    public static DataTypeName Int2Vector => ValidatedName("pg_catalog.int2vector");
    public static DataTypeName Oid => ValidatedName("pg_catalog.oid");
    public static DataTypeName Xid => ValidatedName("pg_catalog.xid");
    public static DataTypeName Xid8 => ValidatedName("pg_catalog.xid8");
    public static DataTypeName Cid => ValidatedName("pg_catalog.cid");
    public static DataTypeName RegType => ValidatedName("pg_catalog.regtype");
    public static DataTypeName Tid => ValidatedName("pg_catalog.tid");
    public static DataTypeName PgLsn => ValidatedName("pg_catalog.pg_lsn");
    public static DataTypeName Unknown => ValidatedName("pg_catalog.unknown");
    public static DataTypeName Void => ValidatedName("pg_catalog.void");
}
