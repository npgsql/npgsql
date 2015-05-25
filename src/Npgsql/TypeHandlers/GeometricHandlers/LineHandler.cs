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
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("line", NpgsqlDbType.Line, typeof(NpgsqlLine))]
    internal class LineHandler : TypeHandler<NpgsqlLine>, ISimpleTypeWriter,
        ISimpleTypeReader<NpgsqlLine>,
        ISimpleTypeReader<string>
    {
        public NpgsqlLine Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return new NpgsqlLine(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).ToString();
        }

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is NpgsqlLine))
                throw CreateConversionException(value.GetType());
            return 24;
        }

        public void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            var v = (NpgsqlLine)value;
            buf.WriteDouble(v.A);
            buf.WriteDouble(v.B);
            buf.WriteDouble(v.C);
        }
    }
}
