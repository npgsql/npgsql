using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("macaddr", NpgsqlDbType.MacAddr, typeof(PhysicalAddress))]
    internal class MacaddrHandler : TypeHandler<PhysicalAddress>, ITypeHandler<string>
    {
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

        PhysicalAddress GetValue(object value)
        {
            var val = value is PhysicalAddress ? (PhysicalAddress)value : PhysicalAddress.Parse((string)value);
            if (val.GetAddressBytes().Length != 6)
                throw new FormatException("MAC addresses must have length 6 in PostgreSQL");
            return val;
        }

        internal override int Length(object value)
        {
            return 6;
        }

        internal override void WriteBinary(object value, NpgsqlBuffer buf)
        {
            var val = GetValue(value);
            buf.WriteBytes(val.GetAddressBytes());
        }
    }
}
