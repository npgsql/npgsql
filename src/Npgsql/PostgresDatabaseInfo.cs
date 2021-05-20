using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using Npgsql.Util;
using static Npgsql.Util.Statics;

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
        public async Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
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
        public override bool SupportsUnlisten => Version.IsGreaterOrEqual(6, 4) && !IsRedshift;

        /// <summary>
        /// True if the 'pg_enum' table includes the 'enumsortorder' column; otherwise, false.
        /// </summary>
        public virtual bool HasEnumSortOrder => Version.IsGreaterOrEqual(9, 1);

        /// <summary>
        /// True if the 'pg_type' table includes the 'typcategory' column; otherwise, false.
        /// </summary>
        /// <remarks>
        /// pg_type.typcategory is added after 8.4.
        /// see: https://www.postgresql.org/docs/8.4/static/catalog-pg-type.html#CATALOG-TYPCATEGORY-TABLE
        /// </remarks>
        public virtual bool HasTypeCategory => Version.IsGreaterOrEqual(8, 4);

        internal PostgresDatabaseInfo(NpgsqlConnector conn)
            : base(conn.Host!, conn.Port, conn.Database!, conn.PostgresParameters["server_version"])
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
        internal async Task LoadPostgresInfo(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
        {
            HasIntegerDateTimes =
                conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) &&
                intDateTimes == "on";

            IsRedshift = conn.Settings.ServerCompatibilityMode == ServerCompatibilityMode.Redshift;
            _types = await LoadBackendTypes(conn, timeout, async, conn.Settings.ReplicationMode != ReplicationMode.Off);
        }

        /// <summary>
        /// Generates a raw SQL query string to select type information.
        /// </summary>
        /// <remarks>
        /// Select all types (base, array which is also base, enum, range, composite).
        /// Note that arrays are distinguished from primitive types through them having typreceive=array_recv.
        /// Order by primitives first, container later.
        /// For arrays and ranges, join in the element OID and type (to filter out arrays of unhandled
        /// types).
        /// </remarks>
        static string GenerateLoadTypesQuery(bool withRange, bool withMultirange, bool loadTableComposites, bool addComments =
#if DEBUG
                true
#else
            false
#endif
        )
            => $@"
SELECT ns.nspname, t.oid, t.typname, t.typtype, t.typnotnull, t.elemtypoid
FROM ({(addComments ? @"
    -- Arrays have typtype=b - this subquery identifies them by their typreceive and converts their typtype to a
    -- We first do this for the type (innerest-most subquery), and then for its element type
    -- This also returns the array element, range subtype and domain base type as elemtypoid" : string.Empty)}
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
                {(withMultirange ? "WHEN typ.typtype='m' THEN (SELECT rngtypid FROM pg_range WHERE rngmultitypid = typ.oid)" : "")}
                WHEN typ.typtype='d' THEN typ.typbasetype
            END AS elemtypoid
        FROM pg_catalog.pg_type AS typ
        LEFT JOIN pg_catalog.pg_class AS cls ON (cls.oid = typ.typrelid)
        LEFT JOIN pg_catalog.pg_proc AS proc ON proc.oid = typ.typreceive
        {(withRange ? "LEFT JOIN pg_catalog.pg_range AS rng ON (rng.rngtypid = typ.oid)" : "")}
    ) AS typ
    LEFT JOIN pg_catalog.pg_type AS elemtyp ON elemtyp.oid = elemtypoid
    LEFT JOIN pg_catalog.pg_class AS elemcls ON (elemcls.oid = elemtyp.typrelid)
    LEFT JOIN pg_catalog.pg_proc AS elemproc ON elemproc.oid = elemtyp.typreceive
) AS t
JOIN pg_catalog.pg_namespace AS ns ON (ns.oid = typnamespace)
WHERE
    typtype IN ('b', 'r', 'm', 'e', 'd') OR{(addComments ? " -- Base, range, multirange, enum, domain" : string.Empty)}
    (typtype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "relkind='c'")}) OR{(addComments ? " -- User-defined free-standing composites (not table composites) by default" : string.Empty)}
    (typtype = 'p' AND typname IN ('record', 'void')) OR{(addComments ? " -- Some special supported pseudo-types" : string.Empty)}
    (typtype = 'a' AND ({(addComments ? "  -- Array of..." : string.Empty)}
        elemtyptype IN ('b', 'r', 'm', 'e', 'd') OR{(addComments ? " -- Array of base, range, multirange, enum, domain" : string.Empty)}
        (elemtyptype = 'p' AND elemtypname IN ('record', 'void')) OR{(addComments ? " -- Arrays of special supported pseudo-types" : string.Empty)}
        (elemtyptype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "elemrelkind='c'")}){(addComments ? " -- Array of user-defined free-standing composites (not table composites) by default" : string.Empty)}
    ))
