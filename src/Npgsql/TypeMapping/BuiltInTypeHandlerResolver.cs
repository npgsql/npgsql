using System;
using System.Collections;
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

        static readonly Type ReadOnlyIPAddressType = IPAddress.Loopback.GetType();

        static readonly Dictionary<string, TypeMappingInfo> Mappings = new()
        {
            // Numeric types
            { "smallint",         new(NpgsqlDbType.Smallint, "smallint",         typeof(short), typeof(byte), typeof(sbyte)) },
            { "integer",          new(NpgsqlDbType.Integer,  "integer",          typeof(int)) },
            { "int",              new(NpgsqlDbType.Integer,  "integer",          typeof(int)) },
            { "bigint",           new(NpgsqlDbType.Bigint,   "bigint",           typeof(long)) },
            { "real",             new(NpgsqlDbType.Real,     "real",             typeof(float)) },
            { "double precision", new(NpgsqlDbType.Double,   "double precision", typeof(double)) },
            { "numeric",          new(NpgsqlDbType.Numeric,  "decimal",          typeof(decimal), typeof(BigInteger)) },
            { "decimal",          new(NpgsqlDbType.Numeric,  "decimal",          typeof(decimal), typeof(BigInteger)) },
            { "money",            new(NpgsqlDbType.Money,    "money") },

            // Text types
            { "text",              new(NpgsqlDbType.Text,      "text", typeof(string), typeof(char[]), typeof(char), typeof(ArraySegment<char>)) },
            { "xml",               new(NpgsqlDbType.Xml,       "xml") },
            { "character varying", new(NpgsqlDbType.Varchar,   "character varying") },
            { "varchar",           new(NpgsqlDbType.Varchar,   "character varying") },
            { "character",         new(NpgsqlDbType.Char,      "character") },
            { "name",              new(NpgsqlDbType.Name,      "name") },
            { "refcursor",         new(NpgsqlDbType.Refcursor, "refcursor") },
            { "citext",            new(NpgsqlDbType.Citext,    "citext") },
            { "jsonb",             new(NpgsqlDbType.Jsonb,     "jsonb", typeof(JsonDocument)) },
            { "json",              new(NpgsqlDbType.Json,      "json") },
            { "jsonpath",          new(NpgsqlDbType.JsonPath,  "jsonpath") },

            // Date/time types
#pragma warning disable 618 // NpgsqlDateTime is obsolete, remove in 7.0
            { "timestamp without time zone", new(NpgsqlDbType.Timestamp,   "timestamp without time zone", typeof(DateTime), typeof(NpgsqlDateTime)) },
            { "timestamp",                   new(NpgsqlDbType.Timestamp,   "timestamp without time zone", typeof(DateTime), typeof(NpgsqlDateTime)) },
#pragma warning disable 618
            { "timestamp with time zone",    new(NpgsqlDbType.TimestampTz, "timestamp with time zone",    typeof(DateTimeOffset)) },
            { "timestamptz",                 new(NpgsqlDbType.TimestampTz, "timestamp with time zone",    typeof(DateTimeOffset)) },
            { "date",                        new(NpgsqlDbType.Date,        "date",                        typeof(NpgsqlDate)
#if NET6_0_OR_GREATER
                , typeof(DateOnly)
#endif
            ) },
            { "time without time zone",      new(NpgsqlDbType.Time,        "timeout time zone"
#if NET6_0_OR_GREATER
                , typeof(TimeOnly)
#endif
            ) },
            { "time",                        new(NpgsqlDbType.Time,        "timeout time zone"
#if NET6_0_OR_GREATER
                , typeof(TimeOnly)
#endif
            ) },
            { "time with time zone",         new(NpgsqlDbType.TimeTz,      "time with time zone") },
            { "timetz",                      new(NpgsqlDbType.TimeTz,      "time with time zone") },
            { "interval",                    new(NpgsqlDbType.Interval,    "interval", typeof(TimeSpan), typeof(NpgsqlTimeSpan)) },

            { "timestamp without time zone[]", new(NpgsqlDbType.Array | NpgsqlDbType.Timestamp,   "timestamp without time zone[]") },
            { "timestamp with time zone[]",    new(NpgsqlDbType.Array | NpgsqlDbType.TimestampTz, "timestamp with time zone[]") },
            { "tsrange",                       new(NpgsqlDbType.Range | NpgsqlDbType.Timestamp,   "tsrange") },
            { "tstzrange",                     new(NpgsqlDbType.Range | NpgsqlDbType.TimestampTz, "tstzrange") },
            { "tsmultirange",                  new(NpgsqlDbType.Multirange | NpgsqlDbType.Timestamp,   "tsmultirange") },
            { "tstzmultirange",                new(NpgsqlDbType.Multirange | NpgsqlDbType.TimestampTz, "tstzmultirange") },

            // Network types
            { "cidr",      new(NpgsqlDbType.Cidr,     "cidr") },
#pragma warning disable 618
            { "inet",      new(NpgsqlDbType.Inet,     "inet", typeof(IPAddress), typeof((IPAddress Address, int Subnet)), typeof(NpgsqlInet), ReadOnlyIPAddressType) },
#pragma warning restore 618
            { "macaddr",   new(NpgsqlDbType.MacAddr,  "macaddr", typeof(PhysicalAddress)) },
            { "macaddr8",  new(NpgsqlDbType.MacAddr8, "macaddr8") },

            // Full-text search types
            { "tsquery",   new(NpgsqlDbType.TsQuery,  "tsquery",
                typeof(NpgsqlTsQuery), typeof(NpgsqlTsQueryAnd), typeof(NpgsqlTsQueryEmpty), typeof(NpgsqlTsQueryFollowedBy),
                typeof(NpgsqlTsQueryLexeme), typeof(NpgsqlTsQueryNot), typeof(NpgsqlTsQueryOr), typeof(NpgsqlTsQueryBinOp)
                ) },
            { "tsvector",  new(NpgsqlDbType.TsVector, "tsvector", typeof(NpgsqlTsVector)) },

            // Geometry types
            { "box",      new(NpgsqlDbType.Box,     "box",     typeof(NpgsqlBox)) },
            { "circle",   new(NpgsqlDbType.Circle,  "circle",  typeof(NpgsqlCircle)) },
            { "line",     new(NpgsqlDbType.Line,    "line",    typeof(NpgsqlLine)) },
            { "lseg",     new(NpgsqlDbType.LSeg,    "lseg",    typeof(NpgsqlLSeg)) },
            { "path",     new(NpgsqlDbType.Path,    "path",    typeof(NpgsqlPath)) },
            { "point",    new(NpgsqlDbType.Point,   "point",   typeof(NpgsqlPoint)) },
            { "polygon",  new(NpgsqlDbType.Polygon, "polygon", typeof(NpgsqlPolygon)) },

            // LTree types
            { "lquery",     new(NpgsqlDbType.LQuery,    "lquery") },
            { "ltree",      new(NpgsqlDbType.LTree,     "ltree") },
            { "ltxtquery",  new(NpgsqlDbType.LTxtQuery, "ltxtquery") },

            // UInt types
            { "oid",        new(NpgsqlDbType.Oid,       "oid") },
            { "xid",        new(NpgsqlDbType.Xid,       "xid") },
            { "xid8",       new(NpgsqlDbType.Xid8,      "xid8") },
            { "cid",        new(NpgsqlDbType.Cid,       "cid") },
            { "regtype",    new(NpgsqlDbType.Regtype,   "regtype") },
            { "regconfig",  new(NpgsqlDbType.Regconfig, "regconfig") },

            // Misc types
            { "boolean",     new(NpgsqlDbType.Boolean, "boolean", typeof(bool)) },
            { "bool",        new(NpgsqlDbType.Boolean, "boolean", typeof(bool)) },
            { "bytea",       new(NpgsqlDbType.Bytea,   "bytea", typeof(byte[]), typeof(ArraySegment<byte>)
#if !NETSTANDARD2_0
                , typeof(ReadOnlyMemory<byte>), typeof(Memory<byte>)
#endif
            ) },
            { "uuid",        new(NpgsqlDbType.Uuid,    "uuid", typeof(Guid)) },
            { "bit varying", new(NpgsqlDbType.Varbit,  "bit varying", typeof(BitArray), typeof(BitVector32)) },
            { "varbit",      new(NpgsqlDbType.Varbit,  "bit varying", typeof(BitArray), typeof(BitVector32)) },
            { "bit",         new(NpgsqlDbType.Bit,     "bit") },
            { "hstore",      new(NpgsqlDbType.Hstore,  "hstore", typeof(Dictionary<string, string?>), typeof(IDictionary<string, string?>)
#if !NETSTANDARD2_0 && !NETSTANDARD2_1
                , typeof(ImmutableDictionary<string, string?>)
#endif
            ) },

            // Internal types
            { "int2vector",  new(NpgsqlDbType.Int2Vector,   "int2vector") },
            { "oidvector",   new(NpgsqlDbType.Oidvector,    "oidvector") },
            { "pg_lsn",      new(NpgsqlDbType.PgLsn,        "pg_lsn", typeof(NpgsqlLogSequenceNumber)) },
            { "tid",         new(NpgsqlDbType.Tid,          "tid", typeof(NpgsqlTid)) },
            { "char",        new(NpgsqlDbType.InternalChar, "char") },

            // Special types
            { "unknown",  new(NpgsqlDbType.Unknown, "unknown") },
        };

        internal static void ResetMappings()
        {
            foreach (var mapping in Mappings)
                mapping.Value.Reset();
        }

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
        JsonHandler? _jsonbHandler; // Note that old version of PG (and Redshift) don't have jsonb
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
        UuidHandler? _uuidHandler;
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
                "record"     => RecordHandler(),
                "void"       => VoidHandler(),

                "unknown"    => UnknownHandler(),

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
                { typeof(DateTimeOffset), "timestamp with time zone" },
