using System;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;

namespace Npgsql.EntityFramework7.Query
{
    public class NpgsqlRelationalObjectArrayValueReaderFactory : RelationalValueReaderFactory
    {
        public override IValueReader Create(DbDataReader dataReader)
        {
            Debug.Assert(dataReader != null); // hot path

            return new NpgsqlRelationalObjectArrayValueReader(dataReader);
        }
    }
}