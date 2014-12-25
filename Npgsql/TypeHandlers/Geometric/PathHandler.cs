using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.Geometric
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric path segment type (open or closed).
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    internal class PathHandler : TypeHandler<NpgsqlPath>, ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "path" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        public override NpgsqlPath Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlPath.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    return ReadBinary(buf, fieldDescription, len);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        NpgsqlPath ReadBinary(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
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
            var numPoints = buf.ReadInt32();
            var result = new NpgsqlPath(open);
            for (var i = 0; i < numPoints; i++) {
                result.Add(new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble()));
            }
            return result;
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
