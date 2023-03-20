using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandlers.DateTimeHandlers;
using Npgsql.Internal.TypeHandlers.FullTextSearchHandlers;
using Npgsql.Internal.TypeHandlers.GeometricHandlers;
using Npgsql.Internal.TypeHandlers.InternalTypeHandlers;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using Npgsql.Internal.TypeHandlers.NetworkHandlers;
using Npgsql.Internal.TypeHandlers.NumericHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql.TypeMapping;

sealed class BuiltInTypeHandlerResolver : TypeHandlerResolver
{
    readonly NpgsqlConnector _connector;
    readonly NpgsqlDatabaseInfo _databaseInfo;

    #region Cached handlers

    // Numeric types
    readonly Int16Handler _int16Handler;
    readonly Int32Handler _int32Handler;
    readonly Int64Handler _int64Handler;
    SingleHandler? _singleHandler;
    readonly DoubleHandler _doubleHandler;
    readonly NumericHandler _numericHandler;
    MoneyHandler? _moneyHandler;

    // Text types
    readonly TextHandler _textHandler;
    TextHandler? _xmlHandler;
    TextHandler? _varcharHandler;
    TextHandler? _charHandler;
    TextHandler? _nameHandler;
    TextHandler? _refcursorHandler;
    TextHandler? _citextHandler;
    JsonTextHandler? _jsonbHandler; // Note that old version of PG (and Redshift) don't have jsonb
    JsonTextHandler? _jsonHandler;
    JsonPathHandler? _jsonPathHandler;

    // Date/time types
    readonly TimestampHandler _timestampHandler;
    readonly TimestampTzHandler _timestampTzHandler;
    readonly DateHandler _dateHandler;
    TimeHandler? _timeHandler;
    TimeTzHandler? _timeTzHandler;
    IntervalHandler? _intervalHandler;

    // Network types
    CidrHandler? _cidrHandler;
    InetHandler? _inetHandler;
    MacaddrHandler? _macaddrHandler;
    MacaddrHandler? _macaddr8Handler;

    // Full-text search types
    TsQueryHandler? _tsQueryHandler;
    TsVectorHandler? _tsVectorHandler;

    // Geometry types
    BoxHandler? _boxHandler;
    CircleHandler? _circleHandler;
    LineHandler? _lineHandler;
    LineSegmentHandler? _lineSegmentHandler;
    PathHandler? _pathHandler;
    PointHandler? _pointHandler;
    PolygonHandler? _polygonHandler;

    // LTree types
    LQueryHandler? _lQueryHandler;
    LTreeHandler? _lTreeHandler;
    LTxtQueryHandler? _lTxtQueryHandler;

    // UInt types
    UInt32Handler? _oidHandler;
    UInt32Handler? _xidHandler;
    UInt64Handler? _xid8Handler;
    UInt32Handler? _cidHandler;
    UInt32Handler? _regtypeHandler;
    UInt32Handler? _regconfigHandler;

    // Misc types
    readonly BoolHandler _boolHandler;
    ByteaHandler? _byteaHandler;
    UuidHandler? _uuidHandler;
    BitStringHandler? _bitVaryingHandler;
    BitStringHandler? _bitHandler;
    VoidHandler? _voidHandler;
    HstoreHandler? _hstoreHandler;

    // Internal types
    Int2VectorHandler? _int2VectorHandler;
    OIDVectorHandler? _oidVectorHandler;
    PgLsnHandler? _pgLsnHandler;
    TidHandler? _tidHandler;
    InternalCharHandler? _internalCharHandler;

    // Special types
    UnknownTypeHandler? _unknownHandler;

    // Complex type handlers over timestamp/timestamptz (because DateTime is value-dependent)
    NpgsqlTypeHandler? _timestampArrayHandler;
    NpgsqlTypeHandler? _timestampTzArrayHandler;

    #endregion Cached handlers

