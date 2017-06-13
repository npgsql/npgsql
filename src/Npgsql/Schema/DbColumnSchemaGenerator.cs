using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
#if !NETSTANDARD1_3
using System.Transactions;
#endif

namespace Npgsql.Schema
{
    class DbColumnSchemaGenerator
    {
        readonly RowDescriptionMessage _rowDescription;
        readonly NpgsqlConnection _connection;
        readonly bool _fetchAdditionalInfo;

        internal DbColumnSchemaGenerator(NpgsqlConnection connection, RowDescriptionMessage rowDescription, bool fetchAdditionalInfo)
        {
            _connection = connection;
            _rowDescription = rowDescription;
            _fetchAdditionalInfo = fetchAdditionalInfo;
        }

        #region Columns queries

        static string GenerateColumnsQuery(string columnFieldFilter) =>
$@"SELECT
     typ.oid AS typoid, nspname, relname, attname, typ.typname, attrelid, attnum, attnotnull,
     CASE WHEN typ.typtype = 'd' THEN typ.typtypmod ELSE atttypmod END AS typmod,
     CASE WHEN atthasdef THEN (SELECT pg_get_expr(adbin, cls.oid) FROM pg_attrdef WHERE adrelid = cls.oid AND adnum = attr.attnum) ELSE NULL END AS default,
     CASE WHEN col.is_updatable = 'YES' THEN true ELSE false END AS is_updatable,
     EXISTS (
       SELECT * FROM pg_index
       WHERE pg_index.indrelid = cls.oid AND
             pg_index.indisprimary AND
             attnum = ANY (indkey)
     ) AS isprimarykey,
     EXISTS (
       SELECT * FROM pg_index
       WHERE pg_index.indrelid = cls.oid AND
             pg_index.indisunique AND
             attnum = ANY (indkey)
     ) AS isunique
FROM pg_attribute AS attr
JOIN pg_type AS typ ON attr.atttypid = typ.oid
JOIN pg_class AS cls ON cls.oid = attr.attrelid
JOIN pg_namespace AS ns ON ns.oid = cls.relnamespace
LEFT OUTER JOIN information_schema.columns AS col ON col.table_schema = nspname AND
     col.table_name = relname AND
     col.column_name = attname
WHERE
     atttypid <> 0 AND
     relkind IN ('r', 'v', 'm') AND
     NOT attisdropped AND
     nspname NOT IN ('pg_catalog', 'information_schema') AND
     attnum > 0 AND
     ({columnFieldFilter})
ORDER BY attnum";

        /// <summary>
        /// Stripped-down version of <see cref="GenerateColumnsQuery"/>, mainly to support Amazon Redshift.
        /// </summary>
        static string GenerateOldColumnsQuery(string columnFieldFilter) =>
            $@"SELECT
     typ.oid AS typoid, nspname, relname, attname, typ.typname, attrelid, attnum, attnotnull,
     CASE WHEN typ.typtype = 'd' THEN typ.typtypmod ELSE atttypmod END AS typmod,
     CASE WHEN atthasdef THEN (SELECT pg_get_expr(adbin, cls.oid) FROM pg_attrdef WHERE adrelid = cls.oid AND adnum = attr.attnum) ELSE NULL END AS default,
     TRUE AS is_updatable,  /* Supported only since PG 8.2 */
     FALSE AS isprimarykey, /* Can't do ANY() on pg_index.indkey which is int2vector */
     FALSE AS isunique      /* Can't do ANY() on pg_index.indkey which is int2vector */
FROM pg_attribute AS attr
JOIN pg_type AS typ ON attr.atttypid = typ.oid
JOIN pg_class AS cls ON cls.oid = attr.attrelid
JOIN pg_namespace AS ns ON ns.oid = cls.relnamespace
LEFT OUTER JOIN information_schema.columns AS col ON col.table_schema = nspname AND
     col.table_name = relname AND
     col.column_name = attname
WHERE
     atttypid <> 0 AND
     relkind IN ('r', 'v', 'm') AND
     NOT attisdropped AND
     nspname NOT IN ('pg_catalog', 'information_schema') AND
     attnum > 0 AND
     ({columnFieldFilter})
ORDER BY attnum";

        #endregion Column queries

        internal ReadOnlyCollection<NpgsqlDbColumn> GetColumnSchema()
        {
            var fields = _rowDescription.Fields;
            if (fields.Count == 0)
                return new List<NpgsqlDbColumn>().AsReadOnly();

            var result = new List<NpgsqlDbColumn>(fields.Count);
            for (var i = 0; i < fields.Count; i++)
                result.Add(null);
            var populatedColumns = 0;

            // We have two types of fields - those which correspond to actual database columns
            // and those that don't (e.g. SELECT 8). For the former we load lots of info from
            // the backend (if fetchAdditionalInfo is true), for the latter we only have the RowDescription

            var columnFieldFilter = _rowDescription.Fields
                .Where(f => f.TableOID != 0)  // Only column fields
                .Select(c => $"(attr.attrelid={c.TableOID} AND attr.attnum={c.ColumnAttributeNumber})")
                .Join(" OR ");

            if (_fetchAdditionalInfo && columnFieldFilter != "")
            {
                var query = _connection.PostgreSqlVersion >= new Version(8, 2)
                    ? GenerateColumnsQuery(columnFieldFilter)
                    : GenerateOldColumnsQuery(columnFieldFilter);

#if NETSTANDARD1_3
                using (var connection = _connection.Clone())
#else
                using (new TransactionScope(TransactionScopeOption.Suppress))
                using (var connection = (NpgsqlConnection)((ICloneable)_connection).Clone())
#endif
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        for (; reader.Read(); populatedColumns++)
                        {
                            var column = LoadColumnDefinition(reader, _connection.Connector.TypeHandlerRegistry);

                            var ordinal = fields.FindIndex(f => f.TableOID == column.TableOID && f.ColumnAttributeNumber - 1 == column.ColumnAttributeNumber);
                            Debug.Assert(ordinal >= 0);

                            // The column's ordinal is with respect to the resultset, not its table
                            column.ColumnOrdinal = ordinal;

                            result[ordinal] = column;
                        }
                    }

                    if (populatedColumns == fields.Count)
                    {
                        // All columns were regular table columns that got loaded, we're done
                        Debug.Assert(result.All(c => c != null));
                        return result.AsReadOnly();
                    }
                }
            }

