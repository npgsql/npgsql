using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Handles "conversions" for columns sent by the database with unknown OIDs.
    /// Note that this also happens in the very initial query that loads the OID mappings (chicken and egg problem).
    /// </summary>
    internal class UnknownTypeHandler : SimpleTypeHandler
    {
        static readonly string[] _pgNames = { "<unknown>" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override Type FieldType { get { return typeof(string); } }

        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            output.SetTo(buf.ReadString(len));
        }
    }
}
