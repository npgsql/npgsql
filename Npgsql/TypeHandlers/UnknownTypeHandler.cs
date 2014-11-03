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
    internal class UnknownTypeHandler : TypeHandler
    {
        internal override string PgName { get { return "<unknown>"; } }

        internal override void Read(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            if (field.FormatCode == FormatCode.Binary) {
                throw new InvalidCastException(String.Format("Got unknown type {0} (OID {1}) in binary format", field.Name, field.OID));
            }
            output.SetTo(buf.ReadString(len));
        }
    }
}
