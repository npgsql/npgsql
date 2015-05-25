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
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("point", NpgsqlDbType.Point, typeof(NpgsqlPoint))]
    internal class PointHandler : TypeHandler<NpgsqlPoint>,
        ISimpleTypeReader<NpgsqlPoint>, ISimpleTypeWriter,
        ISimpleTypeReader<string>
    {
        public NpgsqlPoint Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble());
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).ToString();
        }

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is NpgsqlPoint))
                throw CreateConversionException(value.GetType());
            return 16;
        }

        public void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            var v = (NpgsqlPoint)value;
            buf.WriteDouble(v.X);
            buf.WriteDouble(v.Y);
        }
    }
}
