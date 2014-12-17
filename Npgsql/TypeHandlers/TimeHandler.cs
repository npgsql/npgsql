using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    internal class TimeHandler : TypeHandlerWithPsv<DateTime, NpgsqlTime>, ITypeHandler<NpgsqlTime>
    {
        static readonly string[] _pgNames = { "time" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        public override DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTime?
            return (DateTime)((ITypeHandler<NpgsqlTime>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTime ITypeHandler<NpgsqlTime>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlTime.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    // Postgresql time resolution == 1 microsecond == 10 ticks
                    return new NpgsqlTime(buf.ReadInt64() * 10);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }
    }
}
