using System.Linq;
using JetBrains.Annotations;

namespace Microsoft.Data.Entity.Scaffolding
{
    internal static class NpgsqlTableSelectionSetExtensions
    {
        public static bool Allows(this TableSelectionSet _tableSelectionSet, [NotNull] string schemaName, [NotNull] string tableName)
        {
            if (_tableSelectionSet == null
                || (_tableSelectionSet.Schemas.Count == 0
                && _tableSelectionSet.Tables.Count == 0))
            {
                return true;
            }

            if (_tableSelectionSet.Schemas.Contains(schemaName))
            {
                return true;
            }

            return _tableSelectionSet.Tables.Contains($"{schemaName}.{tableName}")
                || _tableSelectionSet.Tables.Contains($"[{schemaName}].[{tableName}]")
                || _tableSelectionSet.Tables.Contains($"{schemaName}.[{tableName}]")
                || _tableSelectionSet.Tables.Contains($"[{schemaName}].{tableName}")
                || _tableSelectionSet.Tables.Contains($"{tableName}")
                || _tableSelectionSet.Tables.Contains($"[{tableName}]");
        }
    }
}