    internal BuiltInTypeHandlerResolver(NpgsqlConnector connector)
    {
        _connector = connector;
        _databaseInfo = connector.DatabaseInfo;

        // Eagerly instantiate some handlers for very common types so we don't need to check later
        _int16Handler = new Int16Handler(PgType("smallint"));
        _int32Handler = new Int32Handler(PgType("integer"));
        _int64Handler = new Int64Handler(PgType("bigint"));
        _doubleHandler = new DoubleHandler(PgType("double precision"));
        _numericHandler = new NumericHandler(PgType("numeric"));
        _textHandler ??= new TextHandler(PgType("text"), _connector.TextEncoding);
        _timestampHandler ??= new TimestampHandler(PgType("timestamp without time zone"));
        _timestampTzHandler ??= new TimestampTzHandler(PgType("timestamp with time zone"));
        _dateHandler ??= new DateHandler(PgType("date"));
        _boolHandler ??= new BoolHandler(PgType("boolean"));
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
        => typeName switch
        {
            // Numeric types
            "smallint"             => _int16Handler,
            "integer" or "int"     => _int32Handler,
            "bigint"               => _int64Handler,
            "real"                 => SingleHandler(),
            "double precision"     => _doubleHandler,
            "numeric" or "decimal" => _numericHandler,
            "money"                => MoneyHandler(),

            // Text types
            "text"                           => _textHandler,
            "xml"                            => XmlHandler(),
            "varchar" or "character varying" => VarcharHandler(),
            "character"                      => CharHandler(),
            "name"                           => NameHandler(),
            "refcursor"                      => RefcursorHandler(),
            "citext"                         => CitextHandler(),
            "jsonb"                          => JsonbHandler(),
            "json"                           => JsonHandler(),
            "jsonpath"                       => JsonPathHandler(),

            // Date/time types
            "timestamp" or "timestamp without time zone" => _timestampHandler,
            "timestamptz" or "timestamp with time zone"  => _timestampTzHandler,
            "date"                                       => _dateHandler,
            "time without time zone"                     => TimeHandler(),
            "time with time zone"                        => TimeTzHandler(),
            "interval"                                   => IntervalHandler(),

            // Network types
            "cidr"     => CidrHandler(),
            "inet"     => InetHandler(),
            "macaddr"  => MacaddrHandler(),
            "macaddr8" => Macaddr8Handler(),

            // Full-text search types
            "tsquery"  => TsQueryHandler(),
            "tsvector" => TsVectorHandler(),

            // Geometry types
            "box"     => BoxHandler(),
            "circle"  => CircleHandler(),
            "line"    => LineHandler(),
            "lseg"    => LineSegmentHandler(),
            "path"    => PathHandler(),
            "point"   => PointHandler(),
            "polygon" => PolygonHandler(),

            // LTree types
            "lquery"    => LQueryHandler(),
            "ltree"     => LTreeHandler(),
            "ltxtquery" => LTxtHandler(),

            // UInt types
            "oid"       => OidHandler(),
            "xid"       => XidHandler(),
            "xid8"      => Xid8Handler(),
            "cid"       => CidHandler(),
            "regtype"   => RegtypeHandler(),
            "regconfig" => RegconfigHandler(),

            // Misc types
            "bool" or "boolean"       => _boolHandler,
            "bytea"                   => ByteaHandler(),
            "uuid"                    => UuidHandler(),
            "bit varying" or "varbit" => BitVaryingHandler(),
            "bit"                     => BitHandler(),
            "hstore"                  => HstoreHandler(),

            // Internal types
            "int2vector" => Int2VectorHandler(),
            "oidvector"  => OidVectorHandler(),
            "pg_lsn"     => PgLsnHandler(),
            "tid"        => TidHandler(),
            "char"       => InternalCharHandler(),
            "record"     => new UnsupportedHandler(PgType("record"), $"Records aren't supported; please call {nameof(NpgsqlSlimDataSourceBuilder.EnableRecords)} on {nameof(NpgsqlSlimDataSourceBuilder)} to enable records."),
            "void"       => VoidHandler(),

            "unknown"    => UnknownHandler(),

            _ => null
        };

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
    {
        if (BuiltInTypeMappingResolver.ClrTypeToDataTypeName(type) is not { } dataTypeName)
        {
            if (!type.IsSubclassOf(typeof(Stream)))
                return null;

            dataTypeName = "bytea";
        }

        return ResolveByDataTypeName(dataTypeName);
    }

    public override NpgsqlTypeHandler? ResolveValueDependentValue(object value)
    {
        // In LegacyTimestampBehavior, DateTime isn't value-dependent, and handled above in ClrTypeToDataTypeNameTable like other types
        if (LegacyTimestampBehavior)
            return null;

        return value switch
        {
            DateTime dateTime => dateTime.Kind == DateTimeKind.Utc ? _timestampTzHandler : _timestampHandler,

            // For arrays/lists, return timestamp or timestamptz based on the kind of the first DateTime; if the user attempts to
            // mix incompatible Kinds, that will fail during validation. For empty arrays it doesn't matter.
            IList<DateTime> array => ArrayHandler(array.Count == 0 ? DateTimeKind.Unspecified : array[0].Kind),

            _ => null
        };

        NpgsqlTypeHandler ArrayHandler(DateTimeKind kind)
            => kind == DateTimeKind.Utc
                ? _timestampTzArrayHandler ??= _timestampTzHandler.CreateArrayHandler(
                    (PostgresArrayType)PgType("timestamp with time zone[]"), _connector.Settings.ArrayNullabilityMode)
                : _timestampArrayHandler ??= _timestampHandler.CreateArrayHandler(
                    (PostgresArrayType)PgType("timestamp without time zone[]"), _connector.Settings.ArrayNullabilityMode);
    }

    public override NpgsqlTypeHandler? ResolveValueTypeGenerically<T>(T value)
    {
        // This method only ever gets called for value types, and relies on the JIT specializing the method for T by eliding all the
        // type checks below.

        // Numeric types
        if (typeof(T) == typeof(byte))
            return _int16Handler;
        if (typeof(T) == typeof(short))
            return _int16Handler;
        if (typeof(T) == typeof(int))
            return _int32Handler;
        if (typeof(T) == typeof(long))
            return _int64Handler;
        if (typeof(T) == typeof(float))
            return SingleHandler();
        if (typeof(T) == typeof(double))
            return _doubleHandler;
        if (typeof(T) == typeof(decimal))
            return _numericHandler;
        if (typeof(T) == typeof(BigInteger))
            return _numericHandler;

        // Text types
        if (typeof(T) == typeof(char))
            return _textHandler;
        if (typeof(T) == typeof(ArraySegment<char>))
            return _textHandler;

        // Date/time types
        // No resolution for DateTime, since that's value-dependent (Kind)
        if (typeof(T) == typeof(DateTimeOffset))
            return _timestampTzHandler;
#if NET6_0_OR_GREATER
        if (typeof(T) == typeof(DateOnly))
            return _dateHandler;
        if (typeof(T) == typeof(TimeOnly))
            return _timeHandler;
#endif
        if (typeof(T) == typeof(TimeSpan))
            return _intervalHandler;
        if (typeof(T) == typeof(NpgsqlInterval))
            return _intervalHandler;

        // Network types
        if (typeof(T) == typeof(IPAddress))
            return InetHandler();
        if (typeof(T) == typeof(PhysicalAddress))
            return _macaddrHandler;
        if (typeof(T) == typeof(TimeSpan))
            return _intervalHandler;

        // Geometry types
        if (typeof(T) == typeof(NpgsqlBox))
            return BoxHandler();
        if (typeof(T) == typeof(NpgsqlCircle))
            return CircleHandler();
        if (typeof(T) == typeof(NpgsqlLine))
            return LineHandler();
        if (typeof(T) == typeof(NpgsqlLSeg))
            return LineSegmentHandler();
        if (typeof(T) == typeof(NpgsqlPath))
            return PathHandler();
        if (typeof(T) == typeof(NpgsqlPoint))
            return PointHandler();
        if (typeof(T) == typeof(NpgsqlPolygon))
            return PolygonHandler();

        // Misc types
        if (typeof(T) == typeof(bool))
            return _boolHandler;
        if (typeof(T) == typeof(Guid))
            return UuidHandler();
        if (typeof(T) == typeof(BitVector32))
            return BitVaryingHandler();

        // Internal types
        if (typeof(T) == typeof(NpgsqlLogSequenceNumber))
            return PgLsnHandler();
        if (typeof(T) == typeof(NpgsqlTid))
            return TidHandler();
        if (typeof(T) == typeof(DBNull))
            return UnknownHandler();

        return null;
    }

    PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);

