using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("macaddr", NpgsqlDbType.MacAddr, typeof(PhysicalAddress))]
    internal class MacaddrHandler : TypeHandler<PhysicalAddress>,
        ISimpleTypeReader<PhysicalAddress>, ISimpleTypeWriter,
        ISimpleTypeReader<string>
    {
        public PhysicalAddress Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
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

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
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

        public int ValidateAndGetLength(object value) { return 6; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var val = GetValue(value);
            buf.WriteBytes(val.GetAddressBytes());
        }
    }
}