ORDER BY CASE
       WHEN typtype IN ('b', 'e', 'p') THEN 0{(addComments ? "           -- First base types, enums, pseudo-types" : string.Empty)}
       WHEN typtype = 'r' THEN 1{(addComments ? "                        -- Ranges after" : string.Empty)}
       WHEN typtype = 'm' THEN 2{(addComments ? "                        -- Multiranges after" : string.Empty)}
       WHEN typtype = 'c' THEN 3{(addComments ? "                        -- Composites after" : string.Empty)}
       WHEN typtype = 'd' AND elemtyptype <> 'a' THEN 4{(addComments ? " -- Domains over non-arrays after" : string.Empty)}
       WHEN typtype = 'a' THEN 5{(addComments ? "                        -- Arrays before" : string.Empty)}
       WHEN typtype = 'd' AND elemtyptype = 'a' THEN 6{(addComments ? "  -- Domains over arrays last" : string.Empty)}
END;
"
#if NET6_0_OR_GREATER
                .ReplaceLineEndings("\n");
#else
    .Replace("\r\n", "\n").Replace('\r', '\n');
#endif

        static string GenerateLoadCompositeTypesQuery(bool loadTableComposites, bool addComments =
#if DEBUG
                true
#else
            false
#endif
        )
            => $@"{(addComments ? @"
-- Load field definitions for (free-standing) composite types" : string.Empty)}
SELECT typ.oid, att.attname, att.atttypid
FROM pg_catalog.pg_type AS typ
JOIN pg_catalog.pg_namespace AS ns ON (ns.oid = typ.typnamespace)
JOIN pg_catalog.pg_class AS cls ON (cls.oid = typ.typrelid)
JOIN pg_catalog.pg_attribute AS att ON (att.attrelid = typ.typrelid)
WHERE
  (typ.typtype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "cls.relkind='c'")}) AND
  attnum > 0 AND{(addComments ? "     -- Don't load system attributes" : string.Empty)}
  NOT attisdropped
ORDER BY typ.oid, att.attnum;
"
#if NET6_0_OR_GREATER
                .ReplaceLineEndings("\n");
#else
    .Replace("\r\n", "\n").Replace('\r', '\n');
#endif

        static string GenerateLoadEnumFieldsQuery(bool withEnumSortOrder, bool addComments =
#if DEBUG
                true
#else
            false
#endif
        )
            => $@"{(addComments ? @"
-- Load enum fields" : string.Empty)}
SELECT pg_type.oid, enumlabel
FROM pg_catalog.pg_enum
JOIN pg_catalog.pg_type ON pg_type.oid=enumtypid
ORDER BY oid{(withEnumSortOrder ? ", enumsortorder" : "")};
"
#if NET6_0_OR_GREATER
                .ReplaceLineEndings("\n");
#else
    .Replace("\r\n", "\n").Replace('\r', '\n');
