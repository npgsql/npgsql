using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text.Json;
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

namespace Npgsql.TypeMapping
{
    class BuiltInTypeHandlerResolver : TypeHandlerResolver
    {
        readonly NpgsqlConnector _connector;
        readonly NpgsqlDatabaseInfo _databaseInfo;

        static readonly Type ReadonlyIpType = IPAddress.Loopback.GetType();

        static readonly Dictionary<string, TypeMappingInfo> Mappings = new()
        {
            // Numeric types
            { "smallint",         new(NpgsqlDbType.Smallint, DbType.Int16,   "smallint",         typeof(short), typeof(byte), typeof(sbyte)) },
            { "integer",          new(NpgsqlDbType.Integer,  DbType.Int32,   "integer",          typeof(int)) },
            { "int",              new(NpgsqlDbType.Integer,  DbType.Int32,   "integer",          typeof(int)) },
            { "bigint",           new(NpgsqlDbType.Bigint,   DbType.Int64,   "bigint",           typeof(long)) },
            { "real",             new(NpgsqlDbType.Real,     DbType.Single,  "real",             typeof(float)) },
            { "double precision", new(NpgsqlDbType.Double,   DbType.Double,  "double precision", typeof(double)) },
            { "numeric",          new(NpgsqlDbType.Numeric,  DbType.Decimal, "decimal",          typeof(decimal), typeof(BigInteger)) },
            { "decimal",          new(NpgsqlDbType.Numeric,  DbType.Decimal, "decimal",          typeof(decimal), typeof(BigInteger)) },
            { "money",            new(NpgsqlDbType.Money,    DbType.Int16,   "money") },

            // Text types
            { "text",              new(NpgsqlDbType.Text,      DbType.String, "text", typeof(string), typeof(char[]), typeof(char), typeof(ArraySegment<char>)) },
            { "xml",               new(NpgsqlDbType.Xml,       DbType.Xml,    "xml") },
            { "character varying", new(NpgsqlDbType.Varchar,   DbType.String, "character varying") },
            { "varchar",           new(NpgsqlDbType.Varchar,   DbType.String, "character varying") },
            { "character",         new(NpgsqlDbType.Char,      DbType.String, "character") },
            { "name",              new(NpgsqlDbType.Name,      DbType.String, "name") },
            { "refcursor",         new(NpgsqlDbType.Refcursor, DbType.String, "refcursor") },
            { "citext",            new(NpgsqlDbType.Citext,    DbType.String, "citext") },
            { "jsonb",             new(NpgsqlDbType.Jsonb,     DbType.Object, "jsonb", typeof(JsonDocument)) },
            { "json",              new(NpgsqlDbType.Json,      DbType.Object, "json") },
            { "jsonpath",          new(NpgsqlDbType.JsonPath,  DbType.Object, "jsonpath") },

            // Date/time types
            { "timestamp without time zone", new(NpgsqlDbType.Timestamp,   DbType.DateTime,       "timestamp without time zone", typeof(NpgsqlDateTime), typeof(DateTime)) },
            { "timestamp",                   new(NpgsqlDbType.Timestamp,   DbType.DateTime,       "timestamp without time zone", typeof(DateTimeOffset)) },
            { "timestamp with time zone",    new(NpgsqlDbType.TimestampTz, DbType.DateTimeOffset, "timestamp with time zone",    typeof(DateTimeOffset)) },
            { "timestamptz",                 new(NpgsqlDbType.TimestampTz, DbType.DateTimeOffset, "timestamp with time zone",    typeof(DateTimeOffset)) },
            { "date",                        new(NpgsqlDbType.Date,        DbType.Date,           "date",                        typeof(NpgsqlDate)
#if NET6_0_OR_GREATER
                , typeof(DateOnly)
#endif
            ) },
            { "time without time zone",      new(NpgsqlDbType.Time,        DbType.Time,     "timeout time zone"
#if NET6_0_OR_GREATER
                , typeof(TimeOnly)
#endif
            ) },
            { "time",                        new(NpgsqlDbType.Time,        DbType.Time,     "timeout time zone"
#if NET6_0_OR_GREATER
                , typeof(TimeOnly)
#endif
            ) },
            { "time with time zone",         new(NpgsqlDbType.TimeTz,      DbType.Object,   "time with time zone") },
            { "timetz",                      new(NpgsqlDbType.TimeTz,      DbType.Object,   "time with time zone") },
            { "interval",                    new(NpgsqlDbType.Interval,    DbType.Object,   "interval", typeof(TimeSpan), typeof(NpgsqlTimeSpan)) },

            { "timestamp without time zone[]", new(NpgsqlDbType.Array | NpgsqlDbType.Timestamp,   DbType.Object, "timestamp without time zone[]") },
            { "timestamp with time zone[]",    new(NpgsqlDbType.Array | NpgsqlDbType.TimestampTz, DbType.Object, "timestamp with time zone[]") },
            { "tsrange",                       new(NpgsqlDbType.Range | NpgsqlDbType.Timestamp,   DbType.Object, "tsrange") },
            { "tstzrange",                     new(NpgsqlDbType.Range | NpgsqlDbType.TimestampTz, DbType.Object, "tstzrange") },
            { "tsmultirange",                  new(NpgsqlDbType.Multirange | NpgsqlDbType.Timestamp,   DbType.Object, "tsmultirange") },
            { "tstzmultirange",                new(NpgsqlDbType.Multirange | NpgsqlDbType.TimestampTz, DbType.Object, "tstzmultirange") },

            // Network types
            { "cidr",      new(NpgsqlDbType.Cidr,     DbType.Object, "cidr") },
#pragma warning disable 618
            { "inet",      new(NpgsqlDbType.Inet,     DbType.Object, "inet", typeof(IPAddress), typeof((IPAddress Address, int Subnet)), typeof(NpgsqlInet), ReadonlyIpType) },
#pragma warning restore 618
            { "macaddr",   new(NpgsqlDbType.MacAddr,  DbType.Object, "macaddr", typeof(PhysicalAddress)) },
            { "macaddr8",  new(NpgsqlDbType.MacAddr8, DbType.Object, "macaddr8") },

            // Full-text search types
            { "tsquery",   new(NpgsqlDbType.TsQuery,  DbType.Object, "tsquery",
                typeof(NpgsqlTsQuery), typeof(NpgsqlTsQueryAnd), typeof(NpgsqlTsQueryEmpty), typeof(NpgsqlTsQueryFollowedBy),
                typeof(NpgsqlTsQueryLexeme), typeof(NpgsqlTsQueryNot), typeof(NpgsqlTsQueryOr), typeof(NpgsqlTsQueryBinOp)
                ) },
            { "tsvector",  new(NpgsqlDbType.TsVector, DbType.Object, "tsvector", typeof(NpgsqlTsVector)) },

            // Geometry types
            { "box",      new(NpgsqlDbType.Box,     DbType.Object, "box",     typeof(NpgsqlBox)) },
            { "circle",   new(NpgsqlDbType.Circle,  DbType.Object, "circle",  typeof(NpgsqlCircle)) },
            { "line",     new(NpgsqlDbType.Line,    DbType.Object, "line",    typeof(NpgsqlLine)) },
            { "lseg",     new(NpgsqlDbType.LSeg,    DbType.Object, "lseg",    typeof(NpgsqlLSeg)) },
            { "path",     new(NpgsqlDbType.Path,    DbType.Object, "path",    typeof(NpgsqlPath)) },
            { "point",    new(NpgsqlDbType.Point,   DbType.Object, "point",   typeof(NpgsqlPoint)) },
            { "polygon",  new(NpgsqlDbType.Polygon, DbType.Object, "polygon", typeof(NpgsqlPolygon)) },

            // LTree types
            { "lquery",     new(NpgsqlDbType.LQuery,    DbType.Object, "lquery") },
            { "ltree",      new(NpgsqlDbType.LTree,     DbType.Object, "ltree") },
            { "ltxtquery",  new(NpgsqlDbType.LTxtQuery, DbType.Object, "ltxtquery") },

            // UInt types
            { "oid",        new(NpgsqlDbType.Oid,       DbType.Object, "oid") },
            { "xid",        new(NpgsqlDbType.Xid,       DbType.Object, "xid") },
            { "xid8",       new(NpgsqlDbType.Xid8,      DbType.Object, "xid8") },
            { "cid",        new(NpgsqlDbType.Cid,       DbType.Object, "cid") },
            { "regtype",    new(NpgsqlDbType.Regtype,   DbType.Object, "regtype") },
            { "regconfig",  new(NpgsqlDbType.Regconfig, DbType.Object, "regconfig") },

            // Misc types
            { "boolean",     new(NpgsqlDbType.Boolean, DbType.Boolean, "boolean", typeof(bool)) },
            { "bool",        new(NpgsqlDbType.Boolean, DbType.Boolean, "boolean", typeof(bool)) },
            { "bytea",       new(NpgsqlDbType.Bytea,   DbType.Binary,  "bytea", typeof(byte[]), typeof(ArraySegment<byte>)
#if !NETSTANDARD2_0
                , typeof(ReadOnlyMemory<byte>), typeof(Memory<byte>)
#endif
            ) },
            { "uuid",        new(NpgsqlDbType.Uuid,    DbType.Guid,    "uuid", typeof(Guid)) },
            { "bit varying", new(NpgsqlDbType.Varbit,  DbType.Object,  "bit varying", typeof(BitArray), typeof(BitVector32)) },
            { "varbit",      new(NpgsqlDbType.Varbit,  DbType.Object,  "bit varying", typeof(BitArray), typeof(BitVector32)) },
            { "bit",         new(NpgsqlDbType.Bit,     DbType.Object,  "bit") },
            { "hstore",      new(NpgsqlDbType.Hstore,  DbType.Object,  "hstore", typeof(Dictionary<string, string?>), typeof(IDictionary<string, string?>)
#if !NETSTANDARD2_0 && !NETSTANDARD2_1
                , typeof(ImmutableDictionary<string, string?>)
#endif
            ) },

            // Internal types
            { "int2vector",  new(NpgsqlDbType.Int2Vector,   DbType.Object, "int2vector") },
            { "oidvector",   new(NpgsqlDbType.Oidvector,    DbType.Object, "oidvector") },
            { "pg_lsn",      new(NpgsqlDbType.PgLsn,        DbType.Object, "pg_lsn", typeof(NpgsqlLogSequenceNumber)) },
            { "tid",         new(NpgsqlDbType.Tid,          DbType.Object, "tid", typeof(NpgsqlTid)) },
            { "char",        new(NpgsqlDbType.InternalChar, DbType.Object, "char") },

            // Special types
            { "unknown",  new(NpgsqlDbType.Unknown, DbType.Object, "unknown") },
        };

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
        readonly JsonHandler _jsonbHandler;
        JsonHandler? _jsonHandler;
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
        readonly UuidHandler _uuidHandler;
        BitStringHandler? _bitVaryingHandler;
        BitStringHandler? _bitHandler;
        RecordHandler? _recordHandler;
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
        NpgsqlTypeHandler? _timestampRangeHandler;
        NpgsqlTypeHandler? _timestampTzRangeHandler;
        NpgsqlTypeHandler? _timestampMultirangeHandler;
        NpgsqlTypeHandler? _timestampTzMultirangeHandler;

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
            _timestampHandler ??= new TimestampHandler(PgType("timestamp without time zone"), _connector.Settings.ConvertInfinityDateTime);
            _timestampTzHandler ??= new TimestampTzHandler(PgType("timestamp with time zone"), _connector.Settings.ConvertInfinityDateTime);
            _dateHandler ??= new DateHandler(PgType("date"), _connector.Settings.ConvertInfinityDateTime);
            _boolHandler ??= new BoolHandler(PgType("boolean"));
            _uuidHandler ??= new UuidHandler(PgType("uuid"));
            _jsonbHandler ??= new JsonHandler(PgType("jsonb"), _connector.TextEncoding, isJsonb: true);
        }

