using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using System.Data;
using Npgsql.TypeHandlers;

namespace Npgsql.Compatibility.CrateDB
{
    class CrateDBDatabaseInfo : DatabaseInfo
    {
        public CrateDBDatabaseInfo(string host, int port, string databaseName) 
            : base(host, port, databaseName)
        {
        }

        static readonly string CrateTypesQueryForBaseTypes = GenerateCrateTypesQuery(true);
        static readonly string CrateTypesQueryForArrayTypes = GenerateCrateTypesQuery(false);

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

        internal override void AddVendorSpecificTypeMappings(INpgsqlTypeMapper mapper)
        {
            // Map CrateDB varchar type to the Npgsql TextHandler.
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

            // Map CrateDB timestampz type to the CrateDBTimestampHandler.
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