    #region Handler accessors

    // Numeric types
    NpgsqlTypeHandler SingleHandler() => _singleHandler ??= new SingleHandler(PgType("real"));
    NpgsqlTypeHandler MoneyHandler()  => _moneyHandler ??= new MoneyHandler(PgType("money"));

    // Text types
    NpgsqlTypeHandler XmlHandler()       => _xmlHandler ??= new TextHandler(PgType("xml"), _connector.TextEncoding);
    NpgsqlTypeHandler VarcharHandler()   => _varcharHandler ??= new TextHandler(PgType("character varying"), _connector.TextEncoding);
    NpgsqlTypeHandler CharHandler()      => _charHandler ??= new TextHandler(PgType("character"), _connector.TextEncoding);
    NpgsqlTypeHandler NameHandler()      => _nameHandler ??= new TextHandler(PgType("name"), _connector.TextEncoding);
    NpgsqlTypeHandler RefcursorHandler() => _refcursorHandler ??= new TextHandler(PgType("refcursor"), _connector.TextEncoding);
    NpgsqlTypeHandler? CitextHandler()   => _citextHandler ??= _databaseInfo.TryGetPostgresTypeByName("citext", out var pgType)
        ? new TextHandler(pgType, _connector.TextEncoding)
        : null;
    NpgsqlTypeHandler JsonbHandler()     => _jsonbHandler ??= new JsonTextHandler(PgType("jsonb"), _connector.TextEncoding, isJsonb: true);
    NpgsqlTypeHandler JsonHandler()      => _jsonHandler ??= new JsonTextHandler(PgType("json"), _connector.TextEncoding, isJsonb: false);
    NpgsqlTypeHandler JsonPathHandler()  => _jsonPathHandler ??= new JsonPathHandler(PgType("jsonpath"), _connector.TextEncoding);

