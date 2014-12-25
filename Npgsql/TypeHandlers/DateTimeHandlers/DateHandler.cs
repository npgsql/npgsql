using System;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    internal class DateHandler : TypeHandlerWithPsv<DateTime, NpgsqlDate>, ITypeHandler<NpgsqlDate>
    {
        static readonly string[] _pgNames = { "date" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        internal const int PostgresEpochJdate = 2451545; // == date2j(2000, 1, 1)
        internal const int MonthsPerYear = 12;

        public override DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlDate?
            return (System.DateTime) ((ITypeHandler<NpgsqlDate>) this).Read(buf, fieldDescription, len);
        }

        NpgsqlDate ITypeHandler<NpgsqlDate>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlDate.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    return ReadBinary(buf);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        /// <remarks>
        /// Copied wholesale from Postgresql backend/utils/adt/datetime.c:j2date
        /// </remarks>
        NpgsqlDate ReadBinary(NpgsqlBuffer buf)
        {
            var binDate = buf.ReadInt32();

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

            return new NpgsqlDate((int) year, (int) month, (int) day);
        }
    }
}
