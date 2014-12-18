using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-net-types.html
    /// </remarks>
    internal class InetHandler : TypeHandlerWithPsv<IPAddress, NpgsqlInet>, ITypeHandler<NpgsqlInet>,
        ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "inet" };
        internal override string[] PgNames { get { return _pgNames; } }

        public override IPAddress Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return ((ITypeHandler<NpgsqlInet>)this).Read(buf, fieldDescription, len).addr;
        }

        NpgsqlInet ITypeHandler<NpgsqlInet>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return new NpgsqlInet(buf.ReadString(len));
        }

        string ITypeHandler<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return ((ITypeHandler<NpgsqlInet>)this).Read(buf, fieldDescription, len).ToString();
        }
    }
}