            // We had some fields which don't correspond to regular table columns (or fetchAdditionalInfo is false).
            // Fill in whatever info we have from the RowDescription itself
            for (var i = 0; i < fields.Count; i++)
            {
                if (result[i] != null)
                    continue;
                var column = SetUpNonColumnField(fields[i]);
                column.ColumnOrdinal = i;
                result[i] = column;
                populatedColumns++;
            }

            if (populatedColumns != fields.Count)
                throw new NpgsqlException("Could not load all columns for the resultset");

            return result.AsReadOnly();
        }

        NpgsqlDbColumn LoadColumnDefinition(NpgsqlDataReader reader, TypeHandlerRegistry registry)
        {
            var columnName = reader.GetString(reader.GetOrdinal("attname"));
            var column = new NpgsqlDbColumn
            {
                AllowDBNull = !reader.GetBoolean(reader.GetOrdinal("attnotnull")),
                BaseCatalogName = _connection.Database,
                BaseColumnName = columnName,
                BaseSchemaName = reader.GetString(reader.GetOrdinal("nspname")),
                BaseServerName = _connection.Host,
                BaseTableName = reader.GetString(reader.GetOrdinal("relname")),
                ColumnName = columnName,
                ColumnOrdinal = reader.GetInt32(reader.GetOrdinal("attnum")) - 1,
                ColumnAttributeNumber = (short)(reader.GetInt16(reader.GetOrdinal("attnum")) - 1),
                IsKey = reader.GetBoolean(reader.GetOrdinal("isprimarykey")),
                IsReadOnly = !reader.GetBoolean(reader.GetOrdinal("is_updatable")),
                IsUnique = reader.GetBoolean(reader.GetOrdinal("isunique")),
                DataTypeName = reader.GetString(reader.GetOrdinal("typname")),

                TableOID = reader.GetFieldValue<uint>(reader.GetOrdinal("attrelid")),
                TypeOID = reader.GetFieldValue<uint>(reader.GetOrdinal("typoid"))
            };

            column.PostgresType = registry.PostgresTypes.ByOID[column.TypeOID];

            var defaultValueOrdinal = reader.GetOrdinal("default");
            column.DefaultValue = reader.IsDBNull(defaultValueOrdinal) ? null : reader.GetString(defaultValueOrdinal);

            column.IsAutoIncrement = column.DefaultValue != null && column.DefaultValue.StartsWith("nextval(");

            ColumnPostConfig(column, reader.GetInt32(reader.GetOrdinal("typmod")));

            return column;
        }

        NpgsqlDbColumn SetUpNonColumnField(FieldDescription field)
        {
            var columnName = field.Name.StartsWith("?column?") ? null : field.Name;
            var column = new NpgsqlDbColumn
            {
                ColumnName = columnName,
                BaseCatalogName = _connection.Database,
                BaseColumnName = columnName,
                BaseServerName = _connection.Host,
                IsReadOnly = true,
                DataTypeName = field.PostgresType.DisplayName,
                TypeOID = field.TypeOID,
                TableOID = field.TableOID,
                ColumnAttributeNumber = field.ColumnAttributeNumber,
                PostgresType = field.PostgresType
            };

            ColumnPostConfig(column, field.TypeModifier);

            return column;
        }

        /// <summary>
        /// Performs some post-setup configuration that's common to both table columns and non-columns.
        /// </summary>
        void ColumnPostConfig(NpgsqlDbColumn column, int typeModifier)
        {
            column.DataType = _connection.Connector.TypeHandlerRegistry.TryGetByOID(column.TypeOID, out var handler)
                ? handler.GetFieldType()
                : null;

            if (column.DataType != null)
            {
                column.IsLong = handler is ByteaHandler;

                if (handler is ICompositeHandler)
                    column.UdtAssemblyQualifiedName = column.DataType.AssemblyQualifiedName;
            }

            if (typeModifier == -1)
                return;

            // If the column's type is a domain, use its base data type to interpret the typmod
            var dataTypeName = column.PostgresType is PostgresDomainType
                ? ((PostgresDomainType)column.PostgresType).BaseType.Name
                : column.DataTypeName;
            switch (dataTypeName)
            {
            case "bpchar":
            case "char":
            case "varchar":
                column.ColumnSize = typeModifier - 4;
                break;
            case "numeric":
            case "decimal":
                // See http://stackoverflow.com/questions/3350148/where-are-numeric-precision-and-scale-for-a-field-found-in-the-pg-catalog-tables
                column.NumericPrecision = ((typeModifier - 4) >> 16) & 65535;
                column.NumericScale = (typeModifier - 4) & 65535;
                break;
            }
        }
    }
}
