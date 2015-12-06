using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Internal;
using Microsoft.Data.Entity.Scaffolding.Internal;
using Microsoft.Data.Entity.Scaffolding.Metadata;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Microsoft.Data.Entity.Scaffolding
{
    public class NpgsqlDatabaseModelFactory : IDatabaseModelFactory
    {
        private NpgsqlConnection _connection;
        private TableSelectionSet _tableSelectionSet;
        private DatabaseModel _databaseModel;
        private Dictionary<string, TableModel> _tables;
        private Dictionary<string, ColumnModel> _tableColumns;

        private static string TableKey(TableModel table) => TableKey(table.Name, table.SchemaName);
        private static string TableKey(string name, string schema = null) => "[" + (schema ?? "") + "].[" + name + "]";
        private static string ColumnKey(TableModel table, string columnName) => TableKey(table) + ".[" + columnName + "]";

        private void ResetState()
        {
            _connection = null;
            _tableSelectionSet = null;
            _databaseModel = new DatabaseModel();
            _tables = new Dictionary<string, TableModel>();
            _tableColumns = new Dictionary<string, ColumnModel>(StringComparer.OrdinalIgnoreCase);
        }

        public DatabaseModel Create(string connectionString, TableSelectionSet tableSelectionSet)
        {
            Check.NotEmpty(connectionString, nameof(connectionString));
            Check.NotNull(tableSelectionSet, nameof(tableSelectionSet));

            ResetState();

            using (_connection = new NpgsqlConnection(connectionString))
            {
                _connection.Open();
                _tableSelectionSet = tableSelectionSet;

                _databaseModel.DatabaseName = _connection.Database;
                _databaseModel.DefaultSchemaName = "public";

                GetTables();
                GetColumns();
                GetConstraints();
                GetIndexes();
                return _databaseModel;
            }
        }

        const string GetTablesQuery = @"
            SELECT nspname, relname
            FROM pg_class AS cl
            JOIN pg_namespace AS ns ON ns.oid = cl.relnamespace
            WHERE
              cl.relkind = 'r' AND
              ns.nspname NOT IN ('pg_catalog', 'information_schema') AND
              relname <> '" + HistoryRepository.DefaultTableName + "'";

        void GetTables()
        {
            using (var command = new NpgsqlCommand(GetTablesQuery, _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var table = new TableModel
                    {
                        SchemaName = reader.GetString(0),
                        Name = reader.GetString(1)
                    };

                    if (_tableSelectionSet.Allows(table.SchemaName, table.Name))
                    {
                        _databaseModel.Tables.Add(table);
                        _tables[TableKey(table)] = table;
                    }
                }
            }
        }

        const string GetColumnsQuery = @"
            SELECT
                nspname, relname, attname, typname, attnum, atttypmod,
                (NOT attnotnull) AS nullable,
                CASE WHEN atthasdef THEN (SELECT pg_get_expr(adbin, cls.oid) FROM pg_attrdef WHERE adrelid = cls.oid AND adnum = attr.attnum) ELSE NULL END AS default
            FROM pg_class AS cls
            JOIN pg_namespace AS ns ON ns.oid = cls.relnamespace
            LEFT OUTER JOIN pg_attribute AS attr ON attrelid = cls.oid
            LEFT OUTER JOIN pg_type AS typ ON attr.atttypid = typ.oid
            WHERE
              relkind = 'r' AND
              nspname NOT IN ('pg_catalog', 'information_schema') AND
              relname <> '" + HistoryRepository.DefaultTableName + @"' AND
              attnum > 0
            ORDER BY attnum";

        private void GetColumns()
        {
            using (var command = new NpgsqlCommand(GetColumnsQuery, _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var schemaName = reader.GetString(0);
                    var tableName = reader.GetString(1);
                    if (!_tableSelectionSet.Allows(schemaName, tableName))
                    {
                        continue;
                    }

                    var columnName = reader.GetString(2);
                    var dataType = reader.GetString(3);
                    var ordinal = reader.GetInt32(4) - 1;
                    var typeModifier = reader.GetInt32(5);
                    var isNullable = reader.GetBoolean(6);
                    int? maxLength = null;
                    int? precision = null;
                    int? scale = null;
                    var defaultValue = reader.IsDBNull(7) ? null : reader.GetString(7);

                    if (typeModifier != -1)
                    {
                        switch (dataType)
                        {
                        case "bpchar":
                        case "char":
                        case "varchar":
                            maxLength = typeModifier - 4;
                            break;
                        case "numeric":
                        case "decimal":
                            // See http://stackoverflow.com/questions/3350148/where-are-numeric-precision-and-scale-for-a-field-found-in-the-pg-catalog-tables
                            precision = ((typeModifier - 4) >> 16) & 65535;
                            scale = (typeModifier - 4) & 65535;
                            break;
                        }
                    }

                    var table = _tables[TableKey(tableName, schemaName)];
                    var column = new NpgsqlColumnModel
                    {
                        Table          = table,
                        Name           = columnName,
                        DataType       = dataType,
                        Ordinal        = ordinal,
                        IsNullable     = isNullable,
                        MaxLength      = maxLength,
                        Precision      = precision,
                        Scale          = scale,
                        DefaultValue   = defaultValue
                    };

                    // Somewhat hacky... We identify serial columns by examining their default expression,
                    // and reverse-engineer these as ValueGenerated.OnAdd
                    if (defaultValue != null && (
                          defaultValue == $"nextval('{tableName}_{columnName}_seq'::regclass)" ||
                          defaultValue == $"nextval('\"{tableName}_{columnName}_seq\"'::regclass)")
                       )
                    {
                        column.IsSerial = true;
                        column.ValueGenerated = ValueGenerated.OnAdd;
                        column.DefaultValue = null;
                    }

                    table.Columns.Add(column);
                    _tableColumns.Add(ColumnKey(table, column.Name), column);
                }
            }
        }

        const string GetIndexesQuery = @"
            SELECT
                nspname, cls.relname, idxcls.relname, indisunique, indkey,
                CASE WHEN indexprs IS NULL THEN NULL ELSE pg_get_expr(indexprs, cls.oid) END
            FROM pg_class AS cls
            JOIN pg_namespace AS ns ON ns.oid = cls.relnamespace
            JOIN pg_index AS idx ON indrelid = cls.oid
            JOIN pg_class AS idxcls ON idxcls.oid = indexrelid
            WHERE
              cls.relkind = 'r' AND
              nspname NOT IN ('pg_catalog', 'information_schema') AND
              cls.relname <> '" + HistoryRepository.DefaultTableName + @"' AND
              NOT indisprimary";

        /// <remarks>
        /// Primary keys are handled as in <see cref="GetConstraints"/>, not here
        /// </remarks>
        void GetIndexes()
        {
            using (var command = new NpgsqlCommand(GetIndexesQuery, _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var schemaName = reader.GetString(0);
                    var tableName = reader.GetString(1);
                    var indexName = reader.GetString(2);

                    if (!_tableSelectionSet.Allows(schemaName, tableName))
                    {
                        continue;
                    }

                    TableModel table;
                    if (!_tables.TryGetValue(TableKey(tableName, schemaName), out table))
                    {
                        continue;
                    }

                    var index = new NpgsqlIndexModel
                    {
                        Table = table,
                        Name = indexName,
                        IsUnique = reader.GetBoolean(3)
                    };

                    table.Indexes.Add(index);

                    var columnIndices = reader.GetFieldValue<short[]>(4);
                    if (columnIndices.Any(i => i == 0))
                    {
                        if (reader.IsDBNull(5)) {
                            throw new Exception($"Seen 0 in indkey for index {indexName} but indexprs is null");
                        }
                        index.Expression = reader.GetString(5);
                    }
                    else foreach (var column in columnIndices.Select(i => table.Columns[i - 1]))
                    {
                        index.Columns.Add(column);
                    }
                }
            }
        }

        const string GetConstraintsQuery = @"
            SELECT
                ns.nspname, cls.relname, conname, contype, conkey, frnns.nspname, frncls.relname, confkey, confdeltype
            FROM pg_class AS cls
            JOIN pg_namespace AS ns ON ns.oid = cls.relnamespace
            JOIN pg_constraint as con ON con.conrelid = cls.oid
            LEFT OUTER JOIN pg_class AS frncls ON frncls.oid = con.confrelid
            LEFT OUTER JOIN pg_namespace as frnns ON frnns.oid = frncls.relnamespace
            WHERE
                cls.relkind = 'r' AND
                ns.nspname NOT IN ('pg_catalog', 'information_schema') AND
                cls.relname <> '" + HistoryRepository.DefaultTableName + @"' AND
                con.contype IN ('p', 'f')";

        void GetConstraints()
        {
            using (var command = new NpgsqlCommand(GetConstraintsQuery, _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var schemaName = reader.GetString(0);
                    var tableName = reader.GetString(1);

                    if (!_tableSelectionSet.Allows(schemaName, tableName))
                    {
                        continue;
                    }
                    var table = _tables[TableKey(tableName, schemaName)];

                    var constraintName = reader.GetString(2);
                    var constraintType = reader.GetChar(3);
                    switch (constraintType)
                    {
                    case 'p':
                        var pkColumnIndices = reader.GetFieldValue<short[]>(4);
                        for (var i = 0; i < pkColumnIndices.Length; i++)
                        {
                            table.Columns[pkColumnIndices[i] - 1].PrimaryKeyOrdinal = i + 1;
                        }
                        continue;

                    case 'f':
                        var foreignSchemaName = reader.GetString(5);
                        var foreignTableName = reader.GetString(6);
                        TableModel principalTable;
                        if (!_tables.TryGetValue(TableKey(foreignTableName, foreignSchemaName), out principalTable))
                        {
                            continue;
                        }

                        var fkInfo = new ForeignKeyModel
                        {
                            Name = constraintName,
                            Table = table,
                            PrincipalTable = principalTable,
                            Columns = reader.GetFieldValue<short[]>(4).Select(i => table.Columns[i-1]).ToList(),
                            PrincipalColumns = reader.GetFieldValue<short[]>(7).Select(i => principalTable.Columns[i-1]).ToList(),
                            OnDelete = ConvertToReferentialAction(reader.GetChar(8))
                        };

                        table.ForeignKeys.Add(fkInfo);
                        break;

                    default:
                        throw new NotSupportedException($"Unknown constraint type code {constraintType} for constraint {constraintName}");
                    }
                }
            }
        }

        static ReferentialAction? ConvertToReferentialAction(char onDeleteAction)
        {
            switch (onDeleteAction)
            {
            case 'a':
                return ReferentialAction.NoAction;
            case 'r':
                return ReferentialAction.Restrict;
            case 'c':
                return ReferentialAction.Cascade;
            case 'n':
                return ReferentialAction.SetNull;
            case 'd':
                return ReferentialAction.SetDefault;
            default:
                throw new ArgumentOutOfRangeException($"Unknown value {onDeleteAction} for foreign key deletion action code");
            }
        }

#if NO
        private void GetForeignKeys()
        {
            var command = _connection.CreateCommand();
            command.CommandText = @"SELECT 
    f.name AS foreign_key_name,
    schema_name(f.schema_id) AS [schema_name],
    object_name(f.parent_object_id) AS table_name,
    object_schema_name(f.referenced_object_id) AS principal_table_schema_name,
    object_name(f.referenced_object_id) AS principal_table_name,
    col_name(fc.parent_object_id, fc.parent_column_id) AS constraint_column_name,
    col_name(fc.referenced_object_id, fc.referenced_column_id) AS referenced_column_name,
    is_disabled,
    delete_referential_action_desc,
    update_referential_action_desc
FROM sys.foreign_keys AS f
INNER JOIN sys.foreign_key_columns AS fc 
   ON f.object_id = fc.constraint_object_id
ORDER BY f.name, fc.constraint_column_id";
            using (var reader = command.ExecuteReader())
            {
                var lastFkName = "";
                ForeignKeyModel fkInfo = null;
                while (reader.Read())
                {
                    var fkName = reader.GetString(0);
                    var schemaName = reader.GetString(1);
                    var tableName = reader.GetString(2);

                    if (!_tableSelectionSet.Allows(schemaName, tableName))
                    {
                        continue;
                    }
                    if (fkInfo == null
                        || lastFkName != fkName)
                    {
                        lastFkName = fkName;
                        var principalSchemaTableName = reader.GetString(3);
                        var principalTableName = reader.GetString(4);
                        var table = _tables[TableKey(tableName, schemaName)];
                        TableModel principalTable;
                        _tables.TryGetValue(TableKey(principalTableName, principalSchemaTableName), out principalTable);

                        fkInfo = new ForeignKeyModel
                        {
                            Table = table,
                            PrincipalTable = principalTable
                        };

                        table.ForeignKeys.Add(fkInfo);
                    }
                    var fromColumnName = reader.GetString(5);
                    var fromColumn = _tableColumns[ColumnKey(fkInfo.Table, fromColumnName)];
                    fkInfo.Columns.Add(fromColumn);

                    if (fkInfo.PrincipalTable != null)
                    {
                        var toColumnName = reader.GetString(6);
                        var toColumn = _tableColumns[ColumnKey(fkInfo.PrincipalTable, toColumnName)];
                        fkInfo.PrincipalColumns.Add(toColumn);
                    }

                    fkInfo.OnDelete = ConvertToReferentialAction(reader.GetString(8));
                }
            }
        }
#endif

#if NO
        private static ReferentialAction? ConvertToReferentialAction(string onDeleteAction)
        {
            switch (onDeleteAction.ToUpperInvariant())
            {
            case "RESTRICT":
                return ReferentialAction.Restrict;

            case "CASCADE":
                return ReferentialAction.Cascade;

            case "SET_NULL":
                return ReferentialAction.SetNull;

            case "SET_DEFAULT":
                return ReferentialAction.SetDefault;

            case "NO_ACTION":
                return ReferentialAction.NoAction;

            default:
                return null;
            }
        }
#endif
            }
}