        public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
            => typeName switch
            {
                // Numeric types
                "smallint"             => _int16Handler,
                "integer" or "int"     => _int32Handler,
                "bigint"               => _int64Handler,
                "real"                 => _singleHandler  ??= new SingleHandler(PgType("real")),
                "double precision"     => _doubleHandler,
                "numeric" or "decimal" => _numericHandler,
                "money"                => _moneyHandler   ??= new MoneyHandler(PgType("money")),

                // Text types
                "text"                           => _textHandler,
                "xml"                            => _xmlHandler ??= new TextHandler(PgType("xml"), _connector.TextEncoding),
                "varchar" or "character varying" => _varcharHandler ??= new TextHandler(PgType("character varying"), _connector.TextEncoding),
                "character"                      => _charHandler ??= new TextHandler(PgType("character"), _connector.TextEncoding),
                "name"                           => _nameHandler ??= new TextHandler(PgType("name"), _connector.TextEncoding),
                "refcursor"                      => _refcursorHandler ??= new TextHandler(PgType("refcursor"), _connector.TextEncoding),
                "citext"                         => _citextHandler ??= new TextHandler(PgType("citext"), _connector.TextEncoding),
                "jsonb"                          => _jsonbHandler,
                "json"                           => _jsonHandler ??= new JsonHandler(PgType("json"), _connector.TextEncoding, isJsonb: false),
                "jsonpath"                       => _jsonPathHandler ??= new JsonPathHandler(PgType("jsonpath"), _connector.TextEncoding),

                // Date/time types
                "timestamp" or "timestamp without time zone" => _timestampHandler,
                "timestamptz" or "timestamp with time zone"  => _timestampTzHandler,
                "date"                                       => _dateHandler,
                "time without time zone"                     => _timeHandler ??= new TimeHandler(PgType("time without time zone")),
                "time with time zone"                        => _timeTzHandler ??= new TimeTzHandler(PgType("time with time zone")),
                "interval"                                   => _intervalHandler ??= new IntervalHandler(PgType("interval")),

                // Network types
                "cidr"     => _cidrHandler ??= new CidrHandler(PgType("cidr")),
                "inet"     => _inetHandler ??= new InetHandler(PgType("inet")),
                "macaddr"  => _macaddrHandler ??= new MacaddrHandler(PgType("macaddr")),
                "macaddr8" => _macaddr8Handler ??= new MacaddrHandler(PgType("macaddr8")),

                // Full-text search types
                "tsquery"  => _tsQueryHandler ??= new TsQueryHandler(PgType("tsquery")),
                "tsvector" => _tsVectorHandler ??= new TsVectorHandler(PgType("tsvector")),

                // Geometry types
                "box"     => _boxHandler ??= new BoxHandler(PgType("box")),
                "circle"  => _circleHandler ??= new CircleHandler(PgType("circle")),
                "line"    => _lineHandler ??= new LineHandler(PgType("line")),
                "lseg"    => _lineSegmentHandler ??= new LineSegmentHandler(PgType("lseg")),
                "path"    => _pathHandler ??= new PathHandler(PgType("path")),
                "point"   => _pointHandler ??= new PointHandler(PgType("point")),
                "polygon" => _polygonHandler ??= new PolygonHandler(PgType("polygon")),

                // LTree types
                "lquery"    => _lQueryHandler ??= new LQueryHandler(PgType("lquery"), _connector.TextEncoding),
                "ltree"     => _lTreeHandler ??= new LTreeHandler(PgType("ltree"), _connector.TextEncoding),
                "ltxtquery" => _lTxtQueryHandler ??= new LTxtQueryHandler(PgType("ltxtquery"), _connector.TextEncoding),

                // UInt types
                "oid"       => _oidHandler ??= new UInt32Handler(PgType("oid")),
                "xid"       => _xidHandler ??= new UInt32Handler(PgType("xid")),
                "xid8"      => _xid8Handler ??= new UInt64Handler(PgType("xid8")),
                "cid"       => _cidHandler ??= new UInt32Handler(PgType("cid")),
                "regtype"   => _regtypeHandler ??= new UInt32Handler(PgType("regtype")),
                "regconfig" => _regconfigHandler ??= new UInt32Handler(PgType("regconfig")),

                // Misc types
                "bool" or "boolean"       => _boolHandler,
                "bytea"                   => _byteaHandler ??= new ByteaHandler(PgType("bytea")),
                "uuid"                    => _uuidHandler,
                "bit varying" or "varbit" => _bitVaryingHandler ??= new BitStringHandler(PgType("bit varying")),
                "bit"                     => _bitHandler ??= new BitStringHandler(PgType("bit")),
                "hstore"                  => _hstoreHandler ??= new HstoreHandler(PgType("hstore"), _textHandler),

                // Internal types
                "int2vector" => _int2VectorHandler ??= new Int2VectorHandler(PgType("int2vector"), PgType("smallint")),
                "oidvector"  => _oidVectorHandler ??= new OIDVectorHandler(PgType("oidvector"), PgType("oid")),
                "pg_lsn"     => _pgLsnHandler ??= new PgLsnHandler(PgType("pg_lsn")),
                "tid"        => _tidHandler ??= new TidHandler(PgType("tid")),
                "char"       => _internalCharHandler ??= new InternalCharHandler(PgType("char")),
                "record"     => _recordHandler ??= new RecordHandler(PgType("record"), _connector.TypeMapper),
                "void"       => _voidHandler ??= new VoidHandler(PgType("void")),

                "unknown"    => _unknownHandler ??= new UnknownTypeHandler(_connector),

                _ => null
            };