    // Date/time types
    NpgsqlTypeHandler TimeHandler()     => _timeHandler ??= new TimeHandler(PgType("time without time zone"));
    NpgsqlTypeHandler TimeTzHandler()   => _timeTzHandler ??= new TimeTzHandler(PgType("time with time zone"));
    NpgsqlTypeHandler IntervalHandler() => _intervalHandler ??= new IntervalHandler(PgType("interval"));

    // Network types
    NpgsqlTypeHandler CidrHandler()     => _cidrHandler ??= new CidrHandler(PgType("cidr"));
    NpgsqlTypeHandler InetHandler()     => _inetHandler ??= new InetHandler(PgType("inet"));
    NpgsqlTypeHandler MacaddrHandler()  => _macaddrHandler ??= new MacaddrHandler(PgType("macaddr"));
    NpgsqlTypeHandler Macaddr8Handler() => _macaddr8Handler ??= new MacaddrHandler(PgType("macaddr8"));

    // Full-text search types
    NpgsqlTypeHandler TsQueryHandler()  => _tsQueryHandler ??= new TsQueryHandler(PgType("tsquery"));
    NpgsqlTypeHandler TsVectorHandler() => _tsVectorHandler ??= new TsVectorHandler(PgType("tsvector"));

    // Geometry types
    NpgsqlTypeHandler BoxHandler()         => _boxHandler ??= new BoxHandler(PgType("box"));
    NpgsqlTypeHandler CircleHandler()      => _circleHandler ??= new CircleHandler(PgType("circle"));
    NpgsqlTypeHandler LineHandler()        => _lineHandler ??= new LineHandler(PgType("line"));
    NpgsqlTypeHandler LineSegmentHandler() => _lineSegmentHandler ??= new LineSegmentHandler(PgType("lseg"));
    NpgsqlTypeHandler PathHandler()        => _pathHandler ??= new PathHandler(PgType("path"));
    NpgsqlTypeHandler PointHandler()       => _pointHandler ??= new PointHandler(PgType("point"));
    NpgsqlTypeHandler PolygonHandler()     => _polygonHandler ??= new PolygonHandler(PgType("polygon"));

