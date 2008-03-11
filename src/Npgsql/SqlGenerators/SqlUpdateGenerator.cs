#if ENTITIES
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Npgsql.SqlGenerators
{
    class SqlUpdateGenerator : SqlBaseGenerator
	{
        public override void BuildCommand(DbCommand command)
        {
            throw new NotImplementedException();
        }
	}
}
#endif