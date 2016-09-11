using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.PostgresTypes
{
    class UnrecognizedBackendType : PostgresType
    {
        internal UnrecognizedBackendType() : base("", "<unknown>", 0) {}

        internal override TypeHandler Activate(TypeHandlerRegistry registry)
        {
            throw new NotSupportedException("Cannot activate the unknown type");
        }
    }
}
