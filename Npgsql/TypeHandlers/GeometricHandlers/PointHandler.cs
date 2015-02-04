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
    /// Type handler for the PostgreSQL geometric point type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("point", NpgsqlDbType.Point, typeof(NpgsqlPoint))]
    internal class PointHandler : TypeHandler<NpgsqlPoint>,
        ISimpleTypeReader<NpgsqlPoint>, ISimpleTypeWriter,
        ISimpleTypeReader<string>
    {
        public NpgsqlPoint Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble());
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }

        public int ValidateAndGetLength(object value)
        {
            return 16;
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var s = value as string;
            var v = s != null ? NpgsqlPoint.Parse(s) : (NpgsqlPoint)value;
            buf.WriteDouble(v.X);
            buf.WriteDouble(v.Y);
        }
    }
}
