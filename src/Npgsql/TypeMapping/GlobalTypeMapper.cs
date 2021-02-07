using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
    class GlobalTypeMapper : TypeMapperBase
    {
        public static GlobalTypeMapper Instance { get; }

        /// <summary>
        /// A counter that is incremented whenever a global mapping change occurs.
        /// Used to invalidate bound type mappers.
        /// </summary>
        internal int ChangeCounter => _changeCounter;

        internal ReaderWriterLockSlim Lock { get; }
            = new(LockRecursionPolicy.SupportsRecursion);

        int _changeCounter;

        static GlobalTypeMapper()
        {
            var instance = new GlobalTypeMapper();
            instance.SetupBuiltInHandlers();
            Instance = instance;
        }

        internal GlobalTypeMapper() : base(new NpgsqlSnakeCaseNameTranslator()) {}

        #region Mapping management

        public override INpgsqlTypeMapper AddMapping(NpgsqlTypeMapping mapping)
        {
            Lock.EnterWriteLock();
            try
            {
                base.AddMapping(mapping);
                RecordChange();

                if (mapping.NpgsqlDbType.HasValue)
                {
                    foreach (var dbType in mapping.DbTypes)
                        _dbTypeToNpgsqlDbType[dbType] = mapping.NpgsqlDbType.Value;

                    if (mapping.InferredDbType.HasValue)
                        _npgsqlDbTypeToDbType[mapping.NpgsqlDbType.Value] = mapping.InferredDbType.Value;

                    foreach (var clrType in mapping.ClrTypes)
                        _typeToNpgsqlDbType[clrType] = mapping.NpgsqlDbType.Value;
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
            Lock.EnterWriteLock();
            try
            {
                var result = base.RemoveMapping(pgTypeName);
                RecordChange();
                return result;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override void Reset()
        {
            Lock.EnterWriteLock();
            try
            {
                Mappings.Clear();
                SetupBuiltInHandlers();
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
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(Int16Handler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "integer",
                    NpgsqlDbType = NpgsqlDbType.Integer,
                    DbTypes = new[] { DbType.Int32 },
                    ClrTypes = new[] { typeof(int) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(Int32Handler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "bigint",
                    NpgsqlDbType = NpgsqlDbType.Bigint,
                    DbTypes = new[] { DbType.Int64 },
                    ClrTypes = new[] { typeof(long) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(Int64Handler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "real",
                    NpgsqlDbType = NpgsqlDbType.Real,
                    DbTypes = new[] { DbType.Single },
                    ClrTypes = new[] { typeof(float) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(SingleHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "double precision",
                    NpgsqlDbType = NpgsqlDbType.Double,
                    DbTypes = new[] { DbType.Double },
                    ClrTypes = new[] { typeof(double) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(DoubleHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "numeric",
                    NpgsqlDbType = NpgsqlDbType.Numeric,
                    DbTypes = new[] { DbType.Decimal, DbType.VarNumeric },
                    InferredDbType = DbType.Decimal,
                    ClrTypes = new[] { typeof(decimal) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(NumericHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "money",
                    NpgsqlDbType = NpgsqlDbType.Money,
                    DbTypes = new[] { DbType.Currency },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(MoneyHandler))
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
                    ClrTypes = new[] { typeof(NpgsqlDate) },
                    TypeHandlerFactory = new DateHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "time without time zone",
                    NpgsqlDbType = NpgsqlDbType.Time,
                    DbTypes = new[] { DbType.Time },
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
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(CidrHandler))
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
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(InetHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "macaddr8",
                    NpgsqlDbType = NpgsqlDbType.MacAddr8,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(MacaddrHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "macaddr",
                    NpgsqlDbType = NpgsqlDbType.MacAddr,
                    ClrTypes = new[] { typeof(PhysicalAddress) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(MacaddrHandler))
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
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(TsQueryHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "tsvector",
                    NpgsqlDbType = NpgsqlDbType.TsVector,
                    ClrTypes = new[] { typeof(NpgsqlTsVector) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(TsVectorHandler))
                }.Build());
            }

            void SetupGeometryHandlers()
            {
                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "box",
                    NpgsqlDbType = NpgsqlDbType.Box,
                    ClrTypes = new[] { typeof(NpgsqlBox) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(BoxHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "circle",
                    NpgsqlDbType = NpgsqlDbType.Circle,
                    ClrTypes = new[] { typeof(NpgsqlCircle) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(CircleHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "line",
                    NpgsqlDbType = NpgsqlDbType.Line,
                    ClrTypes = new[] { typeof(NpgsqlLine) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(LineHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "lseg",
                    NpgsqlDbType = NpgsqlDbType.LSeg,
                    ClrTypes = new[] { typeof(NpgsqlLSeg) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(LineSegmentHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "path",
                    NpgsqlDbType = NpgsqlDbType.Path,
                    ClrTypes = new[] { typeof(NpgsqlPath) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(PathHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "point",
                    NpgsqlDbType = NpgsqlDbType.Point,
                    ClrTypes = new[] { typeof(NpgsqlPoint) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(PointHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "polygon",
                    NpgsqlDbType = NpgsqlDbType.Polygon,
                    ClrTypes = new[] { typeof(NpgsqlPolygon) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(PolygonHandler))
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
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(UInt32Handler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "xid",
                    NpgsqlDbType = NpgsqlDbType.Xid,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(UInt32Handler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "cid",
                    NpgsqlDbType = NpgsqlDbType.Cid,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(UInt32Handler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "regtype",
                    NpgsqlDbType = NpgsqlDbType.Regtype,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(UInt32Handler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "regconfig",
                    NpgsqlDbType = NpgsqlDbType.Regconfig,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(UInt32Handler))
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
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(BoolHandler))
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
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(ByteaHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "bit varying",
                    NpgsqlDbType = NpgsqlDbType.Varbit,
                    ClrTypes = new[] { typeof(BitArray), typeof(BitVector32) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(BitStringHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "bit",
                    NpgsqlDbType = NpgsqlDbType.Bit,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(BitStringHandler))
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
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(UuidHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "record",
                    TypeHandlerFactory = new RecordHandlerFactory()
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "void",
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(VoidHandler))
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
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(PgLsnHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "tid",
                    NpgsqlDbType = NpgsqlDbType.Tid,
                    ClrTypes = new[] { typeof(NpgsqlTid) },
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(TidHandler))
                }.Build());

                AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "char",
                    NpgsqlDbType = NpgsqlDbType.InternalChar,
                    TypeHandlerFactory = new DefaultTypeHandlerFactory(typeof(InternalCharHandler))
                }.Build());
            }
        }

        #endregion Setup for built-in handlers
    }
}
