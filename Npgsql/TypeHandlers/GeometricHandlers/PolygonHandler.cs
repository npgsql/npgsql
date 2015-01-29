using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric polygon type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    // TODO: Should be chunking
    [TypeMapping("polygon", NpgsqlDbType.Polygon, typeof(NpgsqlPolygon))]
    internal class PolygonHandler : TypeHandler<NpgsqlPolygon>,
        ISimpleTypeReader<NpgsqlPolygon>,
        ISimpleTypeReader<string>
    {
        public NpgsqlPolygon Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            var numPoints = buf.ReadInt32();
            var points = new List<NpgsqlPoint>(numPoints);
            for (var i = 0; i < numPoints; i++) {
                if (buf.ReadBytesLeft < sizeof(double) * 2)
                    buf.Ensure(Math.Min(sizeof(double) * 2 * (numPoints - i), buf.Size & -(sizeof(double) * 2)));
                points.Add(new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble()));
            }
            return new NpgsqlPolygon(points);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }
    }
}
