using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql;

/// <summary>
/// Provides the underlying mechanism for reading schema information.
/// </summary>
static class NpgsqlSchema
{
    public static Task<DataTable> GetSchema(bool async, NpgsqlConnection conn, string? collectionName, string?[]? restrictions, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(collectionName);
        if (collectionName.Length == 0)
            throw new ArgumentException("Collection name cannot be empty.", nameof(collectionName));

        return collectionName.ToUpperInvariant() switch
        {
            "METADATACOLLECTIONS"   => Task.FromResult(GetMetaDataCollections()),
            "RESTRICTIONS"          => Task.FromResult(GetRestrictions()),
            "DATASOURCEINFORMATION" => Task.FromResult(GetDataSourceInformation(conn)),
            "DATATYPES"             => Task.FromResult(GetDataTypes(conn)),
            "RESERVEDWORDS"         => Task.FromResult(GetReservedWords()),
            // custom collections for npgsql
            "DATABASES"             => GetDatabases(conn, restrictions, async, cancellationToken),
            "SCHEMATA"              => GetSchemata(conn, restrictions, async, cancellationToken),
            "TABLES"                => GetTables(conn, restrictions, async, cancellationToken),
            "COLUMNS"               => GetColumns(conn, restrictions, async, cancellationToken),
            "VIEWS"                 => GetViews(conn, restrictions, async, cancellationToken),
            "MATERIALIZEDVIEWS"     => GetMaterializedViews(conn, restrictions, async, cancellationToken),
            "USERS"                 => GetUsers(conn, restrictions, async, cancellationToken),
            "INDEXES"               => GetIndexes(conn, restrictions, async, cancellationToken),
            "INDEXCOLUMNS"          => GetIndexColumns(conn, restrictions, async, cancellationToken),
            "CONSTRAINTS"           => GetConstraints(conn, restrictions, collectionName, async, cancellationToken),
            "PRIMARYKEY"            => GetConstraints(conn, restrictions, collectionName, async, cancellationToken),
            "UNIQUEKEYS"            => GetConstraints(conn, restrictions, collectionName, async, cancellationToken),
            "FOREIGNKEYS"           => GetConstraints(conn, restrictions, collectionName, async, cancellationToken),
            "CONSTRAINTCOLUMNS"     => GetConstraintColumns(conn, restrictions, async, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(collectionName), collectionName, "Invalid collection name.")
        };
    }

    /// <summary>
    /// Returns the MetaDataCollections that lists all possible collections.
    /// </summary>
    /// <returns>The MetaDataCollections</returns>
    static DataTable GetMetaDataCollections()
    {
        var table = new DataTable("MetaDataCollections");
        table.Columns.Add("CollectionName", typeof(string));
        table.Columns.Add("NumberOfRestrictions", typeof(int));
        table.Columns.Add("NumberOfIdentifierParts", typeof(int));

        table.Rows.Add("MetaDataCollections", 0, 0);
        table.Rows.Add("DataSourceInformation", 0, 0);
        table.Rows.Add("Restrictions", 0, 0);
        table.Rows.Add("DataTypes", 0, 0);  // TODO: Support type name restriction
        table.Rows.Add("Databases", 1, 1);
        table.Rows.Add("Tables", 4, 3);
        table.Rows.Add("Columns", 4, 4);
        table.Rows.Add("Views", 3, 3);
        table.Rows.Add("Users", 1, 1);
        table.Rows.Add("Indexes", 4, 4);
        table.Rows.Add("IndexColumns", 5, 5);

        return table;
    }

    /// <summary>
    /// Returns the Restrictions that contains the meaning and position of the values in the restrictions array.
    /// </summary>
    /// <returns>The Restrictions</returns>
    static DataTable GetRestrictions()
    {
        var table = new DataTable("Restrictions");

        table.Columns.Add("CollectionName", typeof(string));
        table.Columns.Add("RestrictionName", typeof(string));
        table.Columns.Add("RestrictionDefault", typeof(string));
        table.Columns.Add("RestrictionNumber", typeof(int));

        table.Rows.Add("Database", "Name", "Name", 1);
        table.Rows.Add("Tables", "Catalog", "table_catalog", 1);
        table.Rows.Add("Tables", "Schema", "schema_catalog", 2);
        table.Rows.Add("Tables", "Table", "table_name", 3);
        table.Rows.Add("Tables", "TableType", "table_type", 4);
        table.Rows.Add("Columns", "Catalog", "table_catalog", 1);
        table.Rows.Add("Columns", "Schema", "table_schema", 2);
        table.Rows.Add("Columns", "TableName", "table_name", 3);
        table.Rows.Add("Columns", "Column", "column_name", 4);
        table.Rows.Add("Views", "Catalog", "table_catalog", 1);
        table.Rows.Add("Views", "Schema", "table_schema", 2);
        table.Rows.Add("Views", "Table", "table_name", 3);
        table.Rows.Add("Users", "User", "user_name", 1);
        table.Rows.Add("Indexes", "Catalog", "table_catalog", 1);
        table.Rows.Add("Indexes", "Schema", "table_schema", 2);
        table.Rows.Add("Indexes", "Table", "table_name", 3);
        table.Rows.Add("Indexes", "Index", "index_name", 4);
        table.Rows.Add("IndexColumns", "Catalog", "table_catalog", 1);
        table.Rows.Add("IndexColumns", "Schema", "table_schema", 2);
        table.Rows.Add("IndexColumns", "Table", "table_name", 3);
        table.Rows.Add("IndexColumns", "Index", "index_name", 4);
        table.Rows.Add("IndexColumns", "Column", "column_name", 5);

        return table;
    }

