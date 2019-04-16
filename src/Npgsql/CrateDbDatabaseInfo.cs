using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandlers.DateTimeHandlers;
using Npgsql.TypeMapping;
using NpgsqlTypes;

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
              => Task.FromResult(
                    conn.PostgresParameters.ContainsKey("crate_version")
                          ? (NpgsqlDatabaseInfo)new CrateDbDatabaseInfo(conn)
                          : null
                 );
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
        /// Creates an instance of the CrateDbDatabaseInfo class.
        /// </summary>
        /// <param name="conn"></param>
        public CrateDbDatabaseInfo(NpgsqlConnection conn)
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
        /// Adapts a dictionary of type mappings to CrateDB.
        /// </summary>
        /// <param name="mappings"></param>
        protected internal override void AdaptTypeMappings(IDictionary<string, NpgsqlTypeMapping> mappings)
        {
            // Remove unsupported mappings to reduce the number 
            // of ArgumentExceptions thrown and log entries created.
            var unsupportedTypes = mappings.Where(m => !IsPgTypeSupportedByCrateDb(m.Value.PgTypeName)).ToList();
            foreach (var t in unsupportedTypes)
            {
                mappings.Remove(t.Key);
            }

            // Add mappings specific to CrateDB.
            foreach (var t in CrateDbSpecificTypeMappings())
            {
                if (mappings.ContainsKey(t.PgTypeName))
                    mappings[t.PgTypeName] = t;
                else
                    mappings.Add(t.PgTypeName, t);
            }
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

        static IEnumerable<NpgsqlTypeMapping> CrateDbSpecificTypeMappings()
        {
            // Map CrateDB varchar type to the Npgsql TextHandler.
            yield return new NpgsqlTypeMappingBuilder
            {
                PgTypeName = "character varying",
                NpgsqlDbType = NpgsqlDbType.Varchar,
                DbTypes = new[] { DbType.String, DbType.StringFixedLength, DbType.AnsiString, DbType.AnsiStringFixedLength },
                ClrTypes = new[] { typeof(string), typeof(char[]), typeof(char) },
                InferredDbType = DbType.String,
                TypeHandlerFactory = new TextHandlerFactory()
            }
            .Build();

            // Map CrateDB timestampz type to the CrateDbTimestampHandler.
            yield return new NpgsqlTypeMappingBuilder
            {
                PgTypeName = "timestamp with time zone",
                NpgsqlDbType = NpgsqlDbType.TimestampTz,
                DbTypes = new[] { DbType.DateTime },
                ClrTypes = new[] { typeof(DateTime) },
                TypeHandlerFactory = new TimestampHandlerFactory()
            }
            .Build();
        }

        static bool IsPgTypeSupportedByCrateDb(string pgTypeName)
        {
            var internalName = PgTypeNameToInternalName.ContainsKey(pgTypeName) ? PgTypeNameToInternalName[pgTypeName] : pgTypeName;
            return CrateDbBaseTypes.ContainsKey(internalName) || CrateDbArrayTypes.ContainsKey(internalName);
        }
    }
}
