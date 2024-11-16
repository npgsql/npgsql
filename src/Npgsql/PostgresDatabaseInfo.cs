using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using Npgsql.Util;
using static Npgsql.Util.Statics;

// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace Npgsql;

/// <summary>
/// The default implementation of <see cref="INpgsqlDatabaseInfoFactory"/>, for standard PostgreSQL databases..
/// </summary>
sealed class PostgresDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
{
    /// <inheritdoc />
    public async Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
    {
        var db = new PostgresDatabaseInfo(conn);
        await db.LoadPostgresInfo(conn, timeout, async).ConfigureAwait(false);
        Debug.Assert(db.LongVersion != null);
        return db;
    }
}

/// <summary>
/// The default implementation of NpgsqlDatabase, for standard PostgreSQL databases.
/// </summary>
class PostgresDatabaseInfo : NpgsqlDatabaseInfo
{
    readonly ILogger _connectionLogger;

    /// <summary>
    /// The PostgreSQL types detected in the database.
    /// </summary>
    List<PostgresType>? _types;

    bool? _isRedshift;

    /// <inheritdoc />
    protected override IEnumerable<PostgresType> GetTypes() => _types ?? (IEnumerable<PostgresType>)Array.Empty<PostgresType>();

    /// <summary>
    /// The PostgreSQL version string as returned by the version() function. Populated during loading.
    /// </summary>
    public string LongVersion { get; set; } = "";

    /// <summary>
    /// True if the backend is Amazon Redshift; otherwise, false.
    /// </summary>
    public bool IsRedshift => _isRedshift ??= LongVersion.Contains("redshift", StringComparison.OrdinalIgnoreCase);

    // Note that UNLISTEN is only needed for the reset message, but those don't get generated for Redshift anyway because e.g. DISCARD
    // isn't supported there either. So the IsRedshift check isn't actually used, but is here for completeness.
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
        => _connectionLogger = conn.LoggingConfiguration.ConnectionLogger;

    private protected PostgresDatabaseInfo(string host, int port, string databaseName, string serverVersion)
        : base(host, port, databaseName, serverVersion)
        => _connectionLogger = NullLogger.Instance;

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

        _types = await LoadBackendTypes(conn, timeout, async).ConfigureAwait(false);
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
    static string GenerateLoadTypesQuery(bool withRange, bool withMultirange, bool loadTableComposites)
        => $@"
SELECT ns.nspname, t.oid, t.typname, t.typtype, t.typnotnull, t.elemtypoid
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
                {(withMultirange ? "WHEN typ.typtype='m' THEN (SELECT rngtypid FROM pg_range WHERE rngmultitypid = typ.oid)" : "")}
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
) AS t
JOIN pg_namespace AS ns ON (ns.oid = typnamespace)
WHERE
    typtype IN ('b', 'r', 'm', 'e', 'd') OR -- Base, range, multirange, enum, domain
    (typtype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "relkind='c'")}) OR -- User-defined free-standing composites (not table composites) by default
    (typtype = 'p' AND typname IN ('record', 'void', 'unknown')) OR -- Some special supported pseudo-types
    (typtype = 'a' AND (  -- Array of...
        elemtyptype IN ('b', 'r', 'm', 'e', 'd') OR -- Array of base, range, multirange, enum, domain
        (elemtyptype = 'p' AND elemtypname IN ('record', 'void')) OR -- Arrays of special supported pseudo-types
        (elemtyptype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "elemrelkind='c'")}) -- Array of user-defined free-standing composites (not table composites) by default
    ))
ORDER BY CASE
       WHEN typtype IN ('b', 'e', 'p') THEN 0           -- First base types, enums, pseudo-types
       WHEN typtype = 'c' THEN 1                        -- Composites after (fields loaded later in 2nd pass)
       WHEN typtype = 'r' THEN 2                        -- Ranges after
       WHEN typtype = 'm' THEN 3                        -- Multiranges after
       WHEN typtype = 'd' AND elemtyptype <> 'a' THEN 4 -- Domains over non-arrays after
       WHEN typtype = 'a' THEN 5                        -- Arrays after
       WHEN typtype = 'd' AND elemtyptype = 'a' THEN 6  -- Domains over arrays last
