using System;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("interval", NpgsqlDbType.Interval, new[] { typeof(TimeSpan), typeof(NpgsqlTimeSpan) })]
    class IntervalHandlerFactory : NpgsqlTypeHandlerFactory<TimeSpan>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<TimeSpan> Create(NpgsqlConnection conn)
            => conn.HasIntegerDateTimes
                ? new IntervalHandler()
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class IntervalHandler : NpgsqlSimpleTypeHandlerWithPsv<TimeSpan, NpgsqlTimeSpan>
    {
        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => (TimeSpan)((INpgsqlSimpleTypeHandler<NpgsqlTimeSpan>)this).Read(buf, len, fieldDescription);

        protected override NpgsqlTimeSpan ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var ticks = buf.ReadInt64();
            var day = buf.ReadInt32();
            var month = buf.ReadInt32();
            return new NpgsqlTimeSpan(month, day, ticks * 10);
        }

        public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter parameter)
            => 16;

        public override int ValidateAndGetLength(NpgsqlTimeSpan value, NpgsqlParameter parameter)
            => 16;

        public override void Write(NpgsqlTimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteInt64(value.Ticks / 10); // TODO: round?
            buf.WriteInt32(value.Days);
            buf.WriteInt32(value.Months);
        }

        // TODO: Can write directly from TimeSpan
        public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write(value, buf, parameter);
    }
}
