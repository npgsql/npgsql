using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-uuid.html
    /// </remarks>
    internal class UuidHandler : TypeHandler<Guid>, ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "uuid" };
        internal override string[] PgNames { get { return _pgNames; } }

        public override Guid Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Implement binary
            return new Guid(buf.ReadString(len));
        }

        string ITypeHandler<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }
    }
}