        public override NpgsqlTypeHandler? ResolveByClrType(Type type)
            => ClrTypeToDataTypeNameTable.TryGetValue(type, out var dataTypeName) && ResolveByDataTypeName(dataTypeName) is { } handler
                ? handler
                : null;

        static readonly Dictionary<Type, string> ClrTypeToDataTypeNameTable;

        static BuiltInTypeHandlerResolver()
        {
            ClrTypeToDataTypeNameTable = new()
            {
                // Numeric types
                { typeof(byte),       "smallint" },
                { typeof(short),      "smallint" },
                { typeof(int),        "integer" },
                { typeof(long),       "bigint" },
                { typeof(float),      "real" },
                { typeof(double),     "double precision" },
                { typeof(decimal),    "decimal" },
                { typeof(BigInteger), "decimal" },

                // Text types
                { typeof(string),             "text" },
                { typeof(char[]),             "text" },
                { typeof(char),               "text" },
                { typeof(ArraySegment<char>), "text" },
                { typeof(JsonDocument),       "jsonb" },

                // Date/time types
                // The DateTime entry is for LegacyTimestampBehavior mode only. In regular mode we resolve through
                // ResolveValueDependentValue below
                { typeof(DateTime),       "timestamp without time zone" },
                { typeof(NpgsqlDateTime), "timestamp without time zone" },
                { typeof(DateTimeOffset), "timestamp with time zone" },
                { typeof(NpgsqlDate),     "date" },
#if NET6_0_OR_GREATER
                { typeof(DateOnly),       "date" },
                { typeof(TimeOnly),       "time without time zone" },
#endif
                { typeof(TimeSpan),       "interval" },
                { typeof(NpgsqlTimeSpan), "interval" },

                // Network types
                { typeof(IPAddress),                       "inet" },
                { ReadonlyIpType,                          "inet" },
                { typeof((IPAddress Address, int Subnet)), "inet" },
#pragma warning disable 618
                { typeof(NpgsqlInet),                      "inet" },
#pragma warning restore 618
                { typeof(PhysicalAddress),                 "macaddr" },

                // Full-text types
                { typeof(NpgsqlTsQuery),  "tsquery" },
                { typeof(NpgsqlTsVector), "tsvector" },

                // Geometry types
                { typeof(NpgsqlBox),     "box" },
                { typeof(NpgsqlCircle),  "circle" },
                { typeof(NpgsqlLine),    "line" },
                { typeof(NpgsqlLSeg),    "lseg" },
                { typeof(NpgsqlPath),    "path" },
                { typeof(NpgsqlPoint),   "point" },
                { typeof(NpgsqlPolygon), "polygon" },

                // Misc types
                { typeof(bool),                 "boolean" },
                { typeof(byte[]),               "bytea" },
                { typeof(ArraySegment<byte>),   "bytea" },
#if !NETSTANDARD2_0
                { typeof(ReadOnlyMemory<byte>), "bytea" },
                { typeof(Memory<byte>),         "bytea" },
#endif
                { typeof(Guid),                                "uuid" },
                { typeof(BitArray),                            "bit varying" },
                { typeof(BitVector32),                         "bit varying" },
                { typeof(Dictionary<string, string>),          "hstore" },
#if !NETSTANDARD2_0 && !NETSTANDARD2_1
                { typeof(ImmutableDictionary<string, string>), "hstore" },
#endif

                // Internal types
                { typeof(NpgsqlLogSequenceNumber), "pg_lsn" },
                { typeof(NpgsqlTid),               "tid" },
                { typeof(DBNull),                  "unknown" }
            };

            if (LegacyTimestampBehavior)
                ClrTypeToDataTypeNameTable[typeof(DateTime)] = "timestamp without time zone";
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

                NpgsqlRange<DateTime> range => RangeHandler(!range.LowerBoundInfinite ? range.LowerBound.Kind :
                    !range.UpperBoundInfinite ? range.UpperBound.Kind : DateTimeKind.Unspecified),

                NpgsqlRange<DateTime>[] multirange => MultirangeHandler(GetMultirangeKind(multirange)),
                _ => null
            };

