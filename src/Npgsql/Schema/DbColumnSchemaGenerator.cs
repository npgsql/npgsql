using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandlers.CompositeHandlers;
using Npgsql.Util;

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

        static string GenerateColumnsQuery(Version pgVersion, string columnFieldFilter) =>
$@"SELECT
     typ.oid AS typoid, nspname, relname, attname, attrelid, attnum, attnotnull,
     {(pgVersion.IsGreaterOrEqual(10, 0) ? "attidentity != ''" : "FALSE")} AS isidentity,
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
             pg_index.{(pgVersion.IsGreaterOrEqual(11, 0) ? "indnkeyatts" : "indnatts")} = 1 AND
             attnum = pg_index.indkey[0]
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
     typ.oid AS typoid, nspname, relname, attname, attrelid, attnum, attnotnull,
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

        internal async Task<ReadOnlyCollection<NpgsqlDbColumn>> GetColumnSchema(bool async, CancellationToken cancellationToken = default)
        {
            // This is mainly for Amazon Redshift
            var oldQueryMode = _connection.PostgreSqlVersion < new Version(8, 2);

            var numFields = _rowDescription.Count;
            var result = new List<NpgsqlDbColumn?>(numFields);
            for (var i = 0; i < numFields; i++)
                result.Add(null);
            var populatedColumns = 0;

            if (_fetchAdditionalInfo)
            {
				// We have two types of fields - those which correspond to actual database columns
				// and those that don't (e.g. SELECT 8). For the former we load lots of info from
				// the backend (if fetchAdditionalInfo is true), for the latter we only have the RowDescription

				var columnFieldFilter = _rowDescription
					.Where(f => f.TableOID != 0)  // Only column fields
					.Select(c => $"(attr.attrelid={c.TableOID} AND attr.attnum={c.ColumnAttributeNumber})")
					.Join(" OR ");
				
				if (columnFieldFilter != string.Empty)
				{
					var query = oldQueryMode
						? GenerateOldColumnsQuery(columnFieldFilter)
						: GenerateColumnsQuery(_connection.PostgreSqlVersion, columnFieldFilter);
	
					using var scope = new TransactionScope(
						TransactionScopeOption.Suppress,
						async ? TransactionScopeAsyncFlowOption.Enabled : TransactionScopeAsyncFlowOption.Suppress);
					using var connection = (NpgsqlConnection)((ICloneable)_connection).Clone();
	
					await connection.Open(async, cancellationToken);

                    using var cmd = new NpgsqlCommand(query, connection);
                    var reader = await cmd.ExecuteReader(CommandBehavior.Default, async, cancellationToken);
                    try
                    {
                        while (async ? await reader.ReadAsync(cancellationToken) : reader.Read())
                        {
                            var column = LoadColumnDefinition(reader, _connection.Connector!.TypeMapper.DatabaseInfo, oldQueryMode);
                            for (var ordinal = 0; ordinal < numFields; ordinal++)
                            {
                                var field = _rowDescription[ordinal];
                                if (field.TableOID == column.TableOID &&
                                    field.ColumnAttributeNumber == column.ColumnAttributeNumber)
                                {
                                    populatedColumns++;

                                    if (column.ColumnOrdinal.HasValue)
                                        column = column.Clone();

                                    // The column's ordinal is with respect to the resultset, not its table
                                    column.ColumnOrdinal = ordinal;
                                    result[ordinal] = column;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (async)
                            await reader.DisposeAsync();
                        else
                            reader.Dispose();
                    }
				}
            }

            // We had some fields which don't correspond to regular table columns (or fetchAdditionalInfo is false).
            // Fill in whatever info we have from the RowDescription itself
            for (var i = 0; i < numFields; i++)
            {
                var column = result[i];
                var field = _rowDescription[i];

                if (column is null)
                {
                    column = SetUpNonColumnField(field);
                    column.ColumnOrdinal = i;
                    result[i] = column;
                    populatedColumns++;
                }

                column.ColumnName = field.Name;
                column.IsAliased = column.BaseColumnName is null ? default(bool?) : (column.BaseColumnName != column.ColumnName);
            }

            if (populatedColumns != numFields)
                throw new NpgsqlException("Could not load all columns for the resultset");

            return result.AsReadOnly()!;
        }

        NpgsqlDbColumn LoadColumnDefinition(NpgsqlDataReader reader, NpgsqlDatabaseInfo databaseInfo, bool oldQueryMode)
        {
            // We don't set ColumnName here. It should always contain the column alias rather than
            // the table column name (i.e. in case of "SELECT foo AS foo_alias"). It will be set later.
            var column = new NpgsqlDbColumn
            {
                AllowDBNull = !reader.GetBoolean(reader.GetOrdinal("attnotnull")),
                BaseCatalogName = _connection.Database!,
                BaseSchemaName = reader.GetString(reader.GetOrdinal("nspname")),
                BaseServerName = _connection.Host!,
                BaseTableName = reader.GetString(reader.GetOrdinal("relname")),
                BaseColumnName = reader.GetString(reader.GetOrdinal("attname")),
                ColumnAttributeNumber = reader.GetInt16(reader.GetOrdinal("attnum")),
                IsKey = reader.GetBoolean(reader.GetOrdinal("isprimarykey")),
                IsReadOnly = !reader.GetBoolean(reader.GetOrdinal("is_updatable")),
                IsUnique = reader.GetBoolean(reader.GetOrdinal("isunique")),

                TableOID = reader.GetFieldValue<uint>(reader.GetOrdinal("attrelid")),
                TypeOID = reader.GetFieldValue<uint>(reader.GetOrdinal("typoid"))
            };

            column.PostgresType = databaseInfo.ByOID[column.TypeOID];
            column.DataTypeName = column.PostgresType.DisplayName; // Facets do not get included

            var defaultValueOrdinal = reader.GetOrdinal("default");
            column.DefaultValue = reader.IsDBNull(defaultValueOrdinal) ? null : reader.GetString(defaultValueOrdinal);

            column.IsAutoIncrement =
                !oldQueryMode && reader.GetBoolean(reader.GetOrdinal("isidentity")) ||
                column.DefaultValue != null && column.DefaultValue.StartsWith("nextval(", StringComparison.Ordinal);

            ColumnPostConfig(column, reader.GetInt32(reader.GetOrdinal("typmod")));

            return column;
        }

        NpgsqlDbColumn SetUpNonColumnField(FieldDescription field)
        {
            // ColumnName and BaseColumnName will be set later
            var column = new NpgsqlDbColumn
            {
                BaseCatalogName = _connection.Database!,
                BaseServerName = _connection.Host!,
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
            var typeMapper = _connection.Connector!.TypeMapper;

            column.NpgsqlDbType = typeMapper.GetTypeInfoByOid(column.TypeOID).npgsqlDbType;
            column.DataType = typeMapper.TryResolveByOID(column.TypeOID, out var handler)
                ? handler.GetFieldType()
                : null;

            if (column.DataType != null)
            {
                column.IsLong = handler is ByteaHandler;

                if (handler is ICompositeHandler)
                    column.UdtAssemblyQualifiedName = column.DataType.AssemblyQualifiedName;
            }

            var facets = column.PostgresType.GetFacets(typeModifier);
            if (facets.Size != null)
                column.ColumnSize = facets.Size;
            if (facets.Precision != null)
                column.NumericPrecision = facets.Precision;
            if (facets.Scale != null)
                column.NumericScale = facets.Scale;
        }
    }
}
