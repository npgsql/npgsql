using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

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

        internal readonly List<PostgresBaseType> BaseTypes = new List<PostgresBaseType>();
        internal readonly List<PostgresArrayType> ArrayTypes = new List<PostgresArrayType>();
        internal readonly List<PostgresRangeType> RangeTypes = new List<PostgresRangeType>();
        internal readonly List<PostgresEnumType> EnumTypes = new List<PostgresEnumType>();
        internal readonly List<PostgresDomainType> DomainTypes = new List<PostgresDomainType>();

        internal static async Task<DatabaseInfo> Load(NpgsqlConnector connector, NpgsqlTimeout timeout, bool async)
        {
            var csb = new NpgsqlConnectionStringBuilder(connector.ConnectionString);
            var db = new DatabaseInfo(csb.Host, csb.Port, csb.Database);
            await db.LoadBackendTypes(connector, timeout, async);
            return db;
        }

        static readonly string TypesQueryWithRange = GenerateTypesQuery(true);
        static readonly string TypesQueryWithoutRange = GenerateTypesQuery(false);

        static string GenerateTypesQuery(bool withRange)
        {
            // Select all types (base, array which is also base, enum, range, composite).
            // Note that arrays are distinguished from primitive types through them having typreceive=array_recv.
            // Order by primitives first, container later.
            // For arrays and ranges, join in the element OID and type (to filter out arrays of unhandled
            // types).
            return
                $@"SELECT ns.nspname, a.typname, a.oid, a.typrelid, a.typbasetype,
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
LEFT OUTER JOIN pg_type AS b ON (b.oid = a.typelem)
{(withRange ? "LEFT OUTER JOIN pg_range ON (pg_range.rngtypid = a.oid) " : "")}
WHERE
  (
    a.typtype IN ('b', 'r', 'e', 'd') AND
    (b.typtype IS NULL OR b.typtype IN ('b', 'r', 'e', 'd'))  /* Either non-array or array of supported element type */
  ) OR
  (a.typname IN ('record', 'void') AND a.typtype = 'p')
ORDER BY ord";
        }

        async Task LoadBackendTypes(NpgsqlConnector connector, NpgsqlTimeout timeout, bool async)
        {
            var commandTimeout = 0;  // Default to infinity
            if (timeout.IsSet)
            {
                commandTimeout = (int)timeout.TimeLeft.TotalSeconds;
                if (commandTimeout <= 0)
                    throw new TimeoutException();
            }

            using (var command = new NpgsqlCommand(connector.SupportsRangeTypes ? TypesQueryWithRange : TypesQueryWithoutRange, connector.Connection))
            {
                command.CommandTimeout = commandTimeout;
                command.AllResultTypesAreUnknown = true;
                using (var reader = async ? await command.ExecuteReaderAsync() : command.ExecuteReader())
                {
                    while (async ? await reader.ReadAsync() : reader.Read())
                    {
                        timeout.Check();
                        LoadBackendType(reader, connector);
                    }
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

            uint elementOID;
            var typeChar = reader.GetString(reader.GetOrdinal("type"))[0];
            switch (typeChar)
            {
            case 'b':  // Normal base type
                var baseType = new PostgresBaseType(ns, name, oid);
                Add(ns, name, oid, baseType);
                BaseTypes.Add(baseType);
                return;

            case 'a':   // Array
                elementOID = Convert.ToUInt32(reader[reader.GetOrdinal("elemoid")]);
                Debug.Assert(elementOID > 0);
                if (!ByOID.TryGetValue(elementOID, out var elementPostgresType))
                {
                    Log.Trace($"Array type '{name}' refers to unknown element with OID {elementOID}, skipping", connector.Id);
                    return;
                }
                var arrayType = new PostgresArrayType(ns, name, oid, elementPostgresType);
                Add(ns, name, oid, arrayType);
                ArrayTypes.Add(arrayType);
                return;

            case 'r':   // Range
                elementOID = Convert.ToUInt32(reader[reader.GetOrdinal("elemoid")]);
                Debug.Assert(elementOID > 0);
                if (!ByOID.TryGetValue(elementOID, out elementPostgresType))
                {
                    Log.Trace($"Range type '{name}' refers to unknown subtype with OID {elementOID}, skipping", connector.Id);
                    return;
                }
                var rangeType = new PostgresRangeType(ns, name, oid, elementPostgresType);
                Add(ns, name, oid, rangeType);
                RangeTypes.Add(rangeType);
                return;

            case 'e':   // Enum
                var enumType = new PostgresEnumType(ns, name, oid);
                Add(ns, name, oid, enumType);
                EnumTypes.Add(enumType);
                return;

            case 'd':   // Domain
                var baseTypeOID = Convert.ToUInt32(reader[reader.GetOrdinal("typbasetype")]);
                Debug.Assert(baseTypeOID > 0);
                PostgresType basePostgresType;
                if (!ByOID.TryGetValue(baseTypeOID, out basePostgresType))
                {
                    Log.Trace($"Domain type '{name}' refers to unknown base type with OID {baseTypeOID}, skipping", connector.Id);
                    return;
                }
                var domainType = new PostgresDomainType(ns, name, oid, basePostgresType);
                Add(ns, name, oid, domainType);
                DomainTypes.Add(domainType);
                return;

            case 'p':   // pseudo-type (record, void)
                // Hack this as a base type
                goto case 'b';

            // Note that composite types are only loaded on-demand because of their potential large number
            // (a composite exists for each table, see #1126)

            default:
                throw new ArgumentOutOfRangeException($"Unknown typtype for type '{name}' in pg_type: {typeChar}");
            }
        }

        internal PostgresCompositeType GetComposite(string pgName, NpgsqlConnection connection)
        {
            // First check if the composite type definition has already been loaded from the database
            if (pgName.IndexOf('.') == -1
                ? ByName.TryGetValue(pgName, out var postgresType)
                : ByFullName.TryGetValue(pgName, out postgresType))
            {
                var asComposite = postgresType as PostgresCompositeType;
                if (asComposite == null)
                    throw new ArgumentException($"Type {pgName} was found but is not a composite");
                return asComposite;
            }

            // This is the first time the composite is mapped, the type definition needs to be loaded
            string name, schema;
            var i = pgName.IndexOf('.');
            if (i == -1)
            {
                schema = null;
                name = pgName;
            }
            else
            {
                schema = pgName.Substring(0, i);
                name = pgName.Substring(i + 1);
            }

            using (var cmd = new NpgsqlCommand(GenerateLoadCompositeQuery(schema != null), connection))
            {
                cmd.Parameters.AddWithValue("name", name);
                if (schema != null)
                    cmd.Parameters.AddWithValue("schema", schema);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new ArgumentException($"An PostgreSQL type with the name {pgName} was not found in the database");

                    // Load some info on the composite type itself, do some checks
                    var ns = reader.GetString(0);
                    Debug.Assert(schema == null || ns == schema);
                    var oid = reader.GetFieldValue<uint>(1);
                    var typeChar = reader.GetChar(2);
                    if (typeChar != 'c')
                        throw new ArgumentException($"Type {pgName} was found in the database but is not a composite");
                    if (reader.Read())
                    {
                        // More than one composite type matched, the user didn't specify a schema and the same name
                        // exists in more than one schema
                        Debug.Assert(schema == null);
                        var ns2 = reader.GetString(0);
                        throw new ArgumentException($"More than one composite types with name {name} where found (in schemas {ns} and {ns2}). Please qualify with a schema.");
                    }

                    reader.NextResult(); // Load the fields

                    var fields = new List<PostgresCompositeType.Field>();
                    while (reader.Read())
                    {
                        fields.Add(new PostgresCompositeType.Field
                        {
                            PgName = reader.GetString(0),
                            TypeOID = reader.GetFieldValue<uint>(1)
                        });
                    }

                    var compositeType = new PostgresCompositeType(ns, name, oid, fields);
                    Add(ns, name, oid, compositeType);

                    reader.NextResult(); // Load the array type

                    if (reader.Read())
                    {
                        var arrayNs = reader.GetString(0);
                        var arrayName = reader.GetString(1);
                        var arrayOID = reader.GetFieldValue<uint>(2);

                        Add(arrayNs, arrayName, arrayOID,
                            new PostgresArrayType(arrayNs, arrayName, arrayOID, compositeType));
                    }
                    else
                        Log.Warn($"Could not find array type corresponding to composite {pgName}");

                    return compositeType;
                }
            }
        }

        static string GenerateLoadCompositeQuery(bool withSchema) =>
            $@"SELECT ns.nspname, typ.oid, typ.typtype
FROM pg_type AS typ
JOIN pg_namespace AS ns ON (ns.oid = typ.typnamespace)
WHERE (typ.typname = @name{(withSchema ? " AND ns.nspname = @schema" : "")});

SELECT att.attname, att.atttypid
FROM pg_type AS typ
JOIN pg_namespace AS ns ON (ns.oid = typ.typnamespace)
JOIN pg_attribute AS att ON (att.attrelid = typ.typrelid)
WHERE
  typ.typname = @name{(withSchema ? " AND ns.nspname = @schema" : "")} AND
  attnum > 0 AND     /* Don't load system attributes */
  NOT attisdropped;

SELECT ns.nspname, a.typname, a.oid
FROM pg_type AS a
JOIN pg_type AS b ON (b.oid = a.typelem)
JOIN pg_namespace AS ns ON (ns.oid = b.typnamespace)
WHERE a.typtype = 'b' AND b.typname = @name{(withSchema ? " AND ns.nspname = @schema" : "")}";

        void Add(string ns, string name, uint oid, PostgresType pgType)
        {
            ByOID[oid] = pgType;
            ByFullName[$"{ns}.{name}"] = pgType;
            ByName[name] = ByName.ContainsKey(name)
                ? null
                : pgType;

        }
    }
}
