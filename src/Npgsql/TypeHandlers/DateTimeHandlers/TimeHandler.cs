using System;
using System.Data;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("time", NpgsqlDbType.Time, new[] { DbType.Time })]
    class TimeHandlerFactory : NpgsqlTypeHandlerFactory<TimeSpan>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<TimeSpan> Create(NpgsqlConnection conn)
            => conn.HasIntegerDateTimes
                ? new TimeHandler()
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class TimeHandler : NpgsqlSimpleTypeHandler<TimeSpan>
    {
        // PostgreSQL time resolution == 1 microsecond == 10 ticks
        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => new TimeSpan(buf.ReadInt64() * 10);

        public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter parameter)
            => 8;

        public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64(value.Ticks / 10);
    }
}
