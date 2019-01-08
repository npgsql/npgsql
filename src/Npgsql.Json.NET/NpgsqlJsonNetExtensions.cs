using System;
using Npgsql.Json.NET;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Npgsql
{
    /// <summary>
    /// Extension allowing adding the Json.NET plugin to an Npgsql type mapper.
    /// </summary>
    public static class NpgsqlJsonNetExtensions
    {
        /// <summary>
        /// Sets up JSON.NET mappings for the PostgreSQL json and jsonb types.
        /// </summary>
        /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
        /// <param name="jsonbClrTypes">A list of CLR types to map to PostgreSQL jsonb (no need to specify NpgsqlDbType.Jsonb)</param>
        /// <param name="jsonClrTypes">A list of CLR types to map to PostgreSQL json (no need to specify NpgsqlDbType.Json)</param>
        /// <param name="settings">Optional settings to customize JSON serialization</param>
        public static INpgsqlTypeMapper UseJsonNet(
            this INpgsqlTypeMapper mapper,
            Type[] jsonbClrTypes = null,
            Type[] jsonClrTypes = null,
            JsonSerializerSettings settings = null
        )
        {
            mapper.AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = "jsonb",
                NpgsqlDbType = NpgsqlDbType.Jsonb,
                ClrTypes = jsonbClrTypes,
                TypeHandlerFactory = new JsonbHandlerFactory(settings)
            }.Build());

            mapper.AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = "json",
                NpgsqlDbType = NpgsqlDbType.Json,
                ClrTypes = jsonClrTypes,
                TypeHandlerFactory = new JsonHandlerFactory(settings)
            }.Build());

            return mapper;
        }
    }
}
