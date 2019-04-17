using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;

namespace Npgsql
{
    class CrateDbDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
    {
        /// <summary>
        /// Returns a CrateDbDatabaseInfo instance if the given connection connects to a CrateDB cluster and null otherwhise.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="timeout"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        [NotNull]
        public Task<NpgsqlDatabaseInfo> Load([NotNull] NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
        {
            if (conn.PostgresParameters.ContainsKey("crate_version"))
            {
                var db = new CrateDbDatabaseInfo();
                db.LoadCrateDbInfo(conn);
                return Task.FromResult((NpgsqlDatabaseInfo)db);
            }
            return Task.FromResult<NpgsqlDatabaseInfo>(null);
        }
    }

    /// <summary>
    /// Represents a NpgsqlDatabaseInfo-class for usage with CrateDB.
    /// </summary>
    public class CrateDbDatabaseInfo : NpgsqlDatabaseInfo
    {
        /// <summary>
        /// The version of the CrateDB database we're connected to, as reported in the "crate_version" parameter.
        /// </summary>
        public Version CrateDbVersion { get; protected set; }

        static readonly IDictionary<string, uint> CrateDbBaseTypes = new Dictionary<string, uint>
        {
            { "bool", 16 },
            { "int2", 21 },
            { "int4", 23 },
            { "json", 114 },
            { "float4", 700 },
            { "char", 18 },
            { "int8", 20 },
            { "timestamptz", 1184 },
            { "float8", 701 },
            { "varchar", 1043 }
        };

        static readonly IDictionary<string, uint> CrateDbArrayTypes = new Dictionary<string, uint>
        {
            { "_bool", 1000 },
            { "_int2", 1005 },
            { "_int4", 1007 },
            { "_json", 199 },
            { "_float4", 1021 },
            { "_char", 1002 },
            { "_int8", 1016 },
            { "_timestamptz", 1185 },
            { "_float8", 1022 },
            { "_varchar", 1015 }
        };

        static readonly IDictionary<string, string> PgTypeNameToInternalName = new Dictionary<string, string>
        {
            { "boolean", "bool" },
            { "smallint", "int2" },
            { "integer", "int4" },
            { "real", "float4" },
            { "bigint", "int8" },
            { "timestamp with time zone", "timestamptz" },
            { "double precision", "float8" },
            { "character varying", "varchar" }
        };

        /// <summary>
        /// Loads database information from the PostgreSQL database specified by <paramref name="conn"/>.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        [NotNull]
        internal void LoadCrateDbInfo([NotNull] NpgsqlConnection conn)
        {
            Version = ParseServerVersion(conn.PostgresParameters["server_version"]);
            if (Version.TryParse(conn.PostgresParameters["crate_version"], out var v))
            {
                CrateDbVersion = v;
            }
            HasIntegerDateTimes = conn.PostgresParameters.TryGetValue("integer_datetimes", out var intDateTimes) &&
                intDateTimes == "on";
        }

        /// <summary>
        /// Returns the data types supported by CrateDB.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<PostgresType> GetTypes()
        {
            IEnumerable<PostgresType> baseTypes = CrateDbBaseTypes.Select(p => new PostgresBaseType("pg_catalog", p.Key, p.Value)).ToList();
            return baseTypes.Concat(CrateDbArrayTypes.Select(p => new PostgresArrayType("pg_catalog", p.Key, p.Value, baseTypes.FirstOrDefault(t => string.Equals(t.InternalName, p.Key.Substring(1))))));
        }

        /// <summary>
        /// Returns a boolean value that signals if transactions are supported by the current database.
        /// </summary>
        public override bool SupportsTransactions => false;
        /// <summary>
        /// Returns a boolean value that signals if the DISCARD command is supported by the current database.
        /// </summary>
        public override bool SupportsDiscard => false;
        /// <summary>
        /// Returns a boolean value that signals if the DISCARD TEMP subcommand is supported by the current database.
        /// </summary>
        public override bool SupportsDiscardTemp => false;
        /// <summary>
        /// Returns a boolean value that signals if the DISCARD SEQUENCES subcommand is supported by the current database.
        /// </summary>
        public override bool SupportsDiscardSequences => false;
        /// <summary>
        /// Returns a boolean value that signals if the CLOSE ALL command is supported by the current database.
        /// </summary>
        public override bool SupportsCloseAll => false;
        /// <summary>
        /// Returns a boolean value that signals if the UNLISTEN command is supported by the current database.
        /// </summary>
        public override bool SupportsUnlisten => false;
        /// <summary>
        /// Returns a boolean value that signals if the current database supports range types.
        /// </summary>
        public override bool SupportsRangeTypes => false;
        /// <summary>
        /// Returns a boolean value that signals if the current database supports advisory lock functions.
        /// </summary>
        public override bool SupportsAdvisoryLocks => false;
        /// <summary>
        /// Reports whether the backend uses the newer integer timestamp representation. 
        /// CrateDB used floating point timestamps until version &lt;=3.0 and switched to integer datetimes with version &gt;= 3.1.
        /// </summary>
        public override bool HasIntegerDateTimes { get; protected set; } = true;

        static bool IsPgTypeSupportedByCrateDb(string pgTypeName)
        {
            var internalName = PgTypeNameToInternalName.ContainsKey(pgTypeName) ? PgTypeNameToInternalName[pgTypeName] : pgTypeName;
            return CrateDbBaseTypes.ContainsKey(internalName) || CrateDbArrayTypes.ContainsKey(internalName);
        }
    }
}
