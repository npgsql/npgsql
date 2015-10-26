using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Storage;
using Npgsql;
using NpgsqlTypes;

namespace Microsoft.Data.Entity.Storage.Internal
{
    public class NpgsqlTypeMapping : RelationalTypeMapping
    {
        public new NpgsqlDbType StoreType { get; }

        internal NpgsqlTypeMapping([NotNull] string defaultTypeName, [NotNull] Type clrType, NpgsqlDbType storeType)
            : base(defaultTypeName, clrType)
        {
            StoreType = storeType;
        }

        protected override void ConfigureParameter([NotNull] DbParameter parameter)
        {
         ((NpgsqlParameter)parameter).NpgsqlDbType = StoreType;
        }
    }
}
