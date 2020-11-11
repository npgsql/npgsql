using System;
using System.Data;
using NodaTime;
using Npgsql.NodaTime;
using Npgsql.TypeMapping;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql
{
    /// <summary>
    /// Extension adding the NodaTime plugin to an Npgsql type mapper.
    /// </summary>
    public static class NpgsqlNodaTimeExtensions
    {
        /// <summary>
        /// Sets up NodaTime mappings for the PostgreSQL date/time types.
        /// </summary>
        /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
        public static INpgsqlTypeMapper UseNodaTime(this INpgsqlTypeMapper mapper)
            => mapper
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "timestamp without time zone",
                    NpgsqlDbType = NpgsqlDbType.Timestamp,
                    DbTypes = new[] { DbType.DateTime, DbType.DateTime2 },
                    ClrTypes = new[] { typeof(Instant), typeof(LocalDateTime), typeof(DateTime) },
                    InferredDbType = DbType.DateTime,
                    TypeHandlerFactory = new TimestampHandlerFactory()
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "timestamp with time zone",
                    NpgsqlDbType = NpgsqlDbType.TimestampTz,
                    ClrTypes = new[] { typeof(ZonedDateTime), typeof(OffsetDateTime), typeof(DateTimeOffset) },
                    TypeHandlerFactory = new TimestampTzHandlerFactory()
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "date",
                    NpgsqlDbType = NpgsqlDbType.Date,
                    DbTypes = new[] { DbType.Date },
                    ClrTypes = new[] { typeof(LocalDate),  typeof(NpgsqlDate) },
                    TypeHandlerFactory = new DateHandlerFactory()
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "time without time zone",
                    NpgsqlDbType = NpgsqlDbType.Time,
                    DbTypes = new[] { DbType.Time },
                    ClrTypes = new[] { typeof(LocalTime) },
                    TypeHandlerFactory = new TimeHandlerFactory()
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "time with time zone",
                    NpgsqlDbType = NpgsqlDbType.TimeTz,
                    ClrTypes = new[] { typeof(OffsetTime) },
                    TypeHandlerFactory = new TimeTzHandlerFactory()
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "interval",
                    NpgsqlDbType = NpgsqlDbType.Interval,
                    ClrTypes = new[] { typeof(Period), typeof(Duration), typeof(TimeSpan), typeof(NpgsqlTimeSpan) },
                    TypeHandlerFactory = new IntervalHandlerFactory()
                }.Build());
    }
}
