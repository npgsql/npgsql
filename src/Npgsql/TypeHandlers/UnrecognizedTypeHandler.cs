using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Handles "conversions" for columns sent by the database with unknown OIDs.
    /// This differs from TextHandler in that its a text-only handler (we don't want to receive binary
    /// representations of the types registered here).
    /// Note that this handler is also used in the very initial query that loads the OID mappings
    /// (chicken and egg problem).
    /// Also used for sending parameters with unknown types (OID=0)
    /// </summary>
    internal class UnrecognizedTypeHandler : TextHandler
    {
        internal UnrecognizedTypeHandler()
        {
            OID = 0;
            PgFullName = "<unknown>";
        }

        internal override void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            if (fieldDescription.IsBinaryFormat) {
                buf.Skip(len);
                throw new SafeReadException(new NotSupportedException(String.Format("The field {0} has a type currently unknown to Npgsql (OID {1}). You can retrieve it as a string by marking it as unknown, please see the FAQ.", fieldDescription.Name, fieldDescription.OID)));
            }
            base.PrepareRead(buf, fieldDescription, len);
        }
    }
}
