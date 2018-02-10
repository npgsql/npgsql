using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Npgsql.Logging;
using Npgsql.PostgresTypes;

namespace Npgsql
{
    /// <summary>
    /// Contains information about a single PostgreSQL database, especially its types.
    /// </summary>
    class DatabaseInfo
    {
        public string Host { get; }
        public int Port { get; }
        public string DatabaseName { get; }

        internal Dictionary<uint, PostgresType> ByOID { get; } = new Dictionary<uint, PostgresType>();

        /// <summary>
        /// Indexes backend types by their PostgreSQL name, including namespace (e.g. pg_catalog.int4).
        /// Only used for enums and composites.
        /// </summary>
        internal Dictionary<string, PostgresType> ByFullName { get; } = new Dictionary<string, PostgresType>();

        /// <summary>
        /// Indexes backend types by their PostgreSQL name, not including namespace.
        /// If more than one type exists with the same name (i.e. in different namespaces) this
        /// table will contain an entry with a null value.
        /// Only used for enums and composites.
        /// </summary>
        internal Dictionary<string, PostgresType> ByName { get; } = new Dictionary<string, PostgresType>();

        internal static ConcurrentDictionary<string, DatabaseInfo> Cache = new ConcurrentDictionary<string, DatabaseInfo>();

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        DatabaseInfo(string host, int port, string databaseName)
        {
            Host = host;
            Port = port;
            DatabaseName = databaseName;
        }

        internal readonly List<PostgresBaseType>      BaseTypes      = new List<PostgresBaseType>();
        internal readonly List<PostgresArrayType>     ArrayTypes     = new List<PostgresArrayType>();
        internal readonly List<PostgresRangeType>     RangeTypes     = new List<PostgresRangeType>();
        internal readonly List<PostgresEnumType>      EnumTypes      = new List<PostgresEnumType>();
        internal readonly List<PostgresCompositeType> CompositeTypes = new List<PostgresCompositeType>();
        internal readonly List<PostgresDomainType>    DomainTypes    = new List<PostgresDomainType>();

        internal static async Task<DatabaseInfo> Load(NpgsqlConnector connector, NpgsqlTimeout timeout, bool async)
        {
            var csb = new NpgsqlConnectionStringBuilder(connector.ConnectionString);
            var db = new DatabaseInfo(csb.Host, csb.Port, csb.Database);
            await db.LoadBackendTypes(connector, timeout, async);
            return db;
        }

        // Select all types (base, array which is also base, enum, range, composite).
        // Note that arrays are distinguished from primitive types through them having typreceive=array_recv.
        // Order by primitives first, container later.
        // For arrays and ranges, join in the element OID and type (to filter out arrays of unhandled
        // types).
        static string GenerateTypesQuery(bool withRange, bool loadTableComposites)
            => $@"
/*** Load all supported types ***/
SELECT ns.nspname, a.typname, a.oid, a.typrelid, a.typbasetype,
CASE WHEN pg_proc.proname='array_recv' THEN 'a' ELSE a.typtype END AS type,
CASE
  WHEN pg_proc.proname='array_recv' THEN a.typelem
  {(withRange ? "WHEN a.typtype='r' THEN rngsubtype" : "")}
  ELSE 0
END AS elemoid,
CASE
  WHEN pg_proc.proname IN ('array_recv','oidvectorrecv') THEN 3    /* Arrays last */
  WHEN a.typtype='r' THEN 2                                        /* Ranges before */
  WHEN a.typtype='d' THEN 1                                        /* Domains before */
  ELSE 0                                                           /* Base types first */
END AS ord
FROM pg_type AS a
JOIN pg_namespace AS ns ON (ns.oid = a.typnamespace)
JOIN pg_proc ON pg_proc.oid = a.typreceive
LEFT OUTER JOIN pg_class AS cls ON (cls.oid = a.typrelid)
LEFT OUTER JOIN pg_type AS b ON (b.oid = a.typelem)
LEFT OUTER JOIN pg_class AS elemcls ON (elemcls.oid = b.typrelid)
{(withRange ? "LEFT OUTER JOIN pg_range ON (pg_range.rngtypid = a.oid) " : "")}
WHERE
  a.typtype IN ('b', 'r', 'e', 'd') OR         /* Base, range, enum, domain */
  (a.typtype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "cls.relkind='c'")}) OR /* User-defined free-standing composites (not table composites) by default */
  (pg_proc.proname='array_recv' AND (
    b.typtype IN ('b', 'r', 'e', 'd') OR       /* Array of base, range, enum domain */
    (b.typtype = 'c' AND elemcls.relkind='c')  /* Array of user-defined free-standing composites (not table composites) */
  )) OR
  (a.typtype = 'p' AND a.typname IN ('record', 'void'))  /* Some special supported pseudo-types */
