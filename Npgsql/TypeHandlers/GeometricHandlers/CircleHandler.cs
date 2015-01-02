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
    /// Type handler for the PostgreSQL geometric circle type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    internal class CircleHandler : TypeHandler<NpgsqlCircle>, ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "circle" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Circle };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }

        public override NpgsqlCircle Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlCircle.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    return new NpgsqlCircle(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
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
