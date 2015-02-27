using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("cidr", NpgsqlDbType.Cidr)]
    internal class CidrHandler : TypeHandler<NpgsqlInet>,
        ISimpleTypeReader<NpgsqlInet>, ISimpleTypeWriter, ISimpleTypeReader<string>
    {
        public NpgsqlInet Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return InetHandler.DoRead(buf, fieldDescription, len, true);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).ToString();
        }

        public int ValidateAndGetLength(object value)
        {
            return InetHandler.DoValidateAndGetLength(value);
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            InetHandler.DoWrite(value, buf, true);
        }
    }
}
