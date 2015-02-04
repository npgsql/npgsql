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
    /// Type handler for the PostgreSQL geometric line segment type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("lseg", NpgsqlDbType.LSeg, typeof(NpgsqlLSeg))]
    internal class LineSegmentHandler : TypeHandler<NpgsqlLSeg>,
        ISimpleTypeReader<NpgsqlLSeg>, ISimpleTypeWriter,
        ISimpleTypeReader<string>
    {
        public NpgsqlLSeg Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return new NpgsqlLSeg(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }

        public int ValidateAndGetLength(object value)
        {
            return 32;
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var s = value as string;
            var v = s != null ? NpgsqlLSeg.Parse(s) : (NpgsqlLSeg)value;
            buf.WriteDouble(v.Start.X);
            buf.WriteDouble(v.Start.Y);
            buf.WriteDouble(v.End.X);
            buf.WriteDouble(v.End.Y);
        }
    }
}
