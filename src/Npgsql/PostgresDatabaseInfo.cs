using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using Npgsql.Util;

// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace Npgsql
{
    /// <summary>
    /// The default implementation of <see cref="INpgsqlDatabaseInfoFactory"/>, for standard PostgreSQL databases..
    /// </summary>
    class PostgresDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
    {
        /// <inheritdoc />
        public async Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
        {
            var db = new PostgresDatabaseInfo(conn);
            await db.LoadPostgresInfo(conn, timeout, async);
            Debug.Assert(db.LongVersion != null);
            return db;
        }
    }

    /// <summary>
    /// The default implementation of NpgsqlDatabase, for standard PostgreSQL databases.
    /// </summary>
    class PostgresDatabaseInfo : NpgsqlDatabaseInfo
    {
        /// <summary>
        /// The Npgsql logger instance.
        /// </summary>
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(PostgresDatabaseInfo));

        /// <summary>
        /// The PostgreSQL types detected in the database.
        /// </summary>
        List<PostgresType>? _types;

        /// <inheritdoc />
        protected override IEnumerable<PostgresType> GetTypes() => _types ?? Enumerable.Empty<PostgresType>();

        /// <summary>
        /// The PostgreSQL version string as returned by the version() function. Populated during loading.
        /// </summary>
        public string LongVersion { get; set; } = default!;

        /// <summary>
        /// True if the backend is Amazon Redshift; otherwise, false.
        /// </summary>
        public bool IsRedshift { get; private set; }

        /// <inheritdoc />
        public override bool SupportsUnlisten => Version >= new Version(6, 4, 0) && !IsRedshift;

        /// <summary>
        /// True if the 'pg_enum' table includes the 'enumsortorder' column; otherwise, false.
        /// </summary>
        public virtual bool HasEnumSortOrder => Version >= new Version(9, 1, 0);

        /// <summary>
        /// True if the 'pg_type' table includes the 'typcategory' column; otherwise, false.
        /// </summary>
        /// <remarks>
        /// pg_type.typcategory is added after 8.4.
        /// see: https://www.postgresql.org/docs/8.4/static/catalog-pg-type.html#CATALOG-TYPCATEGORY-TABLE
        /// </remarks>
        public virtual bool HasTypeCategory => Version >= new Version(8, 4, 0);

        internal PostgresDatabaseInfo(NpgsqlConnection conn)
            : base(conn.Host!, conn.Port, conn.Database!, ParseServerVersion(conn.PostgresParameters["server_version"]))
        {
        }

        /// <summary>
        /// Loads database information from the PostgreSQL database specified by <paramref name="conn"/>.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <param name="timeout">The timeout while loading types from the backend.</param>
        /// <param name="async">True to load types asynchronously.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        internal async Task LoadPostgresInfo(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
        {
            HasIntegerDateTimes =
                conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) &&
                intDateTimes == "on";

            IsRedshift = conn.Settings.ServerCompatibilityMode == ServerCompatibilityMode.Redshift;
            _types = await LoadBackendTypes(conn, timeout, async);
        }

        /// <summary>
        /// Generates a raw SQL query string to select type information.
        /// </summary>
        /// <param name="withRange">True to load range types.</param>
        /// <param name="withEnum">True to load enum types.</param>
        /// <param name="withEnumSortOrder"></param>
        /// <param name="loadTableComposites">True to load table composites.</param>
        /// <returns>
        /// A raw SQL query string that selects type information.
        /// </returns>
        /// <remarks>
        /// Select all types (base, array which is also base, enum, range, composite).
        /// Note that arrays are distinguished from primitive types through them having typreceive=array_recv.
        /// Order by primitives first, container later.
        /// For arrays and ranges, join in the element OID and type (to filter out arrays of unhandled
        /// types).
        /// </remarks>
        static string GenerateTypesQuery(bool withRange, bool withEnum, bool withEnumSortOrder,
            bool loadTableComposites)
            => $@"
SELECT version();

