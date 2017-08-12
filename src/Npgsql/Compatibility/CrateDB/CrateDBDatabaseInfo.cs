using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using System.Data;
using Npgsql.TypeHandlers;
using Npgsql.PostgresTypes;

namespace Npgsql.Compatibility.CrateDB
{
    /// <summary>
    /// Adapts type loading queries and type mappings for CrateDB.
    /// </summary>
    /// <remarks>
    /// CrateDB contains support for the PostgreSQL wire protocol and it has a  
    /// trimmed down pg_type table. But it does not have a pg_namespace-, pg_proc-,
    /// pg_range- or pg_attribute-table. Further, the supported datatypes differ from PostgreSQL. 
    /// See: https://crate.io/docs/crate/reference/en/latest/protocols/crate-postgres-sql.html for 
    /// the main differences in implementation and dialect between CrateDB and PostgreSQL.
    /// See: https://crate.io/docs/crate/reference/en/latest/protocols/postgres.html for more 
    /// information about the support of the Postgres Wire Protocol in CrateDB.
    /// </remarks>
    class CrateDBDatabaseInfo : DatabaseInfo
    {
        /// <summary>
        /// Creates an instance of the <see cref="CrateDBDatabaseInfo"/>-class.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="databaseName"></param>
        public CrateDBDatabaseInfo(string host, int port, string databaseName) 
            : base(host, port, databaseName)
        {
        }

        static readonly string CrateTypesQueryForBaseTypes = GenerateCrateTypesQuery(true);
        static readonly string CrateTypesQueryForArrayTypes = GenerateCrateTypesQuery(false);

        // This generates simplified type loading queries for CrateDB as
        // CrateDB has a trimmed down pg_type table and no pg_namespace-, 
        // pg_proc- or pg_range-table.
        static string GenerateCrateTypesQuery(bool forBaseTypes)
        {
            return
$@"select 
    'pg_catalog' as nspname, typname, oid, 0 as typrelid, 0 as typbasetype, 
    '{(forBaseTypes ? "b" : "a")}' as type, 
    {(forBaseTypes ? "0" : "typelem")} as elemoid
from
    pg_catalog.pg_type
where
    {(forBaseTypes ? "" : "not ")}typelem = 0";
        }

        /// <summary>
        /// Overrides DatabaseInfo.LoadBackendTypes to use type loading queries adapted for CrateDB. 
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="timeout"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        protected override async Task LoadBackendTypes(NpgsqlConnector connector, NpgsqlTimeout timeout, bool async)
        {
            await LoadBackendTypes(connector, timeout, async, CrateTypesQueryForBaseTypes);
            await LoadBackendTypes(connector, timeout, async, CrateTypesQueryForArrayTypes);
        }

        async Task LoadBackendTypes(NpgsqlConnector connector, NpgsqlTimeout timeout, bool async, string typesQuery)
        {
            var commandTimeout = 0;  // Default to infinity
            if (timeout.IsSet)
            {
                commandTimeout = (int)timeout.TimeLeft.TotalSeconds;
                if (commandTimeout <= 0)
                    throw new TimeoutException();
            }

            using (var command = new NpgsqlCommand(typesQuery, connector.Connection))
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

        /// <summary>
        /// Overrides DatabaseInfo.TryGetComposite as CrateDB does not support named composite types.
        /// </summary>
        /// <param name="pgName"></param>
        /// <param name="connection"></param>
        /// <param name="compositeType"></param>
        /// <returns></returns>
        internal override bool TryGetComposite(string pgName, NpgsqlConnection connection, out PostgresCompositeType compositeType)
        {
            compositeType = null;
            return false;
        }

        /// <summary>
        /// Overrides DatabaseInfo.AddVendorSpecificTypeMappings to adapt 
        /// the type mappings to the types supported by CrateDB.
        /// </summary>
        /// <param name="mapper"></param>
        internal override void AddVendorSpecificTypeMappings(INpgsqlTypeMapper mapper)
        {
            // CrateDB does not have a text-datatype. Instead there is a varchar-datatype. 
            // Therefore map string to varchar implicitly.
            mapper.AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = "varchar",
                NpgsqlDbType = NpgsqlDbType.Varchar,
                DbTypes = new[] { DbType.String, DbType.StringFixedLength, DbType.AnsiString, DbType.AnsiStringFixedLength },
                ClrTypes = new[] { typeof(string), typeof(char[]), typeof(char) },
                InferredDbType = DbType.String,
                TypeHandlerFactory = new TextHandlerFactory()
            }
            .Build());

            // In CrateDB there is only a timestampz datatype.
            // Therefore map DateTime to timestampz implicitly.
            mapper.AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = "timestampz",
                NpgsqlDbType = NpgsqlDbType.TimestampTz,
                DbTypes = new[] { DbType.DateTime },
                ClrTypes = new[] { typeof(DateTime) },
                TypeHandlerFactory = new CrateDBTimestampHandlerFactory()
            }
            .Build());
        }
    }
}