    // LTree types
    NpgsqlTypeHandler? LQueryHandler() => _lQueryHandler ??= _databaseInfo.TryGetPostgresTypeByName("lquery", out var pgType)
        ? new LQueryHandler(pgType, _connector.TextEncoding)
        : null;
    NpgsqlTypeHandler? LTreeHandler()  => _lTreeHandler ??= _databaseInfo.TryGetPostgresTypeByName("ltree", out var pgType)
        ? new LTreeHandler(pgType, _connector.TextEncoding)
        : null;
    NpgsqlTypeHandler? LTxtHandler()   => _lTxtQueryHandler ??= _databaseInfo.TryGetPostgresTypeByName("ltxtquery", out var pgType)
        ? new LTxtQueryHandler(pgType, _connector.TextEncoding)
        : null;

    // UInt types
    NpgsqlTypeHandler OidHandler()       => _oidHandler ??= new UInt32Handler(PgType("oid"));
    NpgsqlTypeHandler XidHandler()       => _xidHandler ??= new UInt32Handler(PgType("xid"));
    NpgsqlTypeHandler Xid8Handler()      => _xid8Handler ??= new UInt64Handler(PgType("xid8"));
    NpgsqlTypeHandler CidHandler()       => _cidHandler ??= new UInt32Handler(PgType("cid"));
    NpgsqlTypeHandler RegtypeHandler()   => _regtypeHandler ??= new UInt32Handler(PgType("regtype"));
    NpgsqlTypeHandler RegconfigHandler() => _regconfigHandler ??= new UInt32Handler(PgType("regconfig"));

    // Misc types
    NpgsqlTypeHandler ByteaHandler()      => _byteaHandler ??= new ByteaHandler(PgType("bytea"));
    NpgsqlTypeHandler UuidHandler()       => _uuidHandler ??= new UuidHandler(PgType("uuid"));
    NpgsqlTypeHandler BitVaryingHandler() => _bitVaryingHandler ??= new BitStringHandler(PgType("bit varying"));
    NpgsqlTypeHandler BitHandler()        => _bitHandler ??= new BitStringHandler(PgType("bit"));
    NpgsqlTypeHandler? HstoreHandler()    => _hstoreHandler ??= _databaseInfo.TryGetPostgresTypeByName("hstore", out var pgType)
        ? new HstoreHandler(pgType, _textHandler)
        : null;

    // Internal types
    NpgsqlTypeHandler Int2VectorHandler()   => _int2VectorHandler ??= new Int2VectorHandler(PgType("int2vector"), PgType("smallint"));
    NpgsqlTypeHandler OidVectorHandler()    => _oidVectorHandler ??= new OIDVectorHandler(PgType("oidvector"), PgType("oid"));
    NpgsqlTypeHandler PgLsnHandler()        => _pgLsnHandler ??= new PgLsnHandler(PgType("pg_lsn"));
    NpgsqlTypeHandler TidHandler()          => _tidHandler ??= new TidHandler(PgType("tid"));
    NpgsqlTypeHandler InternalCharHandler() => _internalCharHandler ??= new InternalCharHandler(PgType("char"));
    NpgsqlTypeHandler VoidHandler()         => _voidHandler ??= new VoidHandler(PgType("void"));

    NpgsqlTypeHandler UnknownHandler() => _unknownHandler ??= new UnknownTypeHandler(_connector.TextEncoding);

    #endregion Handler accessors
}
