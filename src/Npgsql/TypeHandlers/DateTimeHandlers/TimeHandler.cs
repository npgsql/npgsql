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
            => new TimeHandler(conn.HasIntegerDateTimes);
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class TimeHandler : NpgsqlSimpleTypeHandler<TimeSpan>
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Some PostgreSQL-like databases (e.g. CrateDB) use floating-point representation by default and do not
        /// provide the option of switching to integer format.
        /// </summary>
        readonly bool _integerFormat;

        public TimeHandler(bool integerFormat)
        {
            _integerFormat = integerFormat;
        }

        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            if (_integerFormat)
                // PostgreSQL time resolution == 1 microsecond == 10 ticks
                return new TimeSpan(buf.ReadInt64() * 10);
            else
                return TimeSpan.FromSeconds(buf.ReadDouble());
        }

        public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter parameter)
            => 8;

        public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                buf.WriteInt64(value.Ticks / 10);
            else
                buf.WriteDouble(value.TotalSeconds);
        }
    }
}