SELECT ns.nspname, typ_and_elem_type.*,
   CASE
       WHEN typtype IN ('b', 'e', 'p') THEN 0           -- First base types, enums, pseudo-types
       WHEN typtype = 'r' THEN 1                        -- Ranges after
       WHEN typtype = 'c' THEN 2                        -- Composites after
       WHEN typtype = 'd' AND elemtyptype <> 'a' THEN 3 -- Domains over non-arrays after
       WHEN typtype = 'a' THEN 4                        -- Arrays before
       WHEN typtype = 'd' AND elemtyptype = 'a' THEN 5  -- Domains over arrays last
    END AS ord
FROM (
    -- Arrays have typtype=b - this subquery identifies them by their typreceive and converts their typtype to a
    -- We first do this for the type (innerest-most subquery), and then for its element type
    -- This also returns the array element, range subtype and domain base type as elemtypoid
    SELECT
        typ.oid, typ.typnamespace, typ.typname, typ.typtype, typ.typrelid, typ.typnotnull, typ.relkind,
        elemtyp.oid AS elemtypoid, elemtyp.typname AS elemtypname, elemcls.relkind AS elemrelkind,
        CASE WHEN elemproc.proname='array_recv' THEN 'a' ELSE elemtyp.typtype END AS elemtyptype
    FROM (
        SELECT typ.oid, typnamespace, typname, typrelid, typnotnull, relkind, typelem AS elemoid,
            CASE WHEN proc.proname='array_recv' THEN 'a' ELSE typ.typtype END AS typtype,
            CASE
                WHEN proc.proname='array_recv' THEN typ.typelem
                {(withRange ? "WHEN typ.typtype='r' THEN rngsubtype" : "")}
                WHEN typ.typtype='d' THEN typ.typbasetype
            END AS elemtypoid
        FROM pg_type AS typ
        LEFT JOIN pg_class AS cls ON (cls.oid = typ.typrelid)
        LEFT JOIN pg_proc AS proc ON proc.oid = typ.typreceive
        {(withRange ? "LEFT JOIN pg_range ON (pg_range.rngtypid = typ.oid)" : "")}
    ) AS typ
    LEFT JOIN pg_type AS elemtyp ON elemtyp.oid = elemtypoid
    LEFT JOIN pg_class AS elemcls ON (elemcls.oid = elemtyp.typrelid)
    LEFT JOIN pg_proc AS elemproc ON elemproc.oid = elemtyp.typreceive
) AS typ_and_elem_type
JOIN pg_namespace AS ns ON (ns.oid = typnamespace)
WHERE
    typtype IN ('b', 'r', 'e', 'd') OR -- Base, range, enum, domain
    (typtype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "relkind='c'")}) OR -- User-defined free-standing composites (not table composites) by default
    (typtype = 'p' AND typname IN ('record', 'void')) OR -- Some special supported pseudo-types
    (typtype = 'a' AND (  -- Array of...
        elemtyptype IN ('b', 'r', 'e', 'd') OR -- Array of base, range, enum, domain
        (elemtyptype = 'p' AND elemtypname IN ('record', 'void')) OR -- Arrays of special supported pseudo-types
        (elemtyptype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "elemrelkind='c'")}) -- Array of user-defined free-standing composites (not table composites) by default
    ))
ORDER BY ord;

-- Load field definitions for (free-standing) composite types
SELECT typ.oid, att.attname, att.atttypid
FROM pg_type AS typ
JOIN pg_namespace AS ns ON (ns.oid = typ.typnamespace)
JOIN pg_class AS cls ON (cls.oid = typ.typrelid)
JOIN pg_attribute AS att ON (att.attrelid = typ.typrelid)
WHERE
  (typ.typtype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "cls.relkind='c'")}) AND
  attnum > 0 AND     -- Don't load system attributes
  NOT attisdropped
ORDER BY typ.oid, att.attnum;

