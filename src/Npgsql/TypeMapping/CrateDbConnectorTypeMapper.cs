using JetBrains.Annotations;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandlers.DateTimeHandlers;
using NpgsqlTypes;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Npgsql.TypeMapping
{
    class CrateDbConnectorTypeMapperFactory : IConnectorTypeMapperFactory
    {
        [NotNull]
        public Task<ConnectorTypeMapper> Load(NpgsqlConnection conn, NpgsqlConnector connector)
        {
            if (conn.PostgresParameters.ContainsKey("crate_version"))
            {
                return Task.FromResult((ConnectorTypeMapper)new CrateDbConnectorTypeMapper(connector));
            }
            return Task.FromResult<ConnectorTypeMapper>(null);
        }
    }

    internal class CrateDbConnectorTypeMapper : ConnectorTypeMapper
    {
        internal CrateDbConnectorTypeMapper(NpgsqlConnector connector) : base(connector)
        {
            AdaptCrateDbSpecificMappings();
        }

        public override void Reset()
        {
            ClearBindings();
            ResetMappings();
            AdaptCrateDbSpecificMappings();
            BindTypes();
        }

        /// <summary>
        /// Modifies the type mapping to the specific cratedb type mappings
        /// </summary>
        void AdaptCrateDbSpecificMappings()
        {
            var characterPgTypeName = "character varying";
            Mappings[characterPgTypeName]  = 
            new NpgsqlTypeMappingBuilder
            {
                PgTypeName = characterPgTypeName,
                NpgsqlDbType = NpgsqlDbType.Varchar,
                DbTypes = new[] { DbType.String, DbType.StringFixedLength, DbType.AnsiString, DbType.AnsiStringFixedLength },
                ClrTypes = new[] { typeof(string), typeof(char[]), typeof(char) },
                InferredDbType = DbType.String,
                TypeHandlerFactory = new TextHandlerFactory()
            }
            .Build();

            // Map CrateDB timestampz type to the CrateDbTimestampHandler.
            var timestampWithoutTimeZonePgTypeName = "timestamp with time zone";
            Mappings[timestampWithoutTimeZonePgTypeName] = new NpgsqlTypeMappingBuilder
            {
                PgTypeName = timestampWithoutTimeZonePgTypeName,
                NpgsqlDbType = NpgsqlDbType.TimestampTz,
                DbTypes = new[] { DbType.DateTime },
                ClrTypes = new[] { typeof(DateTime) },
                TypeHandlerFactory = new TimestampHandlerFactory()
            }
            .Build();
        }
    }
}
