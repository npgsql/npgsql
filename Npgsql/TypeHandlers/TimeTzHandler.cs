using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    internal class TimeTzHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeTZ>, ITypeHandler<NpgsqlTimeTZ>
    {
        static readonly string[] _pgNames = { "timetz" };
        internal override string[] PgNames { get { return _pgNames; } }

        public override DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeTZ?
            return (DateTime)((ITypeHandler<NpgsqlTimeTZ>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeTZ ITypeHandler<NpgsqlTimeTZ>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlTimeTZ.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    throw new NotSupportedException();
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }
    }
}
