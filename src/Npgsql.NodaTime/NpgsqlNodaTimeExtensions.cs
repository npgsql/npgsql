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
                    PgTypeName = "timestamp",
                    NpgsqlDbType = NpgsqlDbType.Timestamp,
                    DbTypes = new[] { DbType.DateTime, DbType.DateTime2 },
                    ClrTypes = new[] { typeof(Instant), typeof(LocalDateTime) },
                    InferredDbType = DbType.DateTime,
                    TypeHandlerFactory = new TimestampHandlerFactory()
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "timestamp with time zone",
                    NpgsqlDbType = NpgsqlDbType.TimestampTz,
                    ClrTypes = new[] { typeof(ZonedDateTime), typeof(OffsetDateTime) },
                    TypeHandlerFactory = new TimestampTzHandlerFactory()
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "date",
                    NpgsqlDbType = NpgsqlDbType.Date,
                    DbTypes = new[] { DbType.Date },
                    ClrTypes = new[] { typeof(LocalDate) },
                    TypeHandlerFactory = new DateHandlerFactory()
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "time",
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
                    ClrTypes = new[] { typeof(Period) },
                    TypeHandlerFactory = new IntervalHandlerFactory()
                }.Build());
    }
}
