using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("date", NpgsqlDbType.Date, DbType.Date)]
    internal class DateHandler : TypeHandlerWithPsv<DateTime, NpgsqlDate>,
        ISimpleTypeReader<DateTime>, ISimpleTypeReader<NpgsqlDate>, ISimpleTypeWriter
    {
        internal const int PostgresEpochJdate = 2451545; // == date2j(2000, 1, 1)
        internal const int MonthsPerYear = 12;

        public DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlDate?
            return (System.DateTime) ((ISimpleTypeReader<NpgsqlDate>) this).Read(buf, fieldDescription, len);
        }

        /// <remarks>
        /// Copied wholesale from Postgresql backend/utils/adt/datetime.c:j2date
        /// </remarks>
        NpgsqlDate ISimpleTypeReader<NpgsqlDate>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            var binDate = buf.ReadInt32();

            if (binDate == int.MaxValue)
                return new NpgsqlDate(DateTime.MaxValue);
            if (binDate == int.MinValue)
                return new NpgsqlDate(DateTime.MinValue);

            // TODO: Is this really necessary? binDate is the number of days since 2000-01-01 and NpgsqlDate stores the number of days since 0001-01-01,
            // so we should just shift the integer by the number of days between 0001-01-01 and 2000-01-01?

            var julian = (uint)(binDate + PostgresEpochJdate);
            uint quad;
            uint extra;
            uint y;

            julian += 32044;
            quad = julian / 146097;
            extra = (julian - quad * 146097) * 4 + 3;
            julian += 60 + quad * 3 + extra / 146097;
            quad = julian / 1461;
            julian -= quad * 1461;
            y = julian * 4 / 1461;
            julian = ((y != 0) ? ((julian + 305) % 365) : ((julian + 306) % 366))
                    + 123;
            y += quad * 4;
            var year = y - 4800;
            quad = julian * 2141 / 65536;
            var day = julian - 7834 * quad / 256;
            var month = (quad + 10) % MonthsPerYear + 1;

            return new NpgsqlDate((int)year, (int)month, (int)day);
        }

        public int ValidateAndGetLength(object value) { return 4; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            if (value is DateTime)
            {
                value = new NpgsqlDate((DateTime)value);
            }
            else if (value is string)
            {
                value = NpgsqlDate.Parse((string)value);
            }

            var dt = (NpgsqlDate)value;
            if (dt == new NpgsqlDate(DateTime.MinValue))
                buf.WriteInt32(int.MinValue);
            else if (dt == new NpgsqlDate(DateTime.MaxValue))
                buf.WriteInt32(int.MaxValue);
            else
                buf.WriteInt32(dt.DaysSinceEra - 730119);
        }
    }
}