ORDER BY ord;

/*** Load field definitions for (free-standing) composite types ***/
SELECT ns.nspname, typ.typname, att.attname, att.atttypid
FROM pg_type AS typ
JOIN pg_namespace AS ns ON (ns.oid = typ.typnamespace)
JOIN pg_class AS cls ON (cls.oid = typ.typrelid)
JOIN pg_attribute AS att ON (att.attrelid = typ.typrelid)
WHERE
  (typ.typtype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "cls.relkind='c'")}) AND
  attnum > 0 AND     /* Don't load system attributes */
  NOT attisdropped
ORDER BY typ.typname, att.attnum;
";

        async Task LoadBackendTypes(NpgsqlConnector connector, NpgsqlTimeout timeout, bool async)
        {
            var commandTimeout = 0;  // Default to infinity
            if (timeout.IsSet)
            {
                commandTimeout = (int)timeout.TimeLeft.TotalSeconds;
                if (commandTimeout <= 0)
                    throw new TimeoutException();
            }

            var typeLoadingQuery = GenerateTypesQuery(connector.SupportsRangeTypes, connector.Settings.LoadTableComposites);

            using (var command = new NpgsqlCommand(typeLoadingQuery, connector.Connection))
            {
                command.CommandTimeout = commandTimeout;
                command.AllResultTypesAreUnknown = true;
                using (var reader = async ? await command.ExecuteReaderAsync() : command.ExecuteReader())
                {
                    // First load the types themselves
                    while (async ? await reader.ReadAsync() : reader.Read())
                    {
                        timeout.Check();
                        LoadBackendType(reader, connector);
                    }

                    if (async)
                        await reader.NextResultAsync();
                    else
                        reader.NextResult();

                    // Now load the composite types' fields
                    LoadCompositeFields(reader);
                }
            }
        }

        void LoadBackendType(DbDataReader reader, NpgsqlConnector connector)
        {
            var ns = reader.GetString(reader.GetOrdinal("nspname"));
            var name = reader.GetString(reader.GetOrdinal("typname"));
            var oid = Convert.ToUInt32(reader[reader.GetOrdinal("oid")]);

            Debug.Assert(name != null);
            Debug.Assert(oid != 0);

            var typeChar = reader.GetString(reader.GetOrdinal("type"))[0];
            switch (typeChar)
            {
            case 'b':  // Normal base type
                var baseType = new PostgresBaseType(ns, name, oid);
                Add(baseType);
                BaseTypes.Add(baseType);
                return;

            case 'a': // Array
            {
                var elementOID = Convert.ToUInt32(reader[reader.GetOrdinal("elemoid")]);
                Debug.Assert(elementOID > 0);
                if (!ByOID.TryGetValue(elementOID, out var elementPostgresType))
                {
                    Log.Trace($"Array type '{name}' refers to unknown element with OID {elementOID}, skipping", connector.Id);
                    return;
                }

                var arrayType = new PostgresArrayType(ns, name, oid, elementPostgresType);
                Add(arrayType);
                ArrayTypes.Add(arrayType);
                return;
            }

            case 'r': // Range
            {
                var elementOID = Convert.ToUInt32(reader[reader.GetOrdinal("elemoid")]);
                Debug.Assert(elementOID > 0);
                if (!ByOID.TryGetValue(elementOID, out var subtypePostgresType))
                {
                    Log.Trace($"Range type '{name}' refers to unknown subtype with OID {elementOID}, skipping", connector.Id);
                    return;
                }

                var rangeType = new PostgresRangeType(ns, name, oid, subtypePostgresType);
                Add(rangeType);
                RangeTypes.Add(rangeType);
                return;
            }

            case 'e':   // Enum
                var enumType = new PostgresEnumType(ns, name, oid);
                Add(enumType);
                EnumTypes.Add(enumType);
                return;

            case 'c':   // Composite
                var compositeType = new PostgresCompositeType(ns, name, oid);
                Add(compositeType);
                CompositeTypes.Add(compositeType);
                return;

            case 'd':   // Domain
                var baseTypeOID = Convert.ToUInt32(reader[reader.GetOrdinal("typbasetype")]);
                Debug.Assert(baseTypeOID > 0);
                if (!ByOID.TryGetValue(baseTypeOID, out var basePostgresType))
                {
                    Log.Trace($"Domain type '{name}' refers to unknown base type with OID {baseTypeOID}, skipping", connector.Id);
                    return;
                }
                var domainType = new PostgresDomainType(ns, name, oid, basePostgresType);
                Add(domainType);
                DomainTypes.Add(domainType);
                return;

            case 'p':   // pseudo-type (record, void)
                // Hack this as a base type
                goto case 'b';

            default:
                throw new ArgumentOutOfRangeException($"Unknown typtype for type '{name}' in pg_type: {typeChar}");
            }
        }

        void LoadCompositeFields(DbDataReader reader)
        {
            PostgresCompositeType currentComposite = null;
            while (reader.Read())
            {
                var ns = reader.GetString(reader.GetOrdinal("nspname"));
                var name = reader.GetString(reader.GetOrdinal("typname"));
                var fullName = $"{ns}.{name}";
                if (fullName != currentComposite?.FullName)
                {
                    currentComposite = ByFullName[fullName] as PostgresCompositeType;
                    if (currentComposite == null)
                    {
                        Log.Error($"Ignoring non-composite type {fullName} when trying to load composite fields");
                        continue;
                    }
                }
                currentComposite.Fields.Add(new PostgresCompositeType.Field
                {
                    PgName = reader.GetString(reader.GetOrdinal("attname")),
                    TypeOID = Convert.ToUInt32(reader[reader.GetOrdinal("atttypid")])
                });
            }

            // Our pass above loaded composite fields with their type OID only, do a second
            // pass to resolve them to PostgresType references (since types can come in any order)
            foreach (var compositeType in CompositeTypes.ToArray())
            {
                foreach (var field in compositeType.Fields)
                {
                    if (!ByOID.TryGetValue(field.TypeOID, out field.Type))
                    {
                        Log.Error($"Skipping composite type {compositeType.DisplayName} with field {field.PgName} with type OID {field.TypeOID}, which could not be resolved to a PostgreSQL type.");
                        Remove(compositeType);
                        CompositeTypes.Remove(compositeType);
                        goto outer;
                    }
                }
                // ReSharper disable once RedundantJumpStatement
                outer: continue;
            }
        }

        void Add(PostgresType pgType)
        {
            ByOID[pgType.OID] = pgType;
            ByFullName[pgType.FullName] = pgType;
            // If more than one type exists with the same partial name, we place a null value.
            // This allows us to detect this case later and force the user to use full names only.
            ByName[pgType.Name] = ByName.ContainsKey(pgType.Name)
                ? null
                : pgType;
        }

        void Remove(PostgresType pgType)
        {
            ByOID.Remove(pgType.OID);
            ByName.Remove(pgType.Name);
            ByFullName.Remove(pgType.FullName);
        }
    }
}
