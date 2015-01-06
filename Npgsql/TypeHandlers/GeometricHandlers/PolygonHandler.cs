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
    [TypeMapping("polygon", NpgsqlDbType.Polygon, typeof(NpgsqlPolygon))]
    internal class PolygonHandler : TypeHandler<NpgsqlPolygon>, ITypeHandler<string>
    {
        public override bool IsChunking { get { return true; } }

        public override NpgsqlPolygon Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlPolygon.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    return ReadBinary(buf, fieldDescription, len);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        NpgsqlPolygon ReadBinary(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
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

        string ITypeHandler<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return buf.ReadString(len);
                case FormatCode.Binary:
                    return Read(buf, fieldDescription, len).ToString();
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }
    }
}