    static NpgsqlCommand BuildCommand(NpgsqlConnection conn, StringBuilder query, string?[]? restrictions, params string[]? names)
        => BuildCommand(conn, query, restrictions, true, names);

    static NpgsqlCommand BuildCommand(NpgsqlConnection conn, StringBuilder query, string?[]? restrictions, bool addWhere, params string[]? names)
    {
        var command = new NpgsqlCommand();

        if (restrictions != null && names != null)
        {
            for (var i = 0; i < restrictions.Length && i < names.Length; ++i)
            {
                if (restrictions[i] is { Length: > 0 } restriction)
                {
                    if (addWhere)
                    {
                        query.Append(" WHERE ");
                        addWhere = false;
                    }
                    else
                    {
                        query.Append(" AND ");
                    }

                    var paramName = RemoveSpecialChars(names[i]);

                    query.AppendFormat("{0} = :{1}", names[i], paramName);

                    command.Parameters.Add(new NpgsqlParameter(paramName, restriction));
                }
            }
        }
        command.CommandText = query.ToString();
        command.Connection = conn;

        return command;
    }

    static string RemoveSpecialChars(string paramName)
        => paramName.Replace("(", "").Replace(")", "").Replace(".", "");


    static Task<DataTable> GetDatabases(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable("Databases")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("database_name"),
                new DataColumn("owner"),
                new DataColumn("encoding")
            }
        };

        var sql = new StringBuilder();

        sql.Append(
            """
SELECT d.datname, u.usename, pg_catalog.pg_encoding_to_char(d.encoding)
FROM pg_catalog.pg_database d
LEFT JOIN pg_catalog.pg_user u ON d.datdba = u.usesysid
""");

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, "datname"),
            dataTable,
            (reader, row) =>
        {
            row["database_name"] = GetFieldValueOrDBNull<string>(reader, 0);
            row["owner"] = GetFieldValueOrDBNull<string>(reader, 1);
            row["encoding"] = GetFieldValueOrDBNull<string>(reader, 2);
        }, cancellationToken);
    }

    static Task<DataTable> GetSchemata(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable("Schemata")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("catalog_name"),
                new DataColumn("schema_name"),
                new DataColumn("schema_owner")
            }
        };

        var sql = new StringBuilder(
            """
SELECT * FROM (
    SELECT current_database(), nspname, r.rolname
    FROM pg_catalog.pg_namespace
    LEFT JOIN pg_catalog.pg_roles r ON r.oid = nspowner
) tmp
""");

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, "catalog_name", "schema_name", "schema_owner"),
            dataTable,
            (reader, row) =>
            {
                row["catalog_name"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["schema_name"] = GetFieldValueOrDBNull<string>(reader, 1);
                row["schema_owner"] = GetFieldValueOrDBNull<string>(reader, 2);
            }, cancellationToken);
    }

    static Task<DataTable> GetTables(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable("Tables")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("table_catalog"),
                new DataColumn("table_schema"),
                new DataColumn("table_name"),
                new DataColumn("table_type")
            }
        };

        var sql = new StringBuilder();

        sql.Append(
            """
SELECT table_catalog, table_schema, table_name, table_type
FROM information_schema.tables
WHERE
    table_type IN ('BASE TABLE', 'FOREIGN', 'FOREIGN TABLE') AND
    table_schema NOT IN ('pg_catalog', 'information_schema')
""");

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, false, "table_catalog", "table_schema", "table_name", "table_type"),
            dataTable,
            (reader, row) =>
            {
                row["table_catalog"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["table_schema"] = GetFieldValueOrDBNull<string>(reader, 1);
                row["table_name"] = GetFieldValueOrDBNull<string>(reader, 2);
                row["table_type"] = GetFieldValueOrDBNull<string>(reader, 3);
            }, cancellationToken);
    }

    static Task<DataTable> GetColumns(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable("Columns")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("table_catalog"),
                new DataColumn("table_schema"),
                new DataColumn("table_name"),
                new DataColumn("column_name"),
                new DataColumn("ordinal_position", typeof(int)),
                new DataColumn("column_default"),
                new DataColumn("is_nullable"),
                new DataColumn("data_type"),
                new DataColumn("character_maximum_length", typeof(int)),
                new DataColumn("character_octet_length", typeof(int)),
                new DataColumn("numeric_precision", typeof(int)),
                new DataColumn("numeric_precision_radix", typeof(int)),
                new DataColumn("numeric_scale", typeof(int)),
                new DataColumn("datetime_precision", typeof(int)),
                new DataColumn("character_set_catalog"),
                new DataColumn("character_set_schema"),
                new DataColumn("character_set_name"),
                new DataColumn("collation_catalog")
            }
        };

        var sql = new StringBuilder(
            """
SELECT
    table_catalog,
    table_schema,
    table_name,
    column_name,
    ordinal_position,
    column_default,
    is_nullable,
    CASE WHEN udt_schema is NULL THEN udt_name ELSE format_type(typ.oid, NULL) END,
    character_maximum_length,
    character_octet_length,
    numeric_precision,
    numeric_precision_radix,
    numeric_scale,
    datetime_precision,
    character_set_catalog,
    character_set_schema,
    character_set_name,
    collation_catalog
FROM information_schema.columns
JOIN pg_namespace AS ns ON ns.nspname = udt_schema
JOIN pg_type AS typ ON typnamespace = ns.oid AND typname = udt_name
""");

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, "table_catalog", "table_schema", "table_name", "column_name"),
            dataTable,
            (reader, row) =>
            {
                row["table_catalog"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["table_schema"] = GetFieldValueOrDBNull<string>(reader, 1);
                row["table_name"] = GetFieldValueOrDBNull<string>(reader, 2);
                row["column_name"] = GetFieldValueOrDBNull<string>(reader, 3);
                row["ordinal_position"] = GetFieldValueOrDBNull<int>(reader, 4);
                row["column_default"] = GetFieldValueOrDBNull<string>(reader, 5);
                row["is_nullable"] = GetFieldValueOrDBNull<string>(reader, 6);
                row["data_type"] = GetFieldValueOrDBNull<string>(reader, 7);
                row["character_maximum_length"] = GetFieldValueOrDBNull<int>(reader, 8);
                row["character_octet_length"] = GetFieldValueOrDBNull<int>(reader, 9);
                row["numeric_precision"] = GetFieldValueOrDBNull<int>(reader, 10);
                row["numeric_precision_radix"] = GetFieldValueOrDBNull<int>(reader, 11);
                row["numeric_scale"] = GetFieldValueOrDBNull<int>(reader, 12);
                row["datetime_precision"] = GetFieldValueOrDBNull<int>(reader, 13);
                row["character_set_catalog"] = GetFieldValueOrDBNull<string>(reader, 14);
                row["character_set_schema"] = GetFieldValueOrDBNull<string>(reader, 15);
                row["character_set_name"] = GetFieldValueOrDBNull<string>(reader, 16);
                row["collation_catalog"] = GetFieldValueOrDBNull<string>(reader, 17);
            }, cancellationToken);
    }

    static Task<DataTable> GetViews(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable("Views")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("table_catalog"),
                new DataColumn("table_schema"),
                new DataColumn("table_name"),
                new DataColumn("check_option"),
                new DataColumn("is_updatable")
            }
        };

        var sql = new StringBuilder(
            """
SELECT table_catalog, table_schema, table_name, check_option, is_updatable
FROM information_schema.views
WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
""");

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, false, "table_catalog", "table_schema", "table_name"),
            dataTable,
            (reader, row) =>
            {
                row["table_catalog"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["table_schema"] = GetFieldValueOrDBNull<string>(reader, 1);
                row["table_name"] = GetFieldValueOrDBNull<string>(reader, 2);
                row["check_option"] = GetFieldValueOrDBNull<string>(reader, 3);
                row["is_updatable"] = GetFieldValueOrDBNull<string>(reader, 3);
            }, cancellationToken);
    }

    static Task<DataTable> GetMaterializedViews(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable("MaterializedViews")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("table_catalog"),
                new DataColumn("table_schema"),
                new DataColumn("table_name"),
                new DataColumn("table_owner"),
                new DataColumn("has_indexes", typeof(bool)),
                new DataColumn("is_populated", typeof(bool))
            }
        };

        var sql = new StringBuilder();

        sql.Append("""SELECT current_database(), schemaname, matviewname, matviewowner, hasindexes, ispopulated FROM pg_catalog.pg_matviews""");

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, "current_database()", "schemaname", "matviewname", "matviewowner"),
            dataTable,
            (reader, row) =>
            {
                row["table_catalog"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["table_schema"] = GetFieldValueOrDBNull<string>(reader, 1);
                row["table_name"] = GetFieldValueOrDBNull<string>(reader, 2);
                row["table_owner"] = GetFieldValueOrDBNull<string>(reader, 3);
                row["has_indexes"] = GetFieldValueOrDBNull<bool>(reader, 4);
                row["is_populated"] = GetFieldValueOrDBNull<bool>(reader, 5);
            }, cancellationToken);
    }

    static Task<DataTable> GetUsers(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable("Users")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("user_name"),
                new DataColumn("user_sysid", typeof(uint))
            }
        };

        var sql = new StringBuilder();

        sql.Append("SELECT usename, usesysid FROM pg_catalog.pg_user");

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, "usename"),
            dataTable,
            (reader, row) =>
            {
                row["user_name"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["user_sysid"] = GetFieldValueOrDBNull<uint>(reader, 1);
            }, cancellationToken);
    }

    static Task<DataTable> GetIndexes(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable("Indexes")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("table_catalog"),
                new DataColumn("table_schema"),
                new DataColumn("table_name"),
                new DataColumn("index_name"),
                new DataColumn("type_desc")
            }
        };

        var sql = new StringBuilder(
            """
SELECT current_database(),
    n.nspname,
    t.relname,
    i.relname,
    ''
FROM
    pg_catalog.pg_class i
    JOIN pg_catalog.pg_index ix ON ix.indexrelid = i.oid
    JOIN pg_catalog.pg_class t ON ix.indrelid = t.oid
    LEFT JOIN pg_catalog.pg_user u ON u.usesysid = i.relowner
    LEFT JOIN pg_catalog.pg_namespace n ON n.oid = i.relnamespace
WHERE
    i.relkind = 'i' AND
    n.nspname NOT IN ('pg_catalog', 'pg_toast') AND
    t.relkind = 'r'
""");

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, false, "current_database()", "n.nspname", "t.relname", "i.relname"),
            dataTable,
            (reader, row) =>
            {
                row["table_catalog"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["table_schema"] = GetFieldValueOrDBNull<string>(reader, 1);
                row["table_name"] = GetFieldValueOrDBNull<string>(reader, 2);
                row["index_name"] = GetFieldValueOrDBNull<string>(reader, 3);
                row["type_desc"] = GetFieldValueOrDBNull<string>(reader, 4);
            }, cancellationToken);
    }

    static Task<DataTable> GetIndexColumns(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable("IndexColumns")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("constraint_catalog"),
                new DataColumn("constraint_schema"),
                new DataColumn("constraint_name"),
                new DataColumn("table_catalog"),
                new DataColumn("table_schema"),
                new DataColumn("table_name"),
                new DataColumn("column_name"),
                new DataColumn("index_name")
            }
        };

        var sql = new StringBuilder(
            """
SELECT
    current_database(),
    t_ns.nspname,
    ix_cls.relname,
    current_database(),
    ix_ns.nspname,
    t.relname,
    a.attname,
    ix_cls.relname
FROM
    pg_class t
    JOIN pg_index ix ON t.oid = ix.indrelid
    JOIN pg_class ix_cls ON ix.indexrelid = ix_cls.oid
    JOIN pg_attribute a ON t.oid = a.attrelid
    LEFT JOIN pg_namespace t_ns ON t.relnamespace = t_ns.oid
    LEFT JOIN pg_namespace ix_ns ON ix_cls.relnamespace = ix_ns.oid
WHERE
    ix_cls.relkind = 'i' AND
    t_ns.nspname NOT IN ('pg_catalog', 'pg_toast') AND
    a.attnum = ANY(ix.indkey) AND
    t.relkind = 'r'
""");

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, false, "current_database()", "t_ns.nspname", "t.relname", "ix_cls.relname", "a.attname"),
            dataTable,
            (reader, row) =>
            {
                row["constraint_catalog"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["constraint_schema"] = GetFieldValueOrDBNull<string>(reader, 1);
                row["constraint_name"] = GetFieldValueOrDBNull<string>(reader, 2);
                row["table_catalog"] = GetFieldValueOrDBNull<string>(reader, 3);
                row["table_schema"] = GetFieldValueOrDBNull<string>(reader, 4);
                row["table_name"] = GetFieldValueOrDBNull<string>(reader, 5);
                row["column_name"] = GetFieldValueOrDBNull<string>(reader, 6);
                row["index_name"] = GetFieldValueOrDBNull<string>(reader, 7);
            }, cancellationToken);
    }

    static Task<DataTable> GetConstraints(NpgsqlConnection conn, string?[]? restrictions, string? constraintType, bool async, CancellationToken cancellationToken = default)
    {
        var sql = new StringBuilder(
            """
SELECT
    current_database(),
    pgn.nspname,
    pgc.conname,
    current_database(),
    pgtn.nspname,
    pgt.relname,
    constraint_type,
    pgc.condeferrable,
    pgc.condeferred
FROM
    pg_catalog.pg_constraint pgc
    JOIN pg_catalog.pg_namespace pgn ON pgc.connamespace = pgn.oid
    JOIN pg_catalog.pg_class pgt ON pgc.conrelid = pgt.oid
    JOIN pg_catalog.pg_namespace pgtn ON pgt.relnamespace = pgtn.oid
    JOIN (
        SELECT 'PRIMARY KEY' AS constraint_type, 'p' AS contype
        UNION ALL
        SELECT 'FOREIGN KEY' AS constraint_type, 'f' AS contype
        UNION ALL
        SELECT 'UNIQUE KEY' AS constraint_type, 'u' AS contype
) mapping_table ON mapping_table.contype = pgc.contype
""");

        switch (constraintType)
        {
        case "ForeignKeys":
            sql.Append(" and pgc.contype='f'");
            break;
        case "PrimaryKey":
            sql.Append(" and pgc.contype='p'");
            break;
        case "UniqueKeys":
            sql.Append(" and pgc.contype='u'");
            break;
        default:
            constraintType = "Constraints";
            break;
        }

        var dataTable = new DataTable(constraintType)
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("CONSTRAINT_CATALOG"),
                new DataColumn("CONSTRAINT_SCHEMA"),
                new DataColumn("CONSTRAINT_NAME"),
                new DataColumn("TABLE_CATALOG"),
                new DataColumn("TABLE_SCHEMA"),
                new DataColumn("TABLE_NAME"),
                new DataColumn("CONSTRAINT_TYPE"),
                new DataColumn("IS_DEFERRABLE", typeof(bool)),
                new DataColumn("INITIALLY_DEFERRED", typeof(bool))
            }
        };

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, false, "current_database()", "pgtn.nspname", "pgt.relname", "pgc.conname"),
            dataTable,
            (reader, row) =>
            {
                row["CONSTRAINT_CATALOG"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["CONSTRAINT_SCHEMA"] = GetFieldValueOrDBNull<string>(reader, 1);
                row["CONSTRAINT_NAME"] = GetFieldValueOrDBNull<string>(reader, 2);
                row["TABLE_CATALOG"] = GetFieldValueOrDBNull<string>(reader, 3);
                row["TABLE_SCHEMA"] = GetFieldValueOrDBNull<string>(reader, 4);
                row["TABLE_NAME"] = GetFieldValueOrDBNull<string>(reader, 5);
                row["CONSTRAINT_TYPE"] = GetFieldValueOrDBNull<string>(reader, 6);
                row["IS_DEFERRABLE"] = GetFieldValueOrDBNull<bool>(reader, 7);
                row["INITIALLY_DEFERRED"] = GetFieldValueOrDBNull<bool>(reader, 8);
            }, cancellationToken);
    }

    static Task<DataTable> GetConstraintColumns(NpgsqlConnection conn, string?[]? restrictions, bool async, CancellationToken cancellationToken = default)
    {
        var sql = new StringBuilder(
            """
SELECT current_database(),
    n.nspname,
    c.conname,
    current_database(),
    n.nspname,
    t.relname,
    a.attname,
    a.attnum,
    mapping_table.constraint_type
FROM pg_constraint c
    JOIN pg_namespace n on n.oid = c.connamespace
    JOIN pg_class t on t.oid = c.conrelid AND t.relkind = 'r'
    JOIN pg_attribute a on t.oid = a.attrelid AND a.attnum = ANY(c.conkey)
    JOIN (
        SELECT 'PRIMARY KEY' AS constraint_type, 'p' AS contype
        UNION ALL
        SELECT 'FOREIGN KEY' AS constraint_type, 'f' AS contype
        UNION ALL
        SELECT 'UNIQUE KEY' AS constraint_type, 'u' AS contype
) mapping_table ON
    mapping_table.contype = c.contype
    AND n.nspname NOT IN ('pg_catalog', 'pg_toast')
""");

        var dataTable = new DataTable("ConstraintColumns")
        {
            Locale = CultureInfo.InvariantCulture,
            Columns =
            {
                new DataColumn("constraint_catalog"),
                new DataColumn("constraint_schema"),
                new DataColumn("constraint_name"),
                new DataColumn("table_catalog"),
                new DataColumn("table_schema"),
                new DataColumn("table_name"),
                new DataColumn("column_name"),
                new DataColumn("ordinal_number", typeof(int)),
                new DataColumn("constraint_type")
            }
        };

        return ParseResults(
            async,
            BuildCommand(conn, sql, restrictions, false, "current_database()", "n.nspname", "t.relname", "c.conname", "a.attname"),
            dataTable,
            (reader, row) =>
            {
                row["constraint_catalog"] = GetFieldValueOrDBNull<string>(reader, 0);
                row["constraint_schema"] = GetFieldValueOrDBNull<string>(reader, 1);
                row["constraint_name"] = GetFieldValueOrDBNull<string>(reader, 2);
                row["table_catalog"] = GetFieldValueOrDBNull<string>(reader, 3);
                row["table_schema"] = GetFieldValueOrDBNull<string>(reader, 4);
                row["table_name"] = GetFieldValueOrDBNull<string>(reader, 5);
                row["column_name"] = GetFieldValueOrDBNull<string>(reader, 6);
                row["ordinal_number"] = GetFieldValueOrDBNull<int>(reader, 7);
                row["constraint_type"] = GetFieldValueOrDBNull<string>(reader, 8);
            }, cancellationToken);
    }

    static DataTable GetDataSourceInformation(NpgsqlConnection conn)
    {
        var table = new DataTable("DataSourceInformation");
        var row = table.Rows.Add();

        table.Columns.Add("CompositeIdentifierSeparatorPattern", typeof(string));
        // TODO: DefaultCatalog? Was in XML (unfilled) but isn't in docs
        table.Columns.Add("DataSourceProductName", typeof(string));
        table.Columns.Add("DataSourceProductVersion", typeof(string));
        table.Columns.Add("DataSourceProductVersionNormalized", typeof(string));
        table.Columns.Add("GroupByBehavior", typeof(GroupByBehavior));
        table.Columns.Add("IdentifierPattern", typeof(string));
        table.Columns.Add("IdentifierCase", typeof(IdentifierCase));
        table.Columns.Add("OrderByColumnsInSelect", typeof(bool));
        table.Columns.Add("ParameterMarkerFormat", typeof(string));
        table.Columns.Add("ParameterMarkerPattern", typeof(string));
        table.Columns.Add("ParameterNameMaxLength", typeof(int));
        table.Columns.Add("QuotedIdentifierPattern", typeof(string));
        table.Columns.Add("QuotedIdentifierCase", typeof(IdentifierCase));
        table.Columns.Add("ParameterNamePattern", typeof(string));
        table.Columns.Add("StatementSeparatorPattern", typeof(string));
        table.Columns.Add("StringLiteralPattern", typeof(string));
        table.Columns.Add("SupportedJoinOperators", typeof(SupportedJoinOperators));

        var version = conn.PostgreSqlVersion;
        var normalizedVersion = $"{version.Major:00}.{version.Minor:00}";
        if (version.Build >= 0)
            normalizedVersion += $".{version.Build:00}";

        row["CompositeIdentifierSeparatorPattern"] = @"\.";
        row["DataSourceProductName"] = "Npgsql";
        row["DataSourceProductVersion"] = version.ToString();
        row["DataSourceProductVersionNormalized"] = normalizedVersion;
        row["GroupByBehavior"] = GroupByBehavior.Unrelated;
        row["IdentifierPattern"] = @"(^\[\p{Lo}\p{Lu}\p{Ll}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Nd}@$#_]*$)|(^\[[^\]\0]|\]\]+\]$)|(^\""[^\""\0]|\""\""+\""$)";
        row["IdentifierCase"] = IdentifierCase.Insensitive;
        row["OrderByColumnsInSelect"] = false;
        row["QuotedIdentifierPattern"] = @"""(([^\""]|\""\"")*)""";
        row["QuotedIdentifierCase"] = IdentifierCase.Sensitive;
        row["StatementSeparatorPattern"] = ";";
        row["StringLiteralPattern"] = @"'(([^']|'')*)'";
        row["SupportedJoinOperators"] =
            SupportedJoinOperators.FullOuter |
            SupportedJoinOperators.Inner |
            SupportedJoinOperators.LeftOuter |
            SupportedJoinOperators.RightOuter;

        row["ParameterNameMaxLength"] = 63; // For function out parameters
        row["ParameterMarkerFormat"] = @"{0}";  // TODO: Not sure

        if (NpgsqlCommand.EnableSqlRewriting)
        {
            row["ParameterMarkerPattern"] = @"@[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            row["ParameterNamePattern"] = @"^[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
        }
        else
        {
            row["ParameterMarkerPattern"] = @"$\d+";
            row["ParameterNamePattern"] = @"\d+";
        }

        return table;
    }

    #region DataTypes

    static DataTable GetDataTypes(NpgsqlConnection conn)
    {
        using var _ = conn.StartTemporaryBindingScope(out var connector);

        var table = new DataTable("DataTypes");

        table.Columns.Add("TypeName", typeof(string));
        table.Columns.Add("ColumnSize", typeof(long));
        table.Columns.Add("CreateFormat", typeof(string));
        table.Columns.Add("CreateParameters", typeof(string));
        table.Columns.Add("DataType", typeof(string));
        table.Columns.Add("IsAutoIncrementable", typeof(bool));
        table.Columns.Add("IsBestMatch", typeof(bool));
        table.Columns.Add("IsCaseSensitive", typeof(bool));
        table.Columns.Add("IsConcurrencyType", typeof(bool));
        table.Columns.Add("IsFixedLength", typeof(bool));
        table.Columns.Add("IsFixedPrecisionAndScale", typeof(bool));
        table.Columns.Add("IsLiteralSupported", typeof(bool));
        table.Columns.Add("IsLong", typeof(bool));
        table.Columns.Add("IsNullable", typeof(bool));
        table.Columns.Add("IsSearchable", typeof(bool));
        table.Columns.Add("IsSearchableWithLike", typeof(bool));
        table.Columns.Add("IsUnsigned", typeof(bool));
        table.Columns.Add("LiteralPrefix", typeof(string));
        table.Columns.Add("LiteralSuffix", typeof(string));
        table.Columns.Add("MaximumScale", typeof(short));
        table.Columns.Add("MinimumScale", typeof(short));
        table.Columns.Add("NativeDataType", typeof(string));
        table.Columns.Add("ProviderDbType", typeof(int));

        // Npgsql-specific
        table.Columns.Add("OID", typeof(uint));


        // TODO: Support type name restriction
        try
        {
            var serializerOptions = connector.SerializerOptions;
            PgSerializerOptions.IntrospectionCaller = true;

            var types = new List<PostgresType>();
            types.AddRange(connector.DatabaseInfo.BaseTypes);
            types.AddRange(connector.DatabaseInfo.EnumTypes);
            types.AddRange(connector.DatabaseInfo.CompositeTypes);
            foreach (var baseType in types)
            {
                if (serializerOptions.GetTypeInfoInternal(null, serializerOptions.ToCanonicalTypeId(baseType)) is not { } info)
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, baseType);
                PopulateHardcodedDataTypeInfo(row, baseType);

                row["DataType"] = info.Type.FullName;
                if (baseType.DataTypeName.ToNpgsqlDbType() is { } npgsqlDbType)
                    row["ProviderDbType"] = (int)npgsqlDbType;
            }

            foreach (var arrayType in connector.DatabaseInfo.ArrayTypes)
            {
                if (serializerOptions.GetTypeInfoInternal(null, serializerOptions.ToCanonicalTypeId(arrayType)) is not { } info)
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, arrayType.Element);
                // Populate hardcoded values based on the element type (e.g. citext[] is case-insensitive).
                PopulateHardcodedDataTypeInfo(row, arrayType.Element);

                row["TypeName"] = arrayType.DisplayName;
                row["OID"] = arrayType.OID;
                row["CreateFormat"] += "[]";
                row["DataType"] = info.Type.FullName;
                if (arrayType.DataTypeName.ToNpgsqlDbType() is { } npgsqlDbType)
                    row["ProviderDbType"] = (int)npgsqlDbType;
            }

            foreach (var rangeType in connector.DatabaseInfo.RangeTypes)
            {
                if (serializerOptions.GetTypeInfoInternal(null, serializerOptions.ToCanonicalTypeId(rangeType)) is not { } info)
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, rangeType.Subtype);
                // Populate hardcoded values based on the subtype type (e.g. citext[] is case-insensitive).
                PopulateHardcodedDataTypeInfo(row, rangeType.Subtype);

                row["TypeName"] = rangeType.DisplayName;
                row["OID"] = rangeType.OID;
                row["CreateFormat"] = rangeType.DisplayName.ToUpperInvariant();
                row["DataType"] = info.Type.FullName;
                if (rangeType.DataTypeName.ToNpgsqlDbType() is { } npgsqlDbType)
                    row["ProviderDbType"] = (int)npgsqlDbType;
            }

            foreach (var multirangeType in connector.DatabaseInfo.MultirangeTypes)
            {
                var subtypeType = multirangeType.Subrange.Subtype;
                if (serializerOptions.GetTypeInfoInternal(null, serializerOptions.ToCanonicalTypeId(multirangeType)) is not { } info)
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, subtypeType);
                // Populate hardcoded values based on the subtype type (e.g. citext[] is case-insensitive).
                PopulateHardcodedDataTypeInfo(row, subtypeType);

                row["TypeName"] = multirangeType.DisplayName;
                row["OID"] = multirangeType.OID;
                row["CreateFormat"] = multirangeType.DisplayName.ToUpperInvariant();
                row["DataType"] = info.Type.FullName;
                if (multirangeType.DataTypeName.ToNpgsqlDbType() is { } npgsqlDbType)
                    row["ProviderDbType"] = (int)npgsqlDbType;
            }

            foreach (var domainType in connector.DatabaseInfo.DomainTypes)
            {
                var representationalType = domainType.GetRepresentationalType();
                if (serializerOptions.GetTypeInfoInternal(null, serializerOptions.ToCanonicalTypeId(representationalType)) is not { } info)
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, representationalType);
                // Populate hardcoded values based on the element type (e.g. citext[] is case-insensitive).
                PopulateHardcodedDataTypeInfo(row, representationalType);
                row["TypeName"] = domainType.DisplayName;
                row["OID"] = domainType.OID;
                // A domain is never the best match, since its underlying base type is
                row["IsBestMatch"] = false;

                row["DataType"] = info.Type.FullName;
                if (representationalType.DataTypeName.ToNpgsqlDbType() is { } npgsqlDbType)
                    row["ProviderDbType"] = (int)npgsqlDbType;
            }
        }
        finally
        {
            PgSerializerOptions.IntrospectionCaller = false;
        }

        return table;
    }

    /// <summary>
    /// Populates some generic type information that is common for base types, arrays, enums, etc. Some will
    /// be overridden later.
    /// </summary>
    static void PopulateDefaultDataTypeInfo(DataRow row, PostgresType type)
    {
        row["TypeName"] = type.DisplayName;
        // Skipping ColumnSize at least for now, not very meaningful
        row["CreateFormat"] = type.DisplayName.ToUpperInvariant();
        row["CreateParameters"] = "";
        row["IsAutoIncrementable"] = false;
        // We populate the DataType above from mapping.ClrTypes, which means we take the .NET type from
        // which we *infer* the PostgreSQL type. Since only a single PostgreSQL type gets inferred from a given
        // .NET type, we never have the same DataType in more than one row - so the mapping is always the
        // best match. See the hardcoding override  below for some exceptions.
        row["IsBestMatch"] = true;
        row["IsCaseSensitive"] = true;
        row["IsConcurrencyType"] = false;
        row["IsFixedLength"] = false;
        row["IsFixedPrecisionAndScale"] = false;
        row["IsLiteralSupported"] = false;  // See hardcoding override below
        row["IsLong"] = false;
        row["IsNullable"] = true;
        row["IsSearchable"] = true;
        row["IsSearchableWithLike"] = false;
        row["IsUnsigned"] = DBNull.Value; // See hardcoding override below
        // LiteralPrefix/Suffix: no literal for now except for strings, see hardcoding override below
        row["MaximumScale"] = DBNull.Value;
        row["MinimumScale"] = DBNull.Value;
        // NativeDataType is unset
        row["OID"] = type.OID;
    }

    /// <summary>
    /// Sets some custom, hardcoded info on a DataType row that cannot be loaded/inferred from PostgreSQL
    /// </summary>
    static void PopulateHardcodedDataTypeInfo(DataRow row, PostgresType type)
    {
        switch (type.Name)
        {
        case "varchar":
        case "char":
            row["DataType"] = "String";
            row["IsBestMatch"] = false;
            goto case "text";
        case "text":
            row["CreateFormat"] += "({0})";
            row["CreateParameters"] = "size";
            row["IsSearchableWithLike"] = true;
            row["IsLiteralSupported"] = true;
            row["LiteralPrefix"] = "'";
            row["LiteralSuffix"] = "'";
            return;
        case "numeric":
            row["CreateFormat"] += "({0},{1})";
            row["CreateParameters"] = "precision, scale";
            row["MaximumScale"] = 16383;
            row["MinimumScale"] = 16383;
            row["IsUnsigned"] = false;
            return;
        case "bytea":
            row["IsLong"] = true;
            return;
        case "citext":
            row["IsCaseSensitive"] = false;
            return;
        case "integer":
        case "smallint":
        case "bigint":
        case "double precision":
        case "real":
        case "money":
            row["IsUnsigned"] = false;
            return;
        case "oid":
        case "cid":
        case "regtype":
        case "regconfig":
            row["IsUnsigned"] = true;
            return;
        case "xid":
            row["IsUnsigned"] = true;
            row["IsConcurrencyType"] = true;
            return;
        }
    }

    #endregion DataTypes

    #region Reserved Keywords

    static DataTable GetReservedWords()
    {
        var table = new DataTable("ReservedWords") { Locale = CultureInfo.InvariantCulture };
        table.Columns.Add("ReservedWord", typeof(string));
        foreach (var keyword in ReservedKeywords)
            table.Rows.Add(keyword);
        return table;
    }

    /// <summary>
    /// List of keywords taken from PostgreSQL 9.0 reserved words documentation.
    /// </summary>
    static readonly string[] ReservedKeywords =
    [
        "ALL",
        "ANALYSE",
        "ANALYZE",
        "AND",
        "ANY",
        "ARRAY",
        "AS",
        "ASC",
        "ASYMMETRIC",
        "AUTHORIZATION",
        "BINARY",
        "BOTH",
        "CASE",
        "CAST",
        "CHECK",
        "COLLATE",
        "COLUMN",
        "CONCURRENTLY",
        "CONSTRAINT",
        "CREATE",
        "CROSS",
        "CURRENT_CATALOG",
        "CURRENT_DATE",
        "CURRENT_ROLE",
        "CURRENT_SCHEMA",
        "CURRENT_TIME",
        "CURRENT_TIMESTAMP",
        "CURRENT_USER",
        "DEFAULT",
        "DEFERRABLE",
        "DESC",
        "DISTINCT",
        "DO",
        "ELSE",
        "END",
        "EXCEPT",
        "FALSE",
        "FETCH",
        "FOR",
        "FOREIGN",
        "FREEZE",
        "FROM",
        "FULL",
        "GRANT",
        "GROUP",
        "HAVING",
        "ILIKE",
        "IN",
        "INITIALLY",
        "INNER",
        "INTERSECT",
        "INTO",
        "IS",
        "ISNULL",
        "JOIN",
        "LATERAL",
        "LEADING",
        "LEFT",
        "LIKE",
        "LIMIT",
        "LOCALTIME",
        "LOCALTIMESTAMP",
        "NATURAL",
        "NOT",
        "NOTNULL",
        "NULL",
        "OFFSET",
        "ON",
        "ONLY",
        "OR",
        "ORDER",
        "OUTER",
        "OVER",
        "OVERLAPS",
        "PLACING",
        "PRIMARY",
        "REFERENCES",
        "RETURNING",
        "RIGHT",
        "SELECT",
        "SESSION_USER",
        "SIMILAR",
        "SOME",
        "SYMMETRIC",
        "TABLE",
        "THEN",
        "TO",
        "TRAILING",
        "TRUE",
        "UNION",
        "UNIQUE",
        "USER",
        "USING",
        "VARIADIC",
        "VERBOSE",
        "WHEN",
        "WHERE",
        "WINDOW",
        "WITH"
    ];

    #endregion Reserved Keywords

    static async Task<DataTable> ParseResults(bool async, NpgsqlCommand command, DataTable dataTable, Action<NpgsqlDataReader, DataRow> populateRow, CancellationToken cancellationToken)
    {
        NpgsqlDataReader? reader = null;
        try
        {
            reader = async
                ? await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)
                : command.ExecuteReader();

            dataTable.BeginLoadData();

            while (async ? await reader.ReadAsync(cancellationToken).ConfigureAwait(false) : reader.Read())
                populateRow(reader, dataTable.Rows.Add());

            return dataTable;
        }
        finally
        {
            dataTable.EndLoadData();

            if (async)
            {
                if (reader is not null)
                    await reader.DisposeAsync().ConfigureAwait(false);
                await command.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                reader?.Dispose();
                command.Dispose();
            }
        }
    }

    static object GetFieldValueOrDBNull<T>(NpgsqlDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? DBNull.Value : reader.GetFieldValue<T>(ordinal)!;
}
