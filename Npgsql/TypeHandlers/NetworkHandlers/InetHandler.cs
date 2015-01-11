using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("inet", NpgsqlDbType.Inet, typeof(IPAddress))]
    internal class InetHandler : TypeHandlerWithPsv<IPAddress, NpgsqlInet>,
        ISimpleTypeReader<IPAddress>, ISimpleTypeReader<NpgsqlInet>,
        ISimpleTypeReader<string>
    {
        public IPAddress Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return ((ISimpleTypeReader<NpgsqlInet>)this).Read(buf, fieldDescription, len).addr;
        }

        NpgsqlInet ISimpleTypeReader<NpgsqlInet>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            var addressFamily = buf.ReadByte();
            var mask = buf.ReadByte();
            var isCidr = buf.ReadByte() == 1;
            Contract.Assume(!isCidr);
            var numBytes = buf.ReadByte();
            var bytes = new byte[numBytes];
            for (var i = 0; i < numBytes; i++) {
                bytes[i] = buf.ReadByte();
            }
            return new NpgsqlInet(new IPAddress(bytes), mask);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }
    }
}
