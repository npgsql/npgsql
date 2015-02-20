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
    /// Also used for sending null parameters with unknown types.
    /// </summary>
    internal class UnrecognizedTypeHandler : TextHandler
    {
        internal UnrecognizedTypeHandler()
        {
            OID = 0;
        }

        internal override void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            if (fieldDescription.IsBinaryFormat) {
                throw new NotSupportedException("The type {0} currently unknown to Npgsql. You can retrieve it as a string by marking it as unknown, please see the FAQ.");
            }
            base.PrepareRead(buf, fieldDescription, len);
        }
    }
}