{(withEnum ? $@"
-- Load enum fields
SELECT pg_type.oid, enumlabel
FROM pg_enum
JOIN pg_type ON pg_type.oid=enumtypid
ORDER BY oid{(withEnumSortOrder ? ", enumsortorder" : "")};" : "")}
";

        /// <summary>
        /// Loads type information from the backend specified by <paramref name="conn"/>.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <param name="timeout">The timeout while loading types from the backend.</param>
        /// <param name="async">True to load types asynchronously.</param>
        /// <returns>
        /// A collection of types loaded from the backend.
        /// </returns>
        /// <exception cref="TimeoutException" />
        /// <exception cref="ArgumentOutOfRangeException">Unknown typtype for type '{internalName}' in pg_type: {typeChar}.</exception>
        internal async Task<List<PostgresType>> LoadBackendTypes(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
        {
            var commandTimeout = 0;  // Default to infinity
            if (timeout.IsSet)
            {
                commandTimeout = (int)timeout.TimeLeft.TotalSeconds;
                if (commandTimeout <= 0)
                    throw new TimeoutException();
            }

            var typeLoadingQuery = GenerateTypesQuery(SupportsRangeTypes, SupportsEnumTypes, HasEnumSortOrder, conn.Settings.LoadTableComposites);
            using var command = new NpgsqlCommand(typeLoadingQuery, conn)
            {
                CommandTimeout = commandTimeout,
                AllResultTypesAreUnknown = true
            };

            timeout.CheckAndApply(conn.Connector!);
            using var reader = async ? await command.ExecuteReaderAsync() : command.ExecuteReader();
            var byOID = new Dictionary<uint, PostgresType>();

            // First the PostgreSQL version
            if (async)
            {
                await reader.ReadAsync();
                LongVersion = reader.GetString(0);
                await reader.NextResultAsync();
            }
            else
            {
                reader.Read();
                LongVersion = reader.GetString(0);
                reader.NextResult();
            }

            // Then load the types
            while (async ? await reader.ReadAsync() : reader.Read())
            {
                var ns = reader.GetString("nspname");
                var internalName = reader.GetString("typname");
                var oid = uint.Parse(reader.GetString("oid"), NumberFormatInfo.InvariantInfo);
                Debug.Assert(oid != 0);

                var elementOID = reader.IsDBNull("elemtypoid")
                    ? 0
                    : uint.Parse(reader.GetString("elemtypoid"), NumberFormatInfo.InvariantInfo);

                var typeChar = reader.GetChar("typtype");
                switch (typeChar)
                {
                    case 'b':  // Normal base type
                        var baseType = new PostgresBaseType(ns, internalName, oid);
                        byOID[baseType.OID] = baseType;
                        continue;

                    case 'a': // Array
                    {
                        Debug.Assert(elementOID > 0);
                        if (!byOID.TryGetValue(elementOID, out var elementPostgresType))
                        {
                            Log.Trace($"Array type '{internalName}' refers to unknown element with OID {elementOID}, skipping", conn.ProcessID);
                            continue;
                        }

                        var arrayType = new PostgresArrayType(ns, internalName, oid, elementPostgresType);
                        byOID[arrayType.OID] = arrayType;
                        continue;
                    }

                    case 'r': // Range
                    {
                        Debug.Assert(elementOID > 0);
                        if (!byOID.TryGetValue(elementOID, out var subtypePostgresType))
                        {
                            Log.Trace($"Range type '{internalName}' refers to unknown subtype with OID {elementOID}, skipping", conn.ProcessID);
                            continue;
                        }

                        var rangeType = new PostgresRangeType(ns, internalName, oid, subtypePostgresType);
                        byOID[rangeType.OID] = rangeType;
                        continue;
                    }

                    case 'e':   // Enum
                        var enumType = new PostgresEnumType(ns, internalName, oid);
                        byOID[enumType.OID] = enumType;
                        continue;

                    case 'c':   // Composite
                        var compositeType = new PostgresCompositeType(ns, internalName, oid);
                        byOID[compositeType.OID] = compositeType;
                        continue;

                    case 'd':   // Domain
                        Debug.Assert(elementOID > 0);
                        if (!byOID.TryGetValue(elementOID, out var basePostgresType))
                        {
                            Log.Trace($"Domain type '{internalName}' refers to unknown base type with OID {elementOID}, skipping", conn.ProcessID);
                            continue;
                        }
                        var domainType = new PostgresDomainType(ns, internalName, oid, basePostgresType, reader.GetString("typnotnull") == "t");
                        byOID[domainType.OID] = domainType;
                        continue;

                    case 'p':   // pseudo-type (record, void)
                        goto case 'b';  // Hack this as a base type

                    default:
                        throw new ArgumentOutOfRangeException($"Unknown typtype for type '{internalName}' in pg_type: {typeChar}");
                }
            }

            if (async)
                await reader.NextResultAsync();
            else
                reader.NextResult();

            LoadCompositeFields(reader, byOID);

            if (SupportsEnumTypes)
            {
                if (async)
                    await reader.NextResultAsync();
                else
                    reader.NextResult();

                LoadEnumLabels(reader, byOID);
            }

            return byOID.Values.ToList();
        }

        /// <summary>
        /// Loads composite fields for the composite type specified by the OID.
        /// </summary>
        /// <param name="reader">The reader from which to read composite fields.</param>
        /// <param name="byOID">The OID of the composite type for which fields are read.</param>
        static void LoadCompositeFields(NpgsqlDataReader reader, Dictionary<uint, PostgresType> byOID)
        {
            var currentOID = uint.MaxValue;
            PostgresCompositeType? currentComposite = null;
            var skipCurrent = false;

            while (reader.Read())
            {
                var oid = uint.Parse(reader.GetString("oid"), NumberFormatInfo.InvariantInfo);
                if (oid != currentOID)
                {
                    currentOID = oid;

                    if (!byOID.TryGetValue(oid, out var type))  // See #2020
                    {
                        Log.Warn($"Skipping composite type with OID {oid} which was not found in pg_type");
                        byOID.Remove(oid);
                        skipCurrent = true;
                        continue;
                    }

                    currentComposite = type as PostgresCompositeType;
                    if (currentComposite == null)
                    {
                        Log.Warn($"Type {type.Name} was referenced as a composite type but is a {type.GetType()}");
                        byOID.Remove(oid);
                        skipCurrent = true;
                        continue;
                    }

                    skipCurrent = false;
                }

                if (skipCurrent)
                    continue;

                var fieldName = reader.GetString("attname");
                var fieldTypeOID = uint.Parse(reader.GetString("atttypid"), NumberFormatInfo.InvariantInfo);
                if (!byOID.TryGetValue(fieldTypeOID, out var fieldType))  // See #2020
                {
                    Log.Warn($"Skipping composite type {currentComposite!.DisplayName} with field {fieldName} with type OID {fieldTypeOID}, which could not be resolved to a PostgreSQL type.");
                    byOID.Remove(oid);
                    skipCurrent = true;
                    continue;
                }

                currentComposite!.MutableFields.Add(new PostgresCompositeType.Field(fieldName, fieldType));
            }
        }

        /// <summary>
        /// Loads enum labels for the enum type specified by the OID.
        /// </summary>
        /// <param name="reader">The reader from which to read enum labels.</param>
        /// <param name="byOID">The OID of the enum type for which labels are read.</param>
        static void LoadEnumLabels(NpgsqlDataReader reader, Dictionary<uint, PostgresType> byOID)
        {
            var currentOID = uint.MaxValue;
            PostgresEnumType? currentEnum = null;
            var skipCurrent = false;

            while (reader.Read())
            {
                var oid = uint.Parse(reader.GetString("oid"), NumberFormatInfo.InvariantInfo);
                if (oid != currentOID)
                {
                    currentOID = oid;

                    if (!byOID.TryGetValue(oid, out var type))  // See #2020
                    {
                        Log.Warn($"Skipping enum type with OID {oid} which was not found in pg_type");
                        byOID.Remove(oid);
                        skipCurrent = true;
                        continue;
                    }

                    currentEnum = type as PostgresEnumType;
                    if (currentEnum == null)
                    {
                        Log.Warn($"Type {type.Name} was referenced as an enum type but is a {type.GetType()}");
                        byOID.Remove(oid);
                        skipCurrent = true;
                        continue;
                    }

                    skipCurrent = false;
                }

                if (skipCurrent)
                    continue;

                currentEnum!.MutableLabels.Add(reader.GetString("enumlabel"));
            }
        }
    }
}
