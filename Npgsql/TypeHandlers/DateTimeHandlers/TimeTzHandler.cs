﻿using System;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timetz", NpgsqlDbType.TimeTZ)]
    internal class TimeTzHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeTZ>,
        ISimpleTypeReader<DateTime>, ISimpleTypeReader<NpgsqlTimeTZ>
    {
        public override bool SupportsBinaryWrite
        {
            get
            {
                return false; // TODO: Implement
            }
        }

        public DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeTZ?
            return (DateTime)((ISimpleTypeReader<NpgsqlTimeTZ>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeTZ ISimpleTypeReader<NpgsqlTimeTZ>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // Adjusting from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
            return new NpgsqlTimeTZ(buf.ReadInt64() * 10, new NpgsqlTimeZone(0, 0, -buf.ReadInt32()));
        }
    }
}
