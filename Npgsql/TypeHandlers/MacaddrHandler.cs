using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-net-types.html
    /// </remarks>
    internal class MacaddrHandler : TypeHandler<PhysicalAddress>, ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "macaddr" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        public override PhysicalAddress Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return ReadText(buf, fieldDescription, len);
                case FormatCode.Binary:
                    return ReadBinary(buf, fieldDescription, len);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        PhysicalAddress ReadBinary(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            Contract.Assume(len == 6);
            return new PhysicalAddress(new[] {
                buf.ReadByte(),
                buf.ReadByte(),
                buf.ReadByte(),
                buf.ReadByte(),
                buf.ReadByte(),
                buf.ReadByte()
            });
        }

        PhysicalAddress ReadText(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            string lowerMacAddr = buf.ReadString(len).ToUpper();
            var sb = new StringBuilder();
            foreach (var c in lowerMacAddr)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F'))
                {
                    sb.Append(c);
                }
            }
            return PhysicalAddress.Parse(sb.ToString());            
        }

        string ITypeHandler<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }
    }
}
