using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal abstract class TypeHandler
    {
        internal abstract string PgName { get; }
        internal abstract void Read(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output);
    }
}
