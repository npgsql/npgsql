using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Update;

namespace EntityFramework7.Npgsql
{
    public interface INpgsqlSqlGenerator : ISqlGenerator
    {
        NpgsqlSqlGenerator.ResultsGrouping AppendBulkInsertOperation(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] IReadOnlyList<ModificationCommand> modificationCommands);
        string GenerateLiteral(Guid literal);
    }
}
