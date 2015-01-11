using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Handles "conversions" for columns sent by the database with unknown OIDs.
    /// This differs from TextHandler in that its a text-only handler (we don't want to receive binary
    /// representations of the types registered here).
    /// Note that this handler is also used in the very initial query that loads the OID mappings
    /// (chicken and egg problem).
    /// </summary>
    internal class UnrecognizedTypeHandler : TextHandler
    {
        public override bool SupportsBinaryRead { get { return false; } }
        public override bool SupportsBinaryWrite { get { return false; } }

        public override string Read(NpgsqlBuffer buf, Messages.FieldDescription fieldDescription, int len)
        {
            if (fieldDescription.IsBinaryFormat)
                throw new NotSupportedException("Sorry, this type was sent in binary and no type handler for this type is available in Npgsql.");
            return buf.ReadString(len);
        }
    }
}