#if NET6_0_OR_GREATER
                { typeof(DateOnly),       "date" },
                { typeof(TimeOnly),       "time without time zone" },
#endif
                { typeof(TimeSpan),       "interval" },
                { typeof(NpgsqlInterval), "interval" },
#pragma warning disable 618 // NpgsqlDateTime and NpgsqlDate are obsolete, remove in 7.0
                { typeof(NpgsqlDateTime), "timestamp without time zone" },
                { typeof(NpgsqlDate),     "date" },
                { typeof(NpgsqlTimeSpan), "interval" },
#pragma warning restore 618

                // Network types
                { typeof(IPAddress),                       "inet" },
                // See ReadOnlyIPAddress below
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

            // Recent versions of .NET Core have an internal ReadOnlyIPAddress type (returned e.g. for IPAddress.Loopback)
            // But older versions don't have it
            if (ReadOnlyIPAddressType != typeof(IPAddress))
                ClrTypeToDataTypeNameTable[ReadOnlyIPAddressType] = "inet";

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
            if (typeof(T) == typeof(JsonDocument))
                return JsonbHandler();

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
#pragma warning disable 618 // NpgsqlDate and NpgsqlTimeSpan are obsolete, remove in 7.0
            if (typeof(T) == typeof(NpgsqlDate))
                return _dateHandler;
            if (typeof(T) == typeof(NpgsqlTimeSpan))
                return _intervalHandler;
#pragma warning restore 618

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

        internal static string? ClrTypeToDataTypeName(Type type)
            => ClrTypeToDataTypeNameTable.TryGetValue(type, out var dataTypeName) ? dataTypeName : null;

        public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => DoGetMappingByDataTypeName(dataTypeName);

        internal static TypeMappingInfo? DoGetMappingByDataTypeName(string dataTypeName)
            => Mappings.TryGetValue(dataTypeName, out var mapping) ? mapping : null;

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
        NpgsqlTypeHandler JsonbHandler()     => _jsonbHandler ??= new JsonHandler(PgType("jsonb"), _connector.TextEncoding, isJsonb: true);
        NpgsqlTypeHandler JsonHandler()      => _jsonHandler ??= new JsonHandler(PgType("json"), _connector.TextEncoding, isJsonb: false);
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
        NpgsqlTypeHandler RecordHandler()       => _recordHandler ??= new RecordHandler(PgType("record"), _connector.TypeMapper);
        NpgsqlTypeHandler VoidHandler()         => _voidHandler ??= new VoidHandler(PgType("void"));

        NpgsqlTypeHandler UnknownHandler() => _unknownHandler ??= new UnknownTypeHandler(_connector);

        #endregion Handler accessors
    }
}