#endif

        const string VersionQuery = "SELECT pg_catalog.version();\n";

        /// <summary>
        /// Loads type information from the backend specified by <paramref name="conn"/>.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <param name="timeout">The timeout while loading types from the backend.</param>
        /// <param name="async">True to load types asynchronously.</param>
        /// <param name="isReplicationConnection">True if this is a replication connection.</param>
        /// <returns>
        /// A collection of types loaded from the backend.
        /// </returns>
        /// <exception cref="TimeoutException" />
        /// <exception cref="ArgumentOutOfRangeException">Unknown typtype for type '{internalName}' in pg_type: {typeChar}.</exception>
        internal async Task<List<PostgresType>> LoadBackendTypes(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async,
            bool isReplicationConnection)
        {
            Dictionary<uint, PostgresType> byOID;
            timeout.CheckAndApply(conn);

            // The Lexer/Parser of replication connections is pretty picky and somewhat flawed.
            // Currently (2021-08-20) it does not support
            // - SQL batches containing multiple commands
            // - The <CR> ('\r') in Windows or Mac newlines
            // - Comments are dangerous (they work in some places but don't in others)
            if (isReplicationConnection)
            {
                var q = VersionQuery;
                await conn.WriteQuery(q, async);
                await conn.Flush(async);
                LongVersion = await LoadVersionString(conn, async);
                Expect<ReadyForQueryMessage>(await conn.ReadMessage(async), conn);

                q = GenerateLoadTypesQuery(SupportsRangeTypes, SupportsMultirangeTypes, conn.Settings.LoadTableComposites, addComments: false);
                await conn.WriteQuery(q, async);
                await conn.Flush(async);
                byOID = await LoadTypes(conn, async);
                Expect<ReadyForQueryMessage>(await conn.ReadMessage(async), conn);

                q = GenerateLoadCompositeTypesQuery(conn.Settings.LoadTableComposites, false);
                await conn.WriteQuery(q, async);
                await conn.Flush(async);
                await LoadCompositeTypes(conn, byOID, async);
                Expect<ReadyForQueryMessage>(await conn.ReadMessage(async), conn);

                if (SupportsEnumTypes)
                {
                    q = GenerateLoadEnumFieldsQuery(HasEnumSortOrder, false);
                    await conn.WriteQuery(q, async);
                    await conn.Flush(async);
                    await LoadEnums(conn, byOID, async);
                    Expect<ReadyForQueryMessage>(await conn.ReadMessage(async), conn);
                }
            }
            else
            {
                var batchQuery = new StringBuilder()
                    .Append(VersionQuery)
                    .Append(GenerateLoadTypesQuery(SupportsRangeTypes, SupportsMultirangeTypes, conn.Settings.LoadTableComposites))
                    .Append(GenerateLoadCompositeTypesQuery(conn.Settings.LoadTableComposites));

                if (SupportsEnumTypes)
                    batchQuery.Append(GenerateLoadEnumFieldsQuery(HasEnumSortOrder));

                await conn.WriteQuery(batchQuery.ToString(), async);
                await conn.Flush(async);

                LongVersion = await LoadVersionString(conn, async);
                byOID = await LoadTypes(conn, async);
                await LoadCompositeTypes(conn, byOID, async);
                if (SupportsEnumTypes)
                    await LoadEnums(conn, byOID, async);

                Expect<ReadyForQueryMessage>(await conn.ReadMessage(async), conn);
            }

            return byOID.Values.ToList();

            // Read the PostgreSQL version
            static async Task<string> LoadVersionString(NpgsqlConnector conn, bool async)
            {
                var buf = conn.ReadBuffer;
                Expect<RowDescriptionMessage>(await conn.ReadMessage(async), conn);
                Expect<DataRowMessage>(await conn.ReadMessage(async), conn);
                buf.Skip(2); // Column count
                var version = ReadNonNullableString(buf);
                Expect<CommandCompleteMessage>(await conn.ReadMessage(async), conn);
                return version;
            }

            // Load the types
            static async Task<Dictionary<uint, PostgresType>> LoadTypes(NpgsqlConnector conn, bool async)
            {
                var buf = conn.ReadBuffer;
                var byOID = new Dictionary<uint, PostgresType>();
                IBackendMessage msg = Expect<RowDescriptionMessage>(await conn.ReadMessage(async), conn);
                while (true)
                {
                    msg = await conn.ReadMessage(async);
                    if (msg is not DataRowMessage)
                        break;

                    buf.Skip(2); // Column count
                    var nspname = ReadNonNullableString(buf);
                    var oid = uint.Parse(ReadNonNullableString(buf), NumberFormatInfo.InvariantInfo);
                    Debug.Assert(oid != 0);
                    var typname = ReadNonNullableString(buf);
                    var typtype = ReadNonNullableString(buf)[0];
                    var typnotnull = ReadNonNullableString(buf)[0] == 't';
                    var len = buf.ReadInt32();
                    var elemtypoid = len == -1 ? 0 : uint.Parse(buf.ReadString(len), NumberFormatInfo.InvariantInfo);

                    switch (typtype)
                    {
                    case 'b': // Normal base type
                        var baseType = new PostgresBaseType(nspname, typname, oid);
                        byOID[baseType.OID] = baseType;
                        continue;

                    case 'a': // Array
                    {
                        Debug.Assert(elemtypoid > 0);
                        if (!byOID.TryGetValue(elemtypoid, out var elementPostgresType))
                        {
                            Log.Trace($"Array type '{typname}' refers to unknown element with OID {elemtypoid}, skipping", conn.Id);
                            continue;
                        }

                        var arrayType = new PostgresArrayType(nspname, typname, oid, elementPostgresType);
                        byOID[arrayType.OID] = arrayType;
                        continue;
                    }

                    case 'r': // Range
                    {
                        Debug.Assert(elemtypoid > 0);
                        if (!byOID.TryGetValue(elemtypoid, out var subtypePostgresType))
                        {
                            Log.Trace($"Range type '{typname}' refers to unknown subtype with OID {elemtypoid}, skipping", conn.Id);
                            continue;
                        }

                        var rangeType = new PostgresRangeType(nspname, typname, oid, subtypePostgresType);
                        byOID[rangeType.OID] = rangeType;
                        continue;
                    }

                    case 'm': // Multirange
                        Debug.Assert(elemtypoid > 0);
                        if (!byOID.TryGetValue(elemtypoid, out var type))
                        {
                            Log.Trace($"Multirange type '{typname}' refers to unknown range with OID {elemtypoid}, skipping", conn.Id);
                            continue;
                        }

                        if (type is not PostgresRangeType rangePostgresType)
                        {
                            Log.Trace($"Multirange type '{typname}' refers to non-range type {type.Name}, skipping",
                                conn.Id);
                            continue;
                        }

                        var multirangeType = new PostgresMultirangeType(nspname, typname, oid, rangePostgresType);
                        byOID[multirangeType.OID] = multirangeType;
                        continue;

                    case 'e': // Enum
                        var enumType = new PostgresEnumType(nspname, typname, oid);
                        byOID[enumType.OID] = enumType;
                        continue;

                    case 'c': // Composite
                        var compositeType = new PostgresCompositeType(nspname, typname, oid);
                        byOID[compositeType.OID] = compositeType;
                        continue;

                    case 'd': // Domain
                        Debug.Assert(elemtypoid > 0);
                        if (!byOID.TryGetValue(elemtypoid, out var basePostgresType))
                        {
                            Log.Trace($"Domain type '{typname}' refers to unknown base type with OID {elemtypoid}, skipping",
                                conn.Id);
                            continue;
                        }

                        var domainType = new PostgresDomainType(nspname, typname, oid, basePostgresType, typnotnull);
                        byOID[domainType.OID] = domainType;
                        continue;

                    case 'p': // pseudo-type (record, void)
                        goto case 'b'; // Hack this as a base type

                    default:
                        throw new ArgumentOutOfRangeException($"Unknown typtype for type '{typname}' in pg_type: {typtype}");
                    }
                }

                Expect<CommandCompleteMessage>(msg, conn);
                return byOID;
            }

            // Load the composite type fields
            static async Task LoadCompositeTypes(NpgsqlConnector conn, Dictionary<uint, PostgresType> byOID, bool async)
            {
                var buf = conn.ReadBuffer;
                var currentOID = uint.MaxValue;
                PostgresCompositeType? currentComposite = null;
                var skipCurrent = false;
                IBackendMessage msg = Expect<RowDescriptionMessage>(await conn.ReadMessage(async), conn);
                while (true)
                {
                    msg = await conn.ReadMessage(async);
                    if (msg is not DataRowMessage)
                        break;

                    buf.Skip(2); // Column count
                    var oid = uint.Parse(ReadNonNullableString(buf), NumberFormatInfo.InvariantInfo);
                    var attname = ReadNonNullableString(buf);
                    var atttypid = uint.Parse(ReadNonNullableString(buf), NumberFormatInfo.InvariantInfo);

                    if (oid != currentOID)
                    {
                        currentOID = oid;

                        if (!byOID.TryGetValue(oid, out var type)) // See #2020
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

                    if (!byOID.TryGetValue(atttypid, out var fieldType)) // See #2020
                    {
                        Log.Warn(
                            $"Skipping composite type {currentComposite!.DisplayName} with field {attname} with type OID {atttypid}, which could not be resolved to a PostgreSQL type.");
                        byOID.Remove(oid);
                        skipCurrent = true;
                        continue;
                    }

                    currentComposite!.MutableFields.Add(new PostgresCompositeType.Field(attname, fieldType));
                }

                Expect<CommandCompleteMessage>(msg, conn);
            }

            // Load the enum fields
            static async Task LoadEnums(NpgsqlConnector conn, Dictionary<uint, PostgresType> byOID, bool async)
            {
                var buf = conn.ReadBuffer;
                var currentOID = uint.MaxValue;
                PostgresEnumType? currentEnum = null;
                var skipCurrent = false;
                IBackendMessage msg = Expect<RowDescriptionMessage>(await conn.ReadMessage(async), conn);
                while (true)
                {
                    msg = await conn.ReadMessage(async);
                    if (msg is not DataRowMessage)
                        break;

                    buf.Skip(2); // Column count
                    var oid = uint.Parse(ReadNonNullableString(buf), NumberFormatInfo.InvariantInfo);
                    var enumlabel = ReadNonNullableString(buf);
                    if (oid != currentOID)
                    {
                        currentOID = oid;

                        if (!byOID.TryGetValue(oid, out var type)) // See #2020
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

                    currentEnum!.MutableLabels.Add(enumlabel);
                }

                Expect<CommandCompleteMessage>(msg, conn);
            }

            static string ReadNonNullableString(NpgsqlReadBuffer buffer)
                => buffer.ReadString(buffer.ReadInt32());
        }
    }
}
