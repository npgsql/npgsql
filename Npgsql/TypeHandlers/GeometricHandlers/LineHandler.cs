using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric line type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("line", NpgsqlDbType.Line, typeof(NpgsqlLine))]
    internal class LineHandler : TypeHandler<NpgsqlLine>, ISimpleTypeWriter,
        ISimpleTypeReader<NpgsqlLine>,
        ISimpleTypeReader<string>
    {
        public NpgsqlLine Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return new NpgsqlLine(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }

        public int ValidateAndGetLength(object value)
        {
            return 24;
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var s = value as string;
            var v = s != null ? NpgsqlLine.Parse(s) : (NpgsqlLine)value;
            buf.WriteDouble(v.A);
            buf.WriteDouble(v.B);
            buf.WriteDouble(v.C);
        }
    }
}
