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
    /// Type handler for the PostgreSQL geometric path segment type (open or closed).
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    // TODO: Should be chunking
    [TypeMapping("path", NpgsqlDbType.Path, typeof(NpgsqlPath))]
    internal class PathHandler : TypeHandler<NpgsqlPath>,
        ISimpleTypeReader<NpgsqlPath>,
        ISimpleTypeReader<string>
    {
        public NpgsqlPath Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            bool open;
            var openByte = buf.ReadByte();
            switch (openByte)
            {
                case 1:
                    open = false;
                    break;
                case 0:
                    open = true;
                    break;
                default:
                    throw new Exception("Error decoding binary geometric path: bad open byte");
            }
            buf.Ensure(4);
            var numPoints = buf.ReadInt32();
            var result = new NpgsqlPath(open);
            for (var i = 0; i < numPoints; i++) {
                if (buf.ReadBytesLeft < sizeof(double) * 2)
                    buf.Ensure(Math.Min(sizeof(double) * 2 * (numPoints - i), buf.Size & -(sizeof(double) * 2)));
                result.Add(new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble()));
            }
            return result;
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }
    }
}
