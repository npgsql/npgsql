#region License

// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

#endregion

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.PostgresTypes;

namespace Npgsql
{
    /// <summary>
    /// The default implementation of <see cref="INpgsqlDatabaseInfoFactory"/>, for standard PostgreSQL databases..
    /// </summary>
    class PostgresDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
    {
        /// <inheritdoc />
        [NotNull]
        public async Task<NpgsqlDatabaseInfo> Load([NotNull] NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
        {
            var db = new PostgresDatabaseInfo();
            await db.LoadPostgresInfo(conn, timeout, async);
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
        [NotNull] static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        /// <summary>
        /// The PostgreSQL types detected in the database.
        /// </summary>
        [CanBeNull] List<PostgresType> _types;

        /// <inheritdoc />
        [NotNull]
        protected override IEnumerable<PostgresType> GetTypes() => _types ?? Enumerable.Empty<PostgresType>();

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
        /// Loads database information from the PostgreSQL database specified by <paramref name="conn"/>.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <param name="timeout">The timeout while loading types from the backend.</param>
        /// <param name="async">True to load types asynchronously.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        [NotNull]
        internal async Task LoadPostgresInfo([NotNull] NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
        {
            var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            Host = csb.Host;
            Port = csb.Port;
            Name = csb.Database;

            Version = ParseServerVersion(conn.PostgresParameters["server_version"]);

            HasIntegerDateTimes =
                conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) &&
                intDateTimes == "on";

            IsRedshift = csb.ServerCompatibilityMode == ServerCompatibilityMode.Redshift;
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
        [NotNull]
        static string GenerateTypesQuery(bool withRange, bool withEnum, bool withEnumSortOrder, bool loadTableComposites)
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
    b.typtype IN ('b', 'r', 'e', 'd') OR       /* Array of base, range, enum, domain */
    (b.typtype = 'p' AND b.typname IN ('record', 'void')) OR /* Arrays of special supported pseudo-types */
    (b.typtype = 'c' AND elemcls.relkind='c')  /* Array of user-defined free-standing composites (not table composites) */
  )) OR
  (a.typtype = 'p' AND a.typname IN ('record', 'void'))  /* Some special supported pseudo-types */
ORDER BY ord;

/*** Load field definitions for (free-standing) composite types ***/
SELECT typ.oid, att.attname, att.atttypid
FROM pg_type AS typ
JOIN pg_namespace AS ns ON (ns.oid = typ.typnamespace)
JOIN pg_class AS cls ON (cls.oid = typ.typrelid)
JOIN pg_attribute AS att ON (att.attrelid = typ.typrelid)
WHERE
  (typ.typtype = 'c' AND {(loadTableComposites ? "ns.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')" : "cls.relkind='c'")}) AND
  attnum > 0 AND     /* Don't load system attributes */
  NOT attisdropped
ORDER BY typ.typname, att.attnum;
{(withEnum ? $@"
/*** Load enum fields ***/
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
        [NotNull]
        internal async Task<List<PostgresType>> LoadBackendTypes([NotNull] NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
        {
            var commandTimeout = 0;  // Default to infinity
            if (timeout.IsSet)
            {
                commandTimeout = (int)timeout.TimeLeft.TotalSeconds;
                if (commandTimeout <= 0)
                    throw new TimeoutException();
            }

            var typeLoadingQuery = GenerateTypesQuery(SupportsRangeTypes, SupportsEnumTypes, HasEnumSortOrder, conn.Settings.LoadTableComposites);

            using (var command = new NpgsqlCommand(typeLoadingQuery, conn))
            {
                command.CommandTimeout = commandTimeout;
                command.AllResultTypesAreUnknown = true;
                using (var reader = async ? await command.ExecuteReaderAsync() : command.ExecuteReader())
                {
                    var byOID = new Dictionary<uint, PostgresType>();

                    // First load the types themselves
                    while (async ? await reader.ReadAsync() : reader.Read())
                    {
                        timeout.Check();

                        var ns = reader.GetString(reader.GetOrdinal("nspname"));
                        var internalName = reader.GetString(reader.GetOrdinal("typname"));
                        var oid = Convert.ToUInt32(reader[reader.GetOrdinal("oid")]);

                        Debug.Assert(internalName != null);
                        Debug.Assert(oid != 0);

                        var typeChar = reader.GetString(reader.GetOrdinal("type"))[0];
                        switch (typeChar)
                        {
                        case 'b':  // Normal base type
                            var baseType = new PostgresBaseType(ns, internalName, oid);
                            byOID[baseType.OID] = baseType;
                            continue;

                        case 'a': // Array
                        {
                            var elementOID = Convert.ToUInt32(reader[reader.GetOrdinal("elemoid")]);
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
                            var elementOID = Convert.ToUInt32(reader[reader.GetOrdinal("elemoid")]);
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
                            // Unlike other types, we don't
                            var compositeType = new PostgresCompositeType(ns, internalName, oid);
                            byOID[compositeType.OID] = compositeType;
                            continue;

                        case 'd':   // Domain
                            var baseTypeOID = Convert.ToUInt32(reader[reader.GetOrdinal("typbasetype")]);
                            Debug.Assert(baseTypeOID > 0);
                            if (!byOID.TryGetValue(baseTypeOID, out var basePostgresType))
                            {
                                Log.Trace($"Domain type '{internalName}' refers to unknown base type with OID {baseTypeOID}, skipping", conn.ProcessID);
                                continue;
                            }
                            var domainType = new PostgresDomainType(ns, internalName, oid, basePostgresType);
                            byOID[domainType.OID] = domainType;
                            continue;

                        case 'p':   // pseudo-type (record, void)
                            // Hack this as a base type
                            goto case 'b';

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
            }
        }

        /// <summary>
        /// Loads composite fields for the composite type specified by the OID.
        /// </summary>
        /// <param name="reader">The reader from which to read composite fields.</param>
        /// <param name="byOID">The OID of the composite type for which fields are read.</param>
        static void LoadCompositeFields([NotNull] DbDataReader reader, [NotNull] Dictionary<uint, PostgresType> byOID)
        {
            PostgresCompositeType currentComposite = null;
            while (reader.Read())
            {
                var oid = Convert.ToUInt32(reader[reader.GetOrdinal("oid")]);
                if (oid != currentComposite?.OID)
                {
                    currentComposite = byOID[oid] as PostgresCompositeType;
                    if (currentComposite == null)
                    {
                        Log.Error($"Ignoring unknown composite type with OID {oid} when trying to load composite fields");
                        byOID.Remove(oid);
                        continue;
                    }
                }

                var fieldName = reader.GetString(reader.GetOrdinal("attname"));
                var fieldTypeOID = Convert.ToUInt32(reader[reader.GetOrdinal("atttypid")]);
                if (!byOID.TryGetValue(fieldTypeOID, out var fieldType))
                {
                    Log.Error($"Skipping composite type {currentComposite.DisplayName} with field {fieldName} with type OID {fieldTypeOID}, which could not be resolved to a PostgreSQL type.");
                    byOID.Remove(oid);
                    continue;
                }
                currentComposite.MutableFields.Add(new PostgresCompositeType.Field(fieldName, fieldType));
            }
        }

        /// <summary>
        /// Loads enum labels for the enum type specified by the OID.
        /// </summary>
        /// <param name="reader">The reader from which to read enum labels.</param>
        /// <param name="byOID">The OID of the enum type for which labels are read.</param>
        static void LoadEnumLabels([NotNull] DbDataReader reader, [NotNull] Dictionary<uint, PostgresType> byOID)
        {
            PostgresEnumType currentEnum = null;
            while (reader.Read())
            {
                var oid = Convert.ToUInt32(reader[reader.GetOrdinal("oid")]);
                if (oid != currentEnum?.OID)
                {
                    currentEnum = byOID[oid] as PostgresEnumType;
                    if (currentEnum == null)
                    {
                        Log.Error($"Skipping enum labels for type with OID {oid} which wasn't loaded as an enum");
                        byOID.Remove(oid);
                        continue;
                    }
                }
                currentEnum.MutableLabels.Add(reader.GetString(reader.GetOrdinal("enumlabel")));
            }
        }
    }
}
