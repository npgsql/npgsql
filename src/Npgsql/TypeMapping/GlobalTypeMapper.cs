using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Npgsql.Internal.TypeHandling;
using Npgsql.NameTranslation;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    sealed class GlobalTypeMapper : TypeMapperBase
    {
        public static GlobalTypeMapper Instance { get; }

        internal List<ITypeHandlerResolverFactory> ResolverFactories { get; } = new();
        internal Dictionary<string, IUserTypeMapping> UserTypeMappings { get; } = new();

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
                var removed = UserTypeMappings.Remove(pgName);
                RecordChange();
                return removed;
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
                var removed = UserTypeMappings.Remove(pgName);
                RecordChange();
                return removed;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override void AddTypeResolverFactory(ITypeHandlerResolverFactory resolverFactory)
        {
            Lock.EnterWriteLock();
            try
            {
                ResolverFactories.Insert(0, resolverFactory);
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

        internal void RecordChange() => Interlocked.Increment(ref _changeCounter);

        #endregion Mapping management

        #region NpgsqlDbType/DbType inference for NpgsqlParameter

        [RequiresUnreferencedCodeAttribute("ToNpgsqlDbType uses interface-based reflection and isn't trimming-safe")]
        internal bool TryResolveMappingByClrType(Type clrType, [NotNullWhen(true)] out TypeMappingInfo? typeMapping)
        {
            Lock.EnterReadLock();
            try
            {
                foreach (var resolverFactory in ResolverFactories)
                    if ((typeMapping = resolverFactory.GetMappingByClrType(clrType)) is not null)
                        return true;

                if (clrType.IsArray)
                {
                    if (TryResolveMappingByClrType(clrType.GetElementType()!, out var elementMapping))
                    {
                        typeMapping = new(
                            NpgsqlDbType.Array | elementMapping.NpgsqlDbType,
                            DbType.Object,
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
                        typeMapping = new(
                            NpgsqlDbType.Array | elementMapping.NpgsqlDbType,
                            DbType.Object,
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
                        typeMapping = new(
                            NpgsqlDbType.Range | elementMapping.NpgsqlDbType,
                            DbType.Object,
                            dataTypeName: null);
                        return true;
                    }

                    typeMapping = null;
                    return false;
                }

                throw new NotSupportedException("Can't infer NpgsqlDbType for type " + clrType);
            }
            finally
            {
                Lock.ExitReadLock();
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
                DbType.DateTime              => NpgsqlDbType.Timestamp,
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
                NpgsqlDbType.Jsonb       => DbType.String,
                NpgsqlDbType.Json        => DbType.String,
                NpgsqlDbType.JsonPath    => DbType.String,

                // Date/time types
                NpgsqlDbType.Timestamp   => DbType.DateTime,
                NpgsqlDbType.TimestampTz => DbType.DateTimeOffset,
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