END;";

    static string GenerateLoadCompositeTypesQuery(bool loadTableComposites)
        => $@"
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
ORDER BY typ.oid, att.attnum;";

    static string GenerateLoadEnumFieldsQuery(bool withEnumSortOrder)
        => $@"
-- Load enum fields
SELECT pg_type.oid, enumlabel
FROM pg_enum
JOIN pg_type ON pg_type.oid=enumtypid
ORDER BY oid{(withEnumSortOrder ? ", enumsortorder" : "")};";

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
    internal async Task<List<PostgresType>> LoadBackendTypes(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
    {
        var versionQuery = "SELECT version();";
        var loadTableComposites = conn.DataSource.Configuration.TypeLoading.LoadTableComposites;
        var loadTypesQuery = GenerateLoadTypesQuery(SupportsRangeTypes, SupportsMultirangeTypes, loadTableComposites);
        var loadCompositeTypesQuery = GenerateLoadCompositeTypesQuery(loadTableComposites);
        var loadEnumFieldsQuery = SupportsEnumTypes
            ? GenerateLoadEnumFieldsQuery(HasEnumSortOrder)
            : string.Empty;

        timeout.CheckAndApply(conn);
        // The Lexer (https://github.com/postgres/postgres/blob/master/src/backend/replication/repl_scanner.l)
        // and Parser (https://github.com/postgres/postgres/blob/master/src/backend/replication/repl_gram.y)
        // for replication connections are pretty picky and somewhat flawed.
        // Currently (2022-01-22) they do not support
        // - SQL batches containing multiple commands
        // - The <CR> ('\r') in Windows or Mac newlines
        // - Comments
        // For this reason we need clean up our type loading queries for replication connections and execute
        // them individually instead of batched.
        // Theoretically we cold even use the extended protocol + batching for regular (non-replication)
        // connections but that would branch our code even more for very little gain.
        var isReplicationConnection = conn.Settings.ReplicationMode != ReplicationMode.Off;
        if (isReplicationConnection)
        {
            await conn.WriteQuery(versionQuery, async).ConfigureAwait(false);
            await conn.WriteQuery(SanitizeForReplicationConnection(loadTypesQuery), async).ConfigureAwait(false);
            await conn.WriteQuery(SanitizeForReplicationConnection(loadCompositeTypesQuery), async).ConfigureAwait(false);
            if (SupportsEnumTypes)
                await conn.WriteQuery(SanitizeForReplicationConnection(loadEnumFieldsQuery), async).ConfigureAwait(false);

            static string SanitizeForReplicationConnection(string str)
            {
                var sb = new StringBuilder(str.Length);
                using var c = str.GetEnumerator();
                while (c.MoveNext())
                {
                    switch (c.Current)
                    {
                    case '\r':
                        sb.Append('\n');
                        // Check for a \n after the \r
                        // and swallow it if it exists
                        if (c.MoveNext())
                        {
                            if (c.Current == '-')
                                goto case '-';
                            if (c.Current != '\n')
                                sb.Append(c.Current);
                        }
                        break;
                    case '-':
                        // Check if there is a second dash
                        if (c.MoveNext())
                        {
                            if (c.Current == '\r')
                            {
                                sb.Append('-');
                                goto case '\r';
                            }
                            if (c.Current != '-')
                            {
                                sb.Append('-');
                                sb.Append(c.Current);
                                break;
                            }

                            // Comment mode
                            // Swallow everything until we find a newline
                            while (c.MoveNext())
                            {
                                if (c.Current == '\r')
                                    goto case '\r';
                                if (c.Current == '\n')
                                {
                                    sb.Append('\n');
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        sb.Append(c.Current);
                        break;
                    }
                }

                return sb.ToString();
            }
        }
        else
        {
            var batchQuery = new StringBuilder(
                    versionQuery.Length +
                    loadTypesQuery.Length +
                    loadCompositeTypesQuery.Length +
                    (SupportsEnumTypes
                        ? loadEnumFieldsQuery.Length
                        : 0))
                .AppendLine(versionQuery)
                .AppendLine(loadTypesQuery)
                .AppendLine(loadCompositeTypesQuery);

            if (SupportsEnumTypes)
                batchQuery.AppendLine(loadEnumFieldsQuery);
            await conn.WriteQuery(batchQuery.ToString(), async).ConfigureAwait(false);
        }
        await conn.Flush(async).ConfigureAwait(false);
        var byOID = new Dictionary<uint, PostgresType>();

        // First read the PostgreSQL version
        Expect<RowDescriptionMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);

        // We read the message in non-sequential mode which buffers the whole message.
        // There is no need to ensure data within the message boundaries
        Expect<DataRowMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);
        // Note that here and below we don't assign ReadBuffer to a variable
        // because we might allocate oversize buffer
        conn.ReadBuffer.Skip(2); // Column count
        LongVersion = ReadNonNullableString(conn.ReadBuffer);
        Expect<CommandCompleteMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);
        if (isReplicationConnection)
            Expect<ReadyForQueryMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);

        // Then load the types
        Expect<RowDescriptionMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);
        IBackendMessage msg;
        var unknownPostgresTypes = new List<PostgresTypeDefinition>();
        while (true)
        {
            msg = await conn.ReadMessage(async).ConfigureAwait(false);
            if (msg is not DataRowMessage)
                break;

            conn.ReadBuffer.Skip(2); // Column count
            var nspname = ReadNonNullableString(conn.ReadBuffer);
            var oid = uint.Parse(ReadNonNullableString(conn.ReadBuffer), NumberFormatInfo.InvariantInfo);
            Debug.Assert(oid != 0);
            var typname = ReadNonNullableString(conn.ReadBuffer);
            var typtype = ReadNonNullableString(conn.ReadBuffer)[0];
            var typnotnull = ReadNonNullableString(conn.ReadBuffer)[0] == 't';
            var len = conn.ReadBuffer.ReadInt32();
            var elemtypoid = len == -1 ? 0 : uint.Parse(conn.ReadBuffer.ReadString(len), NumberFormatInfo.InvariantInfo);

            var postgresTypeDefinition = new PostgresTypeDefinition(nspname, oid, typname, typtype, typnotnull, elemtypoid);
            if (!TryAddPostgresType(postgresTypeDefinition, byOID))
                unknownPostgresTypes.Add(postgresTypeDefinition);
        }

        while (unknownPostgresTypes.Count > 0)
        {
            var hasChanges = false;
            for (var i = unknownPostgresTypes.Count - 1; i >= 0; i--)
            {
                var unknownPostgresType = unknownPostgresTypes[i];
                if (TryAddPostgresType(unknownPostgresType, byOID))
                {
                    unknownPostgresTypes.RemoveAt(i);
                    hasChanges = true;
                }
            }

            if (!hasChanges)
            {
                _connectionLogger.LogWarning("Unable to load '{UnknownTypeCount}' Postgres types while loading database info.",
                    unknownPostgresTypes.Count);
                break;
            }
        }

        Expect<CommandCompleteMessage>(msg, conn);
        if (isReplicationConnection)
            Expect<ReadyForQueryMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);

        // Then load the composite type fields
        Expect<RowDescriptionMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);

        var currentOID = uint.MaxValue;
        PostgresCompositeType? currentComposite = null;
        var skipCurrent = false;

        while (true)
        {
            msg = await conn.ReadMessage(async).ConfigureAwait(false);
            if (msg is not DataRowMessage)
                break;

            conn.ReadBuffer.Skip(2); // Column count
            var oid = uint.Parse(ReadNonNullableString(conn.ReadBuffer), NumberFormatInfo.InvariantInfo);
            var attname = ReadNonNullableString(conn.ReadBuffer);
            var atttypid = uint.Parse(ReadNonNullableString(conn.ReadBuffer), NumberFormatInfo.InvariantInfo);

            if (oid != currentOID)
            {
                currentOID = oid;

                if (!byOID.TryGetValue(oid, out var type))  // See #2020
                {
                    _connectionLogger.LogWarning("Skipping composite type with OID {CompositeTypeOID} which was not found in pg_type", oid);
                    byOID.Remove(oid);
                    skipCurrent = true;
                    continue;
                }

                currentComposite = type as PostgresCompositeType;
                if (currentComposite == null)
                {
                    _connectionLogger.LogWarning("Type {TypeName} was referenced as a composite type but is a {type}", type.Name, type.GetType());
                    byOID.Remove(oid);
                    skipCurrent = true;
                    continue;
                }

                skipCurrent = false;
            }

            if (skipCurrent)
                continue;

            if (!byOID.TryGetValue(atttypid, out var fieldType))  // See #2020
            {
                _connectionLogger.LogWarning("Skipping composite type '{CompositeTypeName}' with field '{fieldName}' with type OID '{FieldTypeOID}', which could not be resolved to a PostgreSQL type.",
                    currentComposite!.DisplayName, attname, atttypid);
                byOID.Remove(oid);
                skipCurrent = true;
                continue;
            }

            currentComposite!.MutableFields.Add(new PostgresCompositeType.Field(attname, fieldType));
        }
        Expect<CommandCompleteMessage>(msg, conn);
        if (isReplicationConnection)
            Expect<ReadyForQueryMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);

        if (SupportsEnumTypes)
        {
            // Then load the enum fields
            Expect<RowDescriptionMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);

            currentOID = uint.MaxValue;
            PostgresEnumType? currentEnum = null;
            skipCurrent = false;

            while (true)
            {
                msg = await conn.ReadMessage(async).ConfigureAwait(false);
                if (msg is not DataRowMessage)
                    break;

                conn.ReadBuffer.Skip(2); // Column count
                var oid = uint.Parse(ReadNonNullableString(conn.ReadBuffer), NumberFormatInfo.InvariantInfo);
                var enumlabel = ReadNonNullableString(conn.ReadBuffer);
                if (oid != currentOID)
                {
                    currentOID = oid;

                    if (!byOID.TryGetValue(oid, out var type))  // See #2020
                    {
                        _connectionLogger.LogWarning("Skipping enum type with OID {OID} which was not found in pg_type", oid);
                        byOID.Remove(oid);
                        skipCurrent = true;
                        continue;
                    }

                    currentEnum = type as PostgresEnumType;
                    if (currentEnum == null)
                    {
                        _connectionLogger.LogWarning("Type type '{TypeName}' was referenced as an enum type but is a {Type}", type.Name, type.GetType());
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
            if (isReplicationConnection)
                Expect<ReadyForQueryMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);
        }

        if (!isReplicationConnection)
            Expect<ReadyForQueryMessage>(await conn.ReadMessage(async).ConfigureAwait(false), conn);

        return [..byOID.Values];

        static string ReadNonNullableString(NpgsqlReadBuffer buffer)
            => buffer.ReadString(buffer.ReadInt32());

        bool TryAddPostgresType(PostgresTypeDefinition postgresTypeDefinition, Dictionary<uint, PostgresType> byOID)
        {
            switch (postgresTypeDefinition.Type)
            {
            case 'b': // Normal base type
                var baseType = new PostgresBaseType(postgresTypeDefinition.Namespace, postgresTypeDefinition.Name, postgresTypeDefinition.OID);
                byOID[baseType.OID] = baseType;
                return true;

            case 'a': // Array
            {
                Debug.Assert(postgresTypeDefinition.ElemTypeOID > 0);
                if (!byOID.TryGetValue(postgresTypeDefinition.ElemTypeOID, out var elementPostgresType))
                {
                    _connectionLogger.LogTrace("Array type '{ArrayTypeName}' refers to unknown element with OID {ElementTypeOID}, skipping",
                        postgresTypeDefinition.Name, postgresTypeDefinition.ElemTypeOID);
                    return false;
                }

                var arrayType = new PostgresArrayType(postgresTypeDefinition.Namespace, postgresTypeDefinition.Name, postgresTypeDefinition.OID, elementPostgresType);
                byOID[arrayType.OID] = arrayType;
                return true;
            }

            case 'r': // Range
            {
                Debug.Assert(postgresTypeDefinition.ElemTypeOID > 0);
                if (!byOID.TryGetValue(postgresTypeDefinition.ElemTypeOID, out var subtypePostgresType))
                {
                    _connectionLogger.LogTrace("Range type '{RangeTypeName}' refers to unknown subtype with OID {ElementTypeOID}, skipping",
                        postgresTypeDefinition.Name, postgresTypeDefinition.ElemTypeOID);
                    return false;
                }

                var rangeType = new PostgresRangeType(postgresTypeDefinition.Namespace, postgresTypeDefinition.Name, postgresTypeDefinition.OID, subtypePostgresType);
                byOID[rangeType.OID] = rangeType;
                return true;
            }

            case 'm': // Multirange
                Debug.Assert(postgresTypeDefinition.ElemTypeOID > 0);
                if (!byOID.TryGetValue(postgresTypeDefinition.ElemTypeOID, out var type))
                {
                    _connectionLogger.LogTrace("Multirange type '{MultirangeTypeName}' refers to unknown range with OID {ElementTypeOID}, skipping",
                        postgresTypeDefinition.Name, postgresTypeDefinition.ElemTypeOID);
                    return false;
                }

                if (type is not PostgresRangeType rangePostgresType)
                {
                    _connectionLogger.LogTrace("Multirange type '{MultirangeTypeName}' refers to non-range type '{TypeName}', skipping",
                        postgresTypeDefinition.Name, type.Name);
                    return false;
                }

                var multirangeType = new PostgresMultirangeType(postgresTypeDefinition.Namespace, postgresTypeDefinition.Name, postgresTypeDefinition.OID, rangePostgresType);
                byOID[multirangeType.OID] = multirangeType;
                return true;

            case 'e': // Enum
                var enumType = new PostgresEnumType(postgresTypeDefinition.Namespace, postgresTypeDefinition.Name, postgresTypeDefinition.OID);
                byOID[enumType.OID] = enumType;
                return true;

            case 'c': // Composite
                var compositeType = new PostgresCompositeType(postgresTypeDefinition.Namespace, postgresTypeDefinition.Name, postgresTypeDefinition.OID);
                byOID[compositeType.OID] = compositeType;
                return true;

            case 'd': // Domain
                Debug.Assert(postgresTypeDefinition.ElemTypeOID > 0);
                if (!byOID.TryGetValue(postgresTypeDefinition.ElemTypeOID, out var basePostgresType))
                {
                    _connectionLogger.LogTrace("Domain type '{DomainTypeName}' refers to unknown base type with OID {ElementTypeOID}, skipping",
                        postgresTypeDefinition.Name, postgresTypeDefinition.ElemTypeOID);
                    return false;
                }

                var domainType = new PostgresDomainType(postgresTypeDefinition.Namespace, postgresTypeDefinition.Name, postgresTypeDefinition.OID, basePostgresType, postgresTypeDefinition.NotNull);
                byOID[domainType.OID] = domainType;
                return true;

            case 'p': // pseudo-type (record, void)
                goto case 'b'; // Hack this as a base type

            default:
                throw new ArgumentOutOfRangeException($"Unknown typtype for type '{postgresTypeDefinition.Name}' in pg_type: {postgresTypeDefinition.Type}");
            }
        }
    }
}

readonly record struct PostgresTypeDefinition(string Namespace, uint OID, string Name, char Type, bool NotNull, uint ElemTypeOID);
