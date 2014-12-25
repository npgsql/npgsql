using System;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    internal class TimeStampTzHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeStampTZ>, ITypeHandler<NpgsqlTimeStampTZ>
    {
        static readonly string[] _pgNames = { "timestamptz" };
        internal override string[] PgNames { get { return _pgNames; } }

        public override DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            return (DateTime)((ITypeHandler<NpgsqlTimeStampTZ>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeStampTZ ITypeHandler<NpgsqlTimeStampTZ>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlTimeStampTZ.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    throw new NotSupportedException();
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }
    }
}
