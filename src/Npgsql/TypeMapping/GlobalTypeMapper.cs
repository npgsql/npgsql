using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.NameTranslation;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql.TypeMapping
{
    sealed class GlobalTypeMapper : TypeMapperBase
    {
        public static GlobalTypeMapper Instance { get; }

        internal List<TypeHandlerResolverFactory> ResolverFactories { get; } = new();
        public ConcurrentDictionary<string, IUserTypeMapping> UserTypeMappings { get; } = new();

        readonly ConcurrentDictionary<Type, TypeMappingInfo> _mappingsByClrType = new();

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

        public override INpgsqlTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(typeof(TEnum), nameTranslator);

            Lock.EnterWriteLock();
            try
            {
                UserTypeMappings[pgName] = new UserEnumTypeMapping<TEnum>(pgName, nameTranslator);
                RecordChange();
                return this;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(typeof(TEnum), nameTranslator);

            Lock.EnterWriteLock();
            try
            {
                if (UserTypeMappings.TryRemove(pgName, out _))
                {
                    RecordChange();
                    return true;
                }

                return false;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override INpgsqlTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(typeof(T), nameTranslator);

            Lock.EnterWriteLock();
            try
            {
                UserTypeMappings[pgName] = new UserCompositeTypeMapping<T>(pgName, nameTranslator);
                RecordChange();
                return this;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override INpgsqlTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(clrType, nameTranslator);

            Lock.EnterWriteLock();
            try
            {
                UserTypeMappings[pgName] =
                    (IUserTypeMapping)Activator.CreateInstance(typeof(UserCompositeTypeMapping<>).MakeGenericType(clrType),
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                        new object[] { clrType, nameTranslator }, null)!;

                RecordChange();

                return this;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            => UnmapComposite(typeof(T), pgName, nameTranslator);

        public override bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(clrType, nameTranslator);

            Lock.EnterWriteLock();
            try
            {
                if (UserTypeMappings.TryRemove(pgName, out _))
                {
                    RecordChange();
                    return true;
                }

                return false;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override void AddTypeResolverFactory(TypeHandlerResolverFactory resolverFactory)
        {
            Lock.EnterWriteLock();
            try
            {
                // Since EFCore.PG plugins (and possibly other users) repeatedly call NpgsqlConnection.GlobalTypeMapped.UseNodaTime,
                // we replace an existing resolver of the same CLR type.
                var type = resolverFactory.GetType();

                if (ResolverFactories[0].GetType() == type)
                    ResolverFactories[0] = resolverFactory;
                else
                {
                    for (var i = 0; i < ResolverFactories.Count; i++)
                        if (ResolverFactories[i].GetType() == type)
                            ResolverFactories.RemoveAt(i);

                    ResolverFactories.Insert(0, resolverFactory);
                }

                RecordChange();
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
                ResolverFactories.Clear();
                ResolverFactories.Add(new BuiltInTypeHandlerResolverFactory());

                UserTypeMappings.Clear();

                RecordChange();
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        internal void RecordChange()
        {
            _mappingsByClrType.Clear();
            Interlocked.Increment(ref _changeCounter);
        }

        #endregion Mapping management

        #region NpgsqlDbType/DbType inference for NpgsqlParameter

        [RequiresUnreferencedCodeAttribute("ToNpgsqlDbType uses interface-based reflection and isn't trimming-safe")]
        internal bool TryResolveMappingByValue(object value, [NotNullWhen(true)] out TypeMappingInfo? typeMapping)
        {
            Lock.EnterReadLock();
            try
            {
                // We resolve as follows:
                // 1. Cached by-type lookup (fast path). This will work for almost all types after the very first resolution.
                // 2. Value-dependent type lookup (e.g. DateTime by Kind) via the resolvers. This includes complex types (e.g. array/range
                //    over DateTime), and the results cannot be cached.
                // 3. Uncached by-type lookup (for the very first resolution of a given type)

                var type = value.GetType();
                if (_mappingsByClrType.TryGetValue(type, out typeMapping))
                    return true;

                foreach (var resolverFactory in ResolverFactories)
                    if ((typeMapping = resolverFactory.GetMappingByValueDependentValue(value)) is not null)
                        return true;

                return TryResolveMappingByClrType(value.GetType(), out typeMapping);
            }
            finally
            {
                Lock.ExitReadLock();
            }

            bool TryResolveMappingByClrType(Type clrType, [NotNullWhen(true)] out TypeMappingInfo? typeMapping)
            {
                if (_mappingsByClrType.TryGetValue(clrType, out typeMapping))
                    return true;

                foreach (var resolverFactory in ResolverFactories)
                {
                    if ((typeMapping = resolverFactory.GetMappingByClrType(clrType)) is not null)
                    {
                        _mappingsByClrType[clrType] = typeMapping;
                        return true;
                    }
                }

                if (clrType.IsArray)
                {
                    if (TryResolveMappingByClrType(clrType.GetElementType()!, out var elementMapping))
                    {
                        _mappingsByClrType[clrType] = typeMapping = new(
                            NpgsqlDbType.Array | elementMapping.NpgsqlDbType,
                            elementMapping.DataTypeName + "[]");
                        return true;
                    }

                    typeMapping = null;
                    return false;
                }

                var typeInfo = clrType.GetTypeInfo();

                var ilist = typeInfo.ImplementedInterfaces.FirstOrDefault(x =>
                    x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
                if (ilist != null)
                {
                    if (TryResolveMappingByClrType(ilist.GetGenericArguments()[0], out var elementMapping))
                    {
                        _mappingsByClrType[clrType] = typeMapping = new(
                            NpgsqlDbType.Array | elementMapping.NpgsqlDbType,
                            elementMapping.DataTypeName + "[]");
                        return true;
                    }

                    typeMapping = null;
                    return false;
                }

                if (typeInfo.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
                {
                    if (TryResolveMappingByClrType(clrType.GetGenericArguments()[0], out var elementMapping))
                    {
                        _mappingsByClrType[clrType] = typeMapping = new(
                            NpgsqlDbType.Range | elementMapping.NpgsqlDbType,
                            dataTypeName: null);
                        return true;
                    }

                    typeMapping = null;
                    return false;
                }

                typeMapping = null;
                return false;
            }
        }

        #endregion NpgsqlDbType/DbType inference for NpgsqlParameter

        #region Static translation tables

        public static string? NpgsqlDbTypeToDataTypeName(NpgsqlDbType npgsqlDbType)
            => npgsqlDbType switch
            {
                // Numeric types
                NpgsqlDbType.Smallint => "smallint",
                NpgsqlDbType.Integer  => "integer",
                NpgsqlDbType.Bigint   => "bigint",
                NpgsqlDbType.Real     => "real",
                NpgsqlDbType.Double   => "double precision",
                NpgsqlDbType.Numeric  => "numeric",
                NpgsqlDbType.Money    => "money",

                // Text types
                NpgsqlDbType.Text      => "text",
                NpgsqlDbType.Xml       => "xml",
                NpgsqlDbType.Varchar   => "character varying",
                NpgsqlDbType.Char      => "character",
                NpgsqlDbType.Name      => "name",
                NpgsqlDbType.Refcursor => "refcursor",
                NpgsqlDbType.Citext    => "citext",
                NpgsqlDbType.Jsonb     => "jsonb",
                NpgsqlDbType.Json      => "json",
                NpgsqlDbType.JsonPath  => "jsonpath",

                // Date/time types
                NpgsqlDbType.Timestamp   => "timestamp without time zone",
                NpgsqlDbType.TimestampTz => "timestamp with time zone",
                NpgsqlDbType.Date        => "date",
                NpgsqlDbType.Time        => "time without time zone",
                NpgsqlDbType.TimeTz      => "time with time zone",
                NpgsqlDbType.Interval    => "interval",

                // Network types
                NpgsqlDbType.Cidr     => "cidr",
                NpgsqlDbType.Inet     => "inet",
                NpgsqlDbType.MacAddr  => "macaddr",
                NpgsqlDbType.MacAddr8 => "macaddr8",

                // Full-text search types
                NpgsqlDbType.TsQuery   => "tsquery",
                NpgsqlDbType.TsVector  => "tsvector",

                // Geometry types
                NpgsqlDbType.Box     => "box",
                NpgsqlDbType.Circle  => "circle",
                NpgsqlDbType.Line    => "line",
                NpgsqlDbType.LSeg    => "lseg",
                NpgsqlDbType.Path    => "path",
                NpgsqlDbType.Point   => "point",
                NpgsqlDbType.Polygon => "polygon",

                // LTree types
                NpgsqlDbType.LQuery    => "lquery",
                NpgsqlDbType.LTree     => "ltree",
                NpgsqlDbType.LTxtQuery => "ltxtquery",

                // UInt types
                NpgsqlDbType.Oid       => "oid",
                NpgsqlDbType.Xid       => "xid",
                NpgsqlDbType.Xid8      => "xid8",
                NpgsqlDbType.Cid       => "cid",
                NpgsqlDbType.Regtype   => "regtype",
                NpgsqlDbType.Regconfig => "regconfig",

                // Misc types
                NpgsqlDbType.Boolean => "bool",
                NpgsqlDbType.Bytea   => "bytea",
                NpgsqlDbType.Uuid    => "uuid",
                NpgsqlDbType.Varbit  => "bit varying",
                NpgsqlDbType.Bit     => "bit",
                NpgsqlDbType.Hstore  => "hstore",

                NpgsqlDbType.Geometry  => "geometry",
                NpgsqlDbType.Geography => "geography",

                // Built-in range types
                NpgsqlDbType.IntegerRange     => "int4range",
                NpgsqlDbType.BigIntRange      => "int8range",
                NpgsqlDbType.NumericRange     => "numrange",
                NpgsqlDbType.TimestampRange   => "tsrange",
                NpgsqlDbType.TimestampTzRange => "tstzrange",
                NpgsqlDbType.DateRange        => "daterange",

                // Built-in multirange types
                NpgsqlDbType.IntegerMultirange     => "int4multirange",
                NpgsqlDbType.BigIntMultirange      => "int8multirange",
                NpgsqlDbType.NumericMultirange     => "nummultirange",
                NpgsqlDbType.TimestampMultirange   => "tsmultirange",
                NpgsqlDbType.TimestampTzMultirange => "tstzmultirange",
                NpgsqlDbType.DateMultirange        => "datemultirange",

                // Internal types
                NpgsqlDbType.Int2Vector   => "int2vector",
                NpgsqlDbType.Oidvector    => "oidvector",
                NpgsqlDbType.PgLsn        => "pg_lsn",
                NpgsqlDbType.Tid          => "tid",
                NpgsqlDbType.InternalChar => "char",

                // Special types
                NpgsqlDbType.Unknown => "unknown",

                _ => npgsqlDbType.HasFlag(NpgsqlDbType.Array)
                    ? NpgsqlDbTypeToDataTypeName(npgsqlDbType & ~NpgsqlDbType.Array) + "[]"
                    : null // e.g. ranges
            };

        internal static NpgsqlDbType? DbTypeToNpgsqlDbType(DbType dbType)
            => dbType switch
            {
                DbType.AnsiString            => NpgsqlDbType.Text,
                DbType.Binary                => NpgsqlDbType.Bytea,
                DbType.Byte                  => NpgsqlDbType.Smallint,
                DbType.Boolean               => NpgsqlDbType.Boolean,
                DbType.Currency              => NpgsqlDbType.Money,
                DbType.Date                  => NpgsqlDbType.Date,
                DbType.DateTime              => LegacyTimestampBehavior ? NpgsqlDbType.Timestamp : NpgsqlDbType.TimestampTz,
                DbType.Decimal               => NpgsqlDbType.Numeric,
                DbType.VarNumeric            => NpgsqlDbType.Numeric,
                DbType.Double                => NpgsqlDbType.Double,
                DbType.Guid                  => NpgsqlDbType.Uuid,
                DbType.Int16                 => NpgsqlDbType.Smallint,
                DbType.Int32                 => NpgsqlDbType.Integer,
                DbType.Int64                 => NpgsqlDbType.Bigint,
                DbType.Single                => NpgsqlDbType.Real,
                DbType.String                => NpgsqlDbType.Text,
                DbType.Time                  => NpgsqlDbType.Time,
                DbType.AnsiStringFixedLength => NpgsqlDbType.Text,
                DbType.StringFixedLength     => NpgsqlDbType.Text,
                DbType.Xml                   => NpgsqlDbType.Xml,
                DbType.DateTime2             => NpgsqlDbType.Timestamp,
                DbType.DateTimeOffset        => NpgsqlDbType.TimestampTz,

                DbType.Object                => null,
                DbType.SByte                 => null,
                DbType.UInt16                => null,
                DbType.UInt32                => null,
                DbType.UInt64                => null,

                _ => throw new ArgumentOutOfRangeException(nameof(dbType), dbType, null)
            };

        internal static DbType NpgsqlDbTypeToDbType(NpgsqlDbType npgsqlDbType)
            => npgsqlDbType switch
            {
                // Numeric types
                NpgsqlDbType.Smallint    => DbType.Int16,
                NpgsqlDbType.Integer     => DbType.Int32,
                NpgsqlDbType.Bigint      => DbType.Int64,
                NpgsqlDbType.Real        => DbType.Single,
                NpgsqlDbType.Double      => DbType.Double,
                NpgsqlDbType.Numeric     => DbType.Decimal,
                NpgsqlDbType.Money       => DbType.Currency,

                // Text types
                NpgsqlDbType.Text        => DbType.String,
                NpgsqlDbType.Xml         => DbType.Xml,
                NpgsqlDbType.Varchar     => DbType.String,
                NpgsqlDbType.Char        => DbType.String,
                NpgsqlDbType.Name        => DbType.String,
                NpgsqlDbType.Refcursor   => DbType.String,
                NpgsqlDbType.Citext      => DbType.String,
                NpgsqlDbType.Jsonb       => DbType.Object,
                NpgsqlDbType.Json        => DbType.Object,
                NpgsqlDbType.JsonPath    => DbType.String,

                // Date/time types
                NpgsqlDbType.Timestamp   => LegacyTimestampBehavior ? DbType.DateTime : DbType.DateTime2,
                NpgsqlDbType.TimestampTz => LegacyTimestampBehavior ? DbType.DateTimeOffset : DbType.DateTime,
                NpgsqlDbType.Date        => DbType.Date,
                NpgsqlDbType.Time        => DbType.Time,

                // Misc data types
                NpgsqlDbType.Bytea       => DbType.Binary,
                NpgsqlDbType.Boolean     => DbType.Boolean,
                NpgsqlDbType.Uuid        => DbType.Guid,

                NpgsqlDbType.Unknown     => DbType.Object,

                _ => DbType.Object
            };

        #endregion Static translation tables
    }
}
