using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandlers.DateTimeHandlers;
using Npgsql.Internal.TypeHandlers.FullTextSearchHandlers;
using Npgsql.Internal.TypeHandlers.GeometricHandlers;
using Npgsql.Internal.TypeHandlers.InternalTypeHandlers;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using Npgsql.Internal.TypeHandlers.NetworkHandlers;
using Npgsql.Internal.TypeHandlers.NumericHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.NameTranslation;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    sealed class GlobalTypeMapper : TypeMapperBase
    {
        public static GlobalTypeMapper Instance { get; }

        [MemberNotNullWhen(false,
            nameof(_mappingsByNameBuilder),
            nameof(_mappingsByNpgsqlDbTypeBuilder),
            nameof(_mappingsByClrTypeBuilder))]
        bool Initialized { get; set; }

        internal ImmutableDictionary<string, NpgsqlTypeMapping> MappingsByName { get; private set; }
        internal ImmutableDictionary<NpgsqlDbType, NpgsqlTypeMapping> MappingsByNpgsqlDbType { get; private set; }
        internal ImmutableDictionary<Type, NpgsqlTypeMapping> MappingsByClrType { get; private set; }

        ImmutableDictionary<string, NpgsqlTypeMapping>.Builder? _mappingsByNameBuilder;
        ImmutableDictionary<NpgsqlDbType, NpgsqlTypeMapping>.Builder? _mappingsByNpgsqlDbTypeBuilder;
        ImmutableDictionary<Type, NpgsqlTypeMapping>.Builder? _mappingsByClrTypeBuilder;

        /// <summary>
        /// A counter that is incremented whenever a global mapping change occurs.
        /// Used to invalidate bound type mappers.
        /// </summary>
        internal int ChangeCounter => _changeCounter;

        internal ReaderWriterLockSlim Lock { get; }
            = new(LockRecursionPolicy.SupportsRecursion);

        int _changeCounter;

        static GlobalTypeMapper()
            => Instance = new GlobalTypeMapper();

        GlobalTypeMapper() : base(new NpgsqlSnakeCaseNameTranslator())
            => Reset();

        #region Mapping management

        public override INpgsqlTypeMapper AddMapping(NpgsqlTypeMapping mapping)
        {
            Lock.EnterWriteLock();
            try
            {
                if (Initialized)
                {
                    MappingsByName = MappingsByName.SetItem(mapping.PgTypeName, mapping);
                    if (mapping.NpgsqlDbType is not null)
                        MappingsByNpgsqlDbType = MappingsByNpgsqlDbType.SetItem(mapping.NpgsqlDbType.Value, mapping);
                    foreach (var clrType in mapping.ClrTypes)
                        MappingsByClrType = MappingsByClrType.SetItem(clrType, mapping);

                    RecordChange();
                }
                else
                {
                    _mappingsByNameBuilder[mapping.PgTypeName] = mapping;
                    if (mapping.NpgsqlDbType is not null)
                        _mappingsByNpgsqlDbTypeBuilder[mapping.NpgsqlDbType.Value] = mapping;
                    foreach (var clrType in mapping.ClrTypes)
                        _mappingsByClrTypeBuilder[clrType] = mapping;
                }

                if (mapping.NpgsqlDbType.HasValue)
                {
                    _npgsqlDbTypeToPgTypeName[mapping.NpgsqlDbType.Value] = mapping.PgTypeName;
                    _npgsqlDbTypeToPgTypeName[mapping.NpgsqlDbType.Value | NpgsqlDbType.Array] = mapping.PgTypeName + "[]";

                    foreach (var dbType in mapping.DbTypes)
                        _dbTypeToNpgsqlDbType[dbType] = mapping.NpgsqlDbType.Value;

                    if (mapping.InferredDbType.HasValue)
                        _npgsqlDbTypeToDbType[mapping.NpgsqlDbType.Value] = mapping.InferredDbType.Value;

                    foreach (var clrType in mapping.ClrTypes)
                    {
                        _typeToNpgsqlDbType[clrType] = mapping.NpgsqlDbType.Value;
                        _typeToPgTypeName[clrType] = mapping.PgTypeName;
                    }
                }

                if (mapping.InferredDbType.HasValue)
                    foreach (var clrType in mapping.ClrTypes)
                        _typeToDbType[clrType] = mapping.InferredDbType.Value;

                return this;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override bool RemoveMapping(string pgTypeName)
        {
            Debug.Assert(Initialized);

            Lock.EnterWriteLock();
            try
            {
                var oldMappingsByName = MappingsByName;
                MappingsByName = MappingsByName.Remove(pgTypeName);
                var changed = ReferenceEquals(MappingsByName, oldMappingsByName);
                RecordChange();
                return changed;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override IEnumerable<NpgsqlTypeMapping> Mappings
        {
            get
            {
                Lock.EnterReadLock();
                try
                {
                    return MappingsByName.Values.ToArray();
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }


        [MemberNotNull(nameof(MappingsByName), nameof(MappingsByNpgsqlDbType), nameof(MappingsByClrType))]
        public override void Reset()
        {
            Lock.EnterWriteLock();
            try
            {
                Initialized = false;

                _mappingsByNameBuilder = ImmutableDictionary.CreateBuilder<string, NpgsqlTypeMapping>();
                _mappingsByNpgsqlDbTypeBuilder = ImmutableDictionary.CreateBuilder<NpgsqlDbType, NpgsqlTypeMapping>();
                _mappingsByClrTypeBuilder = ImmutableDictionary.CreateBuilder<Type, NpgsqlTypeMapping>();

                SetupBuiltInHandlers();

                MappingsByName = _mappingsByNameBuilder.ToImmutable();
                MappingsByNpgsqlDbType = _mappingsByNpgsqlDbTypeBuilder.ToImmutable();
                MappingsByClrType = _mappingsByClrTypeBuilder.ToImmutable();

                _mappingsByNameBuilder = null;
                _mappingsByNpgsqlDbTypeBuilder = null;
                _mappingsByClrTypeBuilder = null;

                Initialized = true;

                RecordChange();
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        internal void RecordChange() => Interlocked.Increment(ref _changeCounter);

        #endregion Mapping management

        #region NpgsqlDbType/DbType inference for NpgsqlParameter

        readonly Dictionary<NpgsqlDbType, DbType> _npgsqlDbTypeToDbType = new();
        readonly Dictionary<DbType, NpgsqlDbType> _dbTypeToNpgsqlDbType = new();
        readonly Dictionary<Type, NpgsqlDbType> _typeToNpgsqlDbType = new();
        readonly Dictionary<Type, DbType> _typeToDbType = new();
        readonly Dictionary<NpgsqlDbType, string> _npgsqlDbTypeToPgTypeName = new();
        readonly Dictionary<Type, string> _typeToPgTypeName = new();

        internal DbType ToDbType(NpgsqlDbType npgsqlDbType)
            => _npgsqlDbTypeToDbType.TryGetValue(npgsqlDbType, out var dbType) ? dbType : DbType.Object;

        internal NpgsqlDbType ToNpgsqlDbType(DbType dbType)
        {
            if (!_dbTypeToNpgsqlDbType.TryGetValue(dbType, out var npgsqlDbType))
                throw new NotSupportedException($"The parameter type DbType.{dbType} isn't supported by PostgreSQL or Npgsql");
            return npgsqlDbType;
        }

        internal DbType ToDbType(Type type)
            => _typeToDbType.TryGetValue(type, out var dbType) ? dbType : DbType.Object;

        internal string? ToPgTypeName(NpgsqlDbType npgsqlDbType)
            => _npgsqlDbTypeToPgTypeName.TryGetValue(npgsqlDbType, out var pgTypeName) ? pgTypeName : null;

        internal string? ToPgTypeName(Type type)
            => _typeToPgTypeName.TryGetValue(type, out var pgTypeName) ? pgTypeName : null;

        internal NpgsqlDbType ToNpgsqlDbType(Type type)
        {
            if (_typeToNpgsqlDbType.TryGetValue(type, out var npgsqlDbType))
                return npgsqlDbType;

            if (type.IsArray)
            {
                if (type == typeof(byte[]))
                    return NpgsqlDbType.Bytea;
                return NpgsqlDbType.Array | ToNpgsqlDbType(type.GetElementType()!);
            }

            var typeInfo = type.GetTypeInfo();

            var ilist = typeInfo.ImplementedInterfaces.FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
            if (ilist != null)
                return NpgsqlDbType.Array | ToNpgsqlDbType(ilist.GetGenericArguments()[0]);

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
                return NpgsqlDbType.Range | ToNpgsqlDbType(type.GetGenericArguments()[0]);

            if (type == typeof(DBNull))
                return NpgsqlDbType.Unknown;

            throw new NotSupportedException("Can't infer NpgsqlDbType for type " + type);
        }


        #endregion NpgsqlDbType/DbType inference for NpgsqlParameter

        #region Setup for built-in handlers

        void SetupBuiltInHandlers()
        {
            SetupNumericHandlers();
            SetupTextHandlers();
            SetupDateTimeHandlers();
            SetupNetworkHandlers();
            SetupFullTextSearchHandlers();
            SetupGeometryHandlers();
            SetupLTreeHandlers();
            SetupUIntHandlers();
            SetupMiscHandlers();
            SetupInternalHandlers();

            void SetupNumericHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "smallint",
                    NpgsqlDbType = NpgsqlDbType.Smallint,
                    DbTypes = new[] { DbType.Int16, DbType.Byte, DbType.SByte },
                    InferredDbType = DbType.Int16,
                    ClrTypes = new[] { typeof(short), typeof(byte), typeof(sbyte) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<Int16Handler, short>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "integer",
                    NpgsqlDbType = NpgsqlDbType.Integer,
                    DbTypes = new[] { DbType.Int32 },
                    ClrTypes = new[] { typeof(int) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<Int32Handler, int>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "bigint",
                    NpgsqlDbType = NpgsqlDbType.Bigint,
                    DbTypes = new[] { DbType.Int64 },
                    ClrTypes = new[] { typeof(long) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<Int64Handler, long>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "real",
                    NpgsqlDbType = NpgsqlDbType.Real,
                    DbTypes = new[] { DbType.Single },
                    ClrTypes = new[] { typeof(float) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<SingleHandler, float>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "double precision",
                    NpgsqlDbType = NpgsqlDbType.Double,
                    DbTypes = new[] { DbType.Double },
                    ClrTypes = new[] { typeof(double) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<DoubleHandler, double>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "numeric",
                    NpgsqlDbType = NpgsqlDbType.Numeric,
                    DbTypes = new[] { DbType.Decimal, DbType.VarNumeric },
                    InferredDbType = DbType.Decimal,
                    ClrTypes = new[] { typeof(decimal), typeof(BigInteger) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<NumericHandler, decimal>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "money",
                    NpgsqlDbType = NpgsqlDbType.Money,
                    DbTypes = new[] { DbType.Currency },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<MoneyHandler, decimal>()
                }.Build());
            }

            void SetupTextHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "text",
                    NpgsqlDbType = NpgsqlDbType.Text,
                    DbTypes = new[] { DbType.String, DbType.StringFixedLength, DbType.AnsiString, DbType.AnsiStringFixedLength },
                    InferredDbType = DbType.String,
                    ClrTypes = new[] { typeof(string), typeof(char[]), typeof(char), typeof(ArraySegment<char>) },
                    TypeHandlerFactory = new TextHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "xml",
                    NpgsqlDbType = NpgsqlDbType.Xml,
                    DbTypes = new[] { DbType.Xml },
                    TypeHandlerFactory = new TextHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "character varying",
                    NpgsqlDbType = NpgsqlDbType.Varchar,
                    InferredDbType = DbType.String,
                    TypeHandlerFactory = new TextHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "character",
                    NpgsqlDbType = NpgsqlDbType.Char,
                    InferredDbType = DbType.String,
                    TypeHandlerFactory = new TextHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "name",
                    NpgsqlDbType = NpgsqlDbType.Name,
                    InferredDbType = DbType.String,
                    TypeHandlerFactory = new TextHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "refcursor",
                    NpgsqlDbType = NpgsqlDbType.Refcursor,
                    InferredDbType = DbType.String,
                    TypeHandlerFactory = new TextHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "citext",
                    NpgsqlDbType = NpgsqlDbType.Citext,
                    InferredDbType = DbType.String,
                    TypeHandlerFactory = new TextHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "unknown",
                    TypeHandlerFactory = new TextHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "jsonb",
                    NpgsqlDbType = NpgsqlDbType.Jsonb,
                    ClrTypes = new[] { typeof(JsonDocument) },
                    TypeHandlerFactory = new JsonbHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "json",
                    NpgsqlDbType = NpgsqlDbType.Json,
                    TypeHandlerFactory = new JsonHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "jsonpath",
                    NpgsqlDbType = NpgsqlDbType.JsonPath,
                    TypeHandlerFactory = new JsonPathHandlerFactory()
                }.Build());
            }

            void SetupDateTimeHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "timestamp without time zone",
                    NpgsqlDbType = NpgsqlDbType.Timestamp,
                    DbTypes = new[] { DbType.DateTime, DbType.DateTime2 },
                    ClrTypes = new[] { typeof(NpgsqlDateTime), typeof(DateTime) },
                    InferredDbType = DbType.DateTime,
                    TypeHandlerFactory = new TimestampHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "timestamp with time zone",
                    NpgsqlDbType = NpgsqlDbType.TimestampTz,
                    DbTypes = new[] { DbType.DateTimeOffset },
                    ClrTypes = new[] { typeof(DateTimeOffset) },
                    TypeHandlerFactory = new TimestampTzHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "date",
                    NpgsqlDbType = NpgsqlDbType.Date,
                    DbTypes = new[] { DbType.Date },
                    ClrTypes = new[]
                    {
                        typeof(NpgsqlDate),
#if NET6_0_OR_GREATER
                        typeof(DateOnly)
#endif
                    },
                    TypeHandlerFactory = new DateHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "time without time zone",
                    NpgsqlDbType = NpgsqlDbType.Time,
                    DbTypes = new[] { DbType.Time },
#if NET6_0_OR_GREATER
                    ClrTypes = new[] { typeof(TimeOnly) },
#endif
                    TypeHandlerFactory = new TimeHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "time with time zone",
                    NpgsqlDbType = NpgsqlDbType.TimeTz,
                    TypeHandlerFactory = new TimeTzHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "interval",
                    NpgsqlDbType = NpgsqlDbType.Interval,
                    ClrTypes = new[] { typeof(TimeSpan), typeof(NpgsqlTimeSpan) },
                    TypeHandlerFactory = new IntervalHandlerFactory()
                }.Build());
            }

            void SetupNetworkHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "cidr",
                    NpgsqlDbType = NpgsqlDbType.Cidr,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<CidrHandler, (IPAddress Address, int Subnet)>()
                }.Build());

                var inetClrTypes = new List<Type>
                {
                    typeof(IPAddress), typeof((IPAddress Address, int Subnet)),
#pragma warning disable 618
                    typeof(NpgsqlInet)
#pragma warning restore 618
                };

                // Support ReadOnlyIPAddress, which was added to .NET Core 3.0 as an internal subclass of IPAddress
                if (typeof(IPAddress).GetNestedType("ReadOnlyIPAddress", BindingFlags.NonPublic) is Type readOnlyIpType)
                    inetClrTypes.Add(readOnlyIpType);

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "inet",
                    NpgsqlDbType = NpgsqlDbType.Inet,
                    ClrTypes = inetClrTypes.ToArray(),
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<InetHandler, IPAddress>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "macaddr8",
                    NpgsqlDbType = NpgsqlDbType.MacAddr8,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<MacaddrHandler, PhysicalAddress>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "macaddr",
                    NpgsqlDbType = NpgsqlDbType.MacAddr,
                    ClrTypes = new[] { typeof(PhysicalAddress) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<MacaddrHandler, PhysicalAddress>()
                }.Build());
            }

            void SetupFullTextSearchHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "tsquery",
                    NpgsqlDbType = NpgsqlDbType.TsQuery,
                    ClrTypes = new[]
                    {
                        typeof(NpgsqlTsQuery), typeof(NpgsqlTsQueryAnd), typeof(NpgsqlTsQueryEmpty), typeof(NpgsqlTsQueryFollowedBy),
                        typeof(NpgsqlTsQueryLexeme), typeof(NpgsqlTsQueryNot), typeof(NpgsqlTsQueryOr), typeof(NpgsqlTsQueryBinOp)
                    },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<TsQueryHandler, NpgsqlTsQuery>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "tsvector",
                    NpgsqlDbType = NpgsqlDbType.TsVector,
                    ClrTypes = new[] { typeof(NpgsqlTsVector) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<TsVectorHandler, NpgsqlTsVector>()
                }.Build());
            }

            void SetupGeometryHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "box",
                    NpgsqlDbType = NpgsqlDbType.Box,
                    ClrTypes = new[] { typeof(NpgsqlBox) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<BoxHandler, NpgsqlBox>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "circle",
                    NpgsqlDbType = NpgsqlDbType.Circle,
                    ClrTypes = new[] { typeof(NpgsqlCircle) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<CircleHandler, NpgsqlCircle>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "line",
                    NpgsqlDbType = NpgsqlDbType.Line,
                    ClrTypes = new[] { typeof(NpgsqlLine) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<LineHandler, NpgsqlLine>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "lseg",
                    NpgsqlDbType = NpgsqlDbType.LSeg,
                    ClrTypes = new[] { typeof(NpgsqlLSeg) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<LineSegmentHandler, NpgsqlLSeg>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "path",
                    NpgsqlDbType = NpgsqlDbType.Path,
                    ClrTypes = new[] { typeof(NpgsqlPath) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<PathHandler, NpgsqlPath>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "point",
                    NpgsqlDbType = NpgsqlDbType.Point,
                    ClrTypes = new[] { typeof(NpgsqlPoint) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<PointHandler, NpgsqlPoint>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "polygon",
                    NpgsqlDbType = NpgsqlDbType.Polygon,
                    ClrTypes = new[] { typeof(NpgsqlPolygon) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<PolygonHandler, NpgsqlPolygon>()
                }.Build());
            }

            void SetupLTreeHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "lquery",
                    NpgsqlDbType = NpgsqlDbType.LQuery,
                    TypeHandlerFactory = new LQueryHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "ltree",
                    NpgsqlDbType = NpgsqlDbType.LTree,
                    TypeHandlerFactory = new LQueryHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "ltxtquery",
                    NpgsqlDbType = NpgsqlDbType.LTxtQuery,
                    TypeHandlerFactory = new LQueryHandlerFactory()
                }.Build());
            }

            void SetupUIntHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "oid",
                    NpgsqlDbType = NpgsqlDbType.Oid,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<UInt32Handler, uint>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "xid",
                    NpgsqlDbType = NpgsqlDbType.Xid,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<UInt32Handler, uint>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "cid",
                    NpgsqlDbType = NpgsqlDbType.Cid,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<UInt32Handler, uint>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "regtype",
                    NpgsqlDbType = NpgsqlDbType.Regtype,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<UInt32Handler, uint>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "regconfig",
                    NpgsqlDbType = NpgsqlDbType.Regconfig,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<UInt32Handler, uint>()
                }.Build());
            }

            void SetupMiscHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "boolean",
                    NpgsqlDbType = NpgsqlDbType.Boolean,
                    DbTypes = new[] { DbType.Boolean },
                    ClrTypes = new[] { typeof(bool) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<BoolHandler, bool>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "bytea",
                    NpgsqlDbType = NpgsqlDbType.Bytea,
                    DbTypes = new[] { DbType.Binary },
                    ClrTypes = new[]
                    {
                        typeof(byte[]),
                        typeof(ArraySegment<byte>),
#if !NETSTANDARD2_0
                        typeof(ReadOnlyMemory<byte>),
                        typeof(Memory<byte>)
#endif
                    },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<ByteaHandler, byte[]>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "bit varying",
                    NpgsqlDbType = NpgsqlDbType.Varbit,
                    ClrTypes = new[] { typeof(BitArray), typeof(BitVector32) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<BitStringHandler, BitArray>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "bit",
                    NpgsqlDbType = NpgsqlDbType.Bit,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<BitStringHandler, BitArray>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "hstore",
                    NpgsqlDbType = NpgsqlDbType.Hstore,
                    ClrTypes = new[]
                    {
                        typeof(Dictionary<string, string?>),
                        typeof(IDictionary<string, string?>),
#if !NETSTANDARD2_0 && !NETSTANDARD2_1
                        typeof(System.Collections.Immutable.ImmutableDictionary<string, string?>)
#endif
                    },
                    TypeHandlerFactory = new HstoreHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "uuid",
                    NpgsqlDbType = NpgsqlDbType.Uuid,
                    DbTypes = new[] { DbType.Guid },
                    ClrTypes = new[] { typeof(Guid) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<UuidHandler, Guid>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "record",
                    TypeHandlerFactory = new RecordHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "void",
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<VoidHandler, DBNull>()
                }.Build());
            }

            void SetupInternalHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "int2vector",
                    NpgsqlDbType = NpgsqlDbType.Int2Vector,
                    TypeHandlerFactory = new Int2VectorHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "oidvector",
                    NpgsqlDbType = NpgsqlDbType.Oidvector,
                    TypeHandlerFactory = new OIDVectorHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "pg_lsn",
                    NpgsqlDbType = NpgsqlDbType.PgLsn,
                    ClrTypes = new[] { typeof(NpgsqlLogSequenceNumber) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<PgLsnHandler, NpgsqlLogSequenceNumber>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "tid",
                    NpgsqlDbType = NpgsqlDbType.Tid,
                    ClrTypes = new[] { typeof(NpgsqlTid) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<TidHandler, NpgsqlTid>()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "char",
                    NpgsqlDbType = NpgsqlDbType.InternalChar,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory<InternalCharHandler, char>()
                }.Build());
            }
        }

        #endregion Setup for built-in handlers
    }
}