            NpgsqlTypeHandler ArrayHandler(DateTimeKind kind)
                => kind == DateTimeKind.Utc
                    ? _timestampTzArrayHandler ??= _timestampTzHandler.CreateArrayHandler(
                        (PostgresArrayType)PgType("timestamp with time zone[]"), _connector.Settings.ArrayNullabilityMode)
                    : _timestampArrayHandler ??= _timestampHandler.CreateArrayHandler(
                        (PostgresArrayType)PgType("timestamp without time zone[]"), _connector.Settings.ArrayNullabilityMode);

            NpgsqlTypeHandler RangeHandler(DateTimeKind kind)
                => kind == DateTimeKind.Utc
                    ? _timestampTzRangeHandler ??= _timestampTzHandler.CreateRangeHandler((PostgresRangeType)PgType("tstzrange"))
                    : _timestampRangeHandler ??= _timestampHandler.CreateRangeHandler((PostgresRangeType)PgType("tsrange"));

            NpgsqlTypeHandler MultirangeHandler(DateTimeKind kind)
                => kind == DateTimeKind.Utc
                    ? _timestampTzMultirangeHandler ??= _timestampTzHandler.CreateMultirangeHandler((PostgresMultirangeType)PgType("tstzmultirange"))
                    : _timestampMultirangeHandler ??= _timestampHandler.CreateMultirangeHandler((PostgresMultirangeType)PgType("tsmultirange"));
        }

        static DateTimeKind GetRangeKind(NpgsqlRange<DateTime> range)
            => !range.LowerBoundInfinite
                ? range.LowerBound.Kind
                : !range.UpperBoundInfinite
                    ? range.UpperBound.Kind
                    : DateTimeKind.Unspecified;

        static DateTimeKind GetMultirangeKind(NpgsqlRange<DateTime>[] multirange)
        {
            for (var i = 0; i < multirange.Length; i++)
                if (!multirange[i].IsEmpty)
                    return GetRangeKind(multirange[i]);

            return DateTimeKind.Unspecified;
        }

        internal static string? ValueDependentValueToDataTypeName(object value)
        {
            // In LegacyTimestampBehavior, DateTime isn't value-dependent, and handled above in ClrTypeToDataTypeNameTable like other types
            if (LegacyTimestampBehavior)
                return null;

            return value switch
            {
                DateTime dateTime => dateTime.Kind == DateTimeKind.Utc ? "timestamp with time zone" : "timestamp without time zone",

                // For arrays/lists, return timestamp or timestamptz based on the kind of the first DateTime; if the user attempts to
                // mix incompatible Kinds, that will fail during validation. For empty arrays it doesn't matter.
                IList<DateTime> array => array.Count == 0
                    ? "timestamp without time zone[]"
                    : array[0].Kind == DateTimeKind.Utc ? "timestamp with time zone[]" : "timestamp without time zone[]",

                NpgsqlRange<DateTime> range => GetRangeKind(range) == DateTimeKind.Utc ? "tstzrange" : "tsrange",

                NpgsqlRange<DateTime>[] multirange => GetMultirangeKind(multirange) == DateTimeKind.Utc ? "tstzmultirange" : "tsmultirange",

                _ => null
            };
        }

        internal static string? ClrTypeToDataTypeName(Type type)
            => ClrTypeToDataTypeNameTable.TryGetValue(type, out var dataTypeName) ? dataTypeName : null;

        public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => DoGetMappingByDataTypeName(dataTypeName);

        internal static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
            => Mappings.TryGetValue(dataTypeName, out var mapping) ? mapping : null;

        PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
    }
}
