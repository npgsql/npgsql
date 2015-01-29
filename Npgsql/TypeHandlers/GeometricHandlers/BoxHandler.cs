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
    /// Type handler for the PostgreSQL geometric box type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("box", NpgsqlDbType.Box, typeof(NpgsqlBox))]
    internal class BoxHandler : TypeHandler<NpgsqlBox>,
        ISimpleTypeReader<NpgsqlBox>,
        ISimpleTypeReader<string>
    {
        public NpgsqlBox Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return new NpgsqlBox(
                new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble()),
                new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble())
            );
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }
    }
}
