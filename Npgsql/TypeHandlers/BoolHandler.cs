using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class BoolHandler : TypeHandler<bool>
    {
        static readonly string[] _pgNames = { "bool" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        const byte T = (byte)'T';
        const byte t = (byte)'t';

        public override bool Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    var b = buf.ReadByte();
                    return b == T || b == t;
                case FormatCode.Binary:
                    return buf.ReadByte() != 0;
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }
    }
}
